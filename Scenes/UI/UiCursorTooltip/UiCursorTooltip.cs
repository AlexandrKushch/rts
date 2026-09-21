using System.Linq;
using Godot;
using Godot.Collections;

public partial class UiCursorTooltip : Control
{
    private readonly float Offset = 150;
    
    private PlayerBase _player;

    private TooltipContent _content;

    [Export] private Label Title;
    [Export] private HBoxContainer ResourceCost;

    public static UiCursorTooltip Instance { get; private set; }

    public override void _Ready()
    {
        if (!IsInstanceValid(Instance))
        {
            Instance = this;
        }

        UpdateVisibilityAndContent(false, new TooltipContent());
        
        _player = GetParent().GetParent<PlayerBase>();

        _player.ResourceController.Changed += () =>
        {
            if (!Visible || _content == null) return;
            UpdateResourceCost(_content.ResourcesCost);
        };
    }

    public override void _Process(double delta)
    {
        var mousePos = GetGlobalMousePosition();
        var viewportSize = GetViewport().GetVisibleRect().Size;
        var direction = mousePos.X < viewportSize.X - (viewportSize.X / 5) ? Vector2.Right : Vector2.Left;
        GlobalPosition = GetGlobalMousePosition() + direction * Offset;
    }

    public void UpdateVisibilityAndContent(bool value, TooltipContent content = null)
    {
        Visible = value;
        _content = content;
        SetProcess(value);

        if (value && _content != null)
        {
            Title.Text = _content.Title;
            UpdateResourceCost(_content.ResourcesCost);
        }
    }

    private void UpdateResourceCost(Dictionary<ResourceTypeIds, int> resourceCosts)
    {
        var item = ResourceCost.GetChild<UiResourceCostItem>(0);

        foreach (var child in ResourceCost.GetChildren())
        {
            child.QueueFree();
        }

        foreach (var cost in resourceCosts)
        {
            var newItem = item.Duplicate() as UiResourceCostItem;
            ResourceCost.AddChild(newItem);

            newItem.Id = cost.Key;
            newItem.Icon.Texture = GlobalResources.Instance.GatheringResources[cost.Key].Icon;
            newItem.Value.Text = cost.Value.ToString();

            newItem.Value.AddThemeColorOverride("font_color",
                _player.ResourceController.CheckSpent(cost.Key, cost.Value) ? ColorsGlobal.Default : ColorsGlobal.Error);
        }
    }

    private void UpdateResourceCostColor(Dictionary<ResourceTypeIds, int> resourceCosts)
    {
        var items = ResourceCost.GetChildren().Select(x => x as UiResourceCostItem).ToArray();

        foreach (var item in items)
        {
            var cost = _content.ResourcesCost[item.Id];
            item.Value.AddThemeColorOverride("font_color",
                _player.ResourceController.CheckSpent(item.Id, cost) ? ColorsGlobal.Default : ColorsGlobal.Error);
        }
    }
}
