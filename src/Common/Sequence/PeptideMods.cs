
using System.Collections;
using System.Collections.Generic;

namespace MPToolkit.Common.Sequence
{
    /// <summary>
    /// Composite class to store the modification info for a peptide.
    /// </summary>
    public class PeptideMods : IEnumerable<PeptideMod>
    {
        private Dictionary<int, List<PeptideMod>> Mods = new Dictionary<int, List<PeptideMod>>();

        /// <summary>
        /// Add the mod to the specified position on the peptide.
        /// </summary>
        public void Add(int position, PeptideMod mod)
        {
            if (!Mods.ContainsKey(position))
            {
                Mods[position] = new List<PeptideMod>();
            }
            var m = mod.Clone();
            m.Position = position;
            Mods[position].Add(m);
        }

        /// <summary>
        /// Get all mods present at the specified position on the peptide.
        /// </summary>
        public List<PeptideMod> GetMods(int pos)
        {
            if (Mods.ContainsKey(pos))
            {
                return Mods[pos];
            }
            return new List<PeptideMod>();
        }

        public IEnumerator<PeptideMod> GetEnumerator()
        {
            var positions = new List<int> (Mods.Keys);
            positions.Sort();
            foreach (var pos in positions)
            {
                foreach (var mod in Mods[pos])
                {
                    yield return mod;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PeptideMods Clone()
        {
            var output = new PeptideMods();
            output.Mods = new Dictionary<int, List<PeptideMod>>(this.Mods);
            return output;
        }
    }
}
