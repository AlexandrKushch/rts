using Godot;

public partial class UiResources : HBoxContainer
{
    private UIResourceItem[] _resouorces;

    public override void _Ready()
    {
        var item = GetChild<UIResourceItem>(0).Duplicate() as UIResourceItem;

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        _resouorces = new UIResourceItem[ResourceController.Instance.AvailableResources.Length];
        int i = 0;

        foreach (var resource in ResourceController.Instance.AvailableResources)
        {
            var newItem = item.Duplicate() as UIResourceItem;
            AddChild(newItem);
            newItem.Id = resource.Id;
            newItem.Icon.Texture = resource.Icon;
            newItem.Value.Text = resource.DefaultValue.ToString();
            _resouorces[i] = newItem;
            i++;
        }
    }

    public override void _Process(double delta)
    {
        foreach (var resource in _resouorces)
        {
            resource.Value.Text = ResourceController.Instance.CollectedResources[resource.Id].ToString();
        }        
    }
}
