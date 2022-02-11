
using System;
using System.IO;
using System.Text.Json;

namespace MPToolkit.AScore.Interface
{
    /// <summary>
    /// Defines a writer that serializes the output to Json.
    /// </summary>
    public class JsonOutputWriter
    {
        StreamWriter Writer;

        AScoreOptions Options;

        /// <summary>
        /// Initializes the Json output.  This also
        /// accepts the AScore options used for this batch of peptides
        /// </summary>
        /// <param name="stream">output stream</param>
        /// <param name="options">options used</param>
        public JsonOutputWriter(Stream stream, AScoreOptions options)
        {
            Writer = new StreamWriter(stream);
            Options = options;
        }

        /// <summary>
        /// Write the results for a single peptide to the output stream.
        /// </summary>
        public void Write(AScoreOutput output)
        {
            Writer.Write(JsonSerializer.Serialize(output));
        }
    }
}
