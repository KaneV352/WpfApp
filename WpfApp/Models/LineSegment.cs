using System.Windows;
using System.Windows.Media;

namespace WpfApp.Models;

public class LineSegment : ShapeSegment
{
    private Point _worldStart;
    private Point _worldEnd;
    private Brush _stroke;
    private double _thickness;
    
    public LineSegment(Point worldStart, Point worldEnd, Brush stroke, double thickness)
    {
        _worldStart = worldStart;
        _worldEnd = worldEnd;
        _stroke = stroke;
        _thickness = thickness;

        WorldPoints.Add(worldStart);
        WorldPoints.Add(worldEnd);
    }

    public Point WorldStart
    {
        get => _worldStart;
        set { _worldStart = value; OnPropertyChanged(nameof(WorldStart)); }
    }
    
    public Point WorldEnd
    {
        get => _worldEnd;
        set { _worldEnd = value; OnPropertyChanged(nameof(WorldEnd)); }
    }

    public Brush Stroke
    {
        get => _stroke;
        set { _stroke = value; OnPropertyChanged(nameof(Stroke)); }
    }
    
    public double Thickness
    {
        get => _thickness;
        set { _thickness = value; OnPropertyChanged(nameof(Thickness)); }
    }
}