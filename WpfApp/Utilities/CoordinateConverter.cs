using System.Windows.Controls;

namespace WpfApp.Utilities;

public class CoordinateConverter
{
    public static Point ConvertCanvasTo2D(Canvas canvas, double x, double y)
    {
        // Convert canvas coordinates to 2D coordinates
        double centerX = canvas.ActualWidth / 2;
        double centerY = canvas.ActualHeight / 2;

        // Adjust for the center of the canvas
        return new Point(x - centerX, centerY - y);
    }
    
    public static Point Convert2DToCanvas(Canvas canvas, double x, double y)
    {
        // Convert 2D coordinates back to canvas coordinates
        double centerX = canvas.ActualWidth / 2;
        double centerY = canvas.ActualHeight / 2;

        // Adjust for the center of the canvas
        return new Point(x + centerX, centerY - y);
    }
}