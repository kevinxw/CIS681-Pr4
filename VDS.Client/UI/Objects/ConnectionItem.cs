/*
 * Connection items
 */

//#define DEBUG_ON

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Data;
using CIS681.Fall2012.VDS.Data.Objects;
using CIS681.Fall2012.VDS.UI;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.UI.PathFinder;

namespace CIS681.Fall2012.VDS.UI.Objects {
    public class ConnectionItem : ContentControl, ISelectable, IContentObject<Connection>, IDrawable {
        private Connection contentObject = null;
        public Connection ContentObject {
            get { return contentObject; }
            set {
                if (contentObject != null)
                    contentObject.PropertyChanged -= OnNodeChanged_Refresh;
                if ((contentObject = value) != null)
                    contentObject.PropertyChanged += OnNodeChanged_Refresh;
            }
        }

        public Canvas ContainerCanvas { get; set; }

        #region Properties
        /// <summary>
        /// Is current element selected
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Get all points which consist of this path, including start and end
        /// </summary>
        public List<Point> Points {
            get {
                List<Point> points = new List<Point>();
                if (contentObject.Source != null)
                    points.Add(contentObject.SourcePosition);
                points.AddRange(contentObject.Stops);
                if (contentObject.Sink != null)
                    points.Add(contentObject.SinkPosition);
                return points;
            }
        }
        #endregion

        /// <summary>
        /// Draw the line
        /// </summary>
        /// <param name="dc"></param>
        protected override void OnRender(DrawingContext dc) {
#if DEBUG_ON
            System.Console.WriteLine("{0} Connection rendering", System.DateTime.Now.Millisecond);
#endif
            base.OnRender(dc);
            Pen pen = new Pen(Brushes.Black, 1);
            if (pathGeometry != null)
                dc.DrawGeometry(Brushes.Black, pen, pathGeometry);
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(RenderSize));
        }

        // When source/sink is changed
        private static void OnNodeChanged_Refresh(object sender, PropertyChangedEventArgs e) {
            string[] triggerEvents = new string[] { "Source.Size", "Sink.Size", "Source.Position", "Sink.Position", "Sink", "Source" };
#if DEBUG_ON
            System.Console.WriteLine("{0} Connector node changed {1}", System.DateTime.Now.Millisecond, e.PropertyName);
#endif
            foreach (string str in triggerEvents)
                if (str.Equals(e.PropertyName)) {   // find any match will trigger the operation
                    Connection item = sender as Connection;
                    if (item.Control != null) {
                        item.Control.UpdatePath();
                        item.Control.Draw();
                    }
                    return;
                }
        }

        #region Path Drawing
        private PathGeometry pathGeometry;

        /// <summary>
        /// Re-generate the path
        /// </summary>
        public void Draw() {
            if (Points.Count < 2) pathGeometry = null;
            else {
                PathFigure figure = new PathFigure();
                figure.StartPoint = contentObject.SourcePosition; figure.IsFilled = false; figure.IsClosed = false;
#if DEBUG_ON
                System.Console.WriteLine("{0} connection drawing lines ", System.DateTime.Now.Millisecond);
                foreach (Point p in contentObject.Stops)
                    System.Console.Write("{0} ", p.ToString());
#endif
                figure.Segments.Add(new PolyLineSegment(contentObject.Stops, true));
                PathGeometry geometry = new System.Windows.Media.PathGeometry();
                geometry.Figures.Add(figure);
                // draw different head for different types
                geometry.Figures.Add(DrawHead());
                geometry.Freeze();  // improve the performance
                pathGeometry = geometry;
                if (IsLoaded)
                    InvalidateVisual();
            }
        }

