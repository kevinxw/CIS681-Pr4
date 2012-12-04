/*
 * This is a simple algorithm of calculating orthogonal path..  may be imperfect
 */

//#define DEBUG_ON

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using CIS681.Fall2012.VDS.Data.Client;
using CIS681.Fall2012.VDS.UI.Objects;
using CIS681.Fall2012.VDS.Data;

namespace CIS681.Fall2012.VDS.UI.PathFinder {
    internal class OrthogonalPathFinder : IPathFinder {

        #region Properties
        /// <summary>
        /// The margin need to be leave blank for every model
        /// </summary>
        public double EdgeMargin { get; set; }

        /// <summary>
        /// The connection used to draw path
        /// </summary>
        public Connection Connection { get; set; }

        /// <summary>
        /// Current diagram
        /// </summary>
        private Diagram currentDiagram;
        public Diagram CurrentDiagram {
            get { return currentDiagram; }
            set { currentDiagram = value; }
        }

        #endregion

        #region Constructors
        public OrthogonalPathFinder() {
            EdgeMargin = 15;
        }
        #endregion

        #region Inflate Rects with Margin
        /// <summary>
        /// Get a Rect list in which all rects are expanded with EdgeMargin
        /// </summary>
        /// <returns></returns>
        private List<Rect> inflatedRects = null;
        private List<Rect> InflatedRects {
            get {
                if (inflatedRects != null) return inflatedRects;
                List<Rect> rects = new List<Rect>();
                CurrentDiagram.Models.ForEach(item => rects.Add(GetInflatedRect(item)));
                return inflatedRects = rects;
            }
        }

        /// <summary>
        /// Get an inflated rect by Model
        /// </summary>
        /// <param name="connectorThumb"></param>
        /// <returns></returns>
        private Rect GetInflatedRect(Model model) {
            Rect rect = new Rect(model.Control.Position, model.Control.Size);
            rect.Inflate(EdgeMargin, EdgeMargin);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} rect inflated size : {1} before", System.DateTime.Now.Millisecond, rect.Size.ToString(), model.Control.Size.ToString());
#endif
            return rect;
        }
        #endregion

        /// <summary>
        /// Calculate new start point and new end point
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static Point GetOffsetPoint(Connector connector, Rect rect) {
            // by default the offset point is the center of rect
            Point offsetPoint = rect.Location + new Vector(rect.Width / 2, rect.Height / 2);
            // move one pixel here for each so that it will not intersect with inflated rects
            switch (connector.Type) {
                case ConnectorType.Left:
                    offsetPoint = new Point(rect.Left - 1, connector.Position.Y);
                    break;
                case ConnectorType.Top:
                    offsetPoint = new Point(connector.Position.X, rect.Top - 1);
                    break;
                case ConnectorType.Right:
                    offsetPoint = new Point(rect.Right + 1, connector.Position.Y);
                    break;
                case ConnectorType.Bottom:
                    offsetPoint = new Point(connector.Position.X, rect.Bottom + 1);
                    break;
                case ConnectorType.Center:
                default:
                    break;
            }
            return offsetPoint;
        }

        #region Find Path
        /// <summary>
        /// Get all stop points in the path
        /// </summary>
        /// <returns></returns>
        public List<Point> GetPath() {
            // basic information necessary to generate path
            if (this.Connection.Sink != null) {
                Rect sinkRect = GetInflatedRect(this.Connection.Sink.Owner);
                Point endPoint = GetOffsetPoint(this.Connection.Sink, sinkRect);
                return GetPath(endPoint, this.Connection.Sink.Type);
            }
            return GetPath(this.Connection.SinkPosition);
        }
        /// <summary>
        /// Get the path with one connector and one point
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="endPointType">When this is set to ConnectorType.Center, means there is no restriction actually</param>
        /// <returns></returns>
        public List<Point> GetPath(Point endPoint, ConnectorType endPointType = ConnectorType.Center) {
            List<Point> stops = new List<Point>();
            Point startPoint = Connection.SourcePosition;
            ConnectorType startPointType = ConnectorType.Center;
            // if source position and sink position are not valid
            if (double.IsNaN(startPoint.X) || double.IsNaN(startPoint.Y) || double.IsNaN(endPoint.X) || double.IsNaN(endPoint.Y))
                return stops;
            if (Connection.Source != null) {
                startPointType = Connection.Source.Type;
                Rect sourceRect = GetInflatedRect(Connection.Source.Owner);
                startPoint = GetOffsetPoint(Connection.Source, sourceRect);
            }
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Generating path, start {1} startType {2} end {3} endType {4}", System.DateTime.Now.Millisecond, startPoint.ToString(), startPointType, endPoint.ToString(), endPointType);
#endif
            // add first point, notice this is not the position of source point
            stops.Add(startPoint);
            List<Rect> conflicts = GetConflictedRects(startPoint, endPoint);
            // get the mega rect, whose edge is the path exactly
            Rect megaRect = UnionRects(conflicts);
            megaRect.Union(startPoint); megaRect.Union(endPoint);
            // check if start point or end point is one the edge, if not, we need to add some more stops
            Point startEdgeP = AlignPoint(startPoint, megaRect, startPointType);
            if (!startEdgeP.Equals(startPoint))
                stops.Add(startEdgeP);
            // check end point
            Point endEdgeP = AlignPoint(endPoint, megaRect, endPointType);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} startEdge {1} endEdge {2}", System.DateTime.Now.Millisecond, startEdgeP.ToString(), endEdgeP.ToString());
#endif
            // add two nearest corner point into the stops
            stops.AddRange(DecideCheapestPath(startEdgeP, endEdgeP, megaRect, startPointType, endPointType));
            if (!endEdgeP.Equals(endPoint))
                stops.Add(endEdgeP);
            // add last point
            stops.Add(endPoint);
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} Final result", System.DateTime.Now.Millisecond);
            foreach (Point p in stops)
                System.Console.Write("{0} ", p.ToString());
