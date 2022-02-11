
using System;
using System.Collections.Generic;
using Xunit;

using MPToolkit.AScore.Interface;
using MPToolkit.Common.Sequence;

namespace MPToolkit.AScore
{
    public class PeptideParserTest
    {
        [Fact]
        public void SingleModTest()
        {
            var mods = new List<PeptideMod>() {
                new PeptideMod() {
                    Symbol = '#',
                    Residues = "STY",
                    Mass = 79.98
                },
                new PeptideMod() {
                    Symbol = '*',
                    Residues = "M",
                    Mass = 15.99
                }
            };
            var parser = new PeptideParser(mods);
            var peptide = parser.Parse("K.KEES#EES#DDDM*GFGLFD.-");
            Assert.Equal("KEESEESDDDMGFGLFD", peptide.Sequence);
            Assert.Equal('K', peptide.LeftFlank);
            Assert.Equal('-', peptide.RightFlank);
        }
    }
}
