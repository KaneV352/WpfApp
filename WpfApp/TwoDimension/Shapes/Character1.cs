using System.Windows;
using System.Windows.Media;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Shapes;

public class Character1 : ShapeContainer
{
    public Circle Body { get; private set; }
    public Circle Head { get; private set; }
    public ShapeContainer LeftEye { get; set; }
    public ShapeContainer RightEye { get; set; }
    public Eclipse LeftLeg { get; private set; }
    public Eclipse RightLeg { get; private set; }
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

        var leftEyePoint = new Point(headCenter.X - eyeOffsetX, headCenter.Y + eyeOffsetY);
        LeftEye = new Heart(canvas, leftEyePoint, eyeSize, Brushes.Red, 1, Brushes.Red);
        Segments.AddRange(LeftEye.Segments);

        var rightEyePoint = new Point(headCenter.X + eyeOffsetX, headCenter.Y + eyeOffsetY);
        RightEye = new Heart(canvas, rightEyePoint, eyeSize, Brushes.Red, 1, Brushes.Red);
        Segments.AddRange(RightEye.Segments);

        // Calculate leg dimensions
        double legHeight = bodyRadius * 0.6;
        double legWidth = bodyRadius * 0.4;
        
        // Calculate leg centers
        var leftLegCenter = new Point(center.X - legWidth, center.Y - bodyRadius - legHeight / 2);
        var rightLegCenter = new Point(center.X + legWidth, center.Y - bodyRadius - legHeight / 2);

// Create legs
        LeftLeg = new Eclipse(canvas, leftLegCenter, legWidth, legHeight, Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(LeftLeg.Segments);

        RightLeg = new Eclipse(canvas, rightLegCenter, legWidth, legHeight, Brushes.DarkGreen, 1, Brushes.DarkGreen);
        Segments.AddRange(RightLeg.Segments);

        // Arms
        var armHeight = bodyRadius * 0.6;
        var armWidth = bodyRadius * 0.2;

        var leftArmTopLeft = new Point(center.X - bodyRadius, center.Y);
        var leftArmBottomRight = new Point(leftArmTopLeft.X + armWidth, leftArmTopLeft.Y - armHeight);
        LeftArm = new Rectangle(canvas, leftArmTopLeft, leftArmBottomRight, Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(LeftArm.Segments);

        var rightArmTopLeft = new Point(center.X + bodyRadius - armWidth, center.Y);
        var rightArmBottomRight = new Point(rightArmTopLeft.X + armWidth, rightArmTopLeft.Y - armHeight);
        RightArm = new Rectangle(canvas, rightArmTopLeft, rightArmBottomRight, Brushes.DarkRed, 1, Brushes.DarkRed);
        Segments.AddRange(RightArm.Segments);
    }
}
