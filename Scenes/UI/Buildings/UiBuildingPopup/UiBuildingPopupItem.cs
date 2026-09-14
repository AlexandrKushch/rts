using Godot;

public partial class UiBuildingPopupItem : Control, IHasTooltip
{
    private TextureButton AddQueue;

    public TextureButton RemoveQueue { get; set; }
    public UnitTypeIds Id { get; set; }
    public TextureRect Icon { get; set; }
    public Label InQueue { get; set; }
    public TextureProgressBar ProgressBar { get; set; }

    public bool Expanded { get; set; }

    [Signal]
    public delegate void AddToQueueEventHandler(UnitTypeIds Id);

    [Signal]
    public delegate void RemoveFromQueueEventHandler(UnitTypeIds Id);

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>(nameof(Icon));
        AddQueue = GetNode<TextureButton>(nameof(AddQueue));
        RemoveQueue = GetNode<TextureButton>(nameof(RemoveQueue));
        InQueue = GetNode<Label>(nameof(InQueue));
        ProgressBar = GetNode<TextureProgressBar>(nameof(ProgressBar));

        InQueue.Visible = false;
        RemoveQueue.Visible = false;

        AddQueue.MouseEntered += OnMouseEntered;
        AddQueue.MouseExited += OnMouseExited;
    }

    public void SetExpanded(bool value)
    {
        Expanded = value;
        AddQueue.MouseFilter = value ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        RemoveQueue.MouseFilter = value ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        RemoveQueue.Visible = value;
    }

    public void OnAddButton()
    {
        EmitSignal(SignalName.AddToQueue, (int)Id);
    }

    public void OnRemoveButton()
    {
        EmitSignal(SignalName.RemoveFromQueue, (int)Id);
    }

    public void OnMouseEntered()
    {
        UiCursorTooltip.Instance.UpdateVisibilityAndContent(
            true,
            new TooltipContent
            {
                Title = GlobalResources.Instance.Units[Id].Name.Capitalize(),
                ResourcesCost = GlobalResources.Instance.Units[Id].Cost
            });
    }

    public void OnMouseExited()
    {
        UiCursorTooltip.Instance.UpdateVisibilityAndContent(false);
    }
}
