/*
 * Connectors which connects models and connections
 * There are five connectors "belong" to a model, each manage a connection list
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

//#define DEBUG_ON

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data.Objects {
    [DataContract(Name = "Connector", IsReference = true, Namespace = "http://VDS.Data")]
    public partial class Connector {

        #region Properties
        /// <summary>
        /// Decide connector's position
        /// </summary>
        public Point Position {
            get {
                Point position = Owner.Position;
                double width = Owner.Control.Size.Width, height = Owner.Control.Size.Height;
                //double width = Owner.Size.Width, height = Owner.Size.Height;
                switch (Type) {
                    case ConnectorType.Center: position.Offset(width / 2, height / 2); break;
                    case ConnectorType.Bottom: position.Offset(width / 2, height); break;
                    case ConnectorType.Left: position.Offset(0, height / 2); break;
                    case ConnectorType.Right: position.Offset(width, height / 2); break;
                    case ConnectorType.Top: position.Offset(width / 2, 0); break;
                }
#if DEBUG_ON
                // test value
                System.Console.WriteLine("{0} width {1} height {2} position {3}", System.DateTime.Now.Millisecond, width, height, position.ToString());
#endif
                return position;
            }
        }

        /// <summary>
        /// Type of current Connector
        /// </summary>
        [DataMember(Name = "Type")]
        public ConnectorType Type { get; private set; }

        /// <summary>
        /// Connections connected to current connector
        /// </summary>
        [DataMember(Name = "Connections")]
        private List<Connection> connections = new List<Connection>();
        public List<Connection> Connections { get { return connections; } }

        /// <summary>
        /// Who possess this connector
        /// </summary>
        [DataMember(Name = "Owner")]
        public Model Owner { get; private set; }
        #endregion

        #region Consturctor
        public Connector() { Type = ConnectorType.Center; }
        public Connector(Model parent, ConnectorType type) {
            Owner = parent;
            Type = type;
        }
        #endregion

        /// <summary>
        /// Check whether two objects are the same by comparing its parent and type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(Connector obj) {
            return Owner == obj.Owner && Type == obj.Type;
        }
    }
}
