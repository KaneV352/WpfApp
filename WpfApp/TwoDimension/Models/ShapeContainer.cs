using System.Windows;

namespace WpfApp.TwoDimension.Models;

public class ShapeContainer
{
    public List<ShapeSegment> Segments { get; private set; } = new List<ShapeSegment>();
    
    public void TransformShape(Func<Point, Point> transformation)
    {
        for (int i = 0; i < Segments.Count; i++)
        {
            Segments[i].TransformPoints(transformation, i == Segments.Count - 1);
        }
    }
}