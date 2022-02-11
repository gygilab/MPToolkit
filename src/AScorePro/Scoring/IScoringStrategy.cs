
using MPToolkit.Common.Data;
using MPToolkit.Common.Sequence;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{
    public interface IScoringStrategy
    {
        public double Score(Peptide peptide, List<Centroid> ions, Scan scan, AScoreOutput output);
    }
}
