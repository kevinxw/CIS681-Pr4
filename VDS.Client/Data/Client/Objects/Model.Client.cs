/*
 * Model client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////

namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Model : CIS681.Fall2012.VDS.Data.ModelData {
        /// <summary>
        /// Override the Owner property, return Diagram instead
        /// </summary>
        public new Diagram Owner { get { return base.Owner as Diagram; } set { base.Owner = value; } }

        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            RefreshControl();
        }

        /// <summary>
        /// Refresh ModelItem
        /// </summary>
        partial void RefreshControl();
    }
}
