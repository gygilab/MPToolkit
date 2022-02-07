
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MPToolkit.Common.Data
{
    /// <summary>
    /// Defines common mass constants.
    /// </summary>
    public static class Mass
    {
        public enum Units : int
        {
            PPM = 1,
            DALTON = 2
        }

        public enum IonSeries : int {
            A_NEUTRAL_LOSS = 1,
            B_NEUTRAL_LOSS = 2,
            Y_NEUTRAL_LOSS = 4,

            A_IONS = 8,
            B_IONS = 16,
            C_IONS = 32,

            D_IONS = 64,
            V_IONS = 128,
            W_IONS = 256,

            X_IONS = 512,
            Y_IONS = 1024,
            Z_IONS = 2048
        }

        /// <summary>
        /// Proton mass = Hydrogen - e
        /// </summary>
        public const double Proton = 1.007276466879000;

        /// <summary>
        /// Netural Hydrogen mass
        /// </summary>
        public const double Hydrogen = 1.0078250321;

        public const double Electron = 0.000548579867;

        public const double Nitrogen = 14.0030740052;

        public const double Carbon = 12.0000000;

        public const double Oxygen = 15.9949146221;

        /// <summary>
        /// Mass difference between isotopes of peptides when
        /// assuming "Averagine" amino acid masses.
        /// </summary>
        public const double AVERAGINE_DIFF = 1.00286864;

        /// <summary>
        /// Standard unmodified AA masses.
        /// </summary>
        private static readonly double[] AAMasses = {
            71.03711381, // A
            0, // B
            103.0091845, // C
            115.0269431, // D
            129.0425931, // E
            147.0684139, // F
            57.02146374, // G
            137.0589119, // H
            113.084064, // I
            0, // J
            128.0949631, // K
            113.084064, // L
            131.0404847, // M
            114.0429275, // N
            0, // O
            97.05276388, // P
            128.0585775, // Q
            156.1011111, // R
            87.03202844, // S
            101.0476785, // T
            0, // U
            99.06841395, // V
            186.079313, // W
            113.084064, // X
            163.0633286, // Y
            0, // Z
        };

        /// <summary>
        /// Returns an unmodified amino acid mass
        /// </summary>
        /// <param name="aa">The uppercase character representing the amino acid</param>
        /// <returns>Mass</returns>
        public static double GetAminoAcidMass(char aa)
        {
            return AAMasses[((int) aa) - 65];
        }
    }
}
