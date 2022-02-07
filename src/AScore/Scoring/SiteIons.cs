
using MPToolkit.Common.Data;
using System;
using System.Collections.Generic;

namespace MPToolkit.AScore.Scoring
{
    public class SiteIons
    {
        public static List<Centroid> Filter(List<Centroid> ions, List<Centroid> remove)
        {
            var output = new List<Centroid>();
            int j = 0;
            double delta = 1e-5;
            for (int i = 0; i < ions.Count; ++i)
            {
                while (j < remove.Count && remove[j].Mz < ions[i].Mz - delta)
                {
                    ++j;
                }
                if (j < remove.Count && Math.Abs(ions[i].Mz - remove[j].Mz) < delta)
                {
                    continue;
                }
                output.Add(
                    new Centroid()
                    {
                        Mz = ions[i].Mz,
                        Intensity = ions[i].Intensity
                    });
            }
            return output;
        }
    }
}
