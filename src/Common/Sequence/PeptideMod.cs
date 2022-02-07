
using System.Text.Json.Serialization;

namespace MPToolkit.Common.Sequence {
    public struct PeptideMod {
        /// <summary>
        /// Symbol represening the modification in the annotated peptide.
        /// </summary>
        [JsonPropertyName("symbol")]
        public char Symbol { get; set; }

        /// <summary>
        /// Residues where the modificatio may be applied
        /// e.g. "STY"
        /// </summary>
        [JsonPropertyName("residues")]
        public string Residues { get; set; }

        /// <summary>
        /// Change in mass when the mod is applied.
        /// </summary>
        [JsonPropertyName("mass")]
        public double Mass { get; set; }

        /// <summary>
        /// Stores the position along the peptide for the mod.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Set to true of the mod can be applied at the
        /// peptide n-terminus
        /// </summary>
        [JsonPropertyName("n-term")]
        public bool IsNTerm { get; set; }

        /// <summary>
        /// Set to true if the mod can be applied to the
        /// peptide c-terminus
        /// </summary>
        [JsonPropertyName("c-term")]
        public bool IsCTerm { get; set; }

        /// <summary>
        /// Returns true of the mod can be appled at the input amino acid.
        /// </summary>
        public bool Applies(char aa) {
            foreach (char res in Residues) {
                if (res == aa) {
                    return true;
                }
            }
            return false;
        }

        public PeptideMod Clone() {
            return (PeptideMod) MemberwiseClone();
        }
    }
}
