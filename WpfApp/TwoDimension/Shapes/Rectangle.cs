using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Rectangle : ShapeContainer
{
    public Rectangle(CartesianCanvas canvas, Point topLeft, Point bottomRight, Brush strokeColor, double thickness = 1, Brush? fillColor = null)
    {
        if (topLeft.X > bottomRight.X || topLeft.Y < bottomRight.Y)
        {
            throw new ArgumentException("Invalid rectangle coordinates: top-left must be less than bottom-right.");
        }

        var topRight = new Point(bottomRight.X, topLeft.Y);
        var bottomLeft = new Point(topLeft.X, bottomRight.Y);
        
        var line1 = canvas.AddLine(topLeft, topRight, strokeColor, thickness);
        var line2 = canvas.AddLine(topRight, bottomRight, strokeColor, thickness);
        var line3 = canvas.AddLine(bottomRight, bottomLeft, strokeColor, thickness);
        var line4 = canvas.AddLine(bottomLeft, topLeft, strokeColor, thickness);

        // Add lines to the container
        Segments.Add(line1);
        Segments.Add(line2);
        Segments.Add(line3);
        Segments.Add(line4);
        
        // Optionally fill the rectangle
        if (fillColor == null) return;
        
        // Fill the shape
        var points = new PointCollection
        {
            topLeft,
            topRight,
            bottomRight,
            bottomLeft
        };
        
        var polygon = new FillSegment(points, fillColor, 1, fillColor);
        canvas.AddFill(polygon);
        Segments.Add(polygon);
    }
}