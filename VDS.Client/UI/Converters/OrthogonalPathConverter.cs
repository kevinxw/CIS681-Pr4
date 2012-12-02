/*
 * Convert points into one path geometry
 */

//#define DEBUG_ON

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CIS681.Fall2012.VDS.UI.Converters {
    [ValueConversion(typeof(Point[]), typeof(Geometry))]
    public class OrthogonalPathConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            Point[] points = (Point[])value;
            // draw nothing when there is less than two point
            if (points == null || points.Length < 2) return null;
            PathFigure figure = new PathFigure();
            figure.StartPoint = points[0];
            figure.IsClosed = false;
            for (int i = 1; i < points.Length; i++)
                figure.Segments.Add(new LineSegment(points[i], true));
#if DEBUG_ON
            System.Console.WriteLine("{0} OrthogonalPathConverter: points ", System.DateTime.Now.Millisecond);
            foreach (Point p in points)
                System.Console.Write("{0} ", p.ToString());
#endif
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
