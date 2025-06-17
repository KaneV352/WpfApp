using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Triangle : ShapeContainer
{
    public Triangle(CartesianCanvas canvas, Point point1, Point point2, Point point3, Brush strokeColor, double thickness = 1, Brush? fillColor = null)
    {
        // // Check triangle inequality
        double a = (point1 - point2).Length;
        double b = (point2 - point3).Length;
        double c = (point3 - point1).Length;
        if (a + b <= c || b + c <= a || c + a <= b)
        {
            throw new ArgumentException("The given points do not form a valid triangle.");
        }
        
        // Create line segments for the triangle
        var line1 = canvas.AddLine(point1, point2, strokeColor, thickness);
        var line2 = canvas.AddLine(point2, point3, strokeColor, thickness);
        var line3 = canvas.AddLine(point3, point1, strokeColor, thickness);

        // Add lines to the container
        Segments.Add(line1);
        Segments.Add(line2);
        Segments.Add(line3);

        // Optionally fill the triangle
        if (fillColor == null) return;

        // Fill the shape
        var points = new PointCollection { point1, point2, point3 };
        var polygon = new FillSegment(points, fillColor, thickness, fillColor);
        canvas.AddFill(polygon);
        Segments.Add(polygon);
    }
}