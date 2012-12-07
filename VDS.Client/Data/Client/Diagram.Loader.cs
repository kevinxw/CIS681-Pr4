/*
 * Diagram client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////
namespace CIS681.Fall2012.VDS.Data {
    public partial class Diagram {
        protected override void InitializingData() {
            base.InitializingData();
            // init canvas first! as tab.contect = canvas!
            InitCanvas();
            InitTab();
        }
        protected override void InitializedData() {
            base.InitializedData();
            RefreshCanvas();
            RefreshTab();

            DataUpdated += (s) => {
                if (this.Owner != null)
                    this.Owner.OnDataUpdated();
            };
        }
        protected override void FinalizingData() {
            base.FinalizingData();
            FinalizingCanvasData();
        }
        // canvas part
        partial void InitCanvas();
        partial void RefreshCanvas();
        // canvas tab part
        partial void InitTab();
        partial void RefreshTab();
        partial void FinalizingCanvasData();
    }
}
