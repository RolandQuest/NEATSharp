using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// A single node in a network.
    /// </summary>
    public class Node
    {

        protected double _inputValue = 0;

        /// <summary>
        /// Represents the position/role of the node in the network.
        /// </summary>
        public NodeType Type { get; set; }

        /// <summary>
        /// A unique ID to distinguish nodes of common types.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="id">A unique ID in the network.</param>
        /// <param name="type">The type of the node.</param>
        /// <param name="activationFunction">The function to be used for activation.</param>
        public Node(int id, NodeType type, Func<double,double> activationFunction)
        {
            Type = type;
            Id = id;
            ActivationFunction = activationFunction;

            if(Type == NodeType.BIAS)
            {
                InputValue = 1.0;
                OutputValue = 1.0;
            }
        }
        
        /// <summary>
        /// Creates node from NodeInformation.
        /// </summary>
        /// <param name="id">A unique ID in the network.</param>
        /// <param name="nodeInfo">Information of the node.</param>
        public Node(int id, NodeInformation nodeInfo)
        {
            Type = nodeInfo.Type;
            Id = id;
            ActivationFunction = ActivationFunctions.GetActivationFunction(nodeInfo.Style);

            if (Type == NodeType.BIAS)
            {
                InputValue = 1.0;
                OutputValue = 1.0;
            }
        }

        /// <summary>
        /// The accumulation of input from connections.
        /// </summary>
        public double InputValue
        {
            get
            {
                return _inputValue;
            }
            set
            {
                _inputValue = value;
                Activated = false;
            }
        }

        /// <summary>
        /// The value of the last activation.
        /// </summary>
        public double OutputValue { get; protected set; } = 0;

        /// <summary>
        /// True if the InputValue has not been changed since the last activation.
        /// </summary>
        public bool Activated { get; protected set; } = false;

        /// <summary>
        /// The function being used to activate the node.
        /// </summary>
        public Func<double, double> ActivationFunction { get; set; }
        
        /// <summary>
        /// Applies activation function to the input and derives output.
        /// </summary>
        public void Activate()
        {
            OutputValue = ActivationFunction(_inputValue);
            Activated = true;
        }

        
    }
}
