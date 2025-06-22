using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Heart : ShapeContainer
{
    public Heart(CartesianCanvas canvas, Point center, double size, Brush strokeColor, double thickness = 2, Brush? fillColor = null)
    {
        double radius = size / 2;

        var leftCenter = new Point(center.X - radius / 1.2, center.Y + radius / 2.6);
        var rightCenter = new Point(center.X + radius / 1.2, center.Y + radius / 2.6);

        // Các nửa hình tròn
        var leftCircle = new Circle(canvas, leftCenter, radius / 1.2, strokeColor, thickness, fillColor);
        var rightCircle = new Circle(canvas, rightCenter, radius / 1.2, strokeColor, thickness, fillColor);

        // triangle
        var bottom = new Point(center.X, center.Y - radius * 1.6);
        var left = new Point(center.X - radius * 1.5, center.Y);
        var right = new Point(center.X + radius * 1.5, center.Y);


        var triangle = new Triangle(canvas, left, right, bottom, strokeColor, thickness, fillColor);

        // Gộp segment
        Segments.AddRange(leftCircle.Segments);
        Segments.AddRange(rightCircle.Segments);
        Segments.AddRange(triangle.Segments);
    }
}

