/*
 * An extension of diagram class which binds a tab to it
 */

//#define DEBUG_ON

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CIS681.Fall2012.VDS.UI;
using CIS681.Fall2012.VDS.UI.Promot;

namespace CIS681.Fall2012.VDS.Data {
    public partial class Diagram {
        /// <summary>
        /// The TabItem of current Diagram
        /// </summary>
        public TabItem Tab { get; private set; }

        partial void InitTab() {
            Tab = new TabItem();
            // bind events for tab header
            Tab.Loaded += (s, e) => {
                Button closeButton = UIHelper.FindChild<Button>(Tab);
                TextBlock header = UIHelper.FindChild<TextBlock>(Tab);
                // close diagram event
                closeButton.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(OnRemoveDiagramButton), true);
                header.MouseDown += OnDoubleClickTab_RenameDiagram;
#if DEBUG_ON
                System.Console.WriteLine("{0} Tab loaded", System.DateTime.Now.Millisecond);
#endif
            };
        } 

        partial void RefreshTab() {
            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = Control;
            Tab.Content = scroll;
            // init title
            Tab.Header = Title;
            // bind events
            PropertyChanged += OnDiagramTitleChanged;
            PropertyChanged += OnIsOpenChanged;
#if DEBUG_ON
            System.Console.WriteLine("{0} Tab refreshed", System.DateTime.Now.Millisecond);
#endif
        }

        #region Event Handlers
        /// <summary>
        /// On title changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDiagramTitleChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Title") return;
            Diagram d = sender as Diagram;
            d.Tab.Header = string.IsNullOrEmpty(d.Title) ? "(Untitled)" : d.Title;
        }

        /// <summary>
        /// Close or open the diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnIsOpenChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "IsOpen") return;
            Diagram d = sender as Diagram;
            if (d.IsOpen) {
                d.Tab.Visibility = Visibility.Visible;
                // activate current tab
                Project.CurrentProject.ActivatedDiagram = d;
            }
            else {
                // hide the diagram
                d.Tab.Visibility = Visibility.Collapsed;
                ScrollViewer s = d.Tab.Content as ScrollViewer;
                s.Visibility = Visibility.Collapsed;
                Project.CurrentProject.ActivatedDiagram = null;
            }
#if DEBUG_ON
            System.Console.WriteLine("{0} diagram open : {1}", System.DateTime.Now.Millisecond, d.IsOpen);
#endif
        }

        /// <summary>
        /// Close the diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveDiagramButton(object sender, MouseButtonEventArgs e) {
            QuestionDialog dialog = new QuestionDialog("Are you sure to close this diagram?");
            dialog.ShowDialog();
            this.IsOpen = !dialog.Answer;
        }

        /// <summary>
        /// Rename the diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDoubleClickTab_RenameDiagram(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount < 2) return;
            TextDialog dialog = new TextDialog("Please enter a new name for the diagram");
            dialog.ShowDialog();
            this.Title = dialog.Answer;
        }
        #endregion
    }
}
