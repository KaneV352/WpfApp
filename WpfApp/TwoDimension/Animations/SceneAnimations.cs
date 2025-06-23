using System.Windows;
using WpfApp;
using WpfApp.TwoDimension;
using WpfApp.TwoDimension.Animations;
using WpfApp.TwoDimension.Samples;
using WpfApp.TwoDimension.Shapes;

public static class SceneAnimations
{
    public static void StartCharacterCoinScene(CartesianCanvas canvas2D, Animator animator)
    {
        // char and coin
        var character = ShapeSample.CreateCharacter(canvas2D, new Point(-30, 8), 8);
        var coin = new Coin(canvas2D, new Point(character.Body.GetCenter().X + 90, character.Body.GetCenter().Y), 6);

        var moveOffset = new Point(90, 0);
        var moveDuration = TimeSpan.FromSeconds(10);
        var liftOffset = new Point(0, 20);
        var liftDuration = TimeSpan.FromSeconds(0.1);
        var liftDelay = moveDuration - TimeSpan.FromSeconds(1);

        // Loop scale coin trong 10s
        var coinLoopScale = new LoopScaleAnimation(
            coin, 0.2, 1.0, 1.0, 1.0,
            TimeSpan.FromSeconds(0.6),
            TimeSpan.FromSeconds(10),
            autoReverse: true
        );
        animator.AddAnimation(coinLoopScale, TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(coin, liftOffset, liftDuration), liftDelay);
        
        // Di chuyển nhân vật
        animator.AddAnimation(new TranslateAnimation2D(character.Body, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.Head, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.LeftEye, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.RightEye, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.LeftLeg, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.RightLeg, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.LeftArm, moveOffset, moveDuration), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(character.RightArm, moveOffset, moveDuration), TimeSpan.Zero);

        // Loop swing tay chân
        var swingDuration = TimeSpan.FromSeconds(10);
        var swingAngle = 10000;
        
        animator.AddAnimation(new RotateAnimation(character.LeftArm, -swingAngle, swingDuration, character.LeftArm.GetCenter), TimeSpan.Zero);
        animator.AddAnimation(new RotateAnimation(character.RightArm, swingAngle, swingDuration, character.RightArm.GetCenter), TimeSpan.Zero);
        animator.AddAnimation(new RotateAnimation(character.LeftLeg, -swingAngle, swingDuration, character.LeftLeg.GetCenter), TimeSpan.Zero);
        animator.AddAnimation(new RotateAnimation(character.RightLeg, -swingAngle,swingDuration, character.RightLeg.GetCenter), TimeSpan.Zero);

        // heart
        var heartSize = 6.0;
        var heartY = character.Body.GetCenter().Y;
        var heartSpacing = 30;

        var heart2 = ShapeSample.CreateHeart(canvas2D, new Point(0, heartY), heartSize);
        var heart3 = ShapeSample.CreateHeart(canvas2D, new Point(heartSpacing, heartY), heartSize);

        var heartScaleDuration = TimeSpan.FromSeconds(0.4);
        
        //loop
        animator.AddAnimation(new LoopScaleAnimation(
            heart2, 1.0, 1.0, 1.3, 1.3, heartScaleDuration, TimeSpan.FromSeconds(10), true), TimeSpan.Zero);
        animator.AddAnimation(new TranslateAnimation2D(heart2, liftOffset, liftDuration), liftDelay / 3);
        
        animator.AddAnimation(new LoopScaleAnimation(
            heart3, 1.0, 1.0, 1.3, 1.3, heartScaleDuration, TimeSpan.FromSeconds(10), true), TimeSpan.Zero);

        animator.AddAnimation(new TranslateAnimation2D(heart3, liftOffset, liftDuration), liftDelay * 2 / 3);
    }
}
