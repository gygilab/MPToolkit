
using System.Collections.Generic;

namespace MPToolkit.Common.Data.Filter
{
    /// <summary>
    /// Filters for top ions per m/z window.
    /// </summary>
    public class TopIonsFilter : IScanFilter
    {
        private int Depth;

        private double Window;

        public TopIonsFilter(int depth, double window) {
            Depth = depth;
            Window = window;
        }

        /// <summary>
        /// This filter separates the scan into several m/z windows and
        /// within each window ranks the peaks by intensity.  Peaks ranking below
        /// the input peak depth are then removed.
        /// </summary>
        public void Filter(Scan scan)
        {
            List<Centroid> peaks = scan.Centroids;
            if (peaks.Count == 0)
            {
                return;
            }

            // Rank peaks in each window based on intensity.
            int start = 0;
            int i = 0;
            double currentWindowMin = System.Math.Floor(peaks[start].Mz / Window) * Window;
            double currentWindowMax = currentWindowMin + Window;
            while (true)
            {
                if (i == peaks.Count || peaks[i].Mz > currentWindowMax)
                {
                    // Found the m/z range of a window.
                    // Sort and rank window based on intensity.
                    peaks.Sort(start, i - start, Comparer<Centroid>.Create(
                        (a, b) => b.Intensity.CompareTo(a.Intensity))
                    );
                    int rank = 1;
                    for (int j = start; j != i; ++j) {
                        peaks[j] = new Centroid() {
                            Mz = peaks[j].Mz,
                            Intensity = peaks[j].Intensity,
                            Baseline = peaks[j].Baseline,
                            Noise = peaks[j].Noise,
                            Charge = peaks[j].Charge,
                            Rank = rank++
                        };
                    }

                    if (i == peaks.Count)
                    {
                        break;
                    }

                    start = i;
                    currentWindowMin = System.Math.Floor(peaks[start].Mz / Window) * Window;
                    currentWindowMax = currentWindowMin + Window;
                }
                ++i;
            }

            // Return to sorting based on m/z
            peaks.Sort((a, b) => a.Mz.CompareTo(b.Mz));

            // Save only top n.
            var newPeaks = new List<Centroid>(peaks.Count);
            foreach (var peak in peaks)
            {
                if (peak.Rank <= Depth)
                {
                    newPeaks.Add(peak);
                }
            }

            scan.Centroids = newPeaks;
        }
    }
}
