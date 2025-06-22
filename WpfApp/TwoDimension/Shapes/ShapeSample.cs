using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Samples;
using WpfApp.TwoDimension.Shapes;

namespace WpfApp.TwoDimension.Samples
{
    public static class ShapeSample
    {
       // coin
        public static Circle CreateCoin(CartesianCanvas canvas, Point center, double radius)
        {
            return new Circle(
                canvas,
                center,
                radius,
                strokeColor: Brushes.Gold,
                thickness: 1,
                fillColor: Brushes.Gold
            );
        }

        // heart
        public static Heart CreateHeart(CartesianCanvas canvas, Point center, double size)
        {
            return new Heart(
                canvas,
                center,
                size,
                strokeColor: Brushes.Red,
                thickness: 1,
                fillColor: Brushes.Red
            );
        }

        // character
        public static Character1 CreateCharacter(CartesianCanvas canvas, Point center, double bodyRadius)
        {
            return new Character1(canvas, center, bodyRadius);
        }
    }
}


//TEST OBJ

//var character = new Character1(canvas2D, new Point(10, 8), 8);
//var coin = ShapeSample.CreateCoin(canvas2D, new Point(-8, 5), 3);
//var heart = ShapeSample.CreateHeart(canvas2D, new Point(0, 5), 6);

