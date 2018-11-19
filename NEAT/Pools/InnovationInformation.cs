using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEAT
{
    /// <summary>
    /// Holds the specifics of an innovation.
    /// How this class is used, is solely determined by the InnovationType.
    /// Knowledge of the type must be known before can be used effectively.
    /// TODO: Possibly separate these out and not nest classes.
    /// </summary>
    public class InnovationInformation
    {
        /// <summary>
        /// The type of mutation that created this innovation.
        /// </summary>
        public MutationStyle InnovationType { get; private set; }

        /// <summary>
        /// The information relating to the connection.
        /// For InnovationType = MutationStyle.AddNode, this is the original connection that was broke.
        /// For InnovationType = MutationStyle.AddConnection, this is the new connection made.
        /// </summary>
        public ConnectionInformation ConnectionInformation { get; private set; }
        
        /// <summary>
        /// For InnovationType = MutationStyle.AddNode, this is the ActivationStyle of the new node.
        /// </summary>
        public ActivationStyle Style { get; private set; }

        /// <summary>
        /// Information pertaining to the exact details of the innovation.
        /// </summary>
        public NewNodeInformation NewNodeDetails { get; set; } = new NewNodeInformation();

        /// <summary>
        /// Information pertaining to the exact details of the innnovation.
        /// </summary>
        public NewConnectionInformation NewConnectionDetails { get; set; } = new NewConnectionInformation();
        
        /// <summary>
        /// Constructs a MutationStyle.AddNode innovation.
        /// </summary>
        /// <param name="conInfo">The original connection being broke.</param>
        /// <param name="style">The ActivationStyle of the new node.</param>
        public InnovationInformation(ConnectionInformation conInfo, ActivationStyle style)
        {
            InnovationType = MutationStyle.AddNode;
            ConnectionInformation = conInfo;
            Style = style;
        }

        /// <summary>
        /// Constructs a MutationStyle.AddConnection innovation.
        /// </summary>
        /// <param name="conInfo">The connection that was added.</param>
        public InnovationInformation(ConnectionInformation conInfo)
        {
            InnovationType = MutationStyle.AddConnection;
            ConnectionInformation = conInfo;
        }

        /// <summary>
        /// Checks if the two innovations are equivalent.
        /// </summary>
        /// <param name="other">The innovation to compare against.</param>
        /// <returns>True if they are the same.</returns>
        public bool IsSameAs(InnovationInformation other)
        {
            if(InnovationType == MutationStyle.AddConnection)
            {
                return InnovationType == other.InnovationType &&
                        ConnectionInformation.IsSameAs(other.ConnectionInformation);
            }
            else if(InnovationType == MutationStyle.AddNode)
            {
                return InnovationType == other.InnovationType &&
                        ConnectionInformation.IsSameAs(other.ConnectionInformation) &&
                        Style == other.Style;
            }

            return false;
        }


        /*
         * Helper classes
         * */

        /// <summary>
        /// Holds information pertaining to a new node innovation.
        /// </summary>
        public class NewNodeInformation
        {
            /// <summary>
            /// The Id of the new node added.
            /// </summary>
            public int NewNodeId { get; set; }

            /// <summary>
            /// The 'into' connection of the new node.
            /// </summary>
            public int FirstConnectionId { get; set; }

            /// <summary>
            /// The 'out' connection of the new node.
            /// </summary>
            public int SecondConnectionId { get; set; }
        }

        /// <summary>
        /// Holds information pertaining to a new connection innovation.
        /// </summary>
        public class NewConnectionInformation
        {
            /// <summary>
            /// The Id of the new connection.
            /// </summary>
            public int ConnectionId { get; set; }
        }

    }

    

}
