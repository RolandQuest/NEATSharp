using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Represents a gene in a NEAT genome (a connection with properties).
    /// </summary>
    public class Gene
    {
        /// <summary>
        /// The connection this gene represents.
        /// </summary>
        public ConnectionInformation link;

        /// <summary>
        /// The weight of the connection.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// The unique identifier for this connection.
        /// In the original NEAT paper, this is called the innovation number.
        /// </summary>
        public int Id{ get; set; }

        /// <summary>
        /// Should this connection be considered or not for activation.
        /// </summary>
        public bool Frozen { get; set; }
        
        /// <summary>
        /// Does the connection loop to the same node.
        /// </summary>
        public bool SelfLoop
        {
            get
            {
                return link.InNode == link.OutNode;
            }
        }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="l">The Connection this gene represents.</param>
        /// <param name="id">The unique identifier for this connection gene.</param>
        /// <param name="frozen">Frozen status.</param>
        public Gene(ConnectionInformation l, int id, double weight, bool frozen)
        {
            link = l;
            Id = id;
            Weight = weight;
            Frozen = frozen;
        }

        /// <summary>
        /// Copy constructor. References are carried over.
        /// </summary>
        /// <param name="gene">The gene to be copied.</param>
        public Gene(Gene gene)
        {
            link = gene.link;
            Id = gene.Id;
            Weight = gene.Weight;
            Frozen = gene.Frozen;
        }

        public Gene()
        {
            link = null;
            Id = -1;
            Weight = 0;
            Frozen = true;
        }

        /// <summary>
        /// Copies the internals of another Gene instance.
        /// </summary>
        /// <param name="gene">The gene that will be copied.</param>
        public void Copy(Gene gene)
        {
            link = gene.link;
            Id = gene.Id;
            Weight = gene.Weight;
            Frozen = gene.Frozen;
        }

    }
}
