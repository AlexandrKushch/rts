using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class PlayerGlobalResources : Node
{
    private const int UnitAnimationTextureKeyId = 2;

    private PlayerBase _player;

    public Dictionary<UnitTypeIds, UnitType> Units { get; private set; } = new Dictionary<UnitTypeIds, UnitType>();
    public Dictionary<BuildingTypeIds, BuildResource> Buildings { get; private set; } = new Dictionary<BuildingTypeIds, BuildResource>();

    public Dictionary<UnitTypeIds, PackedScene> UnitScenes { get; private set; } = new Dictionary<UnitTypeIds, PackedScene>();

    public override void _Ready()
    {
        _player = GetParent<PlayerBase>();

        foreach (var unit in GlobalResources.Instance.Units)
        {
            var unitDuplicate = new UnitType
            {
                Id = unit.Value.Id,
                Cost = unit.Value.Cost,
                Name = unit.Value.Name,
                Icon = unit.Value.Icon.Duplicate() as Texture2D
            };

            string pattern = @"_(\d{2})\.png$";
            var match = Regex.Match(unit.Value.Icon.ResourcePath, pattern);
            if (match.Success
                && int.TryParse(match.Groups[1].Value, out int index))
            {
                index = (index % 6) + (5 * (int)_player.Team);
                string indexString = index.ToString("00");
                string resourcePath = Regex.Replace(unit.Value.Icon.ResourcePath, pattern, $"_{indexString}.png");

                unitDuplicate.Icon = ResourceLoader.Load<Texture2D>(resourcePath);
            }

            Units.Add(unit.Key, unitDuplicate);
        }

        foreach (var unit in GlobalResources.Instance.UnitScenes)
        {
            var unitDuplicate = unit.Value.Instantiate().Duplicate() as UnitBase;
            SetupColorAnimations(unitDuplicate);
            unitDuplicate.Team = _player.Team;
            unitDuplicate.Meta = Units[unit.Key];
            var scene = new PackedScene();
            var result = scene.Pack(unitDuplicate);

            if (result == Error.Ok)
            {
                UnitScenes.Add(unit.Key, scene);
            }
            else
            {
                GD.PushError($"{unit.Key} with color {_player.Team} has error to create packed scene");
            }
        }

        foreach (var building in GlobalResources.Instance.Buildings)
        {
            var buildingDuplicate = new BuildResource
            {
                Id = building.Value.Id,
                Cost = building.Value.Cost,
                Name = building.Value.Name,
                MaxHp = building.Value.MaxHp,
                TileRequiresToBuild = building.Value.TileRequiresToBuild.ToArray(),
                Produces = building.Value.Produces.Select(x => Units[x.Id]).ToArray(),
                Icon = ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[_player.Team].BuildingsPath}/{building.Value.Name.Capitalize()}.png")
            };
            Buildings.Add(building.Key, buildingDuplicate);
        }
    }

    private void SetupColorAnimations(UnitBase unitDuplicate)
    {
        string unitName = unitDuplicate.Meta.Name.Capitalize();
        var visual = unitDuplicate.GetNode<UnitVisualBase>("Visual");
        var animationPlayer = visual.GetNode<AnimationPlayer>(nameof(AnimationPlayer));

        foreach (var libraryName in animationPlayer.GetAnimationLibraryList())
        {
            var library = animationPlayer.GetAnimationLibrary(libraryName);
            var uniqueLibrary = library.Duplicate(true) as AnimationLibrary;

            animationPlayer.RemoveAnimationLibrary(libraryName);
            animationPlayer.AddAnimationLibrary(libraryName, uniqueLibrary);
        }


        foreach (var animationName in animationPlayer.GetAnimationList())
        {
            if (animationName.Equals(UnitAnimationNames.RESET, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (unitDuplicate is Pawn pawn)
            {
                bool isInteract = animationName.Contains(UnitAnimationNames.Pawn.Interact, StringComparison.OrdinalIgnoreCase);

                var animation = animationPlayer.GetAnimation(animationName);

                // {UnitsPath}/Pawn/Pawn_Idle.png
                animation.TrackSetKeyValue(
                    isInteract ? UnitAnimationTextureKeyId + 1 : UnitAnimationTextureKeyId,
                    0,
                    ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[_player.Team].UnitsPath}{unitName}/{unitName}_{animationName.Replace("/", "_")}.png"));
            }
            else if (visual is MonkVisual)
            {
                var animation = animationPlayer.GetAnimation(animationName);

                // {UnitsPath}/Monk/Idle.png
                animation.TrackSetKeyValue(UnitAnimationTextureKeyId,
                    0,
                    ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[_player.Team].UnitsPath}{unitName}/{animationName}.png"));
            }
            else
            {
                var animation = animationPlayer.GetAnimation(animationName);

                // {UnitsPath}/Warrior/Warrior_Idle.png
                animation.TrackSetKeyValue(UnitAnimationTextureKeyId,
                    0,
                    ResourceLoader.Load<Texture2D>($"{GlobalResources.Instance.Teams[_player.Team].UnitsPath}{unitName}/{unitName}_{animationName}.png"));
            }
        }
    }

}
