using MPToolkit.AScore.Scoring;
using MPToolkit.Common.Sequence;
using MPToolkit.Common.Data;
using MPToolkit.Common.Scoring;
using System.Collections.Generic;

namespace MPToolkit.AScore
{
    /// <summary>
    /// Stores Scoring results for a single peptide.
    /// </summary>
    public class AScoreOutput
    {
        /// <summary>
        /// The input options to AScore
        /// </summary>
        public AScoreOptions Options;

        /// <summary>
        /// The number of mods on the original peptide.
        /// </summary>
        public int ModCount = 0;

        /// <summary>
        /// The peak depth that was used for site scoring 
        /// </summary>
        public int BestPeakDepth = 0;

        /// <summary>
        /// From the peptide that produces the best score.
        /// Used with BestPeakDepth
        /// </summary>
        public double BestPeptideScore = 0;

        /// <summary>
        /// The spectram that the peptide was scored against.
        /// </summary>
        public Scan Scan;

        /// <summary>
        /// Scored peptides with different arrangement of modifications.
        /// </summary>
        public List<Peptide> Peptides;

        /// <summary>
        /// Site scores for the top peptide.
        /// </summary>
        public List<SiteScore> Sites;
    };
}
