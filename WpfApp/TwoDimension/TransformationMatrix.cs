using System.Windows;
using System.Windows.Media;

namespace WpfApp.TwoDimension;

public static class TransformationMatrix
{
    public static Point Rotate(Point point, double angle)
    {
        Matrix rotationMatrix = new Matrix();
        rotationMatrix.Rotate(angle);
        return rotationMatrix.Transform(point);
    }
    
    public static Point RotateAround(Point point, Point center, double angle)
    {
        Matrix rotationAtPoint = new Matrix();
        rotationAtPoint.RotateAt(angle, center.X, center.Y);
        return rotationAtPoint.Transform(point);
    }
    
    public static Point Scale(Point point, double scaleX, double scaleY, Point center)
    {
        Matrix scaleMatrix = new Matrix();
        // Translate the center to the origin
        scaleMatrix.Translate(-center.X, -center.Y);
        // Apply scaling
        scaleMatrix.Scale(scaleX, scaleY);
        // Translate back from the origin
        scaleMatrix.Translate(center.X, center.Y);
        return scaleMatrix.Transform(point);
    }
    
    public static Point Translate(Point point, double offsetX, double offsetY)
    {
        Matrix translationMatrix = new Matrix();
        translationMatrix.Translate(offsetX, offsetY);
        return translationMatrix.Transform(point);
    }
    
    public static Point Symmetric(Point point, Point symmetricPoint)
    {
        Matrix symmetricMatrix = new Matrix();
        symmetricMatrix.Translate(-symmetricPoint.X, -symmetricPoint.Y);
        symmetricMatrix.Scale(-1, -1);
        symmetricMatrix.Translate(symmetricPoint.X, symmetricPoint.Y);
        return symmetricMatrix.Transform(point);
    }
}