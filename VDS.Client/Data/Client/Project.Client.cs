/*
 * Project data loader
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Project : CIS681.Fall2012.VDS.Data.ProjectData {
        protected override void BeforeInitializingData() {
            base.BeforeInitializingData();
            InitTab();
        }
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            RefreshTab();
        }

        partial void InitTab();
        partial void RefreshTab();
    }
}
