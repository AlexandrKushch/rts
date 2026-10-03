using System;
using Godot;

public partial class WarriorVisual : UnitVisualBase
{
    private Tween _jumpingTween;

    [Signal]
    public delegate void OnAttackAnimationFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        _jumpingTween = CreateTween().SetLoops(); //.SetTrans(Tween.TransitionType.Sine);
        double jumpingTweenDuration = 0.2f;
        _jumpingTween.TweenProperty(this, "position", Vector2.Up * 15, jumpingTweenDuration * 0.5f);
        _jumpingTween.TweenProperty(this, "position", Vector2.Zero, jumpingTweenDuration * 0.5f);
        _jumpingTween.Stop();

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Equals(UnitAnimationNames.Warrior.Attack1, StringComparison.OrdinalIgnoreCase)
            || AnimationPlayer.CurrentAnimation.ToString().Equals(UnitAnimationNames.Warrior.Attack2, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        _jumpingTween.Stop();
        base.UpdateMovement(velocity, animationLibraryName);
    }

    public void Attack(Vector2? target)
    {
        _jumpingTween.Stop();
        string animation = UnitAnimationNames.Warrior.Attack1;

        Sprite2D.FlipH = target.HasValue ? target.Value.X < GlobalPosition.X : Sprite2D.FlipH;
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }
    
    public void Charge(Vector2? target)
    {
        _jumpingTween.Stop();
        string animation = UnitAnimationNames.Warrior.Attack2;

        Sprite2D.FlipH = target.HasValue ? target.Value.X < GlobalPosition.X : Sprite2D.FlipH;
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public void Guard(Vector2 velocity)
    {
        string animation = UnitAnimationNames.Warrior.Guard;

        Sprite2D.FlipH = velocity.Normalized().X < 0;

        if (velocity.Length() > 0)
        {        
            _jumpingTween.Play();
        }
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Equals(UnitAnimationNames.Warrior.Attack1, StringComparison.OrdinalIgnoreCase)
            || animation.ToString().Equals(UnitAnimationNames.Warrior.Attack2, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnAttackAnimationFinished);
        }
    }
}
