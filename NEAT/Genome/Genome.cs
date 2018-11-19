using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// A full collection of connections.
    /// </summary>
    public class Genome
    {
        /// <summary>
        /// The Gene connections in the network.
        /// </summary>
        public List<Gene> Genes { get; set; }

        /// <summary>
        /// Gets the personal fitness info of the Genome.
        /// </summary>
        public GenomeFitnessInformation Fitness { get; set; } = new GenomeFitnessInformation();

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="genes">The connection genes of the genome.</param>
        public Genome(List<Gene> genes)
        {
            Genes = genes;
        }

        /// <summary>
        /// Copy constructor. Gene weight values are copied, but reference is new.
        /// </summary>
        /// <param name="genome">The genome to copy.</param>
        public Genome(Genome genome)
        {
            Genes = new List<Gene>();
            foreach(var gene in genome.Genes)
            {
                Genes.Add(new Gene(gene));
            }
        }

        /// <summary>
        /// Constructs a neural network based on the genes.
        /// </summary>
        /// <returns>A network this genome represents.</returns>
        public Network GetPhenotype()
        {
            return new Network(Genes, GetAllNodeInformation(false));
        }
        
        /// <summary>
        /// The number of genes in the genome.
        /// </summary>
        /// <returns>The number of genes in the genome.</returns>
        public int Size()
        {
            return Genes.Count;
        }

        /// <summary>
        /// Sorts the genes by their historical Id markings (innovation numbers in old NEAT).
        /// </summary>
        public void SortGenesByConnectionId()
        {
            Genes.Sort((a, b) => a.Id.CompareTo(b.Id));
        }

        /// <summary>
        /// Retrieves the node information for all nodes in the genome.
        /// </summary>
        /// <returns>A dictionary of node id / node information pairs.</returns>
        public Dictionary<int, NodeInformation> GetAllNodeInformation(bool excludeFrozen)
        {
            List<Gene> genesToUse = Genes;
            if (excludeFrozen)
            {
                genesToUse = genesToUse.Where(x => !x.Frozen).ToList();
            }

            Dictionary<int, NodeInformation> IdSet = new Dictionary<int, NodeInformation>();
            
            foreach (var gene in genesToUse)
            {
                if (!IdSet.ContainsKey(gene.link.InNode))
                {
                    IdSet.Add(gene.link.InNode, NodePool.FindById(gene.link.InNode));
                }
                if (!IdSet.ContainsKey(gene.link.OutNode))
                {
                    IdSet.Add(gene.link.OutNode, NodePool.FindById(gene.link.OutNode));
                }
            }

            return IdSet;
        }

        /// <summary>
        /// Checks if a connection already exists within the genome.
        /// </summary>
        /// <param name="inny">The input node of the connection to test.</param>
        /// <param name="outty">The output node of the connection to test.</param>
        /// <returns>True if a gene configuration is the same.</returns>
        public bool ContainsConnection(int inny, int outty)
        {
            foreach(var gene in Genes)
            {
                if(gene.link.IsSameAs(inny, outty))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Validates the genome's configuration.
        /// Every connection must be connected to an input and output node.
        /// </summary>
        /// <param name="excludeFrozen">Exclude frozen genes from validation (true network).</param>
        /// <returns>True if the genome's configuration is valid.</returns>
        public bool Validate(bool excludeFrozen)
        {
            foreach(var gene in Genes)
            {
                if (!ValidateGene(gene, excludeFrozen))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates a single gene.
        /// </summary>
        /// <param name="gene">The gene to validate.</param>
        /// <param name="excludeFrozen">Exclude frozen genes from validation (true network).</param>
        /// <returns>True if the gene is connected to an input and output node.</returns>
        private bool ValidateGene(Gene gene, bool excludeFrozen)
        {
            List<Gene> checkedGenes = new List<Gene>();
            if (!ValidateGeneForward(gene, checkedGenes, excludeFrozen))
            {
                return false;
            }
            checkedGenes = new List<Gene>();
            if (!ValidateGeneBackward(gene, checkedGenes, excludeFrozen))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates that a gene is connected to an output node.
        /// </summary>
        /// <param name="gene">The gene being validated.</param>
        /// <param name="pathsChecked">A list of genes that have been traversed in the recursion.</param>
        /// <param name="excludeFrozen">Exclude frozen genes from validation (true network).</param>
        /// <returns>True if the gene is connected to an output node.</returns>
        private bool ValidateGeneForward(Gene gene, List<Gene> pathsChecked, bool excludeFrozen)
        {
            pathsChecked.Add(gene);

            if(NodePool.FindById(gene.link.OutNode).Type == NodeType.OUTPUT)
            {
                return true;
            }

            foreach(var path in Genes.Where(x => x.link.InNode == gene.link.OutNode && !pathsChecked.Contains(x)))
            {
                if (excludeFrozen && path.Frozen)
                {
                    continue;
                }
                if(ValidateGeneForward(path, pathsChecked, excludeFrozen))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates that a gene is connected to an input node.
        /// </summary>
        /// <param name="gene">The gene being validated.</param>
        /// <param name="pathsChecked">A list of genes that have been traversed in the recursion.</param>
        /// <param name="excludeFrozen">Exclude frozen genes from validation (true network).</param>
        /// <returns>True if the gene is connected to an input node.</returns>
        private bool ValidateGeneBackward(Gene gene, List<Gene> pathsChecked, bool excludeFrozen)
        {
            NodeInformation origin = NodePool.FindById(gene.link.InNode);

            pathsChecked.Add(gene);

            if (origin.IsInput())
            {
                return true;
            }

            foreach (var path in Genes.Where(x => x.link.OutNode == gene.link.InNode && !pathsChecked.Contains(x)))
            {
                if (excludeFrozen && path.Frozen)
                {
                    continue;
                }
                if (ValidateGeneBackward(path, pathsChecked, excludeFrozen))
                {
                    return true;
                }
            }

            return false;
        }
        
    }
}
