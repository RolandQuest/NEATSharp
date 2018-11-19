using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Represents information involving the fitness of the individual.
    /// </summary>
    public class GenomeFitnessInformation
    {

        /// <summary>
        /// The fitness score of the individual.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The fitness score after environmental factors are applied.
        /// </summary>
        public double AdjustedScore { get; set; }
    }
}
