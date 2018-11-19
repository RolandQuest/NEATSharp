using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    public static class Const
    {
        /// <summary>
        /// When a population has surpassed SpeciesStagnationEpochCycle many generations,
        /// the lowest performing species older than or equal to OverTheHillThresh will be greatly punished.
        /// </summary>
        public static int SpeciesStagnationEpochCycle = 30;

        /// <summary>
        /// When the population has stagnated this many generations something is wrong and we perform special actions.
        /// </summary>
        public static int PopulationDropoffGenerations = 20;

        /// <summary>
        /// When a population has surpassed GenerationStagnationCount many generations,
        /// the lowest performing species older than or equal to OverTheHillThresh will be greatly punished.
        /// </summary>
        public static int OverTheHillThresh = 20;

        /// <summary>
        /// If a species does not improve performance for this many generations,
        /// then it should be considered stagnant.
        /// </summary>
        public static int SpeciesStagnationAge = 10;

        /// <summary>
        /// All species equal to or under this age should be considered young
        /// and given rewards for being young and beautiful.
        /// </summary>
        public static int AgeOfYouthThresh = 10;

        /// <summary>
        /// The coefficient of increasing a genomes fitness score based on the age of the species.
        /// </summary>
        public static double YouthRewardCoefficient = 1.0;

        /// <summary>
        /// Percent of a species that will be able to reproduce.
        /// </summary>
        public static double SurvivalThresh = 0.20;

        /// <summary>
        /// The champion of each species will survive the generation
        /// if the species size is greather than or equal to this value.
        /// </summary>
        public static int ChampSurvivalThresh = 6;

        /// <summary>
        /// The chance a child will be nothing more than the mutation of a single parent.
        /// </summary>
        public static double MutateOnlyProbability = 0.25;

        /// <summary>
        /// The chance a species will breed with another species.
        /// </summary>
        public static double InterSpeciesBreedingProbability = 0.001;

        /// <summary>
        /// The chance a child of mating will not mutate as well.
        /// </summary>
        public static double MateOnlyProbability = 0.20;
        
        /// <summary>
        /// All connection weights |x| are inclusively bounded by MaximumAbsoluteConnectionWeight.
        /// </summary>
        public static double MaximumAbsoluteConnectionWeight = 8.0;

    }
}
