using System;
using System.Windows.Media.Media3D;
using WpfApp.ThreeDimension.Models;
using WpfApp.ThreeDimension; // For TransformationMatrix3D

namespace WpfApp.ThreeDimension.Animations;

public class RotateAnimation3D : Animation3D
{
    private Vector3D _axis; // Axis of rotation
    private double _totalAngle; // Total angle in degrees
    private double _anglePerMillisecond; // Pre-calculated angle increment per millisecond
    private Point3D _centerOfRotation; // Center around which to rotate

    public RotateAnimation3D(ShapeContainer3D shape, Vector3D axis, double totalAngle, TimeSpan duration, Point3D centerOfRotation) : base(shape, duration)
    {
        _axis = axis;
        _totalAngle = totalAngle;
        _anglePerMillisecond = totalAngle / duration.TotalMilliseconds;
        _centerOfRotation = centerOfRotation;
    }

    protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
    {
        // To rotate around a point other than the origin, we translate to the origin, rotate, then translate back.
        Shape.TransformShape(p =>
        {
            // 1. Translate point to origin relative to the center of rotation
            var translatedToOrigin = p - _centerOfRotation;
            var translatedToOriginPoint = new Point3D(translatedToOrigin.X, translatedToOrigin.Y, translatedToOrigin.Z);

            // 2. Rotate around the origin (with the specified axis and delta angle)
            var rotatedPoint = TransformationMatrix3D.RotateAroundAxis(translatedToOriginPoint, _axis,
                _anglePerMillisecond * deltaTime.TotalMilliseconds);

            // 3. Translate back
            return rotatedPoint + (Vector3D)_centerOfRotation;
        });
    }
}