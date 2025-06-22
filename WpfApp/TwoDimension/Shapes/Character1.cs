using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Character1 : ShapeContainer
{
    public Circle Body { get; private set; }
    public Circle Head { get; private set; }
    public Triangle LeftEye { get; private set; }
    public Triangle RightEye { get; private set; }
    public Rectangle LeftLeg { get; private set; }
    public Rectangle RightLeg { get; private set; }
    public Rectangle LeftArm { get; private set; }
    public Rectangle RightArm { get; private set; }

    public Character1(CartesianCanvas canvas, Point center, double bodyRadius)
    {
        double headRadius = bodyRadius / 2;

        // Body
        Body = new Circle(canvas, center, bodyRadius, Brushes.Yellow, 1, Brushes.Yellow);
        Segments.AddRange(Body.Segments);

        // Head
        var headCenter = new Point(center.X, center.Y + bodyRadius + headRadius * 0.3);
        Head = new Circle(canvas, headCenter, headRadius, Brushes.Orange, 1, Brushes.Orange);
        Segments.AddRange(Head.Segments);

        // Eyes
        double eyeSize = headRadius * 0.4;
        double eyeOffsetX = headRadius * 0.5;
        double eyeOffsetY = headRadius * 0.1;

        var leftEyeTop = new Point(headCenter.X - eyeOffsetX, headCenter.Y + eyeOffsetY);
        var leftEyeLeft = new Point(leftEyeTop.X - eyeSize / 2, leftEyeTop.Y - eyeSize);
        var leftEyeRight = new Point(leftEyeTop.X + eyeSize / 2, leftEyeTop.Y - eyeSize);
        LeftEye = new Triangle(canvas, leftEyeLeft, leftEyeRight, leftEyeTop, Brushes.DarkBlue, 1, Brushes.DarkBlue);
        Segments.AddRange(LeftEye.Segments);

        var rightEyeTop = new Point(headCenter.X + eyeOffsetX, headCenter.Y + eyeOffsetY);
        var rightEyeLeft = new Point(rightEyeTop.X - eyeSize / 2, rightEyeTop.Y - eyeSize);
        var rightEyeRight = new Point(rightEyeTop.X + eyeSize / 2, rightEyeTop.Y - eyeSize);
        RightEye = new Triangle(canvas, rightEyeLeft, rightEyeRight, rightEyeTop, Brushes.DarkBlue, 1, Brushes.DarkBlue);
        Segments.AddRange(RightEye.Segments);

        // Legs
        var legHeight = bodyRadius * 0.6;
        LeftLeg = new Rectangle(
            canvas,
            new Point(center.X - bodyRadius * 0.5, center.Y - bodyRadius),
            bodyRadius * 0.3,
            new Point(center.X - bodyRadius * 0.2, center.Y - bodyRadius - legHeight),
            Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(LeftLeg.Segments);

        RightLeg = new Rectangle(
            canvas,
            new Point(center.X + bodyRadius * 0.2, center.Y - bodyRadius),
            bodyRadius * 0.3,
            new Point(center.X + bodyRadius * 0.5, center.Y - bodyRadius - legHeight),
            Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(RightLeg.Segments);

        // Arms
        var armHeight = bodyRadius * 0.6;
        var armWidth = bodyRadius * 0.2;

        LeftArm = new Rectangle(
            canvas,
            new Point(center.X - bodyRadius, center.Y),
            armWidth,
            new Point(center.X - bodyRadius + armWidth, center.Y - armHeight),
            Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(LeftArm.Segments);

        RightArm = new Rectangle(
            canvas,
            new Point(center.X + bodyRadius - armWidth, center.Y),
            armWidth,
            new Point(center.X + bodyRadius, center.Y - armHeight),
            Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(RightArm.Segments);
    }
}