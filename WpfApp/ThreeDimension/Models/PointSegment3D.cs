using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp.ThreeDimension.Models;

public class PointSegment3D : ShapeSegment3D
{
    public Point3D Position => WorldPoints[0];

    private Color _color;
    public Color Color
    {
        get { return _color; }
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged();
            }
        }
    }

    private double _size;
    public double Size
    {
        get { return _size; }
        set
        {
            if (_size != value)
            {
                _size = value;
                OnPropertyChanged();
            }
        }
    }

    public PointSegment3D(Point3D position, Color color, double size)
    {
        Color = color;
        Size = size;
        
        WorldPoints = new List<Point3D> { position }; // Initialize with the position
    }
}
