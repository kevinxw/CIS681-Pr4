/*
 * Load data for client (connection part)
 */

//////
/// Nothing to be TESTED here!
//////
namespace CIS681.Fall2012.VDS.Data.Objects {
    public partial class Connection {
        protected override void InitializedData() {
            base.InitializedData();
            RefreshControl();

            DataUpdated += (s) => {
                if (this.Owner != null)
                    this.Owner.OnDataUpdated();
            };
        }
        // for Connection item
        partial void RefreshControl();
    }
}

