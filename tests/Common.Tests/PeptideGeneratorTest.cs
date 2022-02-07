
using System;
using System.Collections.Generic;
using Xunit;

using MPToolkit.Common.Math;
using MPToolkit.Common.Sequence;

namespace MPToolkit.Common.Sequence
{
    public class PeptideGeneratorTest
    {
        [Fact]
        public void SingleModTest()
        {
            Peptide peptide = new Peptide() {
                Sequence = "MSLTK"
            };
            PeptideMod mod = new PeptideMod() {
                Mass = 79.98,
                Residues = "STY",
                Symbol = '@'
            };
            var generator = new PeptideGenerator(peptide, mod);
        }

        [Fact]
        public void DoubleModTest()
        {
            Peptide peptide = new Peptide() {
                Sequence = "MSLTGYK"
            };
            PeptideMod mod = new PeptideMod() {
                Mass = 79.98,
                Residues = "STY",
                Symbol = '@'
            };
            var generator = new PeptideGenerator(peptide, mod);
        }
    }
}
