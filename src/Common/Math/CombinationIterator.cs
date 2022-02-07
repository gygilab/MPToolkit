
using System.Collections.Generic;

namespace MPToolkit.Common.Math
{
    /// <summary>
    /// Class generates all unique subsets from the input set of the specified size.
    /// </summary>
    public class CombinationIterator
    {
        /// <summary>
        /// Returns Lists that are subsets of the original set with the given sample size.
        /// This implements IEnumerable to allow use in a foreach loop.
        /// </summary>
        /// <param name="set">The input set of elements to select from</param>
        /// <param name="sampleSize">The size of the subsets to generate</param>
        /// <typeparam name="T">The type of the elements in the set</typeparam>
        /// <returns>A subset as a list</returns>
        public IEnumerable<List<T>> Generate<T>(List<T> set, int sampleSize)
        {
            var subset = new List<T>(sampleSize);
            int i = 0;
            return MakeCombinations(set, i, sampleSize, subset);
        }

        /// <summary>
        /// Recursive function to generate all possible subsets of the specified size.
        /// yields when the subset reaches the specified size.
        /// </summary>
        /// <param name="set">input set</param>
        /// <param name="i">current index in the input set</param>
        /// <param name="sampleSize">input speicifying the sample size</param>
        /// <param name="subset">the currently running subset</param>
        private IEnumerable<List<T>> MakeCombinations<T>(List<T> set, int i, int sampleSize, List<T> subset)
        {
            if (subset.Count == sampleSize)
            {
                yield return new List<T>(subset);
            }
            else if (i >= set.Count)
            {
                yield break;
            }
            else
            {
                subset.Add(set[i]);
                foreach (var combo in MakeCombinations(set, i + 1, sampleSize, subset))
                {
                    yield return combo;
                }
                subset.RemoveAt(subset.Count - 1);

                foreach (var combo in MakeCombinations(set, i + 1, sampleSize, subset))
                {
                    yield return combo;
                }
            }
        }
    }
}
