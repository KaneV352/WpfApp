using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Rectangle : ShapeContainer
{
    private CartesianCanvas canvas2D;
    private Point point;
    private double v1;
    private double v2;
    private SolidColorBrush green;
    private int v3;
    private SolidColorBrush lightGreen;

    public Rectangle(CartesianCanvas canvas, Point topLeft, double v, Point bottomRight, Brush strokeColor, double thickness = 1, Brush? fillColor = null)
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

    public Rectangle(CartesianCanvas canvas2D, Point point, double v1, double v2, SolidColorBrush green, int v3, SolidColorBrush lightGreen)
    {
        this.canvas2D = canvas2D;
        this.point = point;
        this.v1 = v1;
        this.v2 = v2;
        this.green = green;
        this.v3 = v3;
        this.lightGreen = lightGreen;
    }
}