using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Shapes;

public class Cube : ShapeContainer3D
{
    public Cube(CartesianCanvas3D canvas, Point3D bottomLeftBackPoint, double size, Color color, double thickness = 0.1)
    {
        // Define the 8 corners of the cube
        var points = new Point3DCollection
        {
            bottomLeftBackPoint, // Bottom-left-back
            new Point3D(bottomLeftBackPoint.X + size, bottomLeftBackPoint.Y, bottomLeftBackPoint.Z), // Bottom-right-back
            new Point3D(bottomLeftBackPoint.X + size, bottomLeftBackPoint.Y + size, bottomLeftBackPoint.Z), // Top-right-back
            new Point3D(bottomLeftBackPoint.X, bottomLeftBackPoint.Y + size, bottomLeftBackPoint.Z), // Top-left-back
            new Point3D(bottomLeftBackPoint.X, bottomLeftBackPoint.Y, bottomLeftBackPoint.Z + size), // Bottom-left-front
            new Point3D(bottomLeftBackPoint.X + size, bottomLeftBackPoint.Y, bottomLeftBackPoint.Z + size), // Bottom-right-front
            new Point3D(bottomLeftBackPoint.X + size, bottomLeftBackPoint.Y + size, bottomLeftBackPoint.Z + size), // Top-right-front
            new Point3D(bottomLeftBackPoint.X, bottomLeftBackPoint.Y + size, bottomLeftBackPoint.Z + size) // Top-left-front
        };

        // Define the edges of the cube using LineSegment3D
        Segments.Add(canvas.AddLine(points[0], points[1], color, thickness)); // Bottom edge back
        Segments.Add(canvas.AddLine(points[1], points[2], color, thickness)); // Right edge back
        Segments.Add(canvas.AddLine(points[2], points[3], color, thickness)); // Top edge back
        Segments.Add(canvas.AddLine(points[3], points[0], color, thickness)); // Left edge back
        Segments.Add(canvas.AddLine(points[4], points[5], color, thickness)); // Bottom edge front
        Segments.Add(canvas.AddLine(points[5], points[6], color, thickness)); // Right edge front
        Segments.Add(canvas.AddLine(points[6], points[7], color, thickness)); // Top edge front
        Segments.Add(canvas.AddLine(points[7], points[4], color, thickness)); // Left edge front
        Segments.Add(canvas.AddLine(points[0], points[4], color, thickness)); // Back to front left edge
        Segments.Add(canvas.AddLine(points[1], points[5], color, thickness)); // Back to front right edge
        Segments.Add(canvas.AddLine(points[2], points[6], color, thickness)); // Back to front top edge
        Segments.Add(canvas.AddLine(points[3], points[7], color, thickness)); // Back to front bottom edge
    }
}