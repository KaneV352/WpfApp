using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp.Utilities;
using Point = WpfApp.Utilities.Point;

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
        this.Loaded += OnLoaded;
        this.SizeChanged += OnSizeChanged;
    }
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Now draw the axes
        CreateAxis();
        LineDrawer.DrawLine(MainCanvas, new Point(0, 0), new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2), Brushes.Green, PixelScale, 1);
    }
    
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Clear the canvas and redraw axes on size change
        MainCanvas.Children.Clear();
        CreateAxis();
        LineDrawer.DrawLine(MainCanvas, new Point(0, 0), new Point(MainCanvas.ActualWidth / 2, MainCanvas.ActualHeight / 2), Brushes.Green, PixelScale, 1);
    }

    private void CreateAxis()
    {
        var width = (int)MainCanvas.ActualWidth;
        var height = (int)MainCanvas.ActualHeight;
        var midWidth = width / 2;
        var midHeight = height / 2;
        
        // Draw the sub lines
        for (int i = PixelScale; i < midWidth; i += PixelScale)
        {
            LineDrawer.DrawLine(MainCanvas, new Point(i, midHeight), new Point(i, -midHeight), Brushes.Gray, 1, 0.2);
        }
        LineDrawer.DrawLine(MainCanvas, new Point(midWidth, midHeight), new Point(midWidth, -midHeight), Brushes.Gray, 1, 0.2);
        for (int i = -PixelScale; i > -midWidth; i -= PixelScale)
        {
            LineDrawer.DrawLine(MainCanvas, new Point(i, midHeight), new Point(i, -midHeight), Brushes.Gray, 1, 0.2);
        }
        LineDrawer.DrawLine(MainCanvas, new Point(-midWidth, midHeight), new Point(-midWidth, -midHeight), Brushes.Gray, 1, 0.2);
        
        for (int i = PixelScale; i < midHeight; i += PixelScale)
        {
            LineDrawer.DrawLine(MainCanvas, new Point(midWidth, i), new Point(-midWidth, i), Brushes.Gray, 1, 0.2);
        }
        LineDrawer.DrawLine(MainCanvas, new Point(midWidth, midHeight), new Point(-midWidth, midHeight), Brushes.Gray, 1, 0.2);
        for (int i = -PixelScale; i > -midHeight; i -= PixelScale)
        {
            LineDrawer.DrawLine(MainCanvas, new Point(midWidth, i), new Point(-midWidth, i), Brushes.Gray, 1, 0.2);
        }
        LineDrawer.DrawLine(MainCanvas, new Point(midWidth, -midHeight), new Point(-midWidth, -midHeight), Brushes.Gray, 1, 0.2);
        
        // Draw the main axes
        LineDrawer.DrawLine(MainCanvas, new Point(0, midHeight), new Point(0, -midHeight), Brushes.Red, 1, 1);
        LineDrawer.DrawLine(MainCanvas, new Point(-midWidth, 0), new Point(midWidth, 0), Brushes.Red, 1, 1);
    }
}