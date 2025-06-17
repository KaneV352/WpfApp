using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Shapes;

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
        
        // canvas3d.AddPoint(new Point3D(0,0,0), Colors.Red, 0.2);
    }
    
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // var circle = new Circle(canvas, new Point(0, 0), 10, Brushes.Aqua, 0.05, Brushes.LightGreen);
        var triangle = new Triangle(canvas, new Point(0, 0), new Point(10, 0), new Point(0, 10), Brushes.Orange, 0.05, Brushes.LightYellow);
        
        // Draw cube lines
        // canvas3d.AddLine(new Point3D(0, 0, 0), new Point3D(1, 0, 0), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(0, 0, 0), new Point3D(0, 1, 0), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(0, 0, 0), new Point3D(0, 0, 1), Colors.Blue, 0.05);
        //
        // canvas3d.AddLine(new Point3D(1, 0, 0), new Point3D(1, 1, 0), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(1, 0, 0), new Point3D(1, 0, 1), Colors.Blue, 0.05);
        //
        // canvas3d.AddLine(new Point3D(0, 1, 0), new Point3D(1, 1, 0), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(0, 1, 0), new Point3D(0, 1, 1), Colors.Blue, 0.05);
        //
        // canvas3d.AddLine(new Point3D(0, 0, 1), new Point3D(1, 0, 1), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(0, 0, 1), new Point3D(0, 1, 1), Colors.Blue, 0.05);
        //
        // canvas3d.AddLine(new Point3D(1, 1, 0), new Point3D(1, 1, 1), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(1, 0, 1), new Point3D(1, 1, 1), Colors.Blue, 0.05);
        // canvas3d.AddLine(new Point3D(0, 1, 1), new Point3D(1, 1, 1), Colors.Blue, 0.05);
    }
    
    private void TranslateShapes(double tx, double ty)
    {
        foreach (var segment in _allSegments)
        {
            segment.TransformShape(p => TransformationMatrix.Translate(p, tx, ty));
        }
    }
    
    // private void AddPoint_Click(object sender, RoutedEventArgs e)
    // {
    //     // Add a random colored point at a random position
    //     var random = new Random();
    //     var color = Color.FromRgb(
    //         (byte)random.Next(256), 
    //         (byte)random.Next(256), 
    //         (byte)random.Next(256));
    //     
    //     canvas3d.AddPoint(
    //         new Point3D(
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4),
    //         color,
    //         0.2 + random.NextDouble() * 0.3);
    // }
    //
    // private void AddLine_Click(object sender, RoutedEventArgs e)
    // {
    //     // Add a random colored line between two random points
    //     var random = new Random();
    //     var color = Color.FromRgb(
    //         (byte)random.Next(256), 
    //         (byte)random.Next(256), 
    //         (byte)random.Next(256));
    //     
    //     canvas3d.AddLine(
    //         new Point3D(
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4),
    //         new Point3D(
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4,
    //             random.NextDouble() * 8 - 4),
    //         color,
    //         0.02 + random.NextDouble() * 0.08);
    // }
    //
    // private void Clear_Click(object sender, RoutedEventArgs e)
    // {
    //     canvas3d.ClearContent();
    // }
}