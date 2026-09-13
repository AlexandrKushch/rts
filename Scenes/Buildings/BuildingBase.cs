using Godot;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp
{
    public int MaxHp { get; set; }
    public int HP { get; set; }

    public bool Built { get; private set; } = false;
    public CollisionPolygon2D CollisionPolygon2D { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }

    private UiBuildingPopup UiBuildingPopup;

    [Export] public BuildResource Resource { get; private set; }

    public override void _Ready()
    {
        UiBuildingPopup = GetNode<UiBuildingPopup>(nameof(UiBuildingPopup));
        CollisionPolygon2D = GetNode<CollisionPolygon2D>(nameof(CollisionPolygon2D));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();

        MaxHp = Resource.MaxHp;

        UnitsController.Instance.SelectionChanged += OnSelectionChanged;
    }

    public bool TryBuildProgressOne()
    {
        if (HP + 1 > MaxHp)
        {
            Built = true;
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

    private void OnSelectionChanged()
    {
        UiBuildingPopup.Selected = UnitsController.Instance.Selections.Count == 1 && UnitsController.Instance.Selections.Any(x => x.EffectedOn == this);

        if (UiBuildingPopup.Selected)
        {
            UiBuildingPopup.Expand();
        }
        else
        {
            UiBuildingPopup.Colapse();
        }
    }
}
