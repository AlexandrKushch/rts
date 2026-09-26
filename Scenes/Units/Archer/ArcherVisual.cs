using Godot;
using System;

public partial class ArcherVisual : UnitVisualBase
{
    [Signal]
    public delegate void OnAttackAnimationFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(UnitAnimationNames.Archer.Shoot, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        base.UpdateMovement(velocity, animationLibraryName);
    }

    public void Attack(Vector2? target)
    {
        string animation = UnitAnimationNames.Archer.Shoot;

        Sprite2D.FlipH = target.HasValue ? target.Value.X < GlobalPosition.X : Sprite2D.FlipH;
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains(UnitAnimationNames.Archer.Shoot, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnAttackAnimationFinished);
        }
    }
}
