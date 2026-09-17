using Godot;

[GlobalClass]
public partial class TeamResource : Resource
{
    [Export] public TeamType Id { get; set; }
    [Export] public string UnitsPath { get; set; }
    [Export] public string BuildingsPath { get; set; }
}
