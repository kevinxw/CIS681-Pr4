/*
 * An interface every path finder should implement
 */

//////
/// Nothing to be TESTED here!
//////

using System.Collections.Generic;
using System.Windows;
using CIS681.Fall2012.VDS.Data.Objects;
using CIS681.Fall2012.VDS.Data;

namespace CIS681.Fall2012.VDS.UI.PathFinder {
    internal interface IPathFinder {
        Diagram CurrentDiagram { get; set; }    // current diagram
        Connection Connection { get; set; } // current connection

        List<Point> GetPath();
        List<Point> GetPath(Point endPoint, ConnectorType endPointType = ConnectorType.Center);
        List<Point> GetPath(Point source, Point sink);
    }
}
