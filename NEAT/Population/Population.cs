using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Represents a single generation in the scheme of time.
    /// </summary>
    public class Population
    {
        /// <summary>
        /// The targeted population size of each generation.
        /// </summary>
        public int TargetPopulationSize;
        
        /// <summary>
        /// Tracks all innovations since the beginning of time.
        /// HistoricalInnovations are only stored from GenerationalInnovations at end of Epoch.
        /// </summary>
        public InnovationPool HistoricalInnovations { get; private set; } = new InnovationPool();

        /// <summary>
        /// Tracks innovations used in the creation of the next population (not this population).
        /// The initial set of innovations are stored here.
        /// </summary>
        public InnovationPool GenerationalInnovations { get; private set; } = new InnovationPool();
        
        /// <summary>
        /// All species in the population. 
        /// </summary>
        public List<Species> Species { get; set; } = new List<Species>();

        /// <summary>
        /// What generation this population is in the grand scheme of things.
        /// </summary>
        public int Generation { get; set; } = 0;

        /// <summary>
        /// The generation that last scored the highest out of all genomes in the population.
        /// </summary>
        public int GenerationPrime { get; set; } = 0;

        /// <summary>
        /// Population level fitness information.
        /// </summary>
        public PopulationFitnessInformation Fitness { get; set; } = new PopulationFitnessInformation();
        
        /// <summary>
        /// Creates an initial population.
        /// </summary>
        /// <param name="nodes">A list of NodeType-ActivationStyle pairing. The node Id will be the position in the list.</param>
        /// <param name="connections">A list of InNodeId-OutNodeId-Weight tuples.</param>
        /// <param name="populationSize">The size of the population to test.</param>
        /// <param name="rando">A random number generator for perturbing the weights.</param>
        public Population(List<Tuple<NodeType,ActivationStyle>> nodes, List<Tuple<int,int,double>> connections, int populationSize, Random rando)
        {
            TargetPopulationSize = populationSize;
            ValidateConstructorParameters(nodes.Count, connections);

            for(int i = 0; i < nodes.Count; i++)
            {
                NodePool.Add(new NodeInformation(nodes[i].Item1, nodes[i].Item2));
            }

            List<Gene> startGenes = new List<Gene>();

            foreach(var tupe in connections)
            {
                ConnectionInformation ci = new ConnectionInformation(tupe.Item1, tupe.Item2);
                startGenes.Add(new Gene(ci, ConnectionPool.Size(), tupe.Item3, false));

                InnovationInformation info = new InnovationInformation(ci);
                info.NewConnectionDetails.ConnectionId = ConnectionPool.Size();
                GenerationalInnovations.Add(info);

                ConnectionPool.Add(ci);
            }

            Genome adam = new Genome(startGenes);
            List<Genome> firstGen = new List<Genome>() { adam };
            
            for(int i = 1; i < TargetPopulationSize; i++)
            {
                Genome copy = new Genome(adam);
                Mutation.MutateTryAllNonStructural(copy, rando);
                firstGen.Add(copy);
            }

            SpeciateNewGeneration(firstGen);
        }

        /// <summary>
        /// Validates the input parameters of the population object.
        /// </summary>
        /// <param name="initialNodeCount">The number of nodes defined initially.</param>
        /// <param name="connections">All of the connections defined.</param>
        private void ValidateConstructorParameters(int initialNodeCount, List<Tuple<int, int, double>> connections)
        {
            List<Tuple<int, int, double>> connectionsSeen = new List<Tuple<int, int, double>>();

            foreach(var tupe in connections)
            {
                if(tupe.Item1 >= initialNodeCount || tupe.Item2 >= initialNodeCount)
                {
                    string message = @"Population parameter 'connections' had undefined nodeId. (" + tupe.Item1 + " -> " + tupe.Item2 + ")";
                    message += "\nLargest nodeId defined was " + (initialNodeCount - 1);
                    throw new Exception(message);
                }
                else if (connectionsSeen.Contains(tupe))
                {
                    string message = "Population parameter 'connections' had repeat connections. (" + tupe.Item1 + " -> " + tupe.Item2 + ")";
                    throw new Exception(message);
                }
                else
                {
                    connectionsSeen.Add(tupe);
                }
            }
        }

        /// <summary>
        /// The current size of the population.
        /// </summary>
        /// <returns>Sum of all genomes across all species.</returns>
        public int Size()
        {
            int pop = 0;
            foreach(var spec in Species)
            {
                pop += spec.Size();
            }
            return pop;
        }

        /// <summary>
        /// The number of species in the population.
        /// </summary>
        /// <returns>Does not take into account species size.</returns>
        public int SpeciesCount()
        {
            return Species.Count;
        }
        
        /// <summary>
        /// Gets a flat list of all genomes in the population.
        /// </summary>
        /// <returns>List of all genomes in population.</returns>
        public List<Genome> GetAllGenomes()
        {
            List<Genome> allGenomes = new List<Genome>();
            foreach(var spec in Species)
            {
                allGenomes.AddRange(spec.Genomes);
            }
            return allGenomes;
        }

        /// <summary>
        /// Sorts the species by average fitness.
        /// </summary>
        public void SortSpeciesByAverageAdjustedFitness()
        {
            Species.Sort((a, b) => a.Fitness.AverageAdjustedScore.CompareTo(b.Fitness.AverageAdjustedScore));
        }

        /// <summary>
        /// Sorts the species by fitness of champion.
        /// </summary>
        public void SortSpeciesByChampionFitness()
        {
            Species.Sort((a, b) => a.Fitness.ChampionScore.CompareTo(b.Fitness.ChampionScore));
        }

        /// <summary>
        /// Handles iterating a generation of the population.
        /// All evaluations must have been done by this point.
        /// </summary>
        /// <param name="rando">A random number generator.</param>
        public void Epoch(Random rando)
        {
            foreach (var spec in Species)
            {
                spec.SetChampions();
            }
            
            SortSpeciesByChampionFitness();
            
            Genome generationChampion = Species.Last().Fitness.Champion;
            if (generationChampion.Fitness.Score >= Fitness.ChampionScore)
            {
                GenerationPrime = Generation;
            }
            Fitness.GenerationalChampion = generationChampion;
            
            Dictionary<Species, int> ExpectedChildren = FindExpectedChildren();
            
            foreach (var spec in Species)
            {
                int numberOfParents = (int) Math.Floor(spec.Size() * Const.SurvivalThresh + 1);
                spec.SortGenomesByFitness();
                spec.Genomes.RemoveRange(0, spec.Size() - numberOfParents);
            }

            List<Genome> nextGenerationGenomes = new List<Genome>();
            foreach(var specCountPair in ExpectedChildren)
            {
                nextGenerationGenomes.AddRange(specCountPair.Key.Reproduce(specCountPair.Value, GenerationalInnovations, Species, rando));
            }
            
            ClearParentGeneration();
            SpeciateNewGeneration(nextGenerationGenomes);
            
            RemoveDeadSpecies();
            ResetSpeciesTemplates();
            AgeSpecies();
            Generation++;
            HistoricalInnovations.AddRange(GenerationalInnovations);
            GenerationalInnovations.Clear();
        }

        /// <summary>
        /// Finds the expected children of each species.
        /// </summary>
        /// <returns>A dictionary of each species that will definitely have children.</returns>
        private Dictionary<Species,int> FindExpectedChildren()
        {
            Dictionary<Species, int> expectedChildren = new Dictionary<Species, int>();
            Species mostExpectingSpecies = null;
            int totalChildren = 0;

            //Original NEAT calls this delta-coding.
            //Original NEAT does not check this first. It overrides the 'else' statement logic. Seemed unneccessary.
            if (Generation - GenerationPrime >= Const.PopulationDropoffGenerations)
            {
                GenerationPrime = Generation;
                int halfPopulationSize = TargetPopulationSize / 2;

                if (Species.Count == 1)
                {
                    expectedChildren[Species.Last()] = TargetPopulationSize;
                    totalChildren = TargetPopulationSize;
                    Species.Last().AgePrime = Species.Last().Age;
                    mostExpectingSpecies = Species.Last();
                }
                else
                {
                    expectedChildren[Species.Last()] = halfPopulationSize;
                    Species.Last().AgePrime = Species.Last().Age;
                    mostExpectingSpecies = Species.Last();

                    expectedChildren[Species[Species.Count - 1]] = halfPopulationSize;
                    Species[Species.Count - 1].AgePrime = Species[Species.Count - 1].Age;

                    totalChildren = 2 * halfPopulationSize;
                }
            }
            else
            {
                if (Generation % Const.SpeciesStagnationEpochCycle == 0)
                {
                    foreach (var spec in Species)
                    {
                        if (spec.Age > Const.OverTheHillThresh)
                        {
                            spec.OnDeathBed = true;
                            break;
                        }
                    }
                }

                double sumOfTotalAdjustedScore = 0.0;
                foreach (var spec in Species)
                {
                    spec.AdjustFitness();
                    sumOfTotalAdjustedScore += spec.Fitness.TotalAdjustedScore;
                }

                int mostExpected = 0;
                foreach (var spec in Species)
                {
                    int children = 0;

                    //Warning: There is no check in the original NEAT that handles this case.
                    if (sumOfTotalAdjustedScore != 0)
                    {
                        children = (int)(spec.Fitness.TotalAdjustedScore * TargetPopulationSize / sumOfTotalAdjustedScore);
                    }

                    if(children > 0)
                    {
                        expectedChildren.Add(spec, children);
                        totalChildren += children;
                    }

                    if(children > mostExpected)
                    {
                        mostExpectingSpecies = spec;
                        mostExpected = children;
                    }
                }
            }
            
            if(totalChildren > 0)
            {
                expectedChildren[mostExpectingSpecies] += TargetPopulationSize - totalChildren;
            }
            else
            {
                expectedChildren.Add(Species.Last(), TargetPopulationSize);
            }
            
            return expectedChildren;
        }
        
        /// <summary>
        /// Clears out the entirety of the current generation.
        /// </summary>
        private void ClearParentGeneration()
        {
            foreach(var spec in Species)
            {
                spec.Genomes = new List<Genome>();
            }
        }

        /// <summary>
        /// Sorts next generation into their respective species.
        /// </summary>
        /// <param name="nextGen">The next generation.</param>
        private void SpeciateNewGeneration(List<Genome> nextGen)
        {
            foreach(var child in nextGen)
            {
                bool speciesFound = false;
                foreach(var spec in Species)
                {
                    if(GenomeCompatibility.Compatibility(spec.Template, child) <= GenomeCompatibility.SpeciesCompatibilityThreshold)
                    {
                        spec.Genomes.Add(child);
                        speciesFound = true;
                        break;
                    }
                }

                if (!speciesFound)
                {
                    //TODO: Find a better way of doing age.
                    Species spec = new Species(child);
                    spec.Age = -1;
                    Species.Add(spec);
                }
            }

        }

        /// <summary>
        /// Sets the Template of each species to the first genome in the list.
        /// </summary>
        private void ResetSpeciesTemplates()
        {
            foreach (var spec in Species)
            {
                spec.Template = spec.Genomes.First();
            }
        }

        /// <summary>
        /// Increments the age of each species.
        /// </summary>
        private void AgeSpecies()
        {
            foreach (var spec in Species)
            {
                spec.Age++;
            }
        }

        /// <summary>
        /// Removes all species where there are no members.
        /// </summary>
        private void RemoveDeadSpecies()
        {
            Species.RemoveAll(x => x.Size() == 0);
        }
    }
}
