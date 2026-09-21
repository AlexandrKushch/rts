using Godot;

public partial class UiResourcesGrid : HBoxContainer
{
    private PlayerBase _player;
    private UIResourceItem[] _resouorces;

    public override void _Ready()
    {
        _player = GetParent().GetParent().GetParent<PlayerBase>();

        var item = GetChild<UIResourceItem>(0).Duplicate() as UIResourceItem;

        foreach (var child in GetChildren())
        {
            child.QueueFree();
        }

        _resouorces = new UIResourceItem[GlobalResources.Instance.GatheringResources.Count];
        int i = 0;

        foreach (var resource in GlobalResources.Instance.GatheringResources)
        {
            var newItem = item.Duplicate() as UIResourceItem;
            AddChild(newItem);
            newItem.Type = resource.Value.Type;
            newItem.Icon.Texture = resource.Value.Icon;
            newItem.Value.Text = resource.Value.DefaultValue.ToString();
            _resouorces[i] = newItem;
            i++;
        }
    }

    public override void _Process(double delta)
    {
        foreach (var resource in _resouorces)
        {
            resource.Value.Text = _player.ResourceController.CollectedResources[resource.Type].ToString();
        }        
    }
}
