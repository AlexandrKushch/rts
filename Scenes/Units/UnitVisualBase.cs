using Godot;
using System;

public partial class UnitVisualBase : Node2D
{
    protected const int TextureKeyId = 2;

    protected Sprite2D Sprite2D;
    protected AnimationPlayer AnimationPlayer;

    [Signal]
    public delegate void OnInteractAnimationFinishedEventHandler();

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public virtual void SetupColor(TeamType team, UnitType unit)
    {
        string unitName = unit.Name.Capitalize();

        foreach (var animationName in AnimationPlayer.GetAnimationList())
        {
            if (animationName.Equals(UnitAnimationNames.RESET, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var animation = AnimationPlayer.GetAnimation(animationName);

            // {UnitsPath}/Warrior/Warrior_Idle.png
            animation.TrackSetKeyValue(TextureKeyId, 0, ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[team].UnitsPath}{unitName}/{unitName}_{animationName}.png"));
        }
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
}
