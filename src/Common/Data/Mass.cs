
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

        public const double Neutron = 1.00286864;

        public const double Nitrogen = 14.0030740052;

        public const double Carbon = 12.0000000;

        public const double Oxygen = 15.9949146221;

        public const double Water = 18.010564686;

        /// <summary>
        /// Mass difference between isotopes of peptides when
        /// assuming "Averagine" amino acid masses.
        /// </summary>
        public const double AveragineDifference = 1.00286864;

    }
}
