
using MPToolkit.Common.Peak;

namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// Removes peaks from the scan that match the precursor peak
    /// minus a neutral loss of phospho.
    /// </summary>
    public class NeutralLossFilter : IScanFilter
    {
        private double Tolerance;

        private Mass.Units Units;

        const double NL_MASS1 = 79.96633;

        const double NL_MASS2 = 97.97689;

        /// <summary>
        /// Initialize the filter.
        /// 
        /// Pass in the peak match tolerance when searching for
        /// precursor neutral losses.
        /// </summary>
        public NeutralLossFilter(double tolerance, Mass.Units units)
        {
            Tolerance = tolerance;
            Units = units;
        }

        /// <summary>
        /// Removes precursor neutral loss peaks from the scan.
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

            double targetMz = precursor.Mz - (NL_MASS1 / precursor.Charge);
            int i;
            while ((i = PeakMatcher.Match(scan, targetMz, Tolerance, (int)Units)) != -1) {
                scan.Centroids.RemoveAt(i);
            }

            targetMz = precursor.Mz - (NL_MASS2 / precursor.Charge);
            while ((i = PeakMatcher.Match(scan, targetMz, Tolerance, (int)Units)) != -1) {
                scan.Centroids.RemoveAt(i);
            }
        }
    }
}
