using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp.ThreeDimension.Models;

public class LineSegment3D : ShapeSegment3D
{
    public Point3D StartPoint => WorldPoints[0];

    public Point3D EndPoint => WorldPoints[1];
    
    private double _thickness;
    public double Thickness
    {
        get { return _thickness; }
        set
        {
            if (_thickness != value)
            {
                _thickness = value;
                OnPropertyChanged();
            }
        }
    }

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

    private int _numSegments; // For parametric interpolation density
    public int NumSegments
    {
        get { return _numSegments; }
        set
        {
            if (_numSegments != value)
            {
                _numSegments = value;
                OnPropertyChanged();
            }
        }
    }

    public LineSegment3D(Point3D start, Point3D end, Color color, int numSegments, double thickness)
    {
        Color = color;
        NumSegments = numSegments;
        Thickness = thickness;
        
        WorldPoints = new List<Point3D> { start, end }; // Initialize with start and end points
    }
}