        /// <summary>
        /// Return a Path Figure based on connection's type
        /// </summary>
        /// <returns></returns>
        private PathFigure DrawHead() {
            PathFigure figure = new PathFigure();
            if (contentObject.Stops.Count < 1) return figure;
            Point start = contentObject.Stops[contentObject.Stops.Count - 1];
            Point end = contentObject.SinkPosition;
            if (start == end) return figure;   // when sink connector is not ready
            Vector v = end - start;
            Point midPoint = start + v / 2;
            Point p1 = midPoint, p2 = midPoint;
#if DEBUG_ON
            System.Console.WriteLine("{0} head:start {1} mid {2} end {3}", System.DateTime.Now.Millisecond, start.ToString(), midPoint.ToString(), end.ToString());
#endif
            switch (contentObject.Type) {
                case "Using":
                    v /= 3;
                    if (v.X == 0) { p1.X -= v.Y; p2.X += v.Y; }
                    else { p1.Y -= v.X; p2.Y += v.X; }
                    figure.StartPoint = start;
                    figure.Segments.Add(new PolyLineSegment(new Point[] { end, p1, end, p2,end }, true));
                    figure.IsClosed = false; figure.IsFilled = false;
                    break;
                case "Inheritance":
                    v /= 3;
                    if (v.X == 0) { p1.X -= v.Y; p2.X += v.Y; }
                    else { p1.Y -= v.X; p2.Y += v.X; }
                    figure.StartPoint = start;
#if DEBUG_ON
                    System.Console.WriteLine("{0} Inheritance:start {1} mid {2} p1 {4} end {3} p2 {5}, mid {2}", System.DateTime.Now.Millisecond, start.ToString(), midPoint.ToString(), end.ToString(),p1.ToString(),p2.ToString());
#endif
                    figure.Segments.Add(new PolyLineSegment(new Point[] { midPoint, p1, end, p2, midPoint }, true));
                    figure.IsClosed = false; figure.IsFilled = false;
                    break;
                case "Aggregation":
                case "Composition":
                default:
                    v /= 4;
                    if (v.X == 0) { p1.X -= v.Y; p2.X += v.Y; }
                    else { p1.Y -= v.X; p2.Y += v.X; }
                    figure.StartPoint = start;
                    Point third = start + v;
                    figure.Segments.Add(new PolyLineSegment(new Point[] { third, p1, end, p2, third }, true));
                    figure.IsClosed = false;
                    break;
            }
            if (contentObject.Type == "Aggregation")
                figure.IsFilled = false;
            return figure;
        }
        private PathFigure DrawTail() {
            PathFigure figure = new PathFigure();
            return figure;
        }

        /// <summary>
        /// Update path based on source and sink connector
        /// </summary>
        public void UpdatePath() {
            IPathFinder pathFinder = new OrthogonalPathFinder {
                Connection = contentObject,
                CurrentDiagram = Project.CurrentProject.Children.FindByCanvas(ContainerCanvas)
            };
            contentObject.Stops.Clear();
            contentObject.Stops.AddRange(pathFinder.GetPath());
        }
        /// <summary>
        /// Update path based on one source connector and one sink point (in most case, where the mouse is at)
        /// </summary>
        /// <param name="sink"></param>
        public void UpdatePath(Point sink) {
            IPathFinder pathFinder = new OrthogonalPathFinder {
                Connection = contentObject,
                CurrentDiagram = Project.CurrentProject.Children.FindByCanvas(ContainerCanvas)
            };
            contentObject.Stops.Clear();
            contentObject.Stops.AddRange(pathFinder.GetPath(sink));
        }
        /// <summary>
        /// Update path based on two points
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sink"></param>
        public void UpdatePath(Point source, Point sink) {
            IPathFinder pathFinder = new OrthogonalPathFinder {
                CurrentDiagram = Project.CurrentProject.Children.FindByCanvas(ContainerCanvas)
            };
            contentObject.Stops.Clear();
            contentObject.Stops.AddRange(pathFinder.GetPath(source, sink));
        }
        #endregion

        #region Constuctor
        public ConnectionItem() {
            ContentObject = new Connection();
            Init();
        }
        public ConnectionItem(Connection conn) {
            ContentObject = conn;
            Init();
        }
        private void Init() {
            ContentObject.Control = this;
        }
        #endregion
    }
}

/*
 * Extends connection
 */
namespace CIS681.Fall2012.VDS.Data.Objects {
    public partial class Connection : IControl<ConnectionItem> {
        public ConnectionItem Control { get; set; }
        partial void RefreshControl() {
            // create corresponding connection item
            new ConnectionItem(this);
        }
    }
}