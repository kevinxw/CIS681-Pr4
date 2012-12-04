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

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Diagram", IsReference = true)]
    public class DiagramData : BaseData {

        #region Properties
        /// <summary>
        /// Is the diagram open in current project
        /// </summary>
        [DataMember(Name = "IsOpen", EmitDefaultValue = false, Order = 0)]
        private bool isOpen = true;
        public bool IsOpen {
            get { return isOpen; }
            set {
                if (isOpen == value) return;
                isOpen = value;
                OnPropertyChanged("IsOpen");
            }
        }

        /// <summary>
        /// Child objects
        /// </summary>
        [DataMember(Name = "Models", EmitDefaultValue = false, Order = 0)]
        public List<ModelData> Models { get; private  set; }
        [DataMember(Name = "Connections", EmitDefaultValue = false, Order = 1)]
        public List<ConnectionData> Connections { get; private  set; }

        /// <summary>
        /// Canvas size, should has value before serializing
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
        private Point startPosition = new Point(0, 0);
        public Point StartPosition {
            get { return startPosition; }
            set {
                if (startPosition.Equals(value)) return;
                startPosition = value;
                OnPropertyChanged("StartPosition");
            }
        }

        /// <summary>
        /// Which project is this diagram belonged to
        /// </summary>
        [DataMember(Name = "Owner")]
        public ProjectData Owner { get; set; }
        #endregion

        /// <summary>
        /// Initialize data
        /// </summary>
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            if (Models == null) Models = new List<ModelData>();
            if (Connections == null) Connections = new List<ConnectionData>();
        }
    }
}
