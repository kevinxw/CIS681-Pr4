/*
 * Diagram, which contains object (models / connections)
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using CIS681.Fall2012.VDS.Data.Objects;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Diagram", IsReference = true)]
    public partial class Diagram : BaseData {

        #region Properties
        /// <summary>
        /// Is the diagram open in current project
        /// </summary>
        [DataMember(Name = "IsOpen", EmitDefaultValue = false, Order = 0)]
        private bool isOpen = true;
        public bool IsOpen {
            get { return isOpen; }
            set {
                isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }

        /// <summary>
        /// Child objects
        /// </summary>
        [DataMember(Name = "Models", EmitDefaultValue = false, Order = 0)]
        private List<Model> models;
        public List<Model> Models { get { return models; } }
        [DataMember(Name = "Connections", EmitDefaultValue = false, Order = 1)]
        private List<Connection> connections;
        public List<Connection> Connections { get { return connections; } }

        /// <summary>
        /// Canvas size
        /// </summary>
        [DataMember(Name = "Size", EmitDefaultValue = false)]
        private Size size = Size.Empty;
        public Size Size {
            get { return size; }
            set {
                if (size.Equals(value)) return;
                size = value;
                OnPropertyChanged("Size");
            }
        }

        /// <summary>
        /// Start Position, when canvas is expanded, what is the coordinate of Canvas.LeftTop?
        /// </summary>
        [DataMember(Name = "StartPosition")]
        private Point startPosition = new Point(0,0);
        public Point StartPosition {
            get { return startPosition; }
            set {
                if (startPosition.Equals(value)) return;
                startPosition = value;
                OnPropertyChanged("StartPosition");
            }
        }
        #endregion

        protected override void RefreshBaseData() {
            if (models == null)
                models = new List<Model>();
            if (connections == null)
                connections = new List<Connection>();
            // after models and connections are loaded, we need to bind events which synchronizing them
            connections.ForEach(item => item.InitEventHandler());
        }
    }
}
