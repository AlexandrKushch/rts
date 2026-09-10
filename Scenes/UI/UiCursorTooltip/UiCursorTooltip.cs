using Godot;
using Godot.Collections;

public partial class UiCursorTooltip : Control
{
    private readonly Vector2 OffsetToRight = new Vector2(25, 0);

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
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GetGlobalMousePosition() + OffsetToRight;
    }

    public void UpdateVisibilityAndContent(bool value, TooltipContent content = null)
    {
        Visible = value;
        SetProcess(value);

        if (value && content != null)
        {
            Title.Text = content.Title;
            UpdateResourceCost(content.ResourcesCost);
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

            newItem.Icon.Texture = GlobalResources.Instance.GatheringResources[cost.Key].Icon;
            newItem.Value.Text = cost.Value.ToString();
        }
    }
}
