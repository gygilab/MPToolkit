
using MPToolkit.Common.Sequence;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{

    public class PeptideScoreComparer : IComparer<Peptide>
    {
        private Peptide Original;

        public PeptideScoreComparer(Peptide original)
        {
            Original = original;
        }
        
        public int Compare(Peptide a, Peptide b)
        {
            if (a.Score == b.Score)
            {
                return a.ToString().Equals(b.ToString()) ? 1 : 0;
            }
            return b.Score.CompareTo(a.Score);
        }
    }
}
