namespace WpfApp.Utilities;

public class Point
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }
    
    public static Point operator *(Point a, double scalar)
    {
        return new Point(a.X * scalar, a.Y * scalar);
    }
    
    public static Point operator /(Point a, double scalar)
    {
        if (scalar == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return new Point(a.X / scalar, a.Y / scalar);
    }
}