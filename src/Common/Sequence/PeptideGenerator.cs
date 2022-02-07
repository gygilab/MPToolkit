
using MPToolkit.Common.Data;
using MPToolkit.Common.Math;
using System.Collections.Generic;
using System.Text;

namespace MPToolkit.Common.Sequence
{
    public class PeptideGenerator
    {
        private double NeutralLossMass;

        private string NeutralLossResidues;

        private bool UseNeutralLoss = false;

        /// <summary>
        /// Stores the mass at each AA position
        /// </summary>
        private List<double> AAMasses;

        /// <summary>
        /// Stores the masses at each AA position plus any
        /// modification masses.
        /// </summary>
        private List<double> ModMasses;

        /// <summary>
        /// Each list is a distinct combination of mods.
        /// Values specify the position to apply the mods.
        /// </summary>
        private List<List<int>> Combinations;

        /// <summary>
        /// Peptide to generate mod combinations.
        /// </summary>
        private Peptide BasePeptide;

        /// <summary>
        /// The index of the generated mod combinations.
        /// </summary>
        private int Index;

        /// <summary>
        /// Maximum number of mod combinations to generate.
        /// </summary>
        private int MaxPeptides = 1000;

        private PeptideMod TargetMod;

        public PeptideGenerator(Peptide peptide, PeptideMod targetMod)
        {
            BasePeptide = peptide.Clone();
            TargetMod = targetMod;
            Init();
        }

        private void Init()
        {
            // Count number of target mod.
            // Remove existing target mods.
            int modCount = 0;
            var mods = new PeptideMods();
            foreach (PeptideMod mod in BasePeptide.Mods)
            {
                if (mod.Symbol == TargetMod.Symbol)
                {
                    ++modCount;
                }
                else
                {
                    mods.Add(mod.Position, mod);
                }
            }
            BasePeptide.Mods = mods;

            // Find possible positions.
            string sequence = BasePeptide.Sequence;
            var modPositions = new List<int>(sequence.Length);
            for (int i = 0; i < sequence.Length; ++i)
            {
                if (TargetMod.Applies(sequence[i]))
                {
                    modPositions.Add(i);
                }
            }
            var generator = new CombinationIterator();
            Combinations = new List<List<int>>(MaxPeptides);
            foreach (var subset in generator.Generate(modPositions, modCount)) {
                Combinations.Add(subset);
                if (Combinations.Count >= MaxPeptides) {
                    break;
                }
            }
            Index = 0;

            // Initialize mass list, including non-target mods.
            ModMasses = new List<double>(BasePeptide.Sequence.Length);
            for (int i = 0; i < sequence.Length; ++i)
            {
                char aa = sequence[i];
                ModMasses.Add(Mass.GetAminoAcidMass(aa));

            }
            foreach (var mod in BasePeptide.Mods)
            {
                ModMasses[mod.Position] += mod.Mass;
            }
            AAMasses = new List<double>(ModMasses);
        }

        public bool AtEnd()
        {
            return Index >= Combinations.Count;
        }

        public void Next()
        {
            ++Index;
        }

        public void SetNeutralLossMod(double mass, string residues)
        {
            NeutralLossMass = mass;
            NeutralLossResidues = residues;
            UseNeutralLoss = true;
        }

        public void SetIndex(int i)
        {
            Index = i;
        }

