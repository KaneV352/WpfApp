using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Animations;

public class ScaleAnimation : Animation2D
{
    private readonly double _scaleXPerMillisecond;
    private readonly double _scaleYPerMillisecond;

    public ScaleAnimation(ShapeContainer shape, double scaleX, double scaleY, TimeSpan duration)
        : base(shape, duration)
    {
        // Calculate the scale increment per millisecond
        _scaleXPerMillisecond = (scaleX - 1) / duration.TotalMilliseconds;
        _scaleYPerMillisecond = (scaleY - 1) / duration.TotalMilliseconds;
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // Calculate the scale factor for this time step
        double scaleX = 1 + _scaleXPerMillisecond * deltaTime.TotalMilliseconds;
        double scaleY = 1 + _scaleYPerMillisecond * deltaTime.TotalMilliseconds;

        // Apply the scaling transformation to the shape
        Shape.TransformShape(p => TransformationMatrix.Scale(p, scaleX, scaleY, Shape.GetCenter()));
    }
}