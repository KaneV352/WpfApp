using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension;
using WpfApp.ThreeDimension.Models;

public class Cuboid : ShapeContainer3D
{
    public Cuboid(CartesianCanvas3D canvas, Point3D origin, double width, double height, double depth, Color color, double thickness = 0.05)
    {
        var p0 = origin;
        var p1 = new Point3D(origin.X + width, origin.Y, origin.Z);
        var p2 = new Point3D(origin.X + width, origin.Y + height, origin.Z);
        var p3 = new Point3D(origin.X, origin.Y + height, origin.Z);

        var p4 = new Point3D(origin.X, origin.Y, origin.Z + depth);
        var p5 = new Point3D(origin.X + width, origin.Y, origin.Z + depth);
        var p6 = new Point3D(origin.X + width, origin.Y + height, origin.Z + depth);
        var p7 = new Point3D(origin.X, origin.Y + height, origin.Z + depth);

        Segments.Add(canvas.AddLine(p0, p1, color, thickness));
        Segments.Add(canvas.AddLine(p1, p2, color, thickness));
        Segments.Add(canvas.AddLine(p2, p3, color, thickness));
        Segments.Add(canvas.AddLine(p3, p0, color, thickness));

        Segments.Add(canvas.AddLine(p4, p5, color, thickness));
        Segments.Add(canvas.AddLine(p5, p6, color, thickness));
        Segments.Add(canvas.AddLine(p6, p7, color, thickness));
        Segments.Add(canvas.AddLine(p7, p4, color, thickness));

        Segments.Add(canvas.AddLine(p0, p4, color, thickness));
        Segments.Add(canvas.AddLine(p1, p5, color, thickness));
        Segments.Add(canvas.AddLine(p2, p6, color, thickness));
        Segments.Add(canvas.AddLine(p3, p7, color, thickness));
    }
}