        /// <summary>
        /// Returns the fragment ions for the peptide with the current
        /// arrangement of mods.
        /// </summary>
        /// <param name="ionSeries">The selected ion series as flags</param>
        /// <param name="maxCharge">Max fragment charge to consider (inclusive).</param>
        /// <param name="minMz">min m/z for fragment ions</param>
        /// <param name="maxMz">max m/z for fragment ions</param>
        /// <returns>list of peaks sorted by m/z in ascending order</returns>
        public List<Centroid> GetMassList(int ionSeries, int maxCharge, double minMz, double maxMz)
        {
            for (int i = 0; i < ModMasses.Count; ++i)
            {
                ModMasses[i] = AAMasses[i];
            }

            foreach (int pos in Combinations[Index])
            {
                ModMasses[pos] += TargetMod.Mass;
            }

            string sequence = BasePeptide.Sequence;

            // Keep track of positions that can produce neutral loss fragments.
            var posCanNL = new List<bool>(ModMasses.Count);
            for (int i = 0; i < sequence.Length; ++i)
            {
                posCanNL.Add(false);
                for (int j = 0; UseNeutralLoss && j < NeutralLossResidues.Length; ++j)
                {
                    if (sequence[i] == NeutralLossResidues[j])
                    {
                        posCanNL[i] = true;
                        break;
                    }
                }
            }

            // Calculate a, b, and c ion series
            double aMass = 0;
            double bMass = Mass.Proton;
            double cMass = Mass.Nitrogen + (3 * Mass.Hydrogen) - Mass.Electron;

            Mass.IonSeries series = (Mass.IonSeries)ionSeries;
            var output = new List<Centroid>();
            bool fragCanNL = false;
            for (int i = 0; i < ModMasses.Count; ++i)
            {
                double residueMass = ModMasses[i];
                bMass += residueMass;
                cMass += residueMass;
                aMass = bMass - Mass.Carbon - Mass.Oxygen;

                fragCanNL |= posCanNL[i];

                for (int j = 1; j <= maxCharge; ++j)
                {
                    if (series.HasFlag(Mass.IonSeries.A_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(aMass, j));
                    }
                    if (series.HasFlag(Mass.IonSeries.B_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(bMass, j));
                    }
                    if (series.HasFlag(Mass.IonSeries.C_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(cMass, j));
                    }

                    // Fragments after neutral loss
                    if (UseNeutralLoss && fragCanNL)
                    {
                        if (series.HasFlag(Mass.IonSeries.A_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(aMass + NeutralLossMass, j));
                        }
                        if (series.HasFlag(Mass.IonSeries.B_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(bMass + NeutralLossMass, j));
                        }
                        if (series.HasFlag(Mass.IonSeries.C_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(cMass + NeutralLossMass, j));
                        }
                    }
                }
            }

            // Calculate x, y, and z ion series.
            fragCanNL = false;
            double xMass = 0;
            double yMass = (3 * Mass.Hydrogen) + Mass.Oxygen - Mass.Electron;
            double zMass = 2.99966565 - Mass.Electron;

            for (int i = (sequence.Length - 1); i > 0; --i)
            {
                double residueMass = ModMasses[i];
                yMass += residueMass;
                zMass += residueMass;
                xMass = yMass - Mass.Carbon - Mass.Oxygen;

                fragCanNL |= posCanNL[i];

                for (int j = 1; j <= maxCharge; ++j)
                {
                    if (series.HasFlag(Mass.IonSeries.X_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(xMass, j));
                    }
                    if (series.HasFlag(Mass.IonSeries.Y_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(yMass, j));
                    }
                    if (series.HasFlag(Mass.IonSeries.Z_IONS))
                    {
                        InsertInRange(output, minMz, maxMz, ionMz(zMass, j));
                    }

                    if (UseNeutralLoss && fragCanNL)
                    {
                        if (series.HasFlag(Mass.IonSeries.X_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(xMass + NeutralLossMass, j));
                        }
                        if (series.HasFlag(Mass.IonSeries.Y_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(yMass + NeutralLossMass, j));
                        }
                        if (series.HasFlag(Mass.IonSeries.Z_IONS))
                        {
                            InsertInRange(output, minMz, maxMz, ionMz(zMass + NeutralLossMass, j));
                        }
                    }
                }
            }

            output.Sort((a, b) => a.Mz.CompareTo(b.Mz));

            return output;
        }

        /// <summary>
        /// Returns the peptide with the current arrangement of mods.
        /// </summary>
        /// <returns></returns>
        public Peptide GetPeptide()
        {
            Peptide output = BasePeptide.Clone();
            output.GeneratorIndex = Index;
            for (int i = Combinations[Index].Count - 1; i >= 0; --i)
            {
                output.Mods.Add(Combinations[Index][i], TargetMod);
            }
            return output;
        }

        private double ionMz(double mass, int charge)
        {
            return (mass + ((charge - 1) * Mass.Proton)) / charge;
        }

        private void InsertInRange(List<Centroid> ions, double minMz, double maxMz, double mz)
        {
            if (mz > minMz && mz < maxMz)
            {
                ions.Add(new Centroid()
                {
                    Mz = mz,
                    Intensity = 1
                });
            }
        }
    }
}
