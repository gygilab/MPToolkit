using CommandLine;
using System;
using System.Collections.Generic;

namespace MPToolkit.AScore.Interface
{
    /// <summary>
    /// Class for processing CLI input arguments
    /// </summary>
    public class CliOptionsParser
    {
        /// <summary>
        /// Parse user input arguments
        /// </summary>
        public AScoreCliOptions Parse(string[] args)
        {
            AScoreCliOptions output = new AScoreCliOptions();
            Parser.Default.ParseArguments<AScoreCliOptions>(args)
                .WithParsed(opt => { output = opt; })
                .WithNotParsed(HandleParseError);
            return output;
        }

        /// <summary>
        /// Handle and report errors in arguments
        /// </summary>
        private void HandleParseError(IEnumerable<Error> Errors)
        {
            List<string> errors = new List<string>();
            foreach(Error error in Errors)
            {
                if(error.Tag != ErrorType.VersionRequestedError && error.Tag != ErrorType.HelpRequestedError)
                {
                    errors.Add(error.Tag.ToString());
                }
            }
            throw new Exception(String.Join("\n", errors));
        }
    }
}
