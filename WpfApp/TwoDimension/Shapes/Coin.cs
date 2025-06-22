using System.Windows;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Coin : ShapeContainer
{
    public Circle OuterCircle { get; }
    public Circle InnerCircle { get; }
    
    public Coin(CartesianCanvas canvas, Point center, double radius)
    {
        OuterCircle = new Circle(
            canvas,
            center,
            radius,
            strokeColor: System.Windows.Media.Brushes.Gold,
            thickness: 1,
            fillColor: System.Windows.Media.Brushes.Gold
        );

        InnerCircle = new Circle(
            canvas,
            center,
            radius * 0.7, // Inner circle is smaller
            strokeColor: System.Windows.Media.Brushes.DarkGoldenrod,
            thickness: 1,
            fillColor: System.Windows.Media.Brushes.DarkGoldenrod
        );
        
        Segments.AddRange(OuterCircle.Segments);
        Segments.AddRange(InnerCircle.Segments);
    }
}