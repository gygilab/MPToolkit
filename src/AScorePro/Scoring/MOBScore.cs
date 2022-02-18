
using MPToolkit.Common.Data;
using MPToolkit.Common.Peak;
using MPToolkit.Common.Scoring;
using MPToolkit.Common.Sequence;

using MathNet.Numerics.Distributions;
using System;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{


    public class MOBScore : IScoringStrategy
    {
        private class MOBPeakMatch
        {
            public double error;
            public int theoIndex;
            public int obsIndex;
        }

        private AScoreOptions options;

        public MOBScore(AScoreOptions options)
        {
            this.options = options;
        }

        public double Score(Peptide peptide, List<Centroid> ions, Scan scan, AScoreOutput output)
        {
            int maxPeakDepth = options.MaxPeakDepth;

            var peaks = scan.Centroids;
            if (peaks.Count > 0 && peaks[0].Rank == 0) {
                throw new InvalidOperationException("Ranks need to be assigned before scoring.");
            }

            var matches = matchPeaks(ions, peaks);
            var matchesByPeakDepth = new List<int>(new int[maxPeakDepth + 1]);
            foreach (var match in matches)
            {
                int obsIndex = match.Value;
                int rank = peaks[obsIndex].Rank;
                if (rank > maxPeakDepth)
                {
                    continue;
                }
                matchesByPeakDepth[rank]++;
            }

            peptide.IonsTotal = ions.Count;
            peptide.IonsMatched = matches.Count;
            peptide.Matches = new List<PeakMatch>();
            foreach (var match in matches)
            {
                peptide.Matches.Add(new PeakMatch() {
                    TheoMz = ions[match.Key].Mz,
                    ObsMz = peaks[match.Value].Mz,
                    Intensity = peaks[match.Value].Intensity,
                    Rank = peaks[match.Value].Rank
                });

            }
            peptide.MatchesByDepth = matchesByPeakDepth;

            return calculateScore(peptide.IonsTotal, peptide.IonsMatched, matchesByPeakDepth, output);
        }

        private double binomialCDFUpper(int trials, int successes, double p)
        {
            if (successes < 1)
            {
                return 1;
            }
            if (successes > trials)
            {
                throw new InvalidOperationException("successes greater than trials");
            }
            if (successes < 0)
            {
                throw new InvalidOperationException("negative successes");
            }
            if (trials < 0)
            {
                throw new InvalidOperationException("negative trials");
            }
            if (trials == 0)
            {
                throw new InvalidOperationException("no trials");
            }
            return Binomial.CDF(1 - p, trials, trials - successes);
        }

        public Dictionary<int, int> matchPeaks(List<Centroid> theo, List<Centroid> obs)
        {
            double tol = options.Tolerance;
            Mass.Units units = options.Units;

            // One match per peak in the observed spectrum.
            // The match that will be kept is the one with the least
            // mass deviation.
            // Matches are saved as (obsIndex, theoIndex)
            var allMatches = new List<MOBPeakMatch>(theo.Count);
            var obsToTheo = new Dictionary<int, int>();
            int obsIndex = 0;
            int prevObsStart = 0;
            for (int theoIndex = 0; theoIndex < theo.Count; ++theoIndex)
            {
                double theoMz = theo[theoIndex].Mz;
                double lowMz = 0;
                double highMz = 0;
                PeakMatcher.Window(theoMz, tol, units, out lowMz, out highMz);

                // advance the obsIndex to the start of the window
                obsIndex = prevObsStart;
                while (obsIndex < obs.Count && obs[obsIndex].Mz < lowMz)
                {
                    ++obsIndex;
                }
                prevObsStart = obsIndex;

                while (obsIndex < obs.Count && obs[obsIndex].Mz < highMz)
                {
                    double error = Math.Abs(obs[obsIndex].Mz - theoMz);
                    allMatches.Add(new MOBPeakMatch()
                    {
                        error = error,
                        obsIndex = obsIndex,
                        theoIndex = theoIndex
                    });
                    obsToTheo[obsIndex] = theoIndex;
                    ++obsIndex;
                }
            }

            // Sort to put matches with least error first.
            allMatches.Sort((a, b) => a.error.CompareTo(b.error));

            // Grant matches to those with the least error first.
            var matches = new Dictionary<int, int>();
            foreach (var allMatch in allMatches)
            {
                obsIndex = allMatch.obsIndex;
                int theoIndex = allMatch.theoIndex;
                if (obsToTheo.ContainsKey(obsIndex) && !matches.ContainsKey(theoIndex))
                {
                    // store new theoretical-experimental pairs,
                    // thus keeping only the best match per theoretical peak
                    matches[theoIndex] = obsIndex;
                    obsToTheo.Remove(obsIndex);
                }
            }

            return matches;
        }

        private double calculateScore(int ionsTotal, int ionsMatched, List<int> matchesByPeakDepth, AScoreOutput output)
        {
            double windowSize = options.Window;
            int maxPeakDepth = options.MaxPeakDepth;

            double tolerance = options.Tolerance;
            if (options.Units == Mass.Units.PPM)
            {
                // ppm*1e3/1e6 = Da at 1000 m/z
                tolerance /= 1000.0;
            }
            double pFactor = tolerance * 2.0 / windowSize;

            // Start of scoring
            double score1 = 0;
            double max_binomial = 0.0;
            double binomial_score = 0.0;
            double p = 0.0;
            int n_cum_matches = 0;
            int n_trials = ionsTotal;
            int peak_depth = 1;
            while (peak_depth <= maxPeakDepth && n_cum_matches < ionsMatched)
            {
                n_cum_matches += matchesByPeakDepth[peak_depth];
                p = Math.Min(0.999999, Math.Max(0.000001, (double)peak_depth * pFactor));
                binomial_score = -10 * Math.Log10(binomialCDFUpper(n_trials, n_cum_matches, p));
                if (binomial_score > max_binomial || binomial_score == 0)
                {
                    max_binomial = binomial_score;
                    peak_depth++;
                }
                else
                {
                    score1 += max_binomial;
                    n_trials -= (n_cum_matches - matchesByPeakDepth[peak_depth]);
                    n_cum_matches = 0;
                    max_binomial = 0.0;
                }
            }
            if (output.BestPeakDepth == 0)
            {
                output.BestPeakDepth = peak_depth - 1;
            }
            score1 += max_binomial;
            return score1;
        }
    }
}
