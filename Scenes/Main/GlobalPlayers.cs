using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GlobalPlayers : Node
{
    public Dictionary<TeamType, PlayerBase> Players { get; set; }

    public static GlobalPlayers Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        Players = GetChildren().Select(x => x as PlayerBase).ToDictionary(x => x.Team);
    }
}
