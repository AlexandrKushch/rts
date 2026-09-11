using System.Linq;
using Godot;
using Godot.Collections;

public partial class UiCursorTooltip : Control
{
    private readonly Vector2 OffsetToRight = new Vector2(25, 0);
    
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
        
        ResourceController.Instance.Changed += () =>
        {
            if (!Visible || _content == null) return;
            UpdateResourceCost(_content.ResourcesCost);
        };
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() + OffsetToRight;
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
                ResourceController.Instance.CheckSpent(cost.Key, cost.Value) ? ColorsGlobal.Default : ColorsGlobal.Error);
        }
    }

    private void UpdateResourceCostColor(Dictionary<ResourceTypeIds, int> resourceCosts)
    {
        var items = ResourceCost.GetChildren().Select(x => x as UiResourceCostItem).ToArray();

        foreach (var item in items)
        {
            var cost = _content.ResourcesCost[item.Id];
            item.Value.AddThemeColorOverride("font_color",
                ResourceController.Instance.CheckSpent(item.Id, cost) ? ColorsGlobal.Default : ColorsGlobal.Error);
        }
    }
}
