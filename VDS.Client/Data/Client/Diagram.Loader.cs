/*
 * Diagram client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////
namespace CIS681.Fall2012.VDS.Data {
    public partial class Diagram {
        protected override void InitData() {
            // init canvas first! as tab.contect = canvas!
            InitCanvas();
            InitTab();
        }
        protected override void RefreshData() {
            RefreshCanvas();
            RefreshTab();
        }
        // canvas part
        partial void InitCanvas();
        partial void RefreshCanvas();
        // canvas tab part
        partial void InitTab();
        partial void RefreshTab();
    }
}
