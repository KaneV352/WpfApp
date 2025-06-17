using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Circle : ShapeContainer
{
    public Circle(CartesianCanvas canvas, Point center, double radius, Brush strokeColor, double thickness = 1, Brush? fillColor = null)
    {
        if (radius <= 0)
        {
            throw new ArgumentException("Radius must be greater than zero.");
        }

        var points = MidpointCircleAlgorithm(center, radius);

        foreach (var point in points)
        {
            Segments.Add(canvas.AddPoint(point, strokeColor, thickness));
        }

        if (fillColor == null) return;

        // Fill the shape
        var polygon = new FillSegment(points, fillColor, thickness, fillColor);
        canvas.AddFill(polygon);
        Segments.Add(polygon);
    }
    
    private PointCollection MidpointCircleAlgorithm(Point center, double radius)
    {
        var points = new List<Point>();
        double x = radius;
        double y = 0;
        double decision = 1 - radius;

        // Add the initial points (the four poles)
        points.Add(new Point(center.X + radius, center.Y));
        points.Add(new Point(center.X - radius, center.Y));
        points.Add(new Point(center.X, center.Y + radius));
        points.Add(new Point(center.X, center.Y - radius));

        while (x > y)
        {
            y++;
            
            // Midpoint is inside or on the perimeter?
            if (decision <= 0)
            {
                decision += 2 * y + 1;
            }
            else 
            {
                x--;
                decision += 2 * (y - x) + 1;
            }

            // All the perimeter points have not been added yet
            if (x < y)
                break;

            // Calculate points for all eight octants
            double offsetX, offsetY;
            
            // Top-right quadrant
            offsetX = x; offsetY = y;
            AddSymmetricPoints(points, center, offsetX, offsetY);
            
            // Bottom-right quadrant
            offsetX = y; offsetY = x;
            AddSymmetricPoints(points, center, offsetX, offsetY);
        }
        
        // Sort points by their angle around the center (ascending order)
        points.Sort((p1, p2) =>
        {
            double angle1 = Math.Atan2(p1.Y - center.Y, p1.X - center.X);
            double angle2 = Math.Atan2(p2.Y - center.Y, p2.X - center.X);
            return angle1.CompareTo(angle2);
        });

        return new PointCollection(points);
    }

    private void AddSymmetricPoints(List<Point> points, Point center, double x, double y)
    {
        // Calculate points for all four quadrants
        points.Add(new Point(center.X + x, center.Y + y));
        points.Add(new Point(center.X - x, center.Y + y));
        points.Add(new Point(center.X + x, center.Y - y));
        points.Add(new Point(center.X - x, center.Y - y));
    }
}