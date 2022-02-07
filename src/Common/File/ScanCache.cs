
using MPToolkit.Common.Data;
using MPToolkit.Common.File;
using System.Collections.Generic;

namespace MPToolkit.Common
{
    public class ScanCache {

        private Dictionary<int, Scan> Scans = new Dictionary<int, Scan>();

        public ScanCache(IScanReader reader) {
            Scans.Clear();
            foreach (Scan scan in reader)
            {
                if (scan.ScanNumber < 1) {
                    continue;
                }
                Scans.Add(scan.ScanNumber, scan);
            }
        }

        public Scan GetScan(int scanNumber) {
            return Scans[scanNumber];
        }
    }
}
