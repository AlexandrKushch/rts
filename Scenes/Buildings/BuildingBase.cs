using Godot;
using System.Linq;

public partial class BuildingBase : StaticBody2D, IDestroyableWithHp
{
    private UiBuildingPopup _popup;
    private PackedScene _uiBuildingPopup;

    public int MaxHp { get; set; }
    public int HP { get; set; }

    public bool Built { get; private set; } = false;
    public CollisionPolygon2D CollisionPolygon2D { get; private set; }
    public NavigationObstacle2D[] Obstacles { get; private set; }

    [Export] public BuildResource Resource { get; private set; }

    public override void _Ready()
    {
        CollisionPolygon2D = GetNode<CollisionPolygon2D>(nameof(CollisionPolygon2D));
        Obstacles = GetChildren().Where(x => x is NavigationObstacle2D).Select(x => x as NavigationObstacle2D).ToArray();

        _uiBuildingPopup = ResourceLoader.Load<PackedScene>("uid://c784niqlcupmb");

        MaxHp = Resource.MaxHp;

        UnitsController.Instance.SelectionChanged += OnSelectionChanged;
    }

    public void ShowBuildingPopup()
    {
        if (IsInstanceValid(_popup)) return;
        _popup = _uiBuildingPopup.Instantiate<UiBuildingPopup>();
        AddChild(_popup);
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
        bool show = UnitsController.Instance.Selections.Count == 1 && UnitsController.Instance.Selections.Any(x => x.EffectedOn == this);

        if (show)
        {
            ShowBuildingPopup();
        }
        else if (IsInstanceValid(_popup))
        {
            _popup.QueueFree();
        }
    }
}
