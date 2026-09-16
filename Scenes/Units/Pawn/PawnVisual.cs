using System;
using Godot;

public partial class PawnVisual : UnitVisualBase
{
    public override void _Ready()
    {
        base._Ready();
    }

    public void UpdateMovement(Vector2 velocity, int collected, ResourceInfo resource)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0
            ? PawnAnimationNames.Run
            : PawnAnimationNames.Idle;

        if (resource != null)
        {
            string resourceName = $"{resource.Name}/";
            string handItem = collected == 0 ? $"{PawnAnimationNames.Instrument}_": string.Empty;

            animation = $"{resourceName}{handItem}{animation}";
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }
}
