/*
 * Load data for client (connection part)
 */

//////
/// Nothing to be TESTED here!
//////
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Connection : CIS681.Fall2012.VDS.Data.ConnectionData {
        /// <summary>
        /// Override the Owner property, return Diagram instead
        /// </summary>
        public new Diagram Owner { get { return base.Owner as Diagram; } set { base.Owner = value; } }

        /// <summary>
        /// Refresh data
        /// </summary>
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
            RefreshControl();
        }

        // for Connection item
        partial void RefreshControl();
    }
}

