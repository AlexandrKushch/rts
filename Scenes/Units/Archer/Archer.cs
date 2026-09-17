using Godot;
using System.Text.RegularExpressions;

public partial class Archer : UnitBase
{
    public UnitVisualBase Visual { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        Visual = GetNode<UnitVisualBase>(nameof(Visual));

        string pattern = @"_(\d{2})\.png$";
        var match = Regex.Match(Meta.Icon.ResourcePath, pattern);
        if (match.Success
            && int.TryParse(match.Groups[1].Value, out int index))
        {
            index += 5 * (int)Team;
            string indexString = index.ToString("00");
            string resourcePath = Regex.Replace(Meta.Icon.ResourcePath, pattern, $"_{indexString}.png");

            Meta.Icon = ResourceLoader.Load<Texture2D>(resourcePath);
        }

        Visual.SetupColor(Team, Meta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Visual.UpdateMovement(Velocity, string.Empty);
    }
}
