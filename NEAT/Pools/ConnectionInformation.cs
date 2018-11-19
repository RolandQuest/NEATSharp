using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// A connection in the neural network.
    /// </summary>
    public class ConnectionInformation
    {
        /// <summary>
        /// The 'from' node.
        /// </summary>
        public int InNode { get; private set; }
        
        /// <summary>
        /// The 'to' node.
        /// </summary>
        public int OutNode { get; private set; }
        
        /// <summary>
        /// Creates a connection between given nodes.
        /// </summary>
        /// <param name="inny">The input 'from' node.</param>
        /// <param name="outty">The 'to' node.</param>
        public ConnectionInformation(int inny, int outty)
        {
            InNode = inny;
            OutNode = outty;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="c">The connection to be copied.</param>
        public ConnectionInformation(ConnectionInformation c)
        {
            InNode = c.InNode;
            OutNode = c.OutNode;
        }

        /// <summary>
        /// Checks if the connections are configured the same way.
        /// </summary>
        /// <param name="connectionInfo">The ConnectionInformation to compare to.</param>
        /// <returns>True if the configuration is the same.</returns>
        public bool IsSameAs(ConnectionInformation connectionInfo)
        {
            return InNode == connectionInfo.InNode && OutNode == connectionInfo.OutNode;
        }

        /// <summary>
        /// Checks if the connections are configured the same way.
        /// </summary>
        /// <param name="inny">The input node of the connection to test.</param>
        /// <param name="outty">The output node of the connection to test.</param>
        /// <returns>True if the configuration is the same.</returns>
        public bool IsSameAs(int inny, int outty)
        {
            return InNode == inny && OutNode == outty;
        }
    }
}
