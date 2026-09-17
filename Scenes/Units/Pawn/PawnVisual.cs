using System;
using Godot;

public partial class PawnVisual : UnitVisualBase
{
    public override void _Ready()
    {
        base._Ready();

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void SetupColor(TeamType team, UnitType unit)
    {
        
        string unitName = unit.Name.Capitalize();

        foreach (var animationName in AnimationPlayer.GetAnimationList())
        {
            GD.Print(animationName);
            // if (animationName.Equals(UnitAnimationNames.RESET, StringComparison.OrdinalIgnoreCase))
            // {
            //     continue;
            // }

            // var animation = AnimationPlayer.GetAnimation(animationName);

            // // {UnitsPath}/Warrior/Warrior_Idle.png
            // animation.TrackSetKeyValue(TextureKeyId, 0, ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[team].UnitsPath}{unitName}/{unitName}_{animationName}.png"));
        }
    }

    public void UpdateMovement(Vector2 velocity, int collected, ResourceInfo resource)
    {
        // if (AnimationPlayer.CurrentAnimation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        // {
        //     return;
        // }

        // Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        // string animation = velocity.Length() > 0
        //     ? PawnAnimationNames.Run
        //     : PawnAnimationNames.Idle;

        // if (resource != null)
        // {
        //     string resourceName = $"{resource.Name}/";
        //     string handItem = collected == 0 ? $"{PawnAnimationNames.Instrument}_": string.Empty;

        //     animation = $"{resourceName}{handItem}{animation}";
        // }

        // if (AnimationPlayer.CurrentAnimation.Equals(animation))
        // {
        //     return;
        // }

        // AnimationPlayer.Play(animation);
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        // if (AnimationPlayer.CurrentAnimation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        // {
        //     return;
        // }
        
        // Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        // string animation = velocity.Length() > 0
        //     ? PawnAnimationNames.Run
        //     : PawnAnimationNames.Idle;

        // if (!string.IsNullOrWhiteSpace(animationLibraryName))
        // {
        //     animation = $"{animationLibraryName}/{animation}";
        // }

        // if (AnimationPlayer.CurrentAnimation.Equals(animation))
        // {
        //     return;
        // }

        // AnimationPlayer.Play(animation);
    }

    public void Interact(string animationLibraryName, Vector2? target)
    {
        // string animation = $"{animationLibraryName}/{PawnAnimationNames.Interact}";
        
        // Sprite2D.FlipH = target.HasValue ? target.Value.X < GlobalPosition.X : Sprite2D.FlipH;

        // if (AnimationPlayer.CurrentAnimation.Equals(animation))
        // {
        //     return;
        // }

        // AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains(PawnAnimationNames.Interact, StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnInteractAnimationFinished);
        }
    }
}
