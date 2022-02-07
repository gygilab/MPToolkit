
namespace MPToolkit.Common.Scoring {
    /// <summary>
    /// Stores results from the scoring functions.
    /// Different scores may store different data in the score members.
    /// </summary>
    public class ScoringOutput {
        public double score1;
        public double score2;
        public double score3;
        public int matchedIons;
        public int totalIons;
    }
}
