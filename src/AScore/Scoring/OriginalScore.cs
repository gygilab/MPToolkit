
using MPToolkit.Common.Data;
using MPToolkit.Common.Scoring;
using MPToolkit.Common.Sequence;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{
    public class OriginalScore : IScoringStrategy
    {
        private AScoreOptions options;

        private static BinomialScore binomialScore;

        private static List<double> weights;

        public OriginalScore(AScoreOptions options)
        {
            this.options = options;
            if (binomialScore == null)
            {
                binomialScore = new BinomialScore();
                weights = new List<double> () { 0.25, 0.5, 0.5, 0.75, 1, 1, 1, 0.75, 0.5, 0.5, 0.5 };
            }
        }

        public double Score(Peptide peptide, List<Centroid> ions, Scan scan, AScoreOutput output)
        {
            int minPeakDepth = 1;
            int maxPeakDepth = options.MaxPeakDepth;
            if (options.PeakDepth > 0)
            {
                // Fixed depth used in site scoring.
                minPeakDepth = options.PeakDepth;
                maxPeakDepth = options.PeakDepth;
            }

            // Generate spectra at each peak depth.
            var filter = new TopIonsFilter();
            var scans = new Dictionary<int, Scan>();
            for (int depth = minPeakDepth; depth <= maxPeakDepth; ++depth)
            {
                Scan s = (Scan)scan.Clone();
                filter.Filter(s, depth, options.Window);
                scans.Add(depth, s);
            }

            var scoringOptions = new ScoringOptions()
            {
                numTopIons = 5,
                mzWindow = options.Window,
                fragmentChargeMax = 2,
                fragmentIonTolerance = options.Tolerance,
                toleranceUnits = options.Units
            };

            // Scores across range of top ions.
            var scores = new List<double>(maxPeakDepth + 1);
            int bestPeakDepth = 0;
            double maxScore = 0;
            int bestIonsMatched = 0;
            for (int j = minPeakDepth; j <= maxPeakDepth; ++j)
            {
                Scan s = scans[j];
                scoringOptions.numTopIons = j;
                binomialScore.SetScoringOptions(scoringOptions);

                ScoringOutput scoringOutput = binomialScore.score(ions, s);
                scores.Add(scoringOutput.score1);

                if (scoringOutput.score1 >= maxScore)
                {
                    // Prefer more peaks
                    maxScore = scoringOutput.score1;
                    bestPeakDepth = j;
                    bestIonsMatched = scoringOutput.matchedIons;
                }
            }

            peptide.IonsTotal = ions.Count;
            peptide.IonsMatched = bestIonsMatched;

            // Get peptide scores across a range of peak depths
            // while site scoring is at fixed peak depth.
            if (options.PeakDepth == 0 && maxScore > output.BestPeptideScore)
            {
                output.BestPeakDepth = bestPeakDepth;
                output.BestPeptideScore = maxScore;
            }

            // Get harcoded weighted average like original ascore.
            double sum = 0;
            double n = 0;
            int scoreIndex = 0;
            for (int j = minPeakDepth; j <= maxPeakDepth; ++j)
            {
                double weight = 0.5;
                if (j < weights.Count)
                {
                    weight = weights[j];
                }
                sum += weight * scores[scoreIndex++];
                n += weight;
            }

            return sum / n;
        }
    }
}
