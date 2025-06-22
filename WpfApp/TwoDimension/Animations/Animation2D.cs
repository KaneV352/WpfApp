using WpfApp.TwoDimension.Models;

namespace WpfApp.TwoDimension.Animations;

public abstract class Animation2D
{
    public ShapeContainer Shape { get; private set; }
    protected TimeSpan _duration;
    protected TimeSpan _elapsedTime;

    //public bool IsFinished => _elapsedTime >= _duration;
    public virtual bool IsFinished => _elapsedTime >= _duration;


    public Animation2D(ShapeContainer shape, TimeSpan duration)
    {
        Shape = shape;
        _duration = duration;
        _elapsedTime = TimeSpan.Zero;
    }

    public virtual void Update(TimeSpan deltaTime)
    {
        if (IsFinished) return;
        _elapsedTime += deltaTime;
        double progress = Math.Min(1.0, _elapsedTime.TotalMilliseconds / _duration.TotalMilliseconds);
        ApplyTransformation(progress, deltaTime);
    }

    protected abstract void ApplyTransformation(double progress, TimeSpan deltaTime);
}