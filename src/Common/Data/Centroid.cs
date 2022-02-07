using System;

namespace MPToolkit.Common.Data
{
    /// <summary>
    /// for pulling spectral information from API scans
    /// </summary>
    public struct Centroid
    {
        public Centroid(double mz, double intensity, double baseline=0, double noise=0)
        {
            Mz = mz;
            Intensity = intensity;
            Baseline = baseline;
            Noise = noise;
            Charge = 0;
            Rank = 0;
        }

        /// <summary>
        /// Centroid m/z
        /// </summary>
        public double Mz { get; set; }

        /// <summary>
        /// Baseline
        /// </summary>
        public double Baseline { get; set; }

        /// <summary>
        /// Centroid intensity
        /// </summary>
        public double Intensity { get; set; }

        /// <summary>
        /// Noise level read from the instrument.
        /// </summary>
        public double Noise { get; set; }

        /// <summary>
        /// Detected charge of the ion
        /// </sumary>
        public int Charge { get; set; }

        /// <summary>
        /// Intensity rank of the peak. Can be assigned
        /// withing a specific window inside the scan.
        /// </summary>
        public int Rank { get; set; }
    }
}
