
using MPToolkit.Common.Data;

namespace MPToolkit.Common.Scoring {
    public class ScoringOptions {
        /// <summary>
        /// Max charge to consider for the fragment ions (inclusive).
        /// </summary>
        public int fragmentChargeMax = 2;

        /// <summary>
        /// Num top ions used when filtering the scan.
        /// </summary>
        public int numTopIons = 10;

        /// <summary>
        /// m/z window used when filtering the scan.
        /// </summary>
        public double mzWindow = 100;
         
        /// <summary>
        /// Tolerance units when matching peaks. Used with fragmentIonTolerance.
        /// </summary>
        public Mass.Units toleranceUnits = Mass.Units.DALTON;

        /// <summary>
        /// Peak match tolerance.
        /// </summary>
        public double fragmentIonTolerance = 0.6;

        /// <summary>
        /// Fragments below lowMassCutoff * max obs m/z are not considered.
        /// </summary>
        public double lowMassCutoff = 0.28;

        /// <summary>
        /// Min m/z to consider when scoring.
        /// </summary>
        public double minMz = 0;

        /// <summary>
        /// max m/z to consider when scoring.
        /// </summary>
        public double maxMz = 1900;
    }
}
