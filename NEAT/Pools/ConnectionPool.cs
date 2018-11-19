using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Pool for tracking unique connections.
    /// The position in the Pool is the unique identifier.
    /// This identifier is called the innovation number in the original NEAT.
    /// </summary>
    public static class ConnectionPool
    {
        private static List<ConnectionInformation> Pool = new List<ConnectionInformation>();
        
        /// <summary>
        /// Adds a new connection to the pool. New historical topography.
        /// </summary>
        /// <param name="connectionInfo">The information of the new connection.</param>
        /// <returns>The connection Id of the newly added connection.</returns>
        public static int Add(ConnectionInformation connectionInfo)
        {
            Pool.Add(connectionInfo);
            return Size() - 1;
        }

        /// <summary>
        /// Gets the information pertaining to the connection.
        /// </summary>
        /// <param name="id">The unique identifier of the desired connection.</param>
        /// <returns>The connection configuration.</returns>
        public static ConnectionInformation FindById(int id)
        {
            return Pool[id];
        }

        /// <summary>
        /// Finds a connection in the pool matching the criteria.
        /// </summary>
        /// <param name="connectionInfo">The information to match.</param>
        /// <returns>The index of the matching connection. (-1) if nothing is found.</returns>
        public static int FindConnection(ConnectionInformation connectionInfo)
        {
            for(int i = 0; i < Pool.Count; i++)
            {
                if (connectionInfo.IsSameAs(Pool[i]))
                {
                    return i;
                }
            }

            return -1;
        }
        
        /// <summary>
        /// The number of connections in the pool.
        /// </summary>
        /// <returns>The size of the pool.</returns>
        public static int Size()
        {
            return Pool.Count;
        }

    }
}
