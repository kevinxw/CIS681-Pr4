/*
 * Connectors which connects models and connections
 * There are five connectors "belong" to a model, each manage a connection list
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Connector", IsReference = true)]
    public class ConnectorData {

        #region Properties
        /// <summary>
        /// Type of current Connector
        /// </summary>
        [DataMember(Name = "Type")]
        public ConnectorType Type { get; protected set; }

        /// <summary>
        /// Connections connected to current connector
        /// </summary>
        [DataMember(Name = "Connections")]
        public List<ConnectionData> Connections { get; private set; }

        /// <summary>
        /// Decide connector's position, but no need to store it
        /// </summary>
        public Point Position {
            get {
                Point position = Owner.Position;
                if (Owner.Size.IsEmpty) return position;
                double width = Owner.Size.Width, height = Owner.Size.Height;
                switch (Type) {
                    case ConnectorType.Center: position.Offset(width / 2, height / 2); break;
                    case ConnectorType.Bottom: position.Offset(width / 2, height); break;
                    case ConnectorType.Left: position.Offset(0, height / 2); break;
                    case ConnectorType.Right: position.Offset(width, height / 2); break;
                    case ConnectorType.Top: position.Offset(width / 2, 0); break;
                }
                return position;
            }
        }

        /// <summary>
        /// Who possess this connector
        /// </summary>
        [DataMember(Name = "Owner")]
        public ModelData Owner { get; set; }
        #endregion

        #region Consturctor
        public ConnectorData() {
            Type = ConnectorType.Center;
            AfterInitializingData();
        }
        public ConnectorData(ModelData parent, ConnectorType type) {
            Owner = parent;
            Type = type;
            AfterInitializingData();
        }
        #endregion

        private void AfterInitializingData() {
            if (Connections == null)
                Connections = new List<ConnectionData>();
        }

        /// <summary>
        /// Connector Data is not a BaseData, so have to write it again
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        private void OnDataDeserialized(StreamingContext context) { AfterInitializingData(); }

        /// <summary>
        /// Check whether two objects are the same by comparing its parent and type
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ConnectorData obj) {
            return Owner != null && Owner.Equals(obj.Owner) && Type == obj.Type;
        }
    }
}
