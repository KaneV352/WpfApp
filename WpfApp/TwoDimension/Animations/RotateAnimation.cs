using System.Windows;
using WpfApp.TwoDimension.Models;

// For TransformationMatrix

namespace WpfApp.TwoDimension.Animations;

public class RotateAnimation : Animation2D
{
    private double _totalAngle; // Total angle in degrees
    private double _anglePerMillisecond; // Pre-calculated angle increment per millisecond
    private Func<Point> _centerOfRotationSource; // Center around which to rotate

    public RotateAnimation(ShapeContainer shape, double totalAngle, TimeSpan duration, Func<Point> centerOfRotationSource) : base(shape, duration)
    {
        _totalAngle = totalAngle;
        _anglePerMillisecond = totalAngle / duration.TotalMilliseconds;
        _centerOfRotationSource = centerOfRotationSource ?? throw new ArgumentNullException(nameof(centerOfRotationSource));
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // Apply the delta rotation for this time step around the specified center
        var centerOfRotation = _centerOfRotationSource();
        Shape.TransformShape(p => TransformationMatrix.RotateAround(p, centerOfRotation,
            _anglePerMillisecond * deltaTime.TotalMilliseconds));
    }
}