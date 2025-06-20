using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Shapes;

public class Cuboid : ShapeContainer3D
{
    public Cuboid(CartesianCanvas3D canvas, Point3D bottomLeftBack, double width, double height, double depth, Color color, double thickness = 0.1)
    {
        var points = new Point3DCollection
        {
            bottomLeftBack,
            new(bottomLeftBack.X + width, bottomLeftBack.Y, bottomLeftBack.Z),
            new(bottomLeftBack.X + width, bottomLeftBack.Y + height, bottomLeftBack.Z),
            new(bottomLeftBack.X, bottomLeftBack.Y + height, bottomLeftBack.Z),
            new(bottomLeftBack.X, bottomLeftBack.Y, bottomLeftBack.Z + depth),
            new(bottomLeftBack.X + width, bottomLeftBack.Y, bottomLeftBack.Z + depth),
            new(bottomLeftBack.X + width, bottomLeftBack.Y + height, bottomLeftBack.Z + depth),
            new(bottomLeftBack.X, bottomLeftBack.Y + height, bottomLeftBack.Z + depth)
        };

        Segments.Add(canvas.AddLine(points[0], points[1], color, thickness));
        Segments.Add(canvas.AddLine(points[1], points[2], color, thickness));
        Segments.Add(canvas.AddLine(points[2], points[3], color, thickness));
        Segments.Add(canvas.AddLine(points[3], points[0], color, thickness));

        Segments.Add(canvas.AddLine(points[4], points[5], color, thickness));
        Segments.Add(canvas.AddLine(points[5], points[6], color, thickness));
        Segments.Add(canvas.AddLine(points[6], points[7], color, thickness));
        Segments.Add(canvas.AddLine(points[7], points[4], color, thickness));

        Segments.Add(canvas.AddLine(points[0], points[4], color, thickness));
        Segments.Add(canvas.AddLine(points[1], points[5], color, thickness));
        Segments.Add(canvas.AddLine(points[2], points[6], color, thickness));
        Segments.Add(canvas.AddLine(points[3], points[7], color, thickness));
    }
}
