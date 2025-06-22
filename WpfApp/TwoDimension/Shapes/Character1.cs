using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Character1 : ShapeContainer
{
    public Character1(CartesianCanvas canvas, Point center, double bodyRadius)
    {
        double headRadius = bodyRadius / 2;

        // Body
        var body = new Circle(canvas, center, bodyRadius, Brushes.Yellow, 1, Brushes.Yellow);
        Segments.AddRange(body.Segments);

        // Head
        var headCenter = new Point(center.X, center.Y + bodyRadius + headRadius * 0.3);
        var head = new Circle(canvas, headCenter, headRadius, Brushes.Orange, 1, Brushes.Orange);
        Segments.AddRange(head.Segments);

        // Eyes (2 triangle eyes pointing down)
        double eyeSize = headRadius * 0.4;
        double eyeOffsetX = headRadius * 0.5;
        double eyeOffsetY = headRadius * 0.1;

        var leftEyeTop = new Point(headCenter.X - eyeOffsetX, headCenter.Y + eyeOffsetY);
        var leftEyeLeft = new Point(leftEyeTop.X - eyeSize / 2, leftEyeTop.Y - eyeSize);
        var leftEyeRight = new Point(leftEyeTop.X + eyeSize / 2, leftEyeTop.Y - eyeSize);
        var leftEye = new Triangle(canvas, leftEyeLeft, leftEyeRight, leftEyeTop, Brushes.DarkBlue, 1, Brushes.DarkBlue);
        Segments.AddRange(leftEye.Segments);

        var rightEyeTop = new Point(headCenter.X + eyeOffsetX, headCenter.Y + eyeOffsetY);
        var rightEyeLeft = new Point(rightEyeTop.X - eyeSize / 2, rightEyeTop.Y - eyeSize);
        var rightEyeRight = new Point(rightEyeTop.X + eyeSize / 2, rightEyeTop.Y - eyeSize);
        var rightEye = new Triangle(canvas, rightEyeLeft, rightEyeRight, rightEyeTop, Brushes.DarkBlue, 1, Brushes.DarkBlue);
        Segments.AddRange(rightEye.Segments);

        // Legs
        var legHeight = bodyRadius * 0.6;
        var leftLeg = new Rectangle(
            canvas,
            new Point(center.X - bodyRadius * 0.5, center.Y - bodyRadius),
            new Point(center.X - bodyRadius * 0.2, center.Y - bodyRadius - legHeight),
            Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(leftLeg.Segments);

        var rightLeg = new Rectangle(
            canvas,
            new Point(center.X + bodyRadius * 0.2, center.Y - bodyRadius),
            new Point(center.X + bodyRadius * 0.5, center.Y - bodyRadius - legHeight),
            Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(rightLeg.Segments);

        // Arms (dọc, sát thân, hướng xuống)
        var armHeight = bodyRadius * 0.6;
        var armWidth = bodyRadius * 0.2;

        var leftArm = new Rectangle(
            canvas,
            new Point(center.X - bodyRadius, center.Y),
            new Point(center.X - bodyRadius + armWidth, center.Y - armHeight),
            Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(leftArm.Segments);

        var rightArm = new Rectangle(
            canvas,
            new Point(center.X + bodyRadius - armWidth, center.Y),
            new Point(center.X + bodyRadius, center.Y - armHeight),
            Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(rightArm.Segments);
    }
}
