// PointDrawer.cs

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp.Utilities
{
    public static class PointDrawer
    {
        /// <summary>
        /// Creates a visual point at specified coordinates
        /// </summary>
        /// <param name="x">X position in application units</param>
        /// <param name="y">Y position in application units</param>
        /// <param name="color">Point color</param>
        /// <param name="size">Size in application units (default: 1 unit)</param>
        private static Shape CreatePoint(Brush color, double size = 1.0)
        {
            var pixelScale = MainWindow.PixelScale; // Get the pixel scale from the main window
            return new Rectangle()
            {
                Width = size * pixelScale,   // Convert to pixels
                Height = size * pixelScale, // Convert to pixels
                Fill = color,
            };
        }

        /// <summary>
        /// Adds a point directly to a canvas
        /// </summary>
        public static void DrawPoint(Canvas canvas, Point point, Brush? color = null, double size = 1.0)
        {
            if (color == null)
                color = Brushes.Black;
            
            var shape = CreatePoint(color, size);
            
            var convertedPoint = CoordinateConverter.Convert2DToCanvas(canvas, point.X, point.Y);
            Canvas.SetLeft(shape, convertedPoint.X);
            Canvas.SetTop(shape, convertedPoint.Y);
            canvas.Children.Add(shape);
        }
    }
}