using System;
using System.Windows;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Animations
{
    public class LoopRotateAnimation : Animation2D
    {
        private readonly double _anglePerMillisecond;
        private readonly Func<Point> _centerOfRotationSource;
        private readonly bool _autoReverse;

        private double _direction = 1;

        public LoopRotateAnimation(
            ShapeContainer shape,
            double totalAngle,
            TimeSpan duration,
            Func<Point> centerOfRotationSource,
            bool autoReverse = true)
            : base(shape, duration)
        {
            _anglePerMillisecond = totalAngle / duration.TotalMilliseconds;
            _centerOfRotationSource = centerOfRotationSource;
            _autoReverse = autoReverse;
        }

        public override bool IsFinished => false;

        public new void Update(TimeSpan deltaTime)
        {
            _elapsedTime += deltaTime;
            if (_elapsedTime >= _duration)
            {
                _elapsedTime = TimeSpan.Zero;
                if (_autoReverse)
                    _direction *= -1;
            }

            ApplyTransformation(0, deltaTime);
        }

        protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
        {
            var center = _centerOfRotationSource();
            double deltaAngle = _direction * _anglePerMillisecond * deltaTime.TotalMilliseconds;
            Shape.TransformShape(p => TransformationMatrix.RotateAround(p, center, deltaAngle));
        }
    }
}
