using Godot;
using System;

public partial class MonkVisual : UnitVisualBase
{
    public override void SetupColor(TeamType team, UnitType unit)
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
            animation.TrackSetKeyValue(TextureKeyId, 0, ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[team].UnitsPath}{unitName}/{animationName}.png"));
        }
    }

}
