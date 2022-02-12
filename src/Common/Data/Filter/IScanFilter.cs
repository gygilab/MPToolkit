
namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// Interface for classes that perform filtering on Scans
    /// </summary>
    public interface IScanFilter
    {
        /// <summary>
        /// Performs filtering and modifies the peak list in the scan.
        /// </summary>
        public void Filter(Scan scan);
    }
}
