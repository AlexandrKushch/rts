using Godot;
using System;

public partial class ArcherVisual : UnitVisualBase
{
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
}
