
using System.Collections.Generic;
using MPToolkit.Common.Data;

namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// Removes the lowest intensity peaks from the scan.
    /// A fraction of the peaks in the scan will be removed
    /// based on the argument passed to the constructor.
    /// </summary>
    public class IntensityFilter : IScanFilter
    {
        double FractionToRemove;

        /// <summary>
        /// Initializes the filter.
        /// 
        /// Pass in a number to indicate the fraction of peak to remove.
        /// </summary>
        public IntensityFilter(double fractionToRemove)
        {
            FractionToRemove = fractionToRemove;
        }

        /// <summary>
        /// Performs filtering on the scan, replacing the peak list.
        /// Peaks are sorted based on Intensity and the
        ///  lowest intensity peaks removed.
        /// </summary>
        /// <param name="scan"></param>
        public void Filter(Scan scan)
        {
            if (FractionToRemove < 0.01 || scan.Centroids.Count < 4)
            {
                return;
            }

            var peaks = new List<Centroid>(scan.Centroids);
            peaks.Sort((a, b) => a.Intensity.CompareTo(b.Intensity));
            int i = (int) System.Math.Floor(peaks.Count * FractionToRemove);
            if (i > peaks.Count - 1)
            {
                return;
            }

            double minIntensity = peaks[i].Intensity;
            var result = new List<Centroid>();
            foreach (var peak in scan.Centroids) {
                if (peak.Intensity > minIntensity) {
                    result.Add(peak);
                }
            }
            scan.Centroids = result;
        }
    }
}
