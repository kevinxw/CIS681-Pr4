/*
 * Project data loader
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data {
    public partial class Project{
        protected override void InitData() {
            InitTab();
        }
        protected override void RefreshData() {
            RefreshTab();
        }

        partial void InitTab();
        partial void RefreshTab();
    }
}
