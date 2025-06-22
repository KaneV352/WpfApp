using System.Windows;
using WpfApp.TwoDimension.Models;

// For TransformationMatrix

namespace WpfApp.TwoDimension.Animations;

public class TranslateAnimation2D : Animation2D
{
    private Point _totalTranslation; // Total translation to achieve
    private Point _translationPerMillisecond; // Pre-calculated translation increment per millisecond

    public TranslateAnimation2D(ShapeContainer shape, Point totalTranslation, TimeSpan duration) : base(shape, duration)
    {
        _totalTranslation = totalTranslation;
        _translationPerMillisecond = new Point(
            totalTranslation.X / duration.TotalMilliseconds,
            totalTranslation.Y / duration.TotalMilliseconds
        );
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // Apply the delta translation for this time step
        Shape.TransformShape(p => TransformationMatrix.Translate(p,
            _translationPerMillisecond.X * deltaTime.TotalMilliseconds,
            _translationPerMillisecond.Y * deltaTime.TotalMilliseconds));
    }
}