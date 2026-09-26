using Godot;
using System;

public partial class SheepVisual : UnitVisualBase
{
    public void Interact()
    {
        string animation = UnitAnimationNames.Sheep.Interact;
        
        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains(UnitAnimationNames.Sheep.Interact, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        base.UpdateMovement(velocity, animationLibraryName);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains(UnitAnimationNames.Sheep.Interact, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnInteractAnimationFinished);
        }
    }
}
