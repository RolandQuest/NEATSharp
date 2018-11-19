using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{

    /// <summary>
    /// Represents the position/role of the node in the network.
    /// </summary>
    public enum NodeType
    {
        SENSOR = 0,
        HIDDEN = 1,
        OUTPUT = 2,
        BIAS = 3
    }

    /// <summary>
    /// Represents the characteristics of a Node in the network.
    /// </summary>
    public class NodeInformation
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// The function used in activation.
        /// </summary>
        public ActivationStyle Style { get; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="type">The type of the node.</param>
        /// <param name="aFun">The function to be used in activation.</param>
        public NodeInformation(NodeType type, ActivationStyle style)
        {
            Type = type;
            Style = style;
        }
        
        /// <summary>
        /// Checks if the nodes are configured the same way.
        /// </summary>
        /// <param name="nodeInfo">The NodeInformation to compare to.</param>
        /// <returns>True if the configuration is the same.</returns>
        public bool IsSameAs(NodeInformation nodeInfo)
        {
            return Type == nodeInfo.Type && Style == nodeInfo.Style;
        }

        /// <summary>
        /// Is the node an input node?
        /// </summary>
        /// <returns>True if the node is an input node.</returns>
        public bool IsInput()
        {
            return Type == NodeType.SENSOR || Type == NodeType.BIAS;
        }
        
    }
}
