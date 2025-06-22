// SceneAnimations.cs
using System;
using System.Windows;
using System.Windows.Controls;
using WpfApp;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Animations;
using WpfApp.TwoDimension.Models;
using WpfApp.TwoDimension.Samples;

public static class SceneAnimations
{
    public static void StartCharacterCoinScene(CartesianCanvas canvas2D, Animator _animator)
    {
        // char and coin
        var character = ShapeSample.CreateCharacter(canvas2D, new Point(-30, 8), 8);
        var coin = ShapeSample.CreateCoin(canvas2D, new Point(60, 0), 6);

        var moveOffset = new Point(90, 0);
        var moveDuration = TimeSpan.FromSeconds(10);
        var liftOffset = new Point(0, 10);
        var liftDuration = TimeSpan.FromSeconds(0.5);
        var liftDelay = moveDuration;

        // Loop scale coin trong 10s
        var coinLoopScale = new LoopScaleAnimation(
            coin, 0.2, 1.0, 1.0, 1.0,
            TimeSpan.FromSeconds(0.6),
            TimeSpan.FromSeconds(10),
            autoReverse: true
        );
        _animator.AddAnimation(coinLoopScale, TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(coin, liftOffset, liftDuration), liftDelay);

        // Di chuyển nhân vật
        _animator.AddAnimation(new TranslateAnimation2D(character.Body, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.Head, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.LeftEye, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.RightEye, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.LeftLeg, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.RightLeg, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.LeftArm, moveOffset, moveDuration), TimeSpan.Zero);
        _animator.AddAnimation(new TranslateAnimation2D(character.RightArm, moveOffset, moveDuration), TimeSpan.Zero);

        // Loop swing tay chân
        var swingDuration = TimeSpan.FromSeconds(0.3);
        var swingAngle = 45;

        _animator.AddAnimation(new LoopRotateAnimation(character.LeftArm, -swingAngle, swingDuration, character.LeftArm.GetCenter), TimeSpan.Zero);
        _animator.AddAnimation(new LoopRotateAnimation(character.RightArm, swingAngle, swingDuration, character.RightArm.GetCenter), TimeSpan.Zero);
        _animator.AddAnimation(new LoopRotateAnimation(character.LeftLeg, -swingAngle, swingDuration, character.LeftLeg.GetCenter), TimeSpan.Zero);
        _animator.AddAnimation(new LoopRotateAnimation(character.RightLeg, swingAngle, swingDuration, character.RightLeg.GetCenter), TimeSpan.Zero);


        // heart
        var heartSize = 6.0;
        var heartY = character.Head.GetCenter().Y + 20;
        var heartSpacing = 30;

        var heart1 = ShapeSample.CreateHeart(canvas2D, new Point(-heartSpacing, heartY), heartSize);
        var heart2 = ShapeSample.CreateHeart(canvas2D, new Point(0, heartY), heartSize);
        var heart3 = ShapeSample.CreateHeart(canvas2D, new Point(heartSpacing, heartY), heartSize);

        var heartScaleDuration = TimeSpan.FromSeconds(0.4);
        
        //loop
        _animator.AddAnimation(new LoopScaleAnimation(
            heart1, 1.0, 1.0, 1.3, 1.3, heartScaleDuration, TimeSpan.FromSeconds(10), true), TimeSpan.Zero);

        _animator.AddAnimation(new LoopScaleAnimation(
            heart2, 1.0, 1.0, 1.3, 1.3, heartScaleDuration, TimeSpan.FromSeconds(10), true), TimeSpan.Zero);

        _animator.AddAnimation(new LoopScaleAnimation(
            heart3, 1.0, 1.0, 1.3, 1.3, heartScaleDuration, TimeSpan.FromSeconds(10), true), TimeSpan.Zero);

    }
}
