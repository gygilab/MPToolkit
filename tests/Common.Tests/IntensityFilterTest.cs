
using System.Collections.Generic;
using Xunit;

namespace MPToolkit.Common.Data.Filter {
    public class IntensityFilterTest {
        [Fact]
        public void FilterTest()
        {
            var scan = GetScan();
            var filter = new IntensityFilter(0.25);
            filter.Filter(scan);
            Assert.Equal(7, scan.Centroids.Count);
            Assert.Equal(436.3101, scan.Centroids[0].Mz);
            Assert.Equal(711.93596, scan.Centroids[6].Mz);
        }

        private Scan GetScan() {
            var scan = new Scan();
            scan.Centroids = new List<Centroid>() {
                new Centroid(436.3101, 1347.1),
                new Centroid(535.4120, 1734.1),
                new Centroid(619.4920, 1171.1),
                new Centroid(631.93596, 1280.1),
                new Centroid(692.4050, 183.1),
                new Centroid(703.4280, 1245.1),
                new Centroid(703.9110, 373.1),
                new Centroid(711.93596, 1001.1),
                new Centroid(792.497, 341.1),
                new Centroid(1017.609, 271.1),
            };
            scan.Precursors = new List<Precursor>() {
                new Precursor(711.93596, 1, 2)
            };
            return scan;
        }
    }
}
