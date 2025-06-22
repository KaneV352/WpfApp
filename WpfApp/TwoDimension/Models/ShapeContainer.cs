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
    
    public Point GetCenter()
    {
        var allPoints = Segments
            .Where(s => s is not FillSegment)
            .SelectMany(s => s.WorldPoints)
            .ToList();
        if (!allPoints.Any()) return new Point(0, 0);

        double avgX = allPoints.Average(p => p.X);
        double avgY = allPoints.Average(p => p.Y);
        return new Point(avgX, avgY);
    }
}