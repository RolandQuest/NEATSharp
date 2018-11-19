using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Tracks innovations already made.
    /// </summary>
    public class InnovationPool
    {
        private List<InnovationInformation> Pool = new List<InnovationInformation>();

        /// <summary>
        /// Adds a new innovation to the pool.
        /// </summary>
        /// <param name="innovInfo">The information uniquely identifying the innovation.</param>
        /// <returns>The position in the Pool (unique Id).</returns>
        public int Add(InnovationInformation innovInfo)
        {
            Pool.Add(innovInfo);
            return Size() - 1;
        }

        /// <summary>
        /// Adds all innovations from another InnovationPool to end of Pool.
        /// </summary>
        /// <param name="pool">The other pool to add.</param>
        /// <returns>The last index of the new pool.</returns>
        public int AddRange(InnovationPool pool)
        {
            for(int i = 0; i < pool.Size(); i++)
            {
                Add(pool.FindById(i));
            }
            return Size() - 1;
        }

        /// <summary>
        /// Starts the pool out fresh.
        /// </summary>
        public void Clear()
        {
            Pool = new List<InnovationInformation>();
        }

        /// <summary>
        /// Retrieves an innovation by its unique position in the pool.
        /// </summary>
        /// <param name="id">The unique position in the pool.</param>
        /// <returns>A unique innovation.</returns>
        public InnovationInformation FindById(int id)
        {
            return Pool[id];
        }

        /// <summary>
        /// Retrieves the id of a matching innovation.
        /// </summary>
        /// <param name="innovInfo">The information to search for.</param>
        /// <returns>The id of the first matching innovation. (-1) if not found.</returns>
        public int FindByInnovation(InnovationInformation innovInfo)
        {
            for(int i = 0; i < Size(); i++)
            {
                if (innovInfo.IsSameAs(Pool[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// The number of innovations in the pool.
        /// </summary>
        /// <returns>The size of the pool.</returns>
        public int Size()
        {
            return Pool.Count;
        }


    }
}
