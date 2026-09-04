using Godot;
using System;

public partial class SheepVisual : Node2D
{
    private Sprite2D Sprite2D;
    private AnimationPlayer AnimationPlayer;

    [Signal]
    public delegate void OnInteractAnimationFinishedEventHandler();
    
    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public void UpdateMovement(Vector2 velocity)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;
        
        string animation = velocity.Length() > 0
            ? PawnAnimationNames.Run
            : PawnAnimationNames.Idle;

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public void Interact()
    {
        string animation = PawnAnimationNames.Interact;
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnInteractAnimationFinished);
        }
    }
}
