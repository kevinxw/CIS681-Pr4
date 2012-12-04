/*
 * Diagram client-end data loader
 */

//////
/// Nothing to be TESTED here!
//////

using System.Windows;
using System.Collections.Generic;
namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Diagram : CIS681.Fall2012.VDS.Data.DiagramData {

        public List<Model> Models { get; private set; }

        protected override void BeforeInitializingData() {
            base.BeforeInitializingData();
            // init canvas first! as tab.contect = canvas!
            InitCanvas();
            InitTab();
        }
        protected override void AfterInitializingData() {
            base.AfterInitializingData();
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
