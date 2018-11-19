using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Represents a weighted connection in a neural network.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// The 'from' node.
        /// </summary>
        public Node InNode { get; set; }

        /// <summary>
        /// The 'to' node.
        /// </summary>
        public Node OutNode { get; set; }

        /// <summary>
        /// The weight of the connection.
        /// </summary>
        public double Weight { get; set; }
        
        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="inny">The 'from' node.</param>
        /// <param name="outty">The 'to' node.</param>
        /// <param name="weight">The weight of the connection.</param>
        public Connection(Node inny, Node outty, double weight)
        {
            InNode = inny;
            OutNode = outty;
            Weight = weight;
        }
    }
}
