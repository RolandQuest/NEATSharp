using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// A loose network of connections that are all considered before activation.
    /// Input and output ordering is determined by the List of Genes passed into constructor.
    /// </summary>
    public class Network
    {
        /// <summary>
        /// A list of connections in the network.
        /// </summary>
        public List<Connection> Connections = new List<Connection>();
        
        /// <summary>
        /// A dictionary of sensor/bias nodes in the network.
        /// </summary>
        public Dictionary<int, Node> SensorNodes = new Dictionary<int, Node>();

        /// <summary>
        /// A dictionary of hidden nodes in the network.
        /// </summary>
        public Dictionary<int, Node> HiddenNodes = new Dictionary<int, Node>();

        /// <summary>
        /// A dictionary of output nodes in the network.
        /// </summary>
        public Dictionary<int, Node> OutputNodes = new Dictionary<int, Node>();

        /// <summary>
        /// A flag to show if activation has occurred enough times that at least one output node is not zero.
        /// Original NEAT outputsoff()
        /// </summary>
        public bool Initialized
        {
            get
            {
                return OutputNodes.Where(x => x.Value.InputValue != 0).Count() != 0;
            }
        }

        /// <summary>
        /// Constructs the network using the implied topography.
        /// </summary>
        /// <param name="allGenes">A list of all the genes in the genome.</param>
        /// <param name="sensorNodeIndices">An ordered list of sensor nodes needed.</param>
        /// <param name="outputNodeIndices">An ordered list of output nodes needed.</param>
        public Network(List<Gene> allGenes, Dictionary<int, NodeInformation> allNodes)
        {
            //TODO: Fix comments.
            foreach(var node in allNodes)
            {
                if (node.Value.IsInput())
                {
                    SensorNodes.Add(node.Key, new Node(node.Key, node.Value));
                }
                else if(node.Value.Type == NodeType.OUTPUT)
                {
                    OutputNodes.Add(node.Key, new Node(node.Key, node.Value));
                }
            }
            
            for (int i = 0; i < allGenes.Count; i++)
            {
                Gene gene = allGenes[i];

                if(gene.Frozen)
                {
                    continue;
                }

                ConnectionInformation link = gene.link;
                Node from = null;
                Node to = null;
                
                foreach(var node in SensorNodes)
                {
                    if(node.Key == link.InNode)
                    {
                        from = node.Value;
                    }
                    if (node.Key == link.OutNode)
                    {
                        to = node.Value;
                    }
                }
                foreach (var node in HiddenNodes)
                {
                    if (node.Key == link.InNode)
                    {
                        from = node.Value;
                    }
                    if (node.Key == link.OutNode)
                    {
                        to = node.Value;
                    }
                }
                foreach (var node in OutputNodes)
                {
                    if (node.Key == link.InNode)
                    {
                        from = node.Value;
                    }
                    if (node.Key == link.OutNode)
                    {
                        to = node.Value;
                    }
                }

                if(from == null)
                {
                    from = new Node(link.InNode, allNodes[link.InNode]);
                    HiddenNodes.Add(from.Id, from);
                }
                if(to == null)
                {
                    to = new Node(link.OutNode, allNodes[link.OutNode]);

                    if (!HiddenNodes.Select(x => x.Key).ToList().Contains(to.Id))
                    {
                        HiddenNodes.Add(to.Id, to);
                    }
                }

                Connections.Add(new Connection(from, to, gene.Weight));
            }
        }
        
        /// <summary>
        /// Loads multiple sensors at once.
        /// </summary>
        /// <param name="inputs">A list of nodeId-value pairings.</param>
        public void LoadSensors(Dictionary<int,double> inputs)
        {
            foreach(var input in inputs)
            {
                LoadSensor(input);
            }
        }

        /// <summary>
        /// Loads a single sensor.
        /// If the given nodeId does not map to a NodeType.SENSOR, it will be ignored (no error).
        /// </summary>
        /// <param name="input">A nodeId-value pairing.</param>
        public void LoadSensor(KeyValuePair<int,double> input)
        {
            if (SensorNodes.Select(x => x.Key).Contains(input.Key))
            {
                if(SensorNodes[input.Key].Type != NodeType.BIAS)
                {
                    SensorNodes[input.Key].InputValue = input.Value;
                    SensorNodes[input.Key].Activate();
                }
            }
        }
        
        /// <summary>
        /// Activates the network for the already set input.
        /// </summary>
        /// <returns>An ordered list of output vales.</returns>
        public Dictionary<int, double> Activate()
        {
            ClearNodeInputValues();
            foreach(var connection in Connections)
            {
                connection.OutNode.InputValue += connection.InNode.OutputValue * connection.Weight;
            }
            ActivateNodes();

            return GetOutputValues();
        }

        /// <summary>
        /// Returns a list of the output values of the output nodes in the network.
        /// </summary>
        /// <returns>A list of double values.</returns>
        public Dictionary<int,double> GetOutputValues()
        {
            Dictionary<int, double> ret = new Dictionary<int, double>();
            foreach(var node in OutputNodes)
            {
                ret.Add(node.Key, node.Value.OutputValue);
            }
            return ret;
        }

        /// <summary>
        /// Activates network until at least one output node is not zero.
        /// </summary>
        public void Initialize()
        {
            while(!Initialized)
            {
                Activate();
            }
        }

        /// <summary>
        /// Runs the activation functions over all hidden/output nodes.
        /// </summary>
        private void ActivateNodes()
        {
            foreach (var node in HiddenNodes)
            {
                node.Value.Activate();
            }
            foreach (var node in OutputNodes)
            {
                node.Value.Activate();
            }
        }

        /// <summary>
        /// Sets non-input nodes InputValues to zero.
        /// </summary>
        private void ClearNodeInputValues()
        {
            foreach (var node in HiddenNodes)
            {
                node.Value.InputValue = 0.0;
            }
            foreach (var node in OutputNodes)
            {
                node.Value.InputValue = 0.0;
            }
        }

    }
}
