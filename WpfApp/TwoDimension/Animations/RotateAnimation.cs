using System.Windows;
using WpfApp.TwoDimension.Models;

// For TransformationMatrix

namespace WpfApp.TwoDimension.Animations;

public class RotateAnimation : Animation2D
{
    private double _totalAngle; // Total angle in degrees
    private double _anglePerMillisecond; // Pre-calculated angle increment per millisecond

    public RotateAnimation(ShapeContainer shape, double totalAngle, TimeSpan duration) : base(shape, duration)
    {
        _totalAngle = totalAngle;
        _anglePerMillisecond = totalAngle / duration.TotalMilliseconds;
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // Apply the delta rotation for this time step around the specified center
        Shape.TransformShape(p => TransformationMatrix.Rotate(p,
            _anglePerMillisecond * deltaTime.TotalMilliseconds));
    }
}