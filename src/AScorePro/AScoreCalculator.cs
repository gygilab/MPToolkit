
using MPToolkit.AScore.Scoring;
using MPToolkit.Common.Scoring;
using MPToolkit.Common.Sequence;
using MPToolkit.Common.Data;
using MPToolkit.Common.Data.Filter;
using System.Collections.Generic;
using System;

namespace MPToolkit.AScore
{
    /// <summary>
    /// The AScoreCalculator class stores the configuration for the
    /// instance of AScore and can run AScore for a peptide and scan.
    /// </summary>
    public class AScoreCalculator
    {
        private AScoreOptions Options;

        public AScoreCalculator(AScoreOptions options)
        {
            this.Options = options;
        }

        public AScoreOptions GetOptions()
        {
            return this.Options;
        }

        /// <summary>
        /// Runs AScore for a single peptide
        /// </summary>
        public AScoreOutput Run(Peptide peptide, Scan inputScan)
        {
            var output = new AScoreOutput();
            output.Options = new AScoreOptions(Options);
            var targetMod = new PeptideMod()
            {
                Symbol = Options.Symbol,
                Residues = Options.Residues
            };

            Scan scan = inputScan.Clone();
            scan.Precursors[0].Mz = peptide.PrecursorMz;
            int fragmentChargeMax = Math.Min(2, scan.Precursors[0].Charge - 1);
            double maxMz = scan.EndMz;
            double minMz = scan.StartMz;
            if (Options.LowMassCutoff)
            {
                minMz = Math.Max(minMz, 0.28 * peptide.PrecursorMz);
            }

            PreProcessScan(scan);

            // Score each peptide
            var peptides = new List<Peptide>();
            var scoring = ScoringFactory.Get(Options);
            // TODO: Set neutral Loss moss
            var generator = new PeptideGenerator(peptide, targetMod);
            for (; !generator.AtEnd() && peptides.Count < Options.MaxPeptides; generator.Next())
            {
                var ions = generator.GetMassList(Options.IonSeries, fragmentChargeMax, minMz, maxMz);
                var p = generator.GetPeptide();
                p.Score = scoring.Score(p, ions, scan, output);
                peptides.Add(p);
            }

            // Sort by score. Prefer original sequence if tied.
            peptides.Sort(new PeptideScoreComparer(peptide));
            output.Peptides = peptides;

            // Use best peak depth for site scoring (original score only).
            Peptide topPeptide = peptides[0];
            Options.PeakDepth = output.BestPeakDepth;
            var siteScoring = ScoringFactory.Get(Options);

            var sites = new List<SiteScore>();
            foreach (var mod in topPeptide.Mods)
            {
                // Dont score other mods.
                if (mod.Symbol != targetMod.Symbol)
                {
                    continue;
                }

                // Default score is 5000 if there is only one possible
                // configuration for the current mod.
                SiteScore siteOutput = new SiteScore();
                siteOutput.Score = 5000;

                Peptide currentPeptide = topPeptide;
                Peptide next;

                // Find the next peptide that doesnt contain the mod at the position.
                for (int i = 1; i < peptides.Count; ++i)
                {
                    next = peptides[i];
                    bool hasMod = false;
                    foreach (var nextMod in next.Mods.GetMods(mod.Position))
                    {
                        if (nextMod.Symbol == mod.Symbol)
                        {
                            hasMod = true;
                            break;
                        }
                    }
                    if (hasMod)
                    {
                        continue;
                    }

                    // Score the mod.
                    // Find and score the site determining ions.
                    generator.SetIndex(currentPeptide.GeneratorIndex);
                    var ions = generator.GetMassList(Options.IonSeries, fragmentChargeMax, minMz, maxMz);

                    generator.SetIndex(next.GeneratorIndex);
                    var nextIons = generator.GetMassList(Options.IonSeries, fragmentChargeMax, minMz, maxMz);

                    var siteIons = SiteIons.Filter(ions, nextIons);
                    siteOutput.Score = siteScoring.Score(currentPeptide, siteIons, scan, output);
                    siteOutput.Peptides.Add(currentPeptide.Clone());
                    siteOutput.SiteIons.Add(new List<Centroid>(siteIons));

                    if (Options.UseDeltaAscore)
                    {
                        // Flip order and get the score difference.
                        var siteIons2 = SiteIons.Filter(nextIons, ions);
                        siteOutput.Score -= siteScoring.Score(peptide, siteIons2, scan, output);
                    }

                    siteOutput.Peptides.Add(next);

                    // Score only once per mod.
                    break;
                }

                // Convert to 1-based position
                siteOutput.Position = mod.Position + 1;
                sites.Add(siteOutput);
            } // mods

            output.Sites = sites;
            return output;
        }

        private void PreProcessScan(Scan scan)
        {
            if (Options.DeisotopingType == Deisotoping.MatchOffset) {
                (new MatchDeisotoper(Options.Tolerance, Options.Units)).Filter(scan);
            }
            else if (Options.DeisotopingType == Deisotoping.Top1Per1) {
                (new TopIonsFilter(1, 1)).Filter(scan);
            }
            var filters = new List<IScanFilter>() {
                new IntensityFilter(Options.FilterLowIntensity),
                new WaterLossFilter(Options.Tolerance, Options.Units),
                new NeutralLossFilter(Options.Tolerance, Options.Units),
                new TopIonsFilter(Options.MaxPeakDepth, Options.Window)
            };
            foreach (var filter in filters)
            {
                filter.Filter(scan);
            }
        }
    }

}
