using System.Windows;
using System.Windows.Media;
using WpfApp.Models;

namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public const int PixelScale = 5;

    public MainWindow()
    {
        InitializeComponent();
        // Defer canvas sizing and axis creation until the window is loaded
        this.Loaded += MainWindow_Loaded;
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Draw points
        var shape1 = new ShapeContainer();
        shape1.Segments.Add(canvas.AddPoint(new Point(2, 3), Brushes.Red));    // Point at (2,3)
        shape1.Segments.Add(canvas.AddPoint(new Point(-1, -2), Brushes.Blue, 10));  // Point at (-1,-2)
        
        // Draw lines
        var shape2 = new ShapeContainer();
        shape2.Segments.Add(canvas.AddLine(new Point(0, 0), new Point(5, 5), Brushes.Green));  // Line from origin to (5,5)
        shape2.Segments.Add(canvas.AddLine(new Point(-3, 4), new Point(2, -1), Brushes.Purple, 2)); // Custom line
    }
}