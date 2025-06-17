using System.Windows.Media;

namespace WpfApp.TwoDimension.Models;

public class FillSegment : ShapeSegment
{
    public FillSegment(PointCollection points, Brush stroke, double thickness, Brush fill)
    {
        WorldPoints = points.ToList();
        Stroke = stroke;
        StrokeThickness = thickness;
        Fill = fill;
    }
}