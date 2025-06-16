using System.Windows;

namespace WpfApp.Models;

public class ShapeContainer
{
    public List<ShapeSegment> Segments { get; private set; } = new List<ShapeSegment>();
    
    public void TransformShape(Func<Point, Point> transformation)
    {
        foreach (var segment in Segments)
        {
            segment.TransformPoints(transformation);
        }
    }
}