/*
 * Model client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data.Objects {
    public partial class Model {
        protected override void RefreshData() {
            RefreshControl();
        }
        // ModelItem
        partial void RefreshControl();
    }
}
