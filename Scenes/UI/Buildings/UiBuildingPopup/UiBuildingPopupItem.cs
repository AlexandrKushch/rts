using Godot;

public partial class UiBuildingPopupItem : Control
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
    }

    public void SetExpanded(bool value)
    {
        Expanded = value;
        AddQueue.MouseFilter = value ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        RemoveQueue.MouseFilter = value ? MouseFilterEnum.Stop : MouseFilterEnum.Ignore;
        RemoveQueue.Visible = value;
    }

    public void AddButtonDown()
    {
        TweenButtonDown(AddQueue);
    }

    public void RemoveButtonDown()
    {
        TweenButtonDown(RemoveQueue);
    }

    public void OnAddButton()
    {
        TweenButtonUp(AddQueue);
        EmitSignal(SignalName.AddToQueue, (int)Id);
    }

    public void OnRemoveButton()
    {
        TweenButtonUp(RemoveQueue);
        EmitSignal(SignalName.RemoveFromQueue, (int)Id);
    }

    private void TweenButtonDown(TextureButton button)
    {
        var tween = CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(0.9f, 0.9f), 0.1f);
    }

    private void TweenButtonUp(TextureButton button)
    {
        var tween = CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(1.0f, 1.0f), 0.1f);
    }
}
