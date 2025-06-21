using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Shapes;

public class Cylinder : ShapeContainer3D
{
    public Cylinder(CartesianCanvas3D canvas, Point3D baseCenter, double radius, double height, Color color, double thickness = 0.05, int segments = 24)
    {
        var topCenter = new Point3D(baseCenter.X, baseCenter.Y + height, baseCenter.Z);

        for (int i = 0; i < segments; i++)
        {
            double angle1 = 2 * Math.PI * i / segments;
            double angle2 = 2 * Math.PI * (i + 1) / segments;

            // Đáy dưới
            var p1 = new Point3D(baseCenter.X + radius * Math.Cos(angle1), baseCenter.Y, baseCenter.Z + radius * Math.Sin(angle1));
            var p2 = new Point3D(baseCenter.X + radius * Math.Cos(angle2), baseCenter.Y, baseCenter.Z + radius * Math.Sin(angle2));
            Segments.Add(canvas.AddLine(p1, p2, color, thickness));

            // Đáy trên
            var p3 = new Point3D(topCenter.X + radius * Math.Cos(angle1), topCenter.Y, topCenter.Z + radius * Math.Sin(angle1));
            var p4 = new Point3D(topCenter.X + radius * Math.Cos(angle2), topCenter.Y, topCenter.Z + radius * Math.Sin(angle2));
            Segments.Add(canvas.AddLine(p3, p4, color, thickness));

            // Cạnh nối đáy dưới – trên
            Segments.Add(canvas.AddLine(p1, p3, color, thickness));
        }
    }
}
