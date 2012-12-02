/*
 * Load data for client (connection part)
 */

//////
/// Nothing to be TESTED here!
//////
namespace CIS681.Fall2012.VDS.Data.Objects {
    public partial class Connection {
        protected override void RefreshData() {
            RefreshControl();
        }
        // for Connection item
        partial void RefreshControl();
    }
}

