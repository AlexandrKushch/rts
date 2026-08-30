using Godot;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp
{
    public int MaxHp { get; set; }
    public int HP { get; set; }

    public CollisionPolygon2D CollisionPolygon2D { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }

    [Export] public BuildResource Resource { get; private set; }
    
    public override void _Ready()
    {
        CollisionPolygon2D = GetNode<CollisionPolygon2D>(nameof(CollisionPolygon2D));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();

        MaxHp = Resource.MaxHp;
    }

    public bool TryBuildProgressOne()
    {
        if (HP + 1 > MaxHp)
        {
            GD.Print("Build complete");
            return false;
        }

        HP++;
        return true;
    }

    public void TakeDamage(int value)
    {
        HP -= value;

        if (HP <= 0)
        {
            Destroy();
        }
    }
    
    public void Destroy()
    {
        QueueFree();
    }
}
