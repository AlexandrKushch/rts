using Godot;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp
{
    public CollisionShape2D CollisionShape2D { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }

    [Export] public int HP { get; set; }
    [Export] public int MaxHp { get; set; }
    
    public override void _Ready()
    {
        CollisionShape2D = GetNode<CollisionShape2D>(nameof(CollisionShape2D));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();
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
