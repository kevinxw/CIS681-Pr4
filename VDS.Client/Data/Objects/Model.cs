/*
 * Basic Model type
 */

//////
/// Data Structure
/// Nothing to be TESTED here!
//////

using System.Runtime.Serialization;
using System.Windows;

namespace CIS681.Fall2012.VDS.Data.Objects {
    [DataContract(Name = "Model", IsReference = true)]
    public partial class Model : BaseObject {

        #region Properties
        /// <summary>
        /// One Model has five connectors at most
        /// </summary>
        [DataMember(Name = "Connectors")]
        public Connector[] Connectors { get; private set; }
        public Connector GetConnector(ConnectorType type) {
            return Connectors[(int)type];
        }

        /// <summary>
        /// Model Size
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
        #endregion

        public Model() { }

        /// <summary>
        /// Initialize data like connectors
        /// </summary>
        protected override void RefreshBaseData() {
            base.RefreshBaseData();
            // notice, the order of these connector types cannot be changed
            // this is corresponded with the order of ConnectoryType
            if (Connectors == null)
                Connectors = new Connector[] {
                new Connector(this,ConnectorType.Top),
                new Connector(this,ConnectorType.Left),
                new Connector(this,ConnectorType.Center),
                new Connector(this,ConnectorType.Right),
                new Connector(this,ConnectorType.Bottom)
            };
        }
    }
}
