using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Stores information about population fitness.
    /// </summary>
    public class PopulationFitnessInformation
    {
        private Genome _champion;
        private Genome _generationalChampion;

        /// <summary>
        /// The savior of the universe.
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
        /// The highest fitness across all organisms across all time.
        /// </summary>
        public double ChampionScore { get; private set; } = 0;

        /// <summary>
        /// The champion of the generation.
        /// </summary>
        public Genome GenerationalChampion
        {
            get
            {
                return _generationalChampion;
            }
            set
            {
                _generationalChampion = value;
                GenerationalChampionScore = _generationalChampion.Fitness.Score;

                if (GenerationalChampionScore >= ChampionScore)
                {
                    Champion = GenerationalChampion;
                }
            }
        }

        /// <summary>
        /// The highest fitness across all organisms across all time.
        /// </summary>
        public double GenerationalChampionScore { get; private set; } = 0;
    }
}
