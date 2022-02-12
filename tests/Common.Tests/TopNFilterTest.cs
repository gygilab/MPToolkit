
using System.Collections.Generic;
using Xunit;

namespace MPToolkit.Common.Data.Filter
{
    public class TopNFilterTest
    {
        [Fact]
        public void RankTest()
        {
            var scan = new Scan();
            scan.Centroids = new List<Centroid>() {
                new Centroid(126.1, 1000),
                new Centroid(127.1, 10000),
                new Centroid(327.1, 100000),
                new Centroid(377.1, 10000)
            };
            var filter = new TopIonsFilter(3, 100);
            filter.Filter(scan);
            Assert.Equal(2, scan.Centroids[0].Rank);
            Assert.Equal(1, scan.Centroids[1].Rank);
            Assert.Equal(1, scan.Centroids[2].Rank);
            Assert.Equal(2, scan.Centroids[3].Rank);
        }

        [Fact]
        public void RemoveTest()
        {
            var scan = new Scan();
            scan.Centroids = new List<Centroid>() {
                new Centroid(126.1, 1000),
                new Centroid(127.1, 10000),
                new Centroid(327.1, 100000),
                new Centroid(377.1, 10000),
                new Centroid(454.1, 10000)
            };
            var filter = new TopIonsFilter(1, 100);
            filter.Filter(scan);
            Assert.Equal(3, scan.Centroids.Count);
            Assert.Equal(127.1, scan.Centroids[0].Mz);
            Assert.Equal(327.1, scan.Centroids[1].Mz);
            Assert.Equal(454.1, scan.Centroids[2].Mz);
        }
    }
}
