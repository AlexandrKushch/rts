using Godot;

public partial class UiQuableItem : Control, IHasTooltip
{
    private UiButton AddQueue;
    public ProducingQueueManager ProducingQueueManager { get; set; }
    public bool Readonly { get; set; }

    public UiButton RemoveQueue { get; set; }
    public UnitTypeIds Id { get; set; }
    public TextureRect Icon { get; set; }
    public Label InQueue { get; set; }
    public TextureProgressBar ProgressBar { get; set; }

    [Signal]
    public delegate void AddToQueueEventHandler(UnitTypeIds Id);

    [Signal]
    public delegate void RemoveFromQueueEventHandler(UnitTypeIds Id);

    public override void _Ready()
    {
        Icon = GetNode<TextureRect>(nameof(Icon));
        AddQueue = GetNode<UiButton>(nameof(AddQueue));
        AddQueue.Readonly = Readonly;
        RemoveQueue = GetNode<UiButton>(nameof(RemoveQueue));
        RemoveQueue.Readonly = Readonly;
        InQueue = GetNode<Label>(nameof(InQueue));
        ProgressBar = GetNode<TextureProgressBar>(nameof(ProgressBar));

        InQueue.Visible = false;
        RemoveQueue.Visible = false;

        if (!Readonly)
        {
            AddQueue.MouseEntered += OnMouseEntered;
            AddQueue.MouseExited += OnMouseExited;
        }

        if (IsInstanceValid(ProducingQueueManager))
        {
            ProducingQueueManager.ProgressComplete += ProgressComplete;
        }

        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        if (ProducingQueueManager.CurrentProgressItem == Id)
        {
            ProgressBar.Value = ProducingQueueManager.CurrentProgress;
        }
    }

    public void OnAddButton()
    {
        if (Readonly || !IsInstanceValid(ProducingQueueManager))
        {
            return;
        }

        ProducingQueueManager.AddToQueue(Id);
        if (ProducingQueueManager.QueueCapacity.ContainsKey(Id))
        {
            UpdateLabel(ProducingQueueManager.QueueCapacity[Id]);
        }
        SetProcess(ProducingQueueManager.QueueCapacity.ContainsKey(Id));
    }

    public void OnRemoveButton()
    {
        if (Readonly || !IsInstanceValid(ProducingQueueManager))
        {
            return;
        }

        if (!ProducingQueueManager.QueueCapacity.ContainsKey(Id))
        {
            return;
        }
        UpdateLabel(ProducingQueueManager.QueueCapacity[Id] - 1);
        ProducingQueueManager.RemoveFromQueue(Id, true);
        SetProcess(ProducingQueueManager.QueueCapacity.ContainsKey(Id));
        ProgressBar.Value = ProducingQueueManager.CurrentProgressItem == Id ? ProducingQueueManager.CurrentProgress : 0;
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

    public void ProgressComplete(UnitTypeIds id)
    {
        if (id != Id)
        {
            return;
        }

        ProgressBar.Value = 0;
        SetProcess(ProducingQueueManager.QueueCapacity.ContainsKey(Id));

        if (ProducingQueueManager.QueueCapacity.ContainsKey(id))
        {
            UpdateLabel(ProducingQueueManager.QueueCapacity[id]);
        }
        else
        {
            UpdateLabel(0);
        }
    }

    public void UpdateLabel(int value)
    {
        InQueue.Visible = value != 0;
        ProgressBar.Value = ProducingQueueManager.CurrentProgressItem == Id ? ProducingQueueManager.CurrentProgress : 0;
        RemoveQueue.Visible = !Readonly && value != 0;
        InQueue.Text = value.ToString();
    }
}
