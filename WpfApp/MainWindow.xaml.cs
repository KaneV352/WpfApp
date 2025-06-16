using System.Windows;
using System.Windows.Media;
using WpfApp.Models;
using WpfApp.Shapes;
using WpfApp.Utilities;

namespace WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly List<ShapeContainer> _allSegments = new List<ShapeContainer>();

    public MainWindow()
    {
        InitializeComponent();
        // Defer canvas sizing and axis creation until the window is loaded
        this.Loaded += MainWindow_Loaded;
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // // Draw points
        // var point1 = canvas.AddPoint(new Point(2, 3), Brushes.Red);
        // var point2 = canvas.AddPoint(new Point(-1, -2), Brushes.Blue, 10);
        // _allSegments.Add(point1);
        // _allSegments.Add(point2);
        //
        // // Draw lines
        // var line1 = canvas.AddLine(new Point(0, 0), new Point(5, 5), Brushes.Green);
        // var line2 = canvas.AddLine(new Point(-3, 4), new Point(2, -1), Brushes.Purple, 2);
        // _allSegments.Add(line1);
        // _allSegments.Add(line2);
        //
        // // Apply translation to all shapes
        // TranslateShapes(10, 10);
        
        // Draw a rectangle
        var rectangle = new Rectangle(canvas, new Point(-3, 10), new Point(3, 0), Brushes.Orange, 2,Brushes.LightYellow);
        _allSegments.Add(rectangle);
        
        var eclipse = new Eclipse(canvas, new Point(0, 0), 30, 20, Brushes.Pink, 2, Brushes.LightPink);
        
        TranslateShapes(10,-10);
    }
    
    private void TranslateShapes(double tx, double ty)
    {
        foreach (var segment in _allSegments)
        {
            segment.TransformShape(p => TransformationMatrix.Translate(p, tx, ty));
        }
    }
}