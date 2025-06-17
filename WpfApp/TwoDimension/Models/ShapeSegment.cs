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
                OnPointsChanged(); // New method to notify about changes
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? PointsChanged; // New event for point changes
    
    protected void OnPropertyChanged(string name) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    
    protected void OnPointsChanged() => 
        PointsChanged?.Invoke(this, EventArgs.Empty);
    
    // Helper method to transform all points
    public void TransformPoints(Func<Point, Point> transformation)
    {
        var newPoints = WorldPoints.Select(transformation).ToList();
        WorldPoints = newPoints;
    }
}