using System;
using System.Windows;
using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Animations
{
    public class LoopScaleAnimation : Animation2D
    {
        private readonly double _minScaleX;
        private readonly double _minScaleY;
        private readonly double _maxScaleX;
        private readonly double _maxScaleY;
        private bool _reversing = false;
        private readonly bool _autoReverse;

        private double _lastScaleX = 1.0;
        private double _lastScaleY = 1.0;

        private readonly TimeSpan _loopDuration;
        private readonly TimeSpan _maxDuration;
        private TimeSpan _totalElapsed = TimeSpan.Zero;

        public LoopScaleAnimation(ShapeContainer shape,
            double minScaleX, double minScaleY,
            double maxScaleX, double maxScaleY,
            TimeSpan loopDuration,
            TimeSpan maxDuration,
            bool autoReverse = true)
            : base(shape, loopDuration)
        {
            _minScaleX = minScaleX;
            _minScaleY = minScaleY;
            _maxScaleX = maxScaleX;
            _maxScaleY = maxScaleY;
            _autoReverse = autoReverse;

            _loopDuration = loopDuration;
            _maxDuration = maxDuration;
        }

        public override bool IsFinished => _totalElapsed >= _maxDuration;

        public override void Update(TimeSpan deltaTime)
        {
            if (IsFinished) return;

            _elapsedTime += deltaTime;
            _totalElapsed += deltaTime;

            if (_elapsedTime >= _loopDuration)
            {
                _elapsedTime = TimeSpan.Zero;
                if (_autoReverse)
                    _reversing = !_reversing;
            }

            double progress = _elapsedTime.TotalMilliseconds / _loopDuration.TotalMilliseconds;
            ApplyTransformation(progress, deltaTime);
        }


        protected override void ApplyTransformation(double progress, TimeSpan deltaTime)
        {
            double scaleX = _reversing
                ? Lerp(_maxScaleX, _minScaleX, progress)
                : Lerp(_minScaleX, _maxScaleX, progress);

            double scaleY = _reversing
                ? Lerp(_maxScaleY, _minScaleY, progress)
                : Lerp(_minScaleY, _maxScaleY, progress);

            double scaleFactorX = scaleX / _lastScaleX;
            double scaleFactorY = scaleY / _lastScaleY;

            Shape.TransformShape(p =>
                TransformationMatrix.Scale(p, scaleFactorX, scaleFactorY, Shape.GetCenter()));

            _lastScaleX = scaleX;
            _lastScaleY = scaleY;
        }

        private double Lerp(double from, double to, double t)
        {
            return from + (to - from) * t;
        }
    }
}
