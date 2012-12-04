/*
 * Represent the diagram as canvas
 */

//#define DEBUG_ON

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CIS681.Fall2012.VDS.UI;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.UI.Operation;

namespace CIS681.Fall2012.VDS.Data.Client {
    public partial class Diagram : IControl<Canvas> {
        /// <summary>
        /// Selected Items on current ContainerCanvas
        /// </summary>
        public SelectCollection SelectedItems { get; private set; }

        public Canvas Control { get; set; }

        public UMLObjectCollection Children { get; private set; }

        /// <summary>
        /// Init the instance
        /// </summary>
        partial void InitCanvas() {
            SelectedItems = new SelectCollection();
        }
        partial void RefreshCanvas() {
            Control = new Canvas();
            // must initialize it here!! deserialization will create an new collection anyway!
            Children = new UMLObjectCollection(Models, Connections, Control);
            Children.Sync();
            Control.Focusable = false;  // cannot be focused by default
            Control.MouseMove += OnMouseMove_ShowPos;
            allowDrop();
            Control.Loaded += OnCanvasLoaded;
            // initialize size
            PropertyChanged += OnSizeChanged;
            if (!Size.IsEmpty) {
                Control.Width = Size.Width;
                Control.Height = Size.Height;
            }
            // fix canvas start position
            if (double.IsNaN(StartPosition.X)) StartPosition.X = 0;
            if (double.IsNaN(StartPosition.Y)) StartPosition.Y = 0;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Canvas refreshed!! ", System.DateTime.Now.Millisecond);
#endif
        }

        #region Event Handlers
        /// <summary>
        /// Refresh canvas, load model adorners
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnCanvasLoaded(object sender, RoutedEventArgs e) {
            Canvas canvas = sender as Canvas;
            Diagram diagram = Project.CurrentProject.Children.FindByCanvas(canvas);
            // draw models and connections
            diagram.Models.ForEach(item => item.Control.Draw());
            diagram.Connections.ForEach(item => item.Control.Draw());
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Canvas Loaded ", System.DateTime.Now.Millisecond);
#endif
        }
        /// <summary>
        /// When diagram's size is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnSizeChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName != "Size") return;
            Diagram d = sender as Diagram;
            d.Control.Width = d.Size.Width;
            d.Control.Height = d.Size.Height;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Canvas resized. size {1} ", System.DateTime.Now.Millisecond, d.Size.ToString());
#endif
        }

        /// <summary>
        /// Show the coordinate of mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnMouseMove_ShowPos(object sender, MouseEventArgs e) {
            Canvas c = sender as Canvas;
            Point p = e.GetPosition(c);
            UpdateMousePos(c, p);
        }
        private static void UpdateMousePos(Canvas c, Point p) {
            Window w = Window.GetWindow(c);
            TextBlock t = w.FindName("MousePos") as TextBlock;
            if (t == null) return;
            t.Text = (int)p.X + "," + (int)p.Y;
        }
        #endregion

        #region Drag-and-Drop
        // bind event handler according the AllowDrop property of ContainerCanvas
        private void allowDrop() {
            Control.Drop += DropEventHandler;
            Control.DragLeave += DragLeaveEventHandler;
            Control.DragEnter += DragEnterEventHandler;
            Control.DragOver += DragOverEventHandler;
        }

        private static void DragEnterEventHandler(object sender, DragEventArgs e) {
            // get the target canvas (diagram)
            Canvas c = sender as Canvas;
            DragDataWrapper data = e.Data.GetData(typeof(DragDataWrapper)) as DragDataWrapper;
            ModelItem m = data.Content as ModelItem;
            Diagram d = Project.CurrentProject.Children.FindByCanvas(c);
            if (d == null || m == null) return;
            switch (data.Type) {
                case DragOperationType.Create:
                    d.Children.Add(m.ContentObject);
#if DEBUG_ON
                    // test value
                    System.Console.WriteLine("{0} Dragging enter canvas", System.DateTime.Now.Millisecond);
#endif
                    break;
            }
        }

        // do drop operation, i.e., recalculate connection path, etc.
        private static void DropEventHandler(object sender, DragEventArgs e) {
            DragDataWrapper data = e.Data.GetData(typeof(DragDataWrapper)) as DragDataWrapper;
            ModelItem m = data.Content as ModelItem;
            // get canvas
            Canvas c = sender as Canvas;
            Diagram d = Project.CurrentProject.Children.FindByCanvas(c);
            if (m == null || d == null) return;
            switch (data.Type) {
                case DragOperationType.Create:
                    m.Draw();
                    d.SelectedItems.Set(m);
#if DEBUG_ON
                    // test value
                    System.Console.WriteLine("{0} Drop on canvas", System.DateTime.Now.Millisecond);
#endif
                    break;
            }
        }

        // drag leave (remove object?)
        private static void DragLeaveEventHandler(object sender, DragEventArgs e) {
            DragDataWrapper data = e.Data.GetData(typeof(DragDataWrapper)) as DragDataWrapper;
            ModelItem m = data.Content as ModelItem;
            Canvas c = sender as Canvas;
            Diagram d = Project.CurrentProject.Children.FindByCanvas(c);
            if (m == null || d == null) return;
            switch (data.Type) {
                case DragOperationType.Create:
                    d.Children.Remove(m.ContentObject);
#if DEBUG_ON
                    // test value
                    System.Console.WriteLine("{0} Dragging leave canvas", System.DateTime.Now.Millisecond);
#endif
                    break;
            }
        }

        private static void DragOverEventHandler(object sender, DragEventArgs e) {
            DragDataWrapper data = e.Data.GetData(typeof(DragDataWrapper)) as DragDataWrapper;
            ModelItem m = data.Content as ModelItem;
            Canvas c = sender as Canvas;
            Diagram d = Project.CurrentProject.Children.FindByCanvas(c);
            // get relative position
            Point p = e.GetPosition(c);
            UpdateMousePos(c, p);
            switch (data.Type) {
                case DragOperationType.Create:
                case DragOperationType.Move:
                    m.Position = new Point(p.X - data.DragStartPosition.X, p.Y - data.DragStartPosition.Y);
#if DEBUG_ON
                    // test value
                    System.Console.WriteLine("{0} Drag over canvas", System.DateTime.Now.Millisecond);
#endif
                    break;
            }
        }
        #endregion
    }
}
