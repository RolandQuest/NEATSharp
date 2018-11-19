using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Represents a group of genomes that are similar in nature.
    /// </summary>
    public class Species
    {
        /// <summary>
        /// Unique identifier for a species.
        /// (-1) implies it has not been set.
        /// TODO: Not used.
        /// </summary>
        public int Id { get; private set; } = -1;

        /// <summary>
        /// The genome to be compared against when determining a species.
        /// </summary>
        public Genome Template { get; set; }

        /// <summary>
        /// Use flag to mark for adjusted fitness penalties.
        /// </summary>
        public bool OnDeathBed { get; set; } = false;

        /// <summary>
        /// The number of generations this species has existed.
        /// </summary>
        public int Age { get; set; } = 0;

        /// <summary>
        /// The age in which the highest ever fitness was recorded for this species.
        /// </summary>
        public int AgePrime { get; set; } = 0;

        /// <summary>
        /// Fitness information pertaining to the species.
        /// </summary>
        public SpeciesFitnessInformation Fitness { get; set; } = new SpeciesFitnessInformation();

        /// <summary>
        /// The genomes in the species.
        /// </summary>
        public List<Genome> Genomes { get; set; } = new List<Genome>();

        /// <summary>
        /// Basic constructor. 
        /// </summary>
        /// <param name="g">The original genome ( the 'og' =] ).</param>
        public Species(Genome g)
        {
            Template = g;
            Genomes.Add(g);
        }

        /// <summary>
        /// Calculates the adjusted fitness of all genomes.
        /// Also syncs up species fitness information.
        /// </summary>
        public void AdjustFitness()
        {

            Fitness.TotalAdjustedScore = 0.0;

            foreach (var genome in Genomes)
            {
                GenomeFitnessInformation genomeFitness = genome.Fitness;
                genomeFitness.AdjustedScore = genomeFitness.Score;

                if((Age - AgePrime + 1 - Const.SpeciesStagnationAge >= 0) || OnDeathBed)
                {
                    genomeFitness.AdjustedScore *= 0.01;
                }
                if(Age <= Const.AgeOfYouthThresh)
                {
                    genomeFitness.AdjustedScore *= Const.YouthRewardCoefficient;
                }
                if(genomeFitness.AdjustedScore < 0)
                {
                    genomeFitness.AdjustedScore = 0;
                }

                genomeFitness.AdjustedScore /= Size();

                Fitness.TotalAdjustedScore += genomeFitness.AdjustedScore;
            }
            
            Fitness.AverageAdjustedScore = Fitness.TotalAdjustedScore / Size();
        }

        /// <summary>
        /// Syncs up champions/legends with new genomes.
        /// </summary>
        public void SetChampions()
        {
            SortGenomesByFitness();

            Fitness.Champion = Genomes.Last();
            if (Fitness.ChampionScore > Fitness.LegendScore)
            {
                AgePrime = Age;
                Fitness.Legend = Fitness.Champion;
            }
        }




        public List<Genome> Reproduce(int expectedChildren, InnovationPool seenInnovations, List<Species> interBredSpecies, Random rando)
        {
            List<Genome> children = new List<Genome>();
            bool champDone = false;

            while(expectedChildren > 0)
            {
                if (!champDone && expectedChildren >= Const.ChampSurvivalThresh)
                {
                    children.Add(Fitness.Champion);
                    champDone = true;
                }
                else if (rando.NextDouble() < Const.MutateOnlyProbability || expectedChildren == 1)
                {
                    Genome child = new Genome(Genomes[rando.Next(Size())]);
                    Mutation.Mutate(child, seenInnovations, rando);
                    children.Add(child);
                }
                else
                {
                    Genome mother = Genomes[rando.Next(Size())];
                    Genome father = null;

                    if(rando.NextDouble() < Const.InterSpeciesBreedingProbability)
                    {
                        Species spec = SelectOtherSpecies(interBredSpecies, rando);
                        father = spec.Fitness.Champion;
                    }
                    else
                    {
                        father = Genomes[rando.Next(Size())];
                    }
                    
                    Genome child = Mating.Mate(mother, father, rando);

                    //TODO: Add mother/father node checking criteria as well?
                    if(rando.NextDouble() >= Const.MateOnlyProbability ||
                        GenomeCompatibility.Compatibility(mother,father) == 0.0)
                    {
                        Mutation.Mutate(child, seenInnovations, rando);
                    }

                    children.Add(child);
                }

                expectedChildren--;
            }

            return children;
        }

        /// <summary>
        /// The size of the species.
        /// </summary>
        /// <returns>A count of the genomes</returns>
        public int Size()
        {
            return Genomes.Count;
        }

        /// <summary>
        /// Sorts species by fitness score.
        /// </summary>
        public void SortGenomesByFitness()
        {
            Genomes.Sort((a, b) => a.Fitness.Score.CompareTo(b.Fitness.Score));
        }

        /// <summary>
        /// Sorts species by adjusted fitness score.
        /// </summary>
        public void SortGenomesByAdjustedFitness()
        {
            Genomes.Sort((a, b) => a.Fitness.AdjustedScore.CompareTo(b.Fitness.AdjustedScore));
        }
        
        /// <summary>
        /// Returns a random species from the available list.
        /// </summary>
        /// <param name="fullList">A full list of available species to choose from.</param>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A random species from the list. If none available, returns this.</returns>
        private Species SelectOtherSpecies(List<Species> fullList, Random rando)
        {
            List<Species> availableOthers = new List<Species>(fullList);
            availableOthers.Remove(this);

            if(availableOthers.Count == 0)
            {
                return this;
            }

            //TODO: Implement something more sophisticated.
            return availableOthers[rando.Next(availableOthers.Count)];
        }
    }
}
