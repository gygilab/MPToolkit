
namespace MPToolkit.Common.Sequence {
    /// <summary>
    /// Stores an instance of Amino Acid masses and defines
    /// methods to apply static modifiations to them.
    /// </summary>
    public class AminoAcidMasses {
        /// <summary>
        /// Standard unmodified AA masses.
        /// </summary>
        private double[] AAMasses = {
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
        /// Mass change to apply at peptide n-terminus
        /// </summary>
        private double NTermMass = 0;

        /// <summary>
        /// Mass change to apply at peptide c-terminus
        /// </summary>
        private double CTermMass = 0;

        /// <summary>
        /// Returns an unmodified amino acid mass
        /// </summary>
        /// <param name="aa">The uppercase character representing the amino acid</param>
        /// <returns>Mass</returns>
        public double GetAminoAcidMass(char aa)
        {
            return AAMasses[((int) aa) - 65];
        }

        /// <summary>
        /// Change amino acid mass by adding a modification
        /// </summary>
        /// <param name="aa">Uppercase character representing an amino acid</param>
        /// <param name="modMass">The modification mass to apply</param>
        public void ModifyAminoAcidMass(char aa, double modMass) {
            AAMasses[((int) aa) - 65] += modMass;
        }

        /// <summary>
        /// Returns mass change to apply at peptide n-terminus
        /// </summary>
        public double GetNTermMass() {
            return NTermMass;
        }

        /// <summary>
        /// Apply a mass modification to the peptide n-terminus
        /// </summary>
        public void ModifyNTermMass(double modMass) {
            NTermMass += modMass;
        }

        /// <summary>
        /// Returns mass change to apply at peptide c-terminus
        /// </summary>
        /// <returns></returns>
        public double GetCTermMass() {
            return CTermMass;
        }

        /// <summary>
        /// Apply a mass modification to the peptide c-terminus
        /// </summary>
        /// <param name="modMass"></param>
        public void ModifyCTermMass(double modMass) {
            CTermMass += modMass;
        }

        /// <summary>
        /// Copy the object
        /// </summary>
        public AminoAcidMasses Clone() {
            return (AminoAcidMasses) MemberwiseClone();
        }
    }
}
