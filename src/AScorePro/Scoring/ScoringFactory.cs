
namespace MPToolkit.AScore.Scoring
{
    public class ScoringFactory
    {
        public static IScoringStrategy Get(AScoreOptions options) {
            if (options.UseMobScore) {
                return new MOBScore(options);
            }
            return new OriginalScore(options);
        }
    }
}
