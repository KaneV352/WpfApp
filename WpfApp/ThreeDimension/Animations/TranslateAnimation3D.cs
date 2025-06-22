using System;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;
using WpfApp.ThreeDimension; // For TransformationMatrix3D

namespace WpfApp.ThreeDimension.Animations;

public class TranslateAnimation3D : Animation3D
{
    private Vector3D _totalTranslation; // Total translation to achieve
    private Vector3D _translationPerMillisecond; // Pre-calculated translation increment per millisecond

    public TranslateAnimation3D(ShapeContainer3D shape, Vector3D totalTranslation, TimeSpan duration) : base(shape, duration)
    {
        _totalTranslation = totalTranslation;
        _translationPerMillisecond = new Vector3D(
            totalTranslation.X / duration.TotalMilliseconds,
            totalTranslation.Y / duration.TotalMilliseconds,
            totalTranslation.Z / duration.TotalMilliseconds
        );
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // Apply the delta translation for this time step
        Shape.TransformShape(p => TransformationMatrix3D.Translate(p,
            new Vector3D(_translationPerMillisecond.X * deltaTime.TotalMilliseconds,
                _translationPerMillisecond.Y * deltaTime.TotalMilliseconds,
                _translationPerMillisecond.Z * deltaTime.TotalMilliseconds)));
    }
}