using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Shapes;

public class Pyramid : ShapeContainer3D
{
    public Pyramid(CartesianCanvas3D canvas, Point3D baseOrigin, double width, double length, double height, Color color, double thickness = 0.05)
    {
        // 4 điểm đáy
        var p0 = baseOrigin;
        var p1 = new Point3D(baseOrigin.X + width, baseOrigin.Y, baseOrigin.Z);
        var p2 = new Point3D(baseOrigin.X + width, baseOrigin.Y, baseOrigin.Z + length);
        var p3 = new Point3D(baseOrigin.X, baseOrigin.Y, baseOrigin.Z + length);

        // Đỉnh
        var apex = new Point3D(baseOrigin.X + width / 2, baseOrigin.Y + height, baseOrigin.Z + length / 2);

        // Đáy
        Segments.Add(canvas.AddLine(p0, p1, color, thickness));
        Segments.Add(canvas.AddLine(p1, p2, color, thickness));
        Segments.Add(canvas.AddLine(p2, p3, color, thickness));
        Segments.Add(canvas.AddLine(p3, p0, color, thickness));

        // Cạnh bên
        Segments.Add(canvas.AddLine(p0, apex, color, thickness));
        Segments.Add(canvas.AddLine(p1, apex, color, thickness));
        Segments.Add(canvas.AddLine(p2, apex, color, thickness));
        Segments.Add(canvas.AddLine(p3, apex, color, thickness));
    }
}

