using Godot;
using System;

public partial class UnitVisualBase : Node2D
{
    protected const int TextureKeyId = 2;

    protected Sprite2D Sprite2D;
    protected OutlineVisual Outline;
    protected AnimationPlayer AnimationPlayer;

    [Signal]
    public delegate void OnInteractAnimationFinishedEventHandler();

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        Outline = GetNode<OutlineVisual>(nameof(Outline));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public virtual void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0
            ? UnitAnimationNames.Run
            : UnitAnimationNames.Idle;

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

    public virtual void Stop()
    {
        AnimationPlayer.Stop();
    }

    public void UpdateOutlineVisible(bool value)
    {
        Outline.UpdateVisible(value);
    }
}
