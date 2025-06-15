using System.ComponentModel;
using System.Windows;

namespace WpfApp.Models;

public abstract class ShapeSegment : INotifyPropertyChanged
{
    public List<Point> WorldPoints { get; set; } = new List<Point>();
    
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}