/*
 * The connector data of client
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Connector : CIS681.Fall2012.VDS.Data.ConnectorData {
        /// <summary>
        /// Override the Owner property, return Model instead
        /// </summary>
        public new Model Owner { get { return base.Owner as Model; } set { base.Owner = value; } }
    }
}
