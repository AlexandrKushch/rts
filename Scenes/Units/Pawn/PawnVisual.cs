using System;
using Godot;

public partial class PawnVisual : Node
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

    public void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0
            ? PawnAnimationNames.Run
            : PawnAnimationNames.Idle;

        if (!string.IsNullOrWhiteSpace(animationLibraryName))
        {
            animation = $"{animationLibraryName}/{animation}";
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public void UpdateMovement(Vector2 velocity, int collected, ResourceType resource)
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

    public void Interact(string animationLibraryName)
    {
        string animation = $"{animationLibraryName}/{PawnAnimationNames.Interact}";
        
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

    public void Stop()
    {
        AnimationPlayer.Stop();
    }
}
