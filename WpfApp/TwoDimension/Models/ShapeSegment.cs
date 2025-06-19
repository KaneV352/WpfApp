using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WpfApp.TwoDimension.Models;

public abstract class ShapeSegment : INotifyPropertyChanged
{
    private List<Point> _worldPoints = new List<Point>();
    public Brush? Fill { get; set; }
    public Brush Stroke { get; set; }
    public double StrokeThickness { get; set; }
    
    public List<Point> WorldPoints
    {
        get => _worldPoints;
        set
        {
            if (_worldPoints != value)
            {
                _worldPoints = value;
                OnPropertyChanged(nameof(WorldPoints));
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected void OnPropertyChanged(string name) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    
    // Helper method to transform all points
    public void TransformPoints(Func<Point, Point> transformation, bool notify = false)
    {
        var newPoints = WorldPoints.Select(transformation).ToList();
        WorldPoints = newPoints;
        if (notify)
        {
            OnPropertyChanged(nameof(WorldPoints));
        }
    }
}