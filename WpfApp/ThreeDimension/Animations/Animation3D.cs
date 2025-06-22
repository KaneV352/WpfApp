using WpfApp.ThreeDimension.Models;

namespace WpfApp.ThreeDimension.Animations;

public abstract class Animation3D
{
    public ShapeContainer3D Shape { get; private set; }
    protected TimeSpan _duration;
    protected TimeSpan _elapsedTime;

    public bool IsFinished => _elapsedTime >= _duration;

    public Animation3D(ShapeContainer3D shape, TimeSpan duration)
    {
        Shape = shape;
        _duration = duration;
        _elapsedTime = TimeSpan.Zero;
    }

    public void Update(TimeSpan deltaTime)
    {
        if (IsFinished) return;
        _elapsedTime += deltaTime;
        double progress = Math.Min(1.0, _elapsedTime.TotalMilliseconds / _duration.TotalMilliseconds);
        ApplyTransformation(progress, deltaTime);
    }

    protected abstract void ApplyTransformation(double progress, TimeSpan deltaTime);
}