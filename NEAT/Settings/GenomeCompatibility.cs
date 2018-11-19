using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Contains static functions and configuration for comparing genomes.
    /// </summary>
    public static class GenomeCompatibility
    {
        
        /// <summary>
        /// Effect excess genes have on compatibility.
        /// </summary>
        public static double ExcessCoefficient = 1.0;

        /// <summary>
        /// Effect disjoint genes have on compatibility.
        /// </summary>
        public static double DisjointCoefficient = 1.0;

        /// <summary>
        /// Effect the average weight difference has on compatibility.
        /// </summary>
        public static double AvgWeightDiffCoefficient = 0.4;

        /// <summary>
        /// The highest acceptable compatibility to be considered the same species.
        /// </summary>
        public static double SpeciesCompatibilityThreshold = 3.0;

        /// <summary>
        /// Threshold for size of genomes to start normalizing differences.
        /// </summary>
        public static int GenomeSizeNormalizationThresh = 20;

        /// <summary>
        /// Classic compatibility function as implemented in the original NEAT.
        /// </summary>
        /// <param name="p1">The first genome.</param>
        /// <param name="p2">The second genome.</param>
        /// <returns>A measure of how far apart the two Genomes are.</returns>
        public static double Compatibility(Genome p1, Genome p2)
        {
            p1.SortGenesByConnectionId();
            p2.SortGenesByConnectionId();

            int p1Gene = 0;
            int p2Gene = 0;
            int p1End = p1.Size();
            int p2End = p2.Size();
            int disjointCount = 0;
            int excessCount = 0;
            int commonGeneCount = 0;

            double totalWeightDifferences = 0;
            
            while (p1Gene != p1End || p2Gene != p2End)
            {
                if (p1Gene == p1End)
                {
                    p2Gene++;
                    excessCount++;
                }
                else if (p2Gene == p2End)
                {
                    p1Gene++;
                    excessCount++;
                }
                else
                {
                    Gene g1 = p1.Genes[p1Gene];
                    Gene g2 = p2.Genes[p2Gene];

                    if (g1.Id == g2.Id)
                    {
                        commonGeneCount++;
                        totalWeightDifferences += Math.Abs(g1.Weight - g2.Weight);
                        p1Gene++;
                        p2Gene++;
                    }
                    else if (g1.Id < g2.Id)
                    {
                        disjointCount++;
                        p1Gene++;
                    }
                    else
                    {
                        disjointCount++;
                        p2Gene++;
                    }
                }
            }

            int N = 1;
            if (p1End >= GenomeSizeNormalizationThresh || p2End >= GenomeSizeNormalizationThresh)
            {
                N = Math.Max(p1End, p2End);
            }

            double ret =
                DisjointCoefficient * disjointCount / N +
                ExcessCoefficient * excessCount / N +
                AvgWeightDiffCoefficient * (totalWeightDifferences / commonGeneCount);

            return ret;
        }

    }
}
