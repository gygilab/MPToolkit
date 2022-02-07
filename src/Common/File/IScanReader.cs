using MPToolkit.Common.Data;
using System.Collections;

namespace MPToolkit.Common.File
{
    public interface IScanReader : IEnumerable
    {
        /// <summary>
        /// Open data file and hold new scan information.
        /// </summary>
        /// <param name="path">Path to the data file.</param>
        void Open(string path, ScanReaderOptions options);

        /// <summary>
        /// Return some metadata about the file.
        /// </summary>
        /// <returns>Returns an instance of ScanFileHeader</returns>
        ScanFileHeader GetHeader();

        /// <summary>
        /// Performs any cleanup.
        /// </summary>
        void Close();
    }
}
