using MPToolkit.AScore.Interface;
using MPToolkit.Common;
using MPToolkit.Common.Sequence;
using MPToolkit.Common.Data;
using MPToolkit.Common.File;
using System;
using System.IO;

namespace MPToolkit.AScore
{
    /// <summary>
    /// This defines the entry point of the AScore CLI application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point of the AScore CLI
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var parser = new CliOptionsParser();
            AScoreCliOptions options = parser.Parse(args);

            FileStream stream = File.Open(options.ParametersPath, FileMode.Open, FileAccess.Read);
            AScoreOptions ascoreOptions = AScoreOptionsReader.Read(stream);

            string peptidesPath = ascoreOptions.PeptidesFile;
            if (!string.IsNullOrEmpty(options.PeptidesPath))
            {
                peptidesPath = options.PeptidesPath;
            }

            string scansPath = ascoreOptions.Scans;
            if (!string.IsNullOrEmpty(options.ScansPath))
            {
                scansPath = options.ScansPath;
            }

            var reader = ScanReaderFactory.GetReader(scansPath);
            reader.Open(scansPath, new ScanReaderOptions());
            var scans = new ScanCache(reader);

            var peptideParser = new PeptideParser(ascoreOptions.DiffMods);

            var aScore = new AScoreCalculator(ascoreOptions);
            if (!string.IsNullOrEmpty(ascoreOptions.Peptide))
            {
                var peptide = peptideParser.Parse(ascoreOptions.Peptide);
                var scan = scans.GetScan(peptide.ScanNumber);
                AScoreSingle(aScore, peptide, scan, ascoreOptions);
            }
            else
            {
                AScoreMulti(aScore, scans, peptideParser, ascoreOptions);
            }
        }

        /// <summary>
        /// Runs Ascore on a single peptide and prints the JSON to stdout
        /// </summary>
        /// <param name="aScore">Instance of AScore</param>
        /// <param name="peptide">The peptide to score</param>
        /// <param name="scan">The scan where the peptide is identified.</param>
        /// <param name="options">AScore options</param>
        private static void AScoreSingle(AScoreCalculator aScore, Peptide peptide, Scan scan, AScoreOptions options)
        {
            var result = aScore.Run(peptide, scan);

            var writer = new JsonOutputWriter(Console.OpenStandardOutput(), options);
            writer.Write(result);
        }

        /// <summary>
        /// Runs AScore on all peptides given in the csv file and
        /// writes the output file.
        /// </summary>
        /// <param name="aScore">Instance of AScore used on all peptides</param>
        /// <param name="scans">Collection of scans</param>
        /// <param name="parser">Unserializes peptides from the text input</param>
        /// <param name="options">AScore options</param>
        private static void AScoreMulti(AScoreCalculator aScore, ScanCache scans, PeptideParser parser, AScoreOptions options)
        {
            var stream = new FileStream(options.Out, FileMode.Create, FileAccess.Write); 
            using (var writer = new StreamWriter(stream))
            {
                // Header
                writer.Write("ID\tModsScored\tPeptidesScored\tPeptide\tScore");

                // Flatten output. Show up to six mods.
                for (int i = 0; i < 6; ++i) {
                    writer.Write("\tSitePosition{0}\tSiteScore{0}", i + 1, i + 1);
                }
                writer.Write("\n");

                var peptides = new PeptidesFile(options.PeptidesFile);
                foreach (PeptidesFileEntry entry in peptides)
                {
                    var peptide = parser.Parse(entry.Peptide);
                    peptide.PrecursorMz = entry.PrecursorMz;
                    peptide.Id = entry.Id;
                    peptide.ScanNumber = entry.ScanNumber;
                    var scan = scans.GetScan(peptide.ScanNumber);
                    var result = aScore.Run(peptide, scan);

                    Peptide topPeptide = result.Peptides[0];
                    writer.Write(topPeptide.Id);
                    writer.Write("\t");
                    writer.Write(result.ModCount);
                    writer.Write("\t");
                    writer.Write(result.Peptides.Count);
                    writer.Write("\t");
                    writer.Write(topPeptide.ToString());
                    writer.Write("\t");
                    writer.Write(topPeptide.Score.ToString("F9"));

                    // Flatten output. Show up to six mods.
                    for (int i = 0; i < 6; ++i) {
                        if (i < result.Sites.Count) {
                            var site = result.Sites[i];
                            writer.Write("\t");
                            writer.Write(site.Position);
                            writer.Write("\t");
                            writer.Write(site.Score.ToString("F9"));
                        }
                        else {
                            writer.Write("\t\\N\t\\N");
                        }
                    }
                    writer.Write("\n");
                }
            }
        }

    }
}
