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
            string handItem = collected == 0 ? $"{resource.InstrumentName}_": string.Empty;

            animation = $"{resourceName}{handItem}{animation}";
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public void Interact(ResourceType resource)
    {
        if (resource == null)
        {
            throw new ArgumentNullException($"{nameof(ResourceType)} is null reference");
        }

        string animation = $"{resource.Name}/{PawnAnimationNames.Interact}";
        
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
