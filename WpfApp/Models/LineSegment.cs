using System.Windows;
using System.Windows.Media;

namespace WpfApp.Models;

public class LineSegment : ShapeSegment
{
    public Brush Stroke { get; }
    public double Thickness { get; }
    
    public Point WorldStart => WorldPoints[0];
    
    public Point WorldEnd => WorldPoints[1];
    
    public LineSegment(Point start, Point end, Brush stroke, double thickness = 1)
    {
        WorldPoints = new List<Point> { start, end };
        Stroke = stroke;
        Thickness = thickness;
    }
}