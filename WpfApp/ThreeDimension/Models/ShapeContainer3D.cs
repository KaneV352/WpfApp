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
}