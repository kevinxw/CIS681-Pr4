/*
 * Display the project as a TabControl, each Tab is a diagram
 */

//#define DEBUG_ON

using System.ComponentModel;
using System.Windows.Controls;

namespace CIS681.Fall2012.VDS.Data {
    public partial class Project {
        /// <summary>
        /// Tab Control
        /// </summary>
        public TabControl Tabs { get; private set; }

        /// <summary>
        /// "real" diagram collection
        /// </summary>
        private DiagramCollection diagrams = null;
        public DiagramCollection Children { get { return diagrams; } }

        partial void InitTab() {
        }
        partial void RefreshTab() {
            Tabs = new TabControl();
            diagrams = new DiagramCollection(children, this);
            diagrams.ActivateNewDiagram = true;
            // rebuild tabs by children
            diagrams.Sync();
            if (activatedDiagram != null)
                Tabs.SelectedItem = activatedDiagram.Tab;
            PropertyChanged += OnTitleChanged;
            PropertyChanged += OnActivatedDiagramChanged;
#if DEBUG_ON
            System.Console.WriteLine("{0} Project refreshed", System.DateTime.Now.Millisecond);
#endif
        }

        #region Event Handlers
        /// <summary>
        /// When title is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnTitleChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Title") return;
            Project proj = sender as Project;
            if (proj != Project.Current) return;
            TextBlock titleBlock = App.Current.MainWindow.FindName("ProjectNameBlock") as TextBlock;
            if (titleBlock == null) return;
            titleBlock.Text = proj.Title;
#if DEBUG_ON
            System.Console.WriteLine("{0} project title changed", System.DateTime.Now.Millisecond);
#endif
        }

        /// <summary>
        /// Switch to new activated diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnActivatedDiagramChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "ActivatedDiagram") return;
            Project proj = sender as Project;
            if (proj != Project.Current) return;
            if (proj.ActivatedDiagram != null)
                proj.Tabs.SelectedItem = proj.ActivatedDiagram.Tab;
#if DEBUG_ON
                System.Console.WriteLine("{0} activated diagram changed", System.DateTime.Now.Millisecond);
#endif
        }
        #endregion
    }

}
