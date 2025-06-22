using System.Windows.Media.Media3D;

namespace WpfApp.ThreeDimension.Models;

public class ShapeContainer3D
{
    public List<ShapeSegment3D> Segments { get; private set; } = new List<ShapeSegment3D>();
    
    public void TransformShape(Func<Point3D, Point3D> transformation)
    {
        for (int i = 0; i < Segments.Count; i++)
        {
            Segments[i].TransformPoints(transformation, i == Segments.Count - 1);
        }
    }
    
    public Point3D GetCenter()
    {
        var allPoints = Segments.SelectMany(s => s.WorldPoints).ToList();
        if (!allPoints.Any()) return new Point3D(0, 0, 0);

        double avgX = allPoints.Average(p => p.X);
        double avgY = allPoints.Average(p => p.Y);
        double avgZ = allPoints.Average(p => p.Z);
        return new Point3D(avgX, avgY, avgZ);
    }
}