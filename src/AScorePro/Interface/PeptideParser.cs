
using MPToolkit.Common.Sequence;
using System.Collections.Generic;
using System.Text;

namespace MPToolkit.AScore.Interface {

    /// <summary>
    /// This parser is used to read in peptides from the input csv.
    /// Input strings should be the annotated peptide sequence,
    /// Differential mods need to be passed into the constructor to
    /// define the mod symbols.
    /// </summary>
    public class PeptideParser {
        private Dictionary<char, PeptideMod> Mods;

        /// <summary>
        /// Constructor.  Pass in the differential mods to
        /// define which mods may appear in the peptid sequence.
        /// </summary>
        /// <param name="mods">Definitions of the diff mods</param>
        public PeptideParser(List<PeptideMod> mods) {
            Mods = new Dictionary<char, PeptideMod>();
            foreach (var mod in mods) {
                Mods.Add(mod.Symbol, mod);
            }
        }

        /// <summary>
        /// Parses the peptides, separating the annotated sequence
        /// to the amino acids and the modifications.  The input string
        /// may have flanking residues and modification information
        /// Ex. "K.M*LAES#DDS#GDEESVSQTDK.T"
        /// </summary>
        /// <param name="peptide">The annotated peptide string.</param>
        /// <returns>The unserialized peptide</returns>
        public Peptide Parse(string peptide) {
            var output = new Peptide();
            if (string.IsNullOrWhiteSpace(peptide)) {
                return output;
            }

            int start = 0;
            if (peptide.Length > 1 && peptide[1] == '.') {
                output.LeftFlank = peptide[0];
                start = 2;
            }
            
            int end = peptide.Length;
            if (peptide.Length > 1 && peptide[peptide.Length - 2] == '.') {
                output.RightFlank = peptide[peptide.Length - 1];
                end = peptide.Length - 2;
            }

            var sb = new StringBuilder();
            for (; start < end; ++start) {
                char c = peptide[start];
                if (char.IsLetter(c)) {
                    sb.Append(c);
                }
                else {
                    var mod = Mods[c];
                    int pos = sb.Length - 1;
                    mod.Position = pos;
                    output.Mods.Add(pos, mod);
                }
            }

            output.Sequence = sb.ToString();
            return output;
        }
    }
}
