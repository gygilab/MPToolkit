
using MPToolkit.Common.Data;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MPToolkit.AScore.Interface {
    public class AScoreOptionsReader {

        /// <summary>
        /// Parses Json options
        /// </summary>
        /// <param name="optionStream">The input stream used to read the Json</param>
        /// <returns>The unserialized options.</returns>
        public static AScoreOptions Read(System.IO.Stream optionStream) {
            var reader = new StreamReader(optionStream);
            string json = reader.ReadToEnd();

            AScoreOptions options = JsonSerializer.Deserialize<AScoreOptions>(json);
            if (options.UnitText == "ppm") {
                options.Units = Mass.Units.PPM;
            }
            else {
                options.Units = Mass.Units.DALTON;
            }
            var ionSeriesOptions = new Dictionary<string, int>() {
                { "nA", 1 },
                { "nB", 2 },
                { "nY", 4 },
                { "a", 8 },
                { "b", 16 },
                { "c", 32 },
                { "d", 64 },
                { "v", 128 },
                { "w", 256 },
                { "x", 512 },
                { "y", 1024 },
                { "z", 2048 }
            };
            options.IonSeries = 0;
            foreach (string series in options.IonSeriesList) {
                options.IonSeries += ionSeriesOptions[series];
            }

            foreach (var mod in options.StaticMods) {
                if (!string.IsNullOrEmpty(mod.Residues)) {
                    foreach (char c in mod.Residues) {
                        options.Masses.ModifyAminoAcidMass(c, mod.Mass);
                    }
                }
                if (mod.IsNTerm) {
                    options.Masses.ModifyNTermMass(mod.Mass);
                }
                if (mod.IsCTerm) {
                    options.Masses.ModifyCTermMass(mod.Mass);
                }
            }
            return options;
        }
    }
}
