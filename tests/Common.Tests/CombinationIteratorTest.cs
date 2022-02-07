using System;
using System.Collections.Generic;
using Xunit;

using MPToolkit.Common.Math;

namespace MPToolkit.Common.Math
{
    public class CombinationIteratorTest
    {
        [Fact]
        public void TestPairs()
        {
            var gen = new CombinationIterator();

            var set = new List<int> { 2, 4, 6, 8 };
            var result = new List<List<int>>(gen.Generate(set, 2));
        }
    }
}
