using System.Windows;
using System.Windows.Media;

namespace WpfApp.Models;

public class PointSegment : ShapeSegment
{
    private Point _worldPoint;
    private Brush _fill;
    private double _size;

    public PointSegment(Point worldPoint, Brush fill, double size)
    {
        _worldPoint = worldPoint;
        _fill = fill;
        _size = size;
        
        WorldPoints.Add(worldPoint);
    }

    public Point WorldPoint
    {
        get => _worldPoint;
        set { _worldPoint = value; OnPropertyChanged(nameof(WorldPoint)); }
    }

    public Brush Fill
    {
        get => _fill;
        set { _fill = value; OnPropertyChanged(nameof(Fill)); }
    }

    public double Size
    {
        get => _size;
        set { _size = value; OnPropertyChanged(nameof(Size)); }
    }
}