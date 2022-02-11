
using MPToolkit.Common.Data;
using MPToolkit.Common.Sequence;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{
    public class SiteScore
    {
        public int Position;
        public int IonsMatched;
        public int IonsTotal;
        public double Score;
        public List<Peptide> Peptides = new List<Peptide>();
        public List<List<Centroid>> SiteIons = new List<List<Centroid>>();
    }
}
