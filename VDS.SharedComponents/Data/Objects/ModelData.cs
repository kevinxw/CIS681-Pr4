/*
 * Basic Model type
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data {
    [DataContract(Name = "Model", IsReference = true)]
    public class ModelData : BaseObject {

        #region Properties
        /// <summary>
        /// One Model has five connectors at most
        /// </summary>
        [DataMember(Name = "Connectors")]
        public ConnectorData[] Connectors { get; private set; }
        public ConnectorData GetConnector(ConnectorType type) {
            return Connectors[(int)type];
        }

        /// <summary>
        /// Model Size
        /// </summary>
        [DataMember(Name = "Size", EmitDefaultValue = false)]
        protected Size size = Size.Empty;
        public Size Size {
            get { return size; }
            set {
                if (size.Equals(value)) return;
                size = value;
                OnPropertyChanged("Size");
            }
        }

        /// <summary>
        /// Which diagram owns this model
        /// </summary>
        [DataMember(Name = "Owner")]
        public DiagramData Owner { get; set; }
        #endregion

        /// <summary>
        /// Initialize data like connectors
        /// </summary>
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            // notice, the order of these connector types cannot be changed
            // this is corresponded with the order of ConnectoryType
            if (Connectors == null)
                Connectors = new ConnectorData[] {
                new ConnectorData(this,ConnectorType.Top),
                new ConnectorData(this,ConnectorType.Left),
                new ConnectorData(this,ConnectorType.Center),
                new ConnectorData(this,ConnectorType.Right),
                new ConnectorData(this,ConnectorType.Bottom)
            };
        }
    }
}