#endif
            OptimizeStops(stops);
            return stops;
        }
        /// <summary>
        /// A simple way to get a path with only one stop
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sink"></param>
        /// <returns></returns>
        public List<Point> GetPath(Point source, Point sink) {
            // there will be only one stop
            Point p1 = new Point(source.X, sink.Y), p2 = new Point(sink.X, source.Y);
            // do hit test when current diagram is not null
            HitTestResult hitRes = null;
            if (CurrentDiagram != null)
                hitRes = VisualTreeHelper.HitTest(CurrentDiagram.Control, p1);
            List<Point> path;
            if (hitRes != null && !(hitRes.VisualHit is ModelItem))
                path = new List<Point> { source, p1, sink };
            else // when p1 test is not successful, take p2 anyway
                path = new List<Point> { source, p2, sink };
            OptimizeStops(path);
            return path;
        }
        #endregion

        /// <summary>
        /// Delete the second point if there are three points on one same edge.
        /// </summary>
        /// <param name="stops">The result will take effect to this parameter itself</param>
        /// <returns></returns>
        private static void OptimizeStops(List<Point> stops) {
            if (stops.Count < 3) return;
            for (int i = 2; i < stops.Count; ) {
                Vector v1 = stops[i] - stops[i - 1], v2 = stops[i - 1] - stops[i - 2];
                if ((v1.X == 0 && v2.X == 0) || (v1.Y == 0 && v2.Y == 0))
                    stops.RemoveAt(i - 1);
                else i++;
            }
        }

        /// <summary>
        /// Check if the point is any edge of the rect
        /// </summary>
        /// <param name="point">The Point should be within the rectangle</param>
        /// <param name="rect"></param>
        /// <returns>if it is not on the edge, return the nearest point, otherwise return itself</returns>
        private static Point AlignPoint(Point point, Rect rect, ConnectorType orientation = ConnectorType.Center) {
            //do a simple mathematics, if (point-rect.lt) has a coordinate of 0, bingo!, same with (rect.br-point)
            Vector v1 = point - rect.TopLeft, v2 = rect.BottomRight - point;
            if (v1.X == 0 || v1.Y == 0 || v2.X == 0 || v2.Y == 0) return point;
            // top, left, right, bottom
            Point[] points = new Point[] { new Point(point.X, rect.Top), new Point(rect.Left, point.Y), new Point(rect.Right, point.Y), new Point(point.X, rect.Bottom) };
            // calculate the nearest point, no backwards
            if (v1.Length < v2.Length) {
                switch (orientation) {
                    case ConnectorType.Right: return points[0];
                    case ConnectorType.Top: return points[1];
                }
                return v1.X < v1.Y ? points[1] : points[0];
            }
            else {
                switch (orientation) {
                    case ConnectorType.Left: return points[3];
                    case ConnectorType.Bottom: return points[2];
                }
                return v2.X < v2.Y ? points[2] : points[3];
            }
        }

        /// <summary>
        /// Decide which route is shorter, and return the corner of the route
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rect"></param>
        /// <param name="startType"></param>
        /// <param name="endType"></param>
        /// <returns></returns>
        private static Point[] DecideCheapestPath(Point start, Point end, Rect rect, ConnectorType startType = ConnectorType.Center, ConnectorType endType = ConnectorType.Center) {
            IEnumerable<Point> p1 = GetNearbyCorners(start, rect);
            IEnumerable<Point> p2 = GetNearbyCorners(end, rect);
            // if they share one corner, then return the shared corner immediately
            foreach (Point _p1 in p1)
                foreach (Point _p2 in p2)
                    if (_p1.Equals(_p2)) return new Point[] { _p2 };
            Point[][] directions = new Point[][]{
                new Point[] { rect.TopLeft, rect.TopRight },    // top left -> right
                new Point[] { rect.BottomLeft, rect.BottomRight },  // bottom left -> right
                new Point[] { rect.TopLeft, rect.BottomLeft },  // left top -> bottom
                new Point[] { rect.TopRight, rect.BottomRight },    // right top -> bottom
                new Point[] { rect.TopRight, rect.TopLeft },    // top right-> left
                new Point[] { rect.BottomRight, rect.BottomLeft }, // bottom right -> left
                new Point[] { rect.BottomLeft, rect.TopLeft },  // left bottom -> top
                new Point[] { rect.BottomRight, rect.TopRight }    // right bottom -> top
            };
            // they are on two opposite edges, decide where edgePoint[0] is, then we know where edgePoint[1] is
            Vector v1 = start - rect.TopLeft, v2 = rect.BottomRight - start;
#if DEBUG_ON
            // test value
            System.Console.WriteLine("{0} cheapest path v1 {0} v2 {1}", System.DateTime.Now.Millisecond, v1.ToString(), v2.ToString());
#endif
            // choose the path, no turn back
            if (v1.X == 0)  // left edge
                switch (startType) {
                    case ConnectorType.Top: return directions[0];
                    case ConnectorType.Bottom: return directions[1];
                    default:
                        double path1 = (start - rect.TopLeft).LengthSquared + (end - rect.TopRight).LengthSquared;
                        double path2 = (start - rect.BottomLeft).LengthSquared + (end - rect.BottomRight).LengthSquared;
                        return path1 > path2 ? directions[0] : directions[1];
                }
            else if (v1.Y == 0)
                switch (startType) {// top
                    case ConnectorType.Left: return directions[2];
                    case ConnectorType.Right: return directions[3];
                    default:
                        double path1 = (start - rect.TopLeft).LengthSquared + (end - rect.BottomLeft).LengthSquared;
                        double path2 = (start - rect.TopRight).LengthSquared + (end - rect.BottomRight).LengthSquared;
                        return path1 > path2 ? directions[2] : directions[3];
                }
            else if (v2.X == 0)
                switch (startType) {// right
                    case ConnectorType.Top: return directions[4];
                    case ConnectorType.Bottom: return directions[5];
                    default:
                        double path1 = (start - rect.TopRight).LengthSquared + (end - rect.TopLeft).LengthSquared;
                        double path2 = (start - rect.BottomRight).LengthSquared + (end - rect.BottomLeft).LengthSquared;
                        return path1 > path2 ? directions[4] : directions[5];
                }
            else
                switch (startType) {// (v2.Y==0)   // bottom
                    case ConnectorType.Left: return directions[6];
                    case ConnectorType.Right: return directions[7];
                    default:
                        double path1 = (start - rect.BottomLeft).LengthSquared + (end - rect.TopLeft).LengthSquared;
                        double path2 = (start - rect.BottomRight).LengthSquared + (end - rect.TopRight).LengthSquared;
                        return path1 > path2 ? directions[6] : directions[7];
                }
        }

        /// <summary>
        /// Get the nearest corner related to one point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        private static IEnumerable<Point> GetNearbyCorners(Point point, Rect rect) {
            if (point.X == rect.Left) { yield return rect.TopLeft; yield return rect.BottomLeft; }
            else if (point.X == rect.Right) { yield return rect.TopRight; yield return rect.BottomRight; }
            if (point.Y == rect.Top) { yield return rect.TopLeft; yield return rect.TopRight; }
            else if (point.Y == rect.Bottom) { yield return rect.BottomLeft; yield return rect.BottomRight; }
        }

        #region Get Conflicted Rects
        /// <summary>
        /// Return a list of rects which is conflicted with the path
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rects"></param>
        /// <returns></returns>
        private static List<Rect> GetConflictedRects(Point start, Point end, List<Rect> rects, bool ignoreStartEndConflict = false) {
            Rect pathRect = new Rect(start, end);
            List<Rect> conflicts = rects.FindAll(item => item.IntersectsWith(pathRect));
            if (ignoreStartEndConflict)
                return conflicts.FindAll(item => !item.Contains(start) && !item.Contains(end));
            return conflicts;
        }
        /// <summary>
        /// Get conflicted rects based on current diagram's model list
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private List<Rect> GetConflictedRects(Point start, Point end, bool ignoreStartEndConflict = false) {
            return GetConflictedRects(start, end, InflatedRects, ignoreStartEndConflict);
        }
        #endregion

        #region Get Intersected Rects
        /// <summary>
        /// Union all rects into one
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="rects"></param>
        /// <returns></returns>
        private static Rect UnionRects(List<Rect> rects) {
            Rect megaRect = Rect.Empty;
            rects.ForEach(item => megaRect.Union(item));
            return megaRect;
        }
        #endregion
    }

}
