/*
 * Model client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data.Objects {
    public partial class Model {
        protected override void InitializedData() {
            base.InitializedData();
            RefreshControl();

            DataUpdated += (s) => {
                if (this.Owner != null)
                    this.Owner.OnDataUpdated();
            };
        }

        protected override void FinalizingData() {
            base.FinalizingData();
            FinalizeControl();
        }
        // ModelItem
        partial void RefreshControl();
        partial void FinalizeControl();
    }
}
