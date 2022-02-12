
using System.Collections.Generic;
using MPToolkit.Common.Data;
using MPToolkit.Common.Peak;

namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// Deisotopes the scan by looking by matching peaks
    /// in an isotopic distribution.
    /// </summary>
    public class MatchDeisotoper : IScanFilter
    {
        private double Tolerance;

        private Mass.Units Units;

        /// <summary>
        /// Initialize the filter.
        /// </summary>
        public MatchDeisotoper(double tolerance, Mass.Units units)
        {
            Tolerance = tolerance;
            Units = units;
        }

        /// <summary>
        /// Iterates through the peaks in the spectrum and removes
        /// peaks if there is a matching peak to the left with
        /// a neutron mass difference.
        /// Modifies the peaks in the scan.
        /// </summary>
        public void Filter(Scan scan)
        {
            if (scan.Centroids.Count < 4)
            {
                return;
            }

            int maxCharge = 1;
            if (scan.Precursors.Count > 1 && scan.Precursors[0].Charge > 0)
            {
                maxCharge = System.Math.Min(2, scan.Precursors[0].Charge);
            }

            var result = new List<Centroid>(scan.Centroids.Count);
            for (int i = scan.Centroids.Count - 1; i >= 0; --i)
            {
                bool found = false;
                for (int charge = 1; charge <= maxCharge; ++charge)
                {
                    double target = scan.Centroids[i].Mz - (Mass.Neutron / charge);
                    double intensity = scan.Centroids[i].Intensity;

                    // Returns -1 if none found.
                    int j = PeakMatcher.Match(scan, target, Tolerance, (int)Units);
                    if (j >= 0 && j != i && intensity < 1.2 * scan.Centroids[j].Intensity)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    result.Add(scan.Centroids[i]);
                }
            }
            result.Sort((a, b) => a.Mz.CompareTo(b.Mz));
            scan.Centroids = result;
        }
    }
}
