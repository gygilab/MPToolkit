
using MPToolkit.Common.Data;
using MPToolkit.Common.Math;
using MPToolkit.Common.Peak;
using MPToolkit.Common.Scoring;

using System.Collections.Generic;

namespace MPToolkit.Common.Scoring
{
    public class BinomialScore
    {
        private ScoringOptions options;

        private bool useCache = true;

        private List<List<double>> cache;

        public BinomialScore()
        {
            cache = new List<List<double>>();
            prepareScoreCache();
        }

        public void SetScoringOptions(ScoringOptions options)
        {
            this.options = options;
        }

        public double calculateScore(int numTrials, int numMatches)
        {
            double probability = cumulativePCached(numTrials, numMatches);
            double score = 5000;
            if (probability > 0)
            {
                score = -10 * System.Math.Log10(probability);
            }
            return score;
        }

        private void prepareScoreCache()
        {
            int max = 300;
            cache.Capacity = 300;
            for (int i = 0; i < max; ++i)
            {
                cache[i] = new List<double>();
                cache[i].Capacity = i + 1;
                for (int j = 0; j <= i; ++j)
                {
                    cache[i][j] = calculateP(i, j);
                }
            }
        }

        private double cumulativePCached(int numTrials, int numMatches)
        {
            if (useCache && numTrials < cache.Count && numMatches < cache[numTrials].Count)
            {
                return cache[numTrials][numMatches];
            }
            return calculateP(numTrials, numMatches);
        }

        private double calculateP(int numTrials, int numMatches)
        {
            if (numMatches == 0)
            {
                return 1.0;
            }

            double e = options.fragmentIonTolerance;
            if (options.toleranceUnits == Mass.Units.PPM)
            {
                // Convert from ppm to error at mid m/z
                e = (750.0 * options.fragmentIonTolerance / 1000000.0);
            }

            // Calculate probability of random peak match
            // based on window size, peak depth, and the fragment ion tolerance.
            double p = options.numTopIons * 2.0 * e / options.mzWindow;

            // Sum the probabilities for each integer number of successes
            // equal to or greater than what was observed.
            double cumulativeP = 0;
            for (int numSuccesses = numMatches; numSuccesses <= numTrials; ++numSuccesses)
            {
                double np = Binomial.P(numTrials, numSuccesses, p);
                if (double.IsNaN(np))
                {
                    np = 0;
                }
                cumulativeP += np;
            }

            return cumulativeP;
        }

        /// <summary>
        /// Runs the binomial score on the given list of theoretical ions against 
        /// the input scan.
        /// </summary>
        /// <param name="ions">Theoretical (fragment) ions</param>
        /// <param name="scan">Observed scan</param>
        /// <returns>An instance of ScoringOutput containing the scores</returns>
        public ScoringOutput score(List<Centroid> ions, Scan scan)
        {
            ScoringOutput output = new ScoringOutput();
            int numTrials = 0;
            int numMatches = 0;
            double sumSquareError = 0;
            double sumError = 0;

            int maxCharge = (int)System.Math.Min(scan.Precursors[0].Charge - 1, options.fragmentChargeMax);
            double minMz = System.Math.Max(options.lowMassCutoff * scan.Precursors[0].Mz, options.minMz);
            double maxMz = options.maxMz;

            List<Centroid> peaks = scan.Centroids;
            if (ions.Count == 0 || peaks.Count == 0)
            {
                return output;
            }

            int i = 0;
            int iLast = ions.Count - 1;
            int j = 0;
            int jLast = peaks.Count - 1;
            double tolerance = options.fragmentIonTolerance;
            Mass.Units units = options.toleranceUnits;

            // Do not count a matched peak more than once.
            bool matched = false;
            while (true)
            {
                double exp = ions[i].Mz;
                double obs = peaks[i].Mz;
                if (!matched && ions[i].Charge <= maxCharge
                        && exp > minMz && obs > minMz
                        && exp < maxMz && obs < maxMz)
                {
                    double error = 0;
                    if (PeakMatcher.WithinError(exp, obs, tolerance, units, out error))
                    {
                        ++numMatches;
                        sumSquareError += error * error;
                        sumError += error;
                        matched = true;
                    }
                }

                if (i == iLast && j == jLast)
                {
                    break;
                }
                else if (i == iLast)
                {
                    ++j;
                }
                else if (j == jLast)
                {
                    ++i;
                    matched = false;
                }
                else
                {
                    if ((peaks[j + 1].Mz - exp) < (ions[i + 1].Mz - obs))
                    {
                        ++j;
                    }
                    else
                    {
                        ++i;
                        matched = false;
                    }
                }
            }

            for (i = 0; i <= iLast; ++i)
            {
                if (ions[i].Charge <= maxCharge && ions[i].Mz > minMz && ions[i].Mz < maxMz)
                {
                    ++numTrials;
                }
            }

            output.score1 = calculateScore(numTrials, numMatches);

            if (numMatches > 0)
            {
                double avgError = sumError / numMatches;
                output.score2 = (sumSquareError / numMatches) - (avgError * avgError); // stddev error
                output.score3 = System.Math.Sqrt(sumSquareError / numMatches); // rms error
            }

            output.matchedIons = numMatches;
            output.totalIons = numTrials;
            return output;
        }
    }
}
