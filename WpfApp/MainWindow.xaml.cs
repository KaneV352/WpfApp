using System.Windows;
using System.Windows.Media;

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
        canvas.AddPoint(new Point(2, 3), Brushes.Red);    // Point at (2,3)
        canvas.AddPoint(new Point(-1, -2), Brushes.Blue, 10);  // Point at (-1,-2)
        
        // Draw lines
        canvas.AddLine(new Point(0, 0), new Point(5, 5), Brushes.Green);  // Line from origin to (5,5)
        canvas.AddLine(new Point(-3, 4), new Point(2, -1), Brushes.Purple, 2); // Custom line
    }
}