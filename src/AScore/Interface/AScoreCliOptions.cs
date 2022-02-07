using CommandLine;

namespace MPToolkit.AScore.Interface
{
    /// <summary>
    /// Defines options that can be passed in through the CLI.
    /// </summary>
    public class AScoreCliOptions
    {
        [Option('j', "parameters", Required = true, HelpText = "Parameter file in json format with AScore options.")]
        public string ParametersPath { get; set; } = "";

        [Option('p', "peptides", HelpText = "CSV of peptides to score.")]
        public string PeptidesPath { get; set; } = "";

        [Option('s', "scans", HelpText = "Scans file.")]
        public string ScansPath { get; set; } = "";
    }
}
