using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading; // Required for DispatcherTimer
using WpfApp.TwoDimension.Animations; // For Animation2D
using WpfApp.ThreeDimension.Animations;
using WpfApp.ThreeDimension.Models;
using WpfApp.TwoDimension.Models; // For Animation3D

namespace WpfApp;

public class Animator
{
    private List<(Animation2D animation, TimeSpan startTime)> _scheduledAnimations2D = new();
    private List<(Animation3D animation, TimeSpan startTime)> _scheduledAnimations3D = new();
    private TimeSpan _currentTime = TimeSpan.Zero;
    private DispatcherTimer _timer;
    private const int _frameRate = 60; // frames per second

    public Animator()
    {
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000.0 / _frameRate);
        _timer.Tick += (sender, args) => OnTick();
    }

    /// <summary>
    /// Starts the animation timer.
    /// </summary>
    public void Start()
    {
        _timer.Start();
    }

    /// <summary>
    /// Stops the animation timer.
    /// </summary>
    public void Stop()
    {
        _timer.Stop();
    }

    /// <summary>
    /// Adds a 2D animation to the animator's schedule.
    /// </summary>
    /// <param name="animation">The 2D animation to add.</param>
    /// <param name="startTime">The time at which the animation should start relative to the animator's start.</param>
    public void AddAnimation(Animation2D animation, TimeSpan startTime)
    {
        _scheduledAnimations2D.Add((animation, startTime));
        _scheduledAnimations2D.Sort((a1, a2) => a1.startTime.CompareTo(a2.startTime)); // Keep animations sorted by start time
    }
    
    /// <summary>
    /// Adds a 3D animation to the animator's schedule.
    /// </summary>
    /// <param name="animation">The 3D animation to add.</param>
    /// <param name="startTime">The time at which the animation should start relative to the animator's start.</param>
    public void AddAnimation(Animation3D animation, TimeSpan startTime)
    {
        _scheduledAnimations3D.Add((animation, startTime));
        _scheduledAnimations3D.Sort((a1, a2) => a1.startTime.CompareTo(a2.startTime)); // Keep animations sorted by start time
    }

    private void OnTick()
    {
        TimeSpan frameDeltaTime = _timer.Interval; // This is the fixed time step for each frame
        _currentTime += frameDeltaTime;

        List<(Animation2D animation, TimeSpan startTime)> animations2DRemove = new();
        foreach (var (animation, startTime) in _scheduledAnimations2D.ToList()) // Iterate over a copy to allow modification
        {
            if (animation.IsFinished)
            {
                animations2DRemove.Add((animation, startTime));
                continue;
            }

            // Only update if current time is past or at the animation's start time
            if (_currentTime >= startTime)
            {
                animation.Update(frameDeltaTime); // Pass the fixed frame delta time to the animation
                if (animation.IsFinished)
                {
                    animations2DRemove.Add((animation, startTime));
                }
            }
        }
        // Remove finished 2D animations
        foreach (var animEntry in animations2DRemove)
        {
            _scheduledAnimations2D.Remove(animEntry);
        }
        
        List<(Animation3D animation, TimeSpan startTime)> animations3DRemove = new();
        foreach (var (animation, startTime) in _scheduledAnimations3D.ToList()) // Iterate over a copy
        {
            if (animation.IsFinished)
            {
                animations3DRemove.Add((animation, startTime));
                continue;
            }

            if (_currentTime >= startTime)
            {
                animation.Update(frameDeltaTime);
                if (animation.IsFinished)
                {
                    animations3DRemove.Add((animation, startTime));
                }
            }
        }
        // Remove finished 3D animations
        foreach (var animEntry in animations3DRemove)
        {
            _scheduledAnimations3D.Remove(animEntry);
        }
    }

    /// <summary>
    /// Clears all scheduled animations and resets the animator's current time.
    /// </summary>
    public void ClearAllAnimations()
    {
        _scheduledAnimations2D.Clear();
        _scheduledAnimations3D.Clear();
        _currentTime = TimeSpan.Zero;
        
        Console.WriteLine("All animations cleared and animator reset.");
    }

    public void RemoveAnimationsForShape(ShapeContainer shape)
    {
        // Remove all 2D animations for the specified shape
        _scheduledAnimations2D.RemoveAll(anim => anim.animation.Shape == shape);
    }
    
    public void RemoveAnimationsForShape(ShapeContainer3D shape)
    {
        // Remove all 3D animations for the specified shape
        _scheduledAnimations3D.RemoveAll(anim => anim.animation.Shape == shape);
    }
}