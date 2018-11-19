using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// An enumeration of different ways to mate two genomes.
    /// </summary>
    public enum MatingStyle
    {
        MultiPoint = 0,
        MultiPointAverage = 1,
        SinglePoint = 2
    }
    
    /// <summary>
    /// Helper functions for mating two genomes.
    /// </summary>
    public static class Mating
    {

        /// <summary>
        /// The probability that an inherited Gene is frozen if either parent is frozen.
        /// </summary>
        public static double FreezeGeneOfFrozenParentProbability = 0.75;

        /// <summary>
        /// The probability to select the stronger parent when selecting at random.
        /// </summary>
        public static double SelectRandomlyTheStronger = 0.50;

        /// <summary>
        /// Holds relative weighted chance of mating a certain style.
        /// </summary>
        public static Dictionary<MatingStyle, int> MatingStyleWeights = new Dictionary<MatingStyle, int>()
        {
            {MatingStyle.MultiPoint, 6 },
            {MatingStyle.MultiPointAverage, 4 },
            {MatingStyle.SinglePoint, 0 },
        };
        
        /// <summary>
        /// Chooses a random mating style based on the weights of each style.
        /// </summary>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A random mating style. SinglePoint by default.</returns>
        public static MatingStyle ChooseMatingStyle(Random rando)
        {
            int total = 0;
            foreach(var item in MatingStyleWeights)
            {
                total += item.Value;
            }
            int pos = rando.Next(total);

            foreach(var style in MatingStyleWeights)
            {
                pos -= style.Value;
                if(pos < 0)
                {
                    return style.Key;
                }
            }

            throw new Exception("ChooseMatingStyle probabably didn't have any positive weights.");
        }
        
        /// <summary>
        /// Returns a mated child of the mother and father genome.
        /// </summary>
        /// <param name="mother">A parent Genome.</param>
        /// <param name="father">A parent Genome.</param>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A new genome based on the mating configuration.</returns>
        public static Genome Mate(Genome mother, Genome father, Random rando)
        {
            MatingStyle style = ChooseMatingStyle(rando);
            Genome child;
            
            switch (style)
            {
                case MatingStyle.MultiPoint:
                case MatingStyle.MultiPointAverage:
                    child = MateMultiPoint(style, mother, father, rando);
                    break;
                case MatingStyle.SinglePoint:
                    child = MateSinglePoint(style, mother, father, rando);
                    break;
                default:
                    throw new Exception("Mate function did not find a valid MatingStyle.");
            }

            return child;
        }

        /// <summary>
        /// Logic for disjoint/excess genes between mother/father in multi-point mating styles.
        /// Delegates common genes to a more specific function.
        /// </summary>
        /// <param name="style">The specific type of multi-point mating style.</param>
        /// <param name="mother">A parent genome.</param>
        /// <param name="father">A parent genome.</param>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A product of the given multi-point mating style.</returns>
        private static Genome MateMultiPoint(MatingStyle style, Genome mother, Genome father, Random rando)
        {
            mother.SortGenesByConnectionId();
            father.SortGenesByConnectionId();

            Genome strongerParent = mother;
            Genome weakerParent = father;

            if (father.Fitness.Score > mother.Fitness.Score)
            {
                strongerParent = father;
                weakerParent = mother;
            }

            List<Gene> childGenes = new List<Gene>();
            int strongerIndex = 0;
            int weakerIndex = 0;

            while (strongerIndex < strongerParent.Size() || weakerIndex < weakerParent.Size())
            {
                Gene chosenGene = new Gene();

                if (strongerIndex == strongerParent.Size())
                {
                    break;
                }
                else if (weakerIndex == weakerParent.Size())
                {
                    chosenGene.Copy(strongerParent.Genes[strongerIndex]);
                    strongerIndex++;
                }
                else
                {
                    int strongerGeneId = strongerParent.Genes[strongerIndex].Id;
                    int weakerGeneId = weakerParent.Genes[weakerIndex].Id;

                    if (strongerGeneId == weakerGeneId)
                    {
                        switch (style)
                        {

                            case MatingStyle.MultiPoint:
                                MateMultiPointRegular(chosenGene, strongerParent.Genes[strongerIndex], weakerParent.Genes[weakerIndex], rando);
                                break;
                            case MatingStyle.MultiPointAverage:
                                MateMultiPointAverage(chosenGene, strongerParent.Genes[strongerIndex], weakerParent.Genes[weakerIndex], rando);
                                break;
                            default:
                                throw new Exception("MateMultiPoint was not given a valid mating style.");
                        }

                        //TODO: If both parents freeze their kid... should it be automatically frozen?
                        chosenGene.Frozen = false;
                        if (strongerParent.Genes[strongerIndex].Frozen && weakerParent.Genes[weakerIndex].Frozen)
                        {
                            chosenGene.Frozen = true;
                        }
                        else if (strongerParent.Genes[strongerIndex].Frozen || weakerParent.Genes[weakerIndex].Frozen)
                        {
                            if (rando.NextDouble() < FreezeGeneOfFrozenParentProbability)
                            {
                                chosenGene.Frozen = true;
                            }
                        }

                        strongerIndex++;
                        weakerIndex++;
                    }
                    else if (strongerGeneId < weakerGeneId)
                    {
                        chosenGene.Copy(strongerParent.Genes[strongerIndex]);
                        strongerIndex++;
                    }
                    else
                    {
                        weakerIndex++;
                        continue;
                    }
                }

                if (GeneIsUnique(chosenGene, childGenes))
                {
                    childGenes.Add(chosenGene);
                }

            }
            
            return new Genome(childGenes);

        }
        
        /// <summary>
        /// Common genes are selected at random.
        /// </summary>
        /// <param name="child">The Gene object where information will be stored.</param>
        /// <param name="stronger">The stronger of the two parents.</param>
        /// <param name="weaker">The weaker of the two parents.</param>
        /// <param name="rando">A random number generator.</param>
        private static void MateMultiPointRegular(Gene child, Gene stronger, Gene weaker, Random rando)
        {
            if (rando.NextDouble() < SelectRandomlyTheStronger)
            {
                child.Copy(stronger);
            }
            else
            {
                child.Copy(weaker);
            }

            child.Frozen = false;
        }

        /// <summary>
        /// Averages the weights of both genes.
        /// </summary>
        /// <param name="child">The Gene object where information will be stored.</param>
        /// <param name="stronger">The stronger of the two parents.</param>
        /// <param name="weaker">The weaker of the two parents.</param>
        /// <param name="rando">A random number generator.</param>
        private static void MateMultiPointAverage(Gene child, Gene stronger, Gene weaker, Random rando)
        {
            //The original NEAT algorithm had the InNode as well as the OutNode both being selected at random.
            //However, the link should be common to both at this time.
            child.link = stronger.link;
            child.Id = stronger.Id;
            child.Frozen = false;
            child.Weight = (stronger.Weight + weaker.Weight) / 2;
        }
        
        /// <summary>
        /// Yarr... this be copied almost exactly as the original NEAT.
        /// In the original NEAT, none of the settings had chance this would happen.
        /// </summary>
        /// <param name="style">The specific type of multi-point mating style.</param>
        /// <param name="mother">A parent genome.</param>
        /// <param name="father">A parent genome.</param>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A product of the given multi-point mating style.</returns>
        private static Genome MateSinglePoint(MatingStyle style, Genome mother, Genome father, Random rando)
        {
            mother.SortGenesByConnectionId();
            father.SortGenesByConnectionId();

            Genome largerParent = mother;
            Genome smallerParent = father;

            if (father.Size() > mother.Size())
            {
                largerParent = father;
                smallerParent = mother;
            }

            List<Gene> childGenes = new List<Gene>();
            int largerIndex = 0;
            int smallerIndex = 0;
            int crossoverPoint = rando.Next(smallerParent.Size());
            int crossoverCounter = 0;

            while (smallerParent.Size() != smallerIndex)
            {
                Gene chosenGene = new Gene();

                if (largerIndex == largerParent.Size())
                {
                    chosenGene.Copy(smallerParent.Genes[smallerIndex]);
                    smallerIndex++;
                }
                else if (smallerIndex == smallerParent.Size())
                {
                    //Should never happen
                    chosenGene.Copy(largerParent.Genes[largerIndex]);
                    largerIndex++;
                }
                else
                {
                    int largerGeneId = largerParent.Genes[largerIndex].Id;
                    int smallerGeneId = smallerParent.Genes[smallerIndex].Id;

                    if (largerGeneId == smallerGeneId)
                    {
                        if(crossoverCounter < crossoverPoint)
                        {
                            chosenGene.Copy(largerParent.Genes[largerIndex]);
                        }
                        else if(crossoverCounter > crossoverPoint)
                        {
                            chosenGene.Copy(smallerParent.Genes[smallerIndex]);
                        }
                        else
                        {
                            switch (style)
                            {
                                case MatingStyle.SinglePoint:
                                    //Function is equivalent to what is needed here despite the misleading name.
                                    MateMultiPointAverage(chosenGene, largerParent.Genes[largerIndex], smallerParent.Genes[smallerIndex], rando);
                                    break;
                                default:
                                    throw new Exception("MateSinglePoint was not given a valid mating style.");
                            }
                        }


                        chosenGene.Frozen = false;
                        //The NEAT implementation did not call for a chance this would happen. It just did it. [ NEAT.1.2.1 => Genome::mate_singlepoint(...) ]
                        if (largerParent.Genes[largerIndex].Frozen || smallerParent.Genes[smallerIndex].Frozen)
                        {
                            chosenGene.Frozen = true;
                        }

                        largerIndex++;
                        smallerIndex++;
                        crossoverCounter++;
                    }
                    else if (largerGeneId < smallerGeneId)
                    {
                        if(crossoverCounter < crossoverPoint)
                        {
                            chosenGene.Copy(largerParent.Genes[largerIndex]);
                            largerIndex++;
                            crossoverCounter++;
                        }
                        else
                        {
                            chosenGene.Copy(smallerParent.Genes[smallerIndex]);
                            smallerIndex++;
                        }
                    }
                    else
                    {
                        smallerIndex++;
                        continue;
                    }
                }

                if (GeneIsUnique(chosenGene, childGenes))
                {
                    childGenes.Add(chosenGene);
                }

            }

            return new Genome(childGenes);
        }
        
        /// <summary>
        /// Checks if given Gene matches another Gene is the provided list.
        /// </summary>
        /// <param name="gene">The gene to test.</param>
        /// <param name="takenGenes">The list of Genes to test against.</param>
        /// <returns>True if the InNode/OutNode pair does NOT match one in the list.</returns>
        private static bool GeneIsUnique(Gene gene, List<Gene> takenGenes)
        {
            foreach (var takenGene in takenGenes)
            {
                if (takenGene.link.InNode == gene.link.InNode && takenGene.link.OutNode == gene.link.OutNode)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
