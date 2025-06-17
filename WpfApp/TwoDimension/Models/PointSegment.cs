using System.Windows;
using System.Windows.Media;

namespace WpfApp.TwoDimension.Models;

public class PointSegment : ShapeSegment
{
    public Brush Fill { get; }
    public double Size { get; }
    
    public Point WorldPoint => WorldPoints[0];
    
    public PointSegment(Point worldPoint, Brush fill, double size = 2)
    {
        WorldPoints = new List<Point> { worldPoint };
        Fill = fill;
        Size = size;
    }
}