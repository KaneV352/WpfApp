using System.Windows;
using System.Windows.Media;
using WpfApp.Models;

namespace WpfApp.Shapes;

public class Eclipse : ShapeContainer
{
    public Eclipse(CartesianCanvas canvas, Point center, double radiusX, double radiusY, Brush stroke, double thickness = 1, Brush? fill = null)
    {
        var points = MidpointEllipseAlgorithm(center, radiusX, radiusY);
         
        foreach (var point in points)
        {
            Segments.Add(canvas.AddPoint(point, stroke, thickness));
        }
        
        if (fill == null) return;
        
        // Fill the shape
        var polygon = new FillSegment(points, stroke, thickness, fill);
        canvas.AddFill(polygon);
        Segments.Add(polygon);
    }
    
    private static PointCollection MidpointEllipseAlgorithm(Point center, double a, double b)
    {
        var pointsList = new List<Point>();
        double x = 0;
        double y = b;
        double aSquared = a * a;
        double bSquared = b * b;

        // Region 1
        double d1 = bSquared - aSquared * b + 0.25 * aSquared;
        while (bSquared * (x + 1) < aSquared * (y - 0.5))
        {
            // Add 4 symmetric points to the list
            pointsList.Add(new Point(center.X + x, center.Y + y));
            pointsList.Add(new Point(center.X - x, center.Y + y));
            pointsList.Add(new Point(center.X + x, center.Y - y));
            pointsList.Add(new Point(center.X - x, center.Y - y));

            if (d1 < 0)
            {
                d1 += bSquared * (2 * x + 3);
            }
            else
            {
                d1 += bSquared * (2 * x + 3) + aSquared * (-2 * y + 2);
                y--;
            }
            x++;
        }

        // Region 2
        double d2 = bSquared * (x + 0.5) * (x + 0.5)
                  + aSquared * (y - 1) * (y - 1)
                  - aSquared * bSquared;
        while (y >= 0)
        {
            // Add 4 symmetric points to the list
            pointsList.Add(new Point(center.X + x, center.Y + y));
            pointsList.Add(new Point(center.X - x, center.Y + y));
            pointsList.Add(new Point(center.X + x, center.Y - y));
            pointsList.Add(new Point(center.X - x, center.Y - y));

            if (d2 < 0)
            {
                d2 += bSquared * (2 * x + 2) + aSquared * (-2 * y + 3);
                x++;
            }
            else
            {
                d2 += aSquared * (-2 * y + 3);
            }
            y--;
        }

        // Sort points by their angle around the center (ascending order)
        pointsList.Sort((p1, p2) =>
        {
            double angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
            double angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);
            return angle1.CompareTo(angle2);
        });

        return new PointCollection(pointsList);

    }
}