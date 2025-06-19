using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfApp.ThreeDimension.Models;

public abstract class ShapeSegment3D
{
    private List<Point3D> _worldPoints = new List<Point3D>();
    public List<Point3D> WorldPoints
    {
        get { return _worldPoints; }
        set
        {
            _worldPoints = value;
        }
    }
    
    private Brush _color;
    public Brush Color
    {
        get { return _color; }
        set
        {
            _color = value;
            OnPropertyChanged(); // Notify when color changes
        }
    }
    
    private double _thickness;
    public double Thickness
    {
        get { return _thickness; }
        set
        {
            _thickness = value;
            OnPropertyChanged(); // Notify when thickness changes
        }
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler PropertyChanged;

    // Protected method to raise the PropertyChanged event
    // Use CallerMemberName to automatically fill the property name
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    // Helper method to transform all points in the segment
    public void TransformPoints(Func<Point3D, Point3D> transformation, bool notify = false)
    {
        var newPoints = WorldPoints.Select(transformation).ToList();
        WorldPoints = newPoints;
        if (notify)
            OnPropertyChanged(); // Notify when world points change
    }
}