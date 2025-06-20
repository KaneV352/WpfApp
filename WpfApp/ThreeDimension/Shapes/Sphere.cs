using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Shapes;

public class Sphere : ShapeContainer3D
{
    public Sphere(CartesianCanvas3D canvas, Point3D center, double radius, Color color, double thickness = 0.05, int segments = 24)
    {
        // Vòng tròn theo trục XY
        for (int i = 0; i < segments; i++)
        {
            double angle1 = 2 * Math.PI * i / segments;
            double angle2 = 2 * Math.PI * (i + 1) / segments;

            var p1 = new Point3D(center.X + radius * Math.Cos(angle1), center.Y + radius * Math.Sin(angle1), center.Z);
            var p2 = new Point3D(center.X + radius * Math.Cos(angle2), center.Y + radius * Math.Sin(angle2), center.Z);
            Segments.Add(canvas.AddLine(p1, p2, color, thickness));

            // Vòng tròn theo trục YZ
            var p3 = new Point3D(center.X, center.Y + radius * Math.Cos(angle1), center.Z + radius * Math.Sin(angle1));
            var p4 = new Point3D(center.X, center.Y + radius * Math.Cos(angle2), center.Z + radius * Math.Sin(angle2));
            Segments.Add(canvas.AddLine(p3, p4, color, thickness));

            // Vòng tròn theo trục XZ
            var p5 = new Point3D(center.X + radius * Math.Cos(angle1), center.Y, center.Z + radius * Math.Sin(angle1));
            var p6 = new Point3D(center.X + radius * Math.Cos(angle2), center.Y, center.Z + radius * Math.Sin(angle2));
            Segments.Add(canvas.AddLine(p5, p6, color, thickness));
        }
    }
}
