/*
 * The connector adorner which produce connectors display
 */

//#define DEBUG_ON

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Data;
using CIS681.Fall2012.VDS.Data.Objects;
using CIS681.Fall2012.VDS.UI.Objects;

namespace CIS681.Fall2012.VDS.UI.Adorner {
    public class ConnectorAdorner : System.Windows.Documents.Adorner {
        // the corresponding thumbs to the connectors
        private AdornerDataThumb<ModelItem, Connector>[] thumbs = new AdornerDataThumb<ModelItem, Connector>[5];
        private Model model;
        private VisualCollection collection;

        public ConnectorAdorner(ModelItem item)
            : base(item) {
            model = item.ContentObject;
            collection = new VisualCollection(this);
            // make thumbs
            for (int i = 0; i < model.Connectors.Length; i++)
                collection.Add(thumbs[i] = GetConnectorThumb(model.Connectors[i]));
            // do not use center currently
            thumbs[(int)ConnectorType.Center].Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Arrange
        /// </summary>
        /// <param name="finalSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size finalSize) {
            ModelItem elem = AdornedElement as ModelItem;
            if (elem == null)
                return finalSize;
            Vector offset = new Vector(thumbs[0].ActualWidth / 2, thumbs[0].ActualHeight / 2);
            Size size = new Size(thumbs[0].ActualWidth, thumbs[0].ActualHeight);
            double halfWidth = elem.Size.Width / 2, halfHeight = elem.Size.Height / 2;
            Point center = new Point(halfWidth, halfHeight) - offset; // center point
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Size - connItem {1} size {2} hWidth {3} hHeight {4}", System.DateTime.Now.Millisecond, drawingConn, size.ToString(), halfWidth, halfHeight);
#endif
            thumbs[(int)ConnectorType.Bottom].Arrange(new Rect(center + new Vector(0, halfHeight), size));
            thumbs[(int)ConnectorType.Left].Arrange(new Rect(center + new Vector(-halfWidth, 0), size));
            thumbs[(int)ConnectorType.Center].Arrange(new Rect(center, size));
            thumbs[(int)ConnectorType.Right].Arrange(new Rect(center + new Vector(halfWidth, 0), size));
            thumbs[(int)ConnectorType.Top].Arrange(new Rect(center + new Vector(0, -halfHeight), size));
            return finalSize;
        }

        private AdornerDataThumb<ModelItem, Connector> GetConnectorThumb(Connector conn) {
            AdornerDataThumb<ModelItem, Connector> thumb = new AdornerDataThumb<ModelItem, Connector>(AdornedElement as ModelItem, conn);
            thumb.Style = FindResource("ConnectorStyle_" + conn.Type.ToString()) as Style;
            thumb.DragDelta += OnDragDelta_DetectPath;
            thumb.DragCompleted += OnDragCompleted_Connect;
            return thumb;
        }

        protected override Visual GetVisualChild(int index) {
            return collection[index];
        }

        protected override int VisualChildrenCount {
            get {
                return collection.Count;
            }
        }

        /// <summary>
        /// Current Connection item being drawn
        /// </summary>
        private static ConnectionItem drawingConn = null;
        /// <summary>
        /// Detect and show path when dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDragDelta_DetectPath(object sender, DragDeltaEventArgs e) {
            AdornerDataThumb<ModelItem, Connector> thumb = sender as AdornerDataThumb<ModelItem, Connector>;
            Point currentPoint = thumb.Data.Position + new Vector(e.HorizontalChange, e.VerticalChange);
            Connector target = HitTest(thumb, currentPoint);
            ModelItem item = thumb.AdornedParent;
            Canvas canvas = item.ContainerCanvas;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Dragging - connItem {1} ", System.DateTime.Now.Millisecond, drawingConn);
#endif
            if (drawingConn == null) {
                // create new connection
                drawingConn = new ConnectionItem();
                // get current connection type
                Window w = Window.GetWindow(canvas);
                ConnectionType connType = (ConnectionType)(w.FindName("CurrentConnectionType") as ComboBox).SelectedIndex;
                drawingConn.ContentObject.Type = connType.ToString();
                drawingConn.ContentObject.Source = thumb.Data;  // bind source here
                Diagram diagram = Project.Current.Children.FindByCanvas(canvas);
                diagram.Children.Add(drawingConn.ContentObject);
            }
            if ((drawingConn.ContentObject.Sink = target) != null)
                drawingConn.UpdatePath();
            else
                drawingConn.UpdatePath(drawingConn.ContentObject.SinkPosition = currentPoint);
            drawingConn.Draw();
            e.Handled = true;
        }

        /// <summary>
        /// When drag is completed, i.e. mouse up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDragCompleted_Connect(object sender, DragCompletedEventArgs e) {
            AdornerDataThumb<ModelItem, Connector> thumb = sender as AdornerDataThumb<ModelItem, Connector>;
            Point currentPoint = thumb.Data.Position + new Vector(e.HorizontalChange, e.VerticalChange);
            Connector target = HitTest(thumb, currentPoint);
            ModelItem item = thumb.AdornedParent;
            Canvas canvas = item.ContainerCanvas;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} DragCompleted - connItem {1} ", System.DateTime.Now.Millisecond, drawingConn);
#endif
            if (drawingConn != null) {
                // if target is empty, cancel the connection
                if (drawingConn.ContentObject.Sink == null) {
                    Diagram diagram = Project.Current.Children.FindByCanvas(canvas);
                    diagram.Children.Remove(drawingConn.ContentObject);
                }
                drawingConn = null;
            }
            e.Handled = true;
        }

        /// <summary>
        /// Do hit test, if the target is a thumb (another connector), then return it
        /// </summary>
        /// <param name="hitPoint"></param>
        /// <returns></returns>
        private static Connector HitTest(AdornerDataThumb<ModelItem, Connector> thumb, Point hitPoint) {
            ModelItem elem = thumb.AdornedParent; Canvas canvas = elem.ContainerCanvas; HitTestResult hitRes = VisualTreeHelper.HitTest(canvas, hitPoint); DependencyObject hitObj = null;
            Model model = elem.ContentObject; ModelItem res = null;
            if (hitRes != null)
                hitObj = hitRes.VisualHit;
            while (hitObj != null && hitObj != canvas && (res = hitObj as ModelItem) == null)
                hitObj = VisualTreeHelper.GetParent(hitObj);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} hitObj {1} ", System.DateTime.Now.Millisecond, hitObj.GetType());
#endif
            // test fails
            if (res == null || res == elem)
                return null;
            // spilit the rectangle into four areas with two line: y=-h/w*x+h, y=h/w*x
            double width = res.Size.Width, height = res.Size.Height;
            Vector relativeVector = hitPoint - res.Position;
            double y1 = relativeVector.X * height / width, y2 = -y1 + height;
            ConnectorType type = ConnectorType.Center;
            // now calculate which connector is that
            if (relativeVector.Y < y1 && relativeVector.Y < y2)
                type = ConnectorType.Top;
            else if (relativeVector.Y > y1 && relativeVector.Y < y2)
                type = ConnectorType.Left;
            else if (relativeVector.Y > y1 && relativeVector.Y > y2)
                type = ConnectorType.Bottom;
            else if (relativeVector.Y < y1 && relativeVector.Y > y2)
                type = ConnectorType.Right;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Connector.Type {1} width {2} height {3}", System.DateTime.Now.Millisecond, type.ToString(), width, height);
#endif
            return res.ContentObject.GetConnector(type);
        }
    }
}
