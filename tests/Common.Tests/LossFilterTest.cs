
using System.Collections.Generic;
using Xunit;

namespace MPToolkit.Common.Data.Filter
{
    public class LossFilterTest
    {
        [Fact]
        public void WaterLossTest()
        {
            var scan = GetScan();
            int peakCount = scan.Centroids.Count;
            var filter = new WaterLossFilter(1.0, Mass.Units.DALTON);
            filter.Filter(scan);
            Assert.Equal(peakCount - 2, scan.Centroids.Count);
        }

        [Fact]
        public void NeutralLossTest()
        {
            var scan = GetScan();
            int peakCount = scan.Centroids.Count;
            var filter = new NeutralLossFilter(0.5, Mass.Units.DALTON);
            filter.Filter(scan);
            Assert.Equal(peakCount - 1, scan.Centroids.Count);
        }

        [Fact]
        public void NoRemoveTest()
        {
            var scan = GetScan();
            int peakCount = scan.Centroids.Count;
            var filter = new WaterLossFilter(20, Mass.Units.PPM);
            filter.Filter(scan);
            Assert.Equal(peakCount, scan.Centroids.Count);
        }

        private Scan GetScan() {
            var scan = new Scan();
            scan.Centroids = new List<Centroid>() {
                new Centroid(436.3101, 1347.1),
                new Centroid(535.4120, 1734.1),
                new Centroid(619.4920, 1171.1),
                new Centroid(671.93596, 1280.1),
                new Centroid(692.4050, 183.1),
                new Centroid(703.4280, 1245.1),
                new Centroid(703.9110, 373.1),
                new Centroid(711.93596, 1001.1),
                new Centroid(792.497, 341.1),
                new Centroid(1017.609, 271.1),
            };
            scan.PeakCount = scan.Centroids.Count;
            scan.Precursors = new List<Precursor>() {
                new Precursor(711.93596, 1, 2)
            };
            return scan;
        }
    }
}
