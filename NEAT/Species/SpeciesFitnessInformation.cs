using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public class SpeciesFitnessInformation
    {
        private Genome _champion;
        private Genome _legend;

        /// <summary>
        /// The genome that scored the highest in the species at this time.
        /// </summary>
        public Genome Champion
        {
            get
            {
                return _champion;
            }
            set
            {
                _champion = value;
                ChampionScore = _champion.Fitness.Score;
            }
        }

        /// <summary>
        /// The highest fitness across all organisms in the species at this time.
        /// </summary>
        public double ChampionScore { get; private set; } = 0;

        /// <summary>
        /// The genome that scored the highest in the species across all time.
        /// </summary>
        public Genome Legend
        {
            get
            {
                return _legend;
            }
            set
            {
                _legend = value;
                LegendScore = _legend.Fitness.Score;
            }
        }

        /// <summary>
        /// The highest fitness across all organisms in the species across all time.
        /// </summary>
        public double LegendScore { get; private set; } = 0;

        /// <summary>
        /// Average of all adjusted scores for all genomes in species.
        /// </summary>
        public double AverageAdjustedScore { get; set; } = 0;

        /// <summary>
        /// The sum of all adjusted fitness scores across all genomes in the species.
        /// </summary>
        public double TotalAdjustedScore { get; set; } = 0;
        
    }
}
