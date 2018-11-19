using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// An enumeration of different ways to mutate a genome.
    /// </summary>
    public enum MutationStyle
    {
        AddNode = 0,
        AddConnection = 1,
        TryAllNonStructural = 2
    }

    /// <summary>
    /// Helper functions for mutating genomes.
    /// </summary>
    public static class Mutation
    {
        /// <summary>
        /// Chance to disturb all connection weights.
        /// </summary>
        public static double MutateConnectionWeightsProbability = 0.90;

        /// <summary>
        /// Chance to mutate all connection weights to a higher degree.
        /// </summary>
        public static double MutateConnectionWeightsSeverelyProbability = 0.50;

        /// <summary>
        /// Chance to change the state of a connections Freeze.
        /// </summary>
        public static double MutateToggleFreezeProbability = 0.25;

        /// <summary>
        /// Chance to unfreeze a random connection.
        /// </summary>
        public static double MutateUnFreezeProbability = 0.25;
        
        /// <summary>
        /// Holds relative weighted chance of mutating a certain style.
        /// </summary>
        public static Dictionary<MutationStyle, int> MutationStyleWeights = new Dictionary<MutationStyle, int>()
        {
            {MutationStyle.AddNode, 3 },
            {MutationStyle.AddConnection, 5 },
            {MutationStyle.TryAllNonStructural, 92 },
        };

        /// <summary>
        /// Chooses a random mutation style based on the weights of each style.
        /// </summary>
        /// <param name="rando">A random number generator.</param>
        /// <returns>A random mutation style. MutationStyle.TryAllNonStructural by default.</returns>
        public static MutationStyle ChooseMutationStyle(Random rando)
        {
            int total = 0;
            foreach (var item in MutationStyleWeights)
            {
                total += item.Value;
            }
            int pos = rando.Next(total);

            foreach (var style in MutationStyleWeights)
            {
                pos -= style.Value;
                if (pos < 0)
                {
                    return style.Key;
                }
            }

            return MutationStyle.TryAllNonStructural;
        }

        /// <summary>
        /// Randomly mutates a genome.
        /// This is a conglomerate of mutations.
        /// </summary>
        /// <param name="genome">The genome to mutate.</param>
        /// <param name="innovationsSeen">A list of previously seen innovations.</param>
        /// <param name="rando">A random number generator.</param>
        public static void Mutate(Genome genome, InnovationPool innovationsSeen, Random rando)
        {
            MutationStyle style = ChooseMutationStyle(rando);

            Genome original = new Genome(genome);

            switch (style)
            {
                case MutationStyle.AddNode:
                    MutateAddNode(genome, innovationsSeen, rando);
                    break;
                case MutationStyle.AddConnection:
                    MutateAddConnection(genome, innovationsSeen, rando);
                    break;
                case MutationStyle.TryAllNonStructural:
                    MutateTryAllNonStructural(genome, rando);
                    break;
                default:
                    throw new Exception("Mutate did not catch a valid MutationStyle.");
            }
        }
        
        /// <summary>
        /// Mutates a genome by breaking a connection up into two separate connections.
        /// </summary>
        /// <param name="genome">The genome to be modified.</param>
        /// <param name="innovationsSeen">A list of previously seen innovations.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateAddNode(Genome genome, InnovationPool innovationsSeen, Random rando)
        {
            
            List<Gene> possibleConnections = new List<Gene>(genome.Genes);
            possibleConnections.RemoveAll(x => x.Frozen || NodePool.FindById(x.link.InNode).Type == NodeType.BIAS);

            if(possibleConnections.Count == 0)
            {
                return;
            }

            //TODO: Note in original algorithm saying uniform distribution is not optimal here.
            Gene geneToSplit = possibleConnections[rando.Next(possibleConnections.Count)];
            geneToSplit.Frozen = true;
            
            ActivationStyle style = ActivationFunctions.ChooseActivationStyle(rando);
            InnovationInformation innovation = new InnovationInformation(geneToSplit.link, style);

            int firstConId = -1;
            int secondConId = -1;

            int registeredInnovationId = innovationsSeen.FindByInnovation(innovation);

            if (registeredInnovationId == -1)
            {
                int newNodeId = NodePool.Add(new NodeInformation(NodeType.HIDDEN, style));

                ConnectionInformation firstConnect = new ConnectionInformation(geneToSplit.link.InNode, newNodeId);
                firstConId = ConnectionPool.Add(firstConnect);

                ConnectionInformation secondConnect = new ConnectionInformation(newNodeId, geneToSplit.link.OutNode);
                secondConId = ConnectionPool.Add(secondConnect);
                
                innovation.NewNodeDetails.NewNodeId = newNodeId;
                innovation.NewNodeDetails.FirstConnectionId = firstConId;
                innovation.NewNodeDetails.SecondConnectionId = secondConId;
                innovationsSeen.Add(innovation);
            }
            else
            {
                InnovationInformation registeredInnovation = innovationsSeen.FindById(registeredInnovationId);
                firstConId = registeredInnovation.NewNodeDetails.FirstConnectionId;
                secondConId = registeredInnovation.NewNodeDetails.SecondConnectionId;
            }
            
            genome.Genes.Add(new Gene(ConnectionPool.FindById(firstConId), firstConId, 1.0, false));
            genome.Genes.Add(new Gene(ConnectionPool.FindById(secondConId), secondConId, geneToSplit.Weight, false));
        }

        /// <summary>
        /// Mutates a given genome by adding a connection.
        /// Connection is guaranteed to not be 'into' a sensor or bias.
        /// </summary>
        /// <param name="genome">The genome to be modified.</param>
        /// <param name="innovationsSeen">A list of previously seen innovations.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateAddConnection(Genome genome, InnovationPool innovationsSeen, Random rando)
        {
            //TODO: I'm getting the node information, but I only need that to construct allNodesNotInput...
            
            Dictionary<int, NodeInformation> allNodes = genome.GetAllNodeInformation(true);
            Dictionary<int, NodeInformation> allNodesNotInput = new Dictionary<int, NodeInformation>(allNodes);

            //TODO: Witnessed a bug where allNodes.Count == 0.
            //      Could be a node where there are only frozen links connecting.

            foreach (var id in allNodesNotInput.Where(kvp => kvp.Value.IsInput()).ToList())
            {
                allNodesNotInput.Remove(id.Key);
            }

            //TODO: Gotta be a better way than a tryCount...
            int tryCount = 0;
            int nodeFromId = -1;
            int nodeToId = -1;
            while (tryCount < 20)
            {
                nodeFromId = allNodes.Keys.ToList()[rando.Next(allNodes.Count)];
                nodeToId = allNodesNotInput.Keys.ToList()[rando.Next(allNodesNotInput.Count)];
                
                if(!genome.ContainsConnection(nodeFromId, nodeToId))
                {
                    break;
                }

                tryCount++;
            }

            if(tryCount == 20)
            {
                return;
            }
            
            ConnectionInformation connectInfo = new ConnectionInformation(nodeFromId, nodeToId);
            InnovationInformation innovation = new InnovationInformation(connectInfo);

            int connectId = -1;
            //TODO: Pull inital weight setting out of here.
            double weight = rando.NextDouble() * 2.0 - 1.0;
            
            int registeredInnovationId = innovationsSeen.FindByInnovation(innovation);

            if (registeredInnovationId == -1)
            {
                connectId = ConnectionPool.Add(connectInfo);
                
                innovation.NewConnectionDetails.ConnectionId = connectId;
                innovationsSeen.Add(innovation);
            }
            else
            {
                connectId = innovationsSeen.FindById(registeredInnovationId).NewConnectionDetails.ConnectionId;
            }
            
            genome.Genes.Add(new Gene(ConnectionPool.FindById(connectId), connectId, weight, false));
        }

        /// <summary>
        /// Goes through and attempts to mutate all non-structural mutations.
        /// </summary>
        /// <param name="genome">The genome to mutate.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateTryAllNonStructural(Genome genome, Random rando)
        {
            if(rando.NextDouble() < MutateConnectionWeightsProbability)
            {
                MutateConnectionWeights(genome, rando);
            }
            if(rando.NextDouble() < MutateToggleFreezeProbability)
            {
                MutateToggleFreeze(genome, rando);
            }
            if(rando.NextDouble() < MutateUnFreezeProbability)
            {
                MutateUnFreeze(genome, rando);
            }
        }

        /// <summary>
        /// Mutates connection weights for every gene.
        /// </summary>
        /// <param name="genome">The genome to be mutated.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateConnectionWeights(Genome genome, Random rando)
        {
            
            //TODO: Implement severeness.
            //TODO: Implement cold/hot Gaussian

            foreach(var gene in genome.Genes.Where(x => !x.Frozen).ToList())
            {

                //TODO: Extract decision making. Dependency Injection.
                
                if(rando.Next(2) == 0)
                {
                    gene.Weight = rando.NextDouble() * 2.50 * Const.MaximumAbsoluteConnectionWeight - Const.MaximumAbsoluteConnectionWeight;
                }
                else
                {
                    double delta = rando.NextDouble() * 2.50 - 1.0;
                    gene.Weight += delta;
                }
                
                gene.Weight = Math.Max(-Const.MaximumAbsoluteConnectionWeight, gene.Weight);
                gene.Weight = Math.Min(Const.MaximumAbsoluteConnectionWeight, gene.Weight);
            }

        }

        /// <summary>
        /// Toggles the frozen status of a single gene if possible.
        /// </summary>
        /// <param name="genome">The genome to be mutated.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateToggleFreeze(Genome genome, Random rando)
        {
            int geneId = rando.Next(genome.Size());
            Gene gene = genome.Genes[geneId];

            gene.Frozen = !gene.Frozen;
            
            if (!genome.Validate(true))
            {
                gene.Frozen = !gene.Frozen;
            }
            
        }

        /// <summary>
        /// Unfreezes a random frozen gene if possible.
        /// </summary>
        /// <param name="genome">The genome to be mutated.</param>
        /// <param name="rando">A random number generator.</param>
        public static void MutateUnFreeze(Genome genome, Random rando)
        {
            List<Gene> possibilities = genome.Genes.Where(x => x.Frozen).ToList();

            if(possibilities.Count == 0)
            {
                return;
            }
            
            int possibleId = rando.Next(possibilities.Count);
            Gene gene = possibilities[possibleId];

            gene.Frozen = false;
            
            if (!genome.Validate(true))
            {
                gene.Frozen = !gene.Frozen;
            }
            
        }


        

    }
}
