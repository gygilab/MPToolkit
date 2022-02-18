
using System.Collections;
using System.IO;

namespace MPToolkit.AScore.Interface
{
    /// <summary>
    /// Stores information for a single row in the peptides file.
    /// </summary>
    public struct PeptidesFileEntry {
        /// <summary>
        /// Not used in AScore, can be any unique number for
        /// identification purposes.
        /// </summary>
        public int Id;

        /// <summary>
        /// The scan number is used to look up the scan from the
        /// scans file.
        /// </summary>
        public int ScanNumber;

        /// <summary>
        /// The annotated peptide sequence. This includes
        /// flanking residues and mod symbols
        /// Ex. "K.M*LAES#DDS#GDEESVSQTDK.T"
        /// </summary>
        public string Peptide;

        /// <summary>
        /// The accurate precursor m/z of the identified peptide.
        /// </summary>
        public double PrecursorMz;
    }

    /// <summary>
    /// Class to parse the peptides csv and returns an enumerator
    /// that can be used in a foreach loop.
    /// </summary>
    public class PeptidesFile : IEnumerable
    {
        private string Path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">The full path to the input file.</param>
        public PeptidesFile(string path)
        {
            Path = path;
        }

        public IEnumerator GetEnumerator()
        {
            var lines = File.ReadLines(Path);
            foreach (var line in lines) {
                if (string.IsNullOrWhiteSpace(line)) {
                    continue;
                }

                if (line.Contains("\tpeptide\t")) {
                    // header line
                    continue;
                }

                yield return ParsePeptide(line.Trim());
            }
        }

        /// <summary>
        /// Reads a line from the csv file and returns the PeptideEntry
        /// </summary>
        private PeptidesFileEntry ParsePeptide(string line) {
            var entry = new PeptidesFileEntry();
            string[] pieces = line.Split("\t");
            entry.Id = int.Parse(pieces[0]);
            entry.ScanNumber = int.Parse(pieces[1]);
            entry.Peptide = pieces[2];
            entry.PrecursorMz = double.Parse(pieces[3]);
            return entry;
        }
    }
}
