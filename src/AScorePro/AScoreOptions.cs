
using MPToolkit.Common.Data;
using MPToolkit.Common.Data.Filter;
using MPToolkit.Common.Sequence;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MPToolkit.AScore
{
    /// <summary>
    /// Stores options for mods that can generate neutral loss fragments
    /// </summary>
    public class NeutralLoss
    {
        /// <summary>
        /// The neutral loss change in mass for the fragment.
        /// </summary>
        [JsonPropertyName("mass")]
        public double Mass { get; set; }

        /// <summary>
        /// A string of modified residues that can generate neutral losses.
        /// e.g. "ST"
        /// </summary>
        [JsonPropertyName("residues")]
        public string Residues { get; set; }
    }

    /// <summary>
    /// Options for the AScore algorithm.
    /// </summary>
    public class AScoreOptions
    {
        /// <summary>
        /// The name of the file containing the scan information.
        /// The supported files are defined in MPToolkit.Common.File
        /// and the appropriate file reader is selected based on
        /// the file extension.
        /// </summary>
        /// <value></value>
        [JsonPropertyName("scans")]
        public string Scans { get; set; }

        /// <summary>
        /// List of fragment ion series to consider. Valid options include
        /// ["a", "b", "c", "x", "y", "z", "nA", "nB", "nY"]
        /// </summary>
        [JsonPropertyName("ion_series")]
        public List<string> IonSeriesList { get; set; }

        /// <summary>
        /// Stores the unserialzed ion series data as flags in this integer.
        /// </summary>
        public int IonSeries;

        /// <summary>
        /// List of Diff mod information.
        /// 
        /// Ex.
        /// {
        ///     "residues": "STY",
        ///     "symbol": "#",
        ///     "mass": 79.96633,
        ///     "n-term": false,
        ///     "c-term": false
        /// }
        /// </summary>
        [JsonPropertyName("diff_mods")]
        public List<PeptideMod> DiffMods { get; set; }

        /// <summary>
        /// List of static mod information.
        /// </summary>
        [JsonPropertyName("static_mods")]
        public List<PeptideMod> StaticMods { get; set; }

        /// <summary>
        /// Options for the modification that can generate neutral loss fragments.
        /// Ex.
        /// "neutral_loss": {
        ///   "mass": -97.9769,
        ///   "residues": "ST"
        /// },
        /// </summary>
        /// <value></value>
        [JsonPropertyName("neutral_loss")]
        public NeutralLoss NeutralLoss { get; set; }

        /// <summary>
        /// During scoring, the scan is filtered to a peak depth
        /// per m/z window.  Typically MaxPeakDepth is used, but this
        /// can be set to use a fixed peak depth during scoring.
        /// </summary>
        [JsonPropertyName("peak_depth")]
        public int PeakDepth { get; set; }

        /// <summary>
        /// Peak depths up to this value are used when scoring.
        /// </summary>
        [JsonPropertyName("max_peak_depth")]
        public int MaxPeakDepth { get; set; }

        /// <summary>
        /// Peak match tolerance. Used with Units option
        /// </summary>
        [JsonPropertyName("tolerance")]
        public double Tolerance { get; set; }

        /// <summary>
        /// Text read from the Unit Option.
        /// </summary>
        [JsonPropertyName("units")]
        public string UnitText { get; set; }

        /// <summary>
        /// Stores parsed unit selection from the option
        /// read into UnitText
        /// </summary>
        public Mass.Units Units;

        /// <summary>
        /// The m/z window size to use when filtering and
        /// scoring peaks in scan
        /// </summary>
        [JsonPropertyName("window")]
        public int Window { get; set; }

        /// <summary>
        /// When enabled, ions below 0.28 of the max m/z
        /// are not considered
        /// </summary>
        [JsonPropertyName("low_mass_cutoff")]
        public bool LowMassCutoff { get; set; }

        /// <summary>
        /// Scans are filtered to remove this fraction
        /// of its lowest intensity peaks.
        /// </summary>
        [JsonPropertyName("filter_low_intensity")]
        public double FilterLowIntensity { get; set; } = 0.25;

        public Deisotoping DeisotopingType { get; set; }

        /// <summary>
        /// When enabled. Modifications on the c-terminus
        /// of the peptide are not considered.
        /// </summary>
        [JsonPropertyName("no_cterm")]
        public bool NoCterm { get; set; }

        /// <summary>
        /// Enable to use MOB Scoring algorithm instead of the
        /// Original AScore algorithm.
        /// </summary>
        /// <value></value>
        [JsonPropertyName("use_mob_score")]
        public bool UseMobScore { get; set; }

        /// <summary>
        /// If true, the score is subtracted by the
        /// site score of the top two peptides when they are reversed.
        /// </summary>
        [JsonPropertyName("use_delta_ascore")]
        public bool UseDeltaAscore { get; set; }

        /// <summary>
        /// The symbol of the modification to score.
        /// The mod should still appear in the Diff mods option.
        /// </summary>
        [JsonPropertyName("symbol")]
        public char Symbol { get; set; }

        /// <summary>
        /// The residues to consider for the modification being scored.
        /// </summary>
        [JsonPropertyName("residues")]
        public string Residues { get; set; }

        /// <summary>
        /// output file path.
        /// </summary>
        [JsonPropertyName("out")]
        public string Out { get; set; }

        /// <summary>
        /// Max mod combinations to consider per peptide
        /// </summary>
        [JsonPropertyName("max_peptides")]
        public int MaxPeptides { get; set; }

        /// <summary>
        /// When scoring a single peptide, this is the precursor m/z
        /// of the peptide.
        /// </summary>
        [JsonPropertyName("mz")]
        public int Mz { get; set; }

        /// <summary>
        /// When scoring a single peptide, this stores the annotated sequence.
        /// </summary>
        /// <value></value>
        [JsonPropertyName("peptide")]
        public string Peptide { get; set; }

        /// <summary>
        /// When scoring a single peptide, this is the scan number.
        /// </summary>
        /// <value></value>
        [JsonPropertyName("scan")]
        public int Scan { get; set; }

        /// <summary>
        /// Path to the csv file containing the input list of peptides to score.
        /// This is used when scoring multiple peptides.
        /// </summary>
        [JsonPropertyName("peptides_file")]
        public string PeptidesFile { get; set; }

        /// <summary>
        /// Instace of amino acid masses with static mods applied.
        /// </summary>
        public AminoAcidMasses Masses = new AminoAcidMasses();

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        public AScoreOptions(AScoreOptions other)
        {
            Scans = other.Scans;
            IonSeries = other.IonSeries;
            DiffMods = new List<PeptideMod>(other.DiffMods);
            StaticMods = new List<PeptideMod>(other.StaticMods);
            NeutralLoss = other.NeutralLoss;
            PeakDepth = other.PeakDepth;
            MaxPeakDepth = other.MaxPeakDepth;
            Tolerance = other.Tolerance;
            Units = other.Units;
            Window = other.Window;
            LowMassCutoff = other.LowMassCutoff;
            FilterLowIntensity = other.FilterLowIntensity;
            DeisotopingType = other.DeisotopingType;
            NoCterm = other.NoCterm;
            UseMobScore = other.UseMobScore;
            UseDeltaAscore = other.UseDeltaAscore;
            Symbol = other.Symbol;
            Residues = other.Residues;
            Out = other.Out;
            MaxPeptides = other.MaxPeptides;
            Mz = other.Mz;
            Peptide = other.Peptide;
            Scan = other.Scan;
            PeptidesFile = other.PeptidesFile;
            Masses = other.Masses.Clone();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public AScoreOptions() { }
    }

}
