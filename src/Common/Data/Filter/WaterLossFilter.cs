
using MPToolkit.Common.Peak;

namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// The water loss filter removes precursor peak 
    /// 1x and 2x water losses from the spectrum.
    /// </summary>
    public class WaterLossFilter : IScanFilter
    {
        private double Tolerance;

        private Mass.Units Units;

        /// <summary>
        /// Initialize the filter.
        /// 
        /// Pass in the peak match tolerance when searching for water losses.
        /// </summary>
        public WaterLossFilter(double tolerance, Mass.Units units)
        {
            Tolerance = tolerance;
            Units = units;
        }

        /// <summary>
        /// Removes precursor water loss peaks from the scan.
        /// Using the precursor m/z value in the scan, this method
        /// searches for peaks subtracting 1x and 2x water loss and removes them.
        /// The peak list in the scan is modified.
        /// </summary>
        public void Filter(Scan scan)
        {
            if (scan.Precursors.Count == 0)
            {
                return;
            }
            var precursor = scan.Precursors[0];
            if (precursor.Mz < 1 || precursor.Charge == 0)
            {
                return;
            }

            // Remove 1x water loss
            double targetMz = precursor.Mz - (Mass.Water / precursor.Charge);
            int i;
            while ((i = PeakMatcher.Match(scan, targetMz, Tolerance, (int)Units)) != -1) {
                scan.Centroids.RemoveAt(i);
            }

            // Remove 2x water loss
            targetMz = precursor.Charge - (2 * Mass.Water / precursor.Charge);
            while ((i = PeakMatcher.Match(scan, targetMz, Tolerance, (int)Units)) != -1) {
                scan.Centroids.RemoveAt(i);
            }
        }
    }
}
