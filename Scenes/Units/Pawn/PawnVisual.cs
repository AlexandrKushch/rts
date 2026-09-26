using System;
using Godot;

public partial class PawnVisual : UnitVisualBase
{
    public override void _Ready()
    {
        base._Ready();

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public void UpdateMovement(Vector2 velocity, int collected, ResourceInfo resource)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(UnitAnimationNames.Pawn.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0
            ? UnitAnimationNames.Run
            : UnitAnimationNames.Idle;

        if (resource != null)
        {
            string instrument = collected == 0 ? UnitAnimationNames.Pawn.Instrument: string.Empty;

            animation = $"{resource.Name.Capitalize()}/{animation}{instrument}";
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        animationLibraryName = animationLibraryName.Capitalize();
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(UnitAnimationNames.Pawn.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        
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

    public void Interact(string animationLibraryName, Vector2? target)
    {
        animationLibraryName = animationLibraryName.Capitalize();
        string animation = $"{animationLibraryName}/{UnitAnimationNames.Pawn.Interact}";
        
        Sprite2D.FlipH = target.HasValue ? target.Value.X < GlobalPosition.X : Sprite2D.FlipH;

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains(UnitAnimationNames.Pawn.Interact, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnInteractAnimationFinished);
        }
    }
}
