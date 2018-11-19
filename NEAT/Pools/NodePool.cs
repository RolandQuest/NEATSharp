using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Pool for tracking NodeInformation by Id.
    /// The position in the Pool is the Id.
    /// </summary>
    public static class NodePool
    {
        private static List<NodeInformation> Pool = new List<NodeInformation>();

        /// <summary>
        /// Adds a new node to the pool.
        /// </summary>
        /// <param name="nodeInfo">The information of the node added.</param>
        /// <returns>The unique identifier of the node.</returns>
        public static int Add(NodeInformation nodeInfo)
        {
            Pool.Add(nodeInfo);
            return Size() - 1;
        }
        
        /// <summary>
        /// Gets the information for a node.
        /// </summary>
        /// <param name="id">The Id of the node.</param>
        /// <returns>The information corresponding to that node.</returns>
        public static NodeInformation FindById(int id)
        {
            return Pool[id];
        }
        
        /// <summary>
        /// The number of nodes in the pool.
        /// </summary>
        /// <returns>The size of the pool.</returns>
        public static int Size()
        {
            return Pool.Count;
        }
        
    }
}
