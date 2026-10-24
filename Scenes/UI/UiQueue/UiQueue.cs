using Godot;
using System.Linq;

public partial class UiQueue : Control
{
    private HBoxContainer _queueContainer;
    public HBoxContainer QueueContainer
    {
        get
        {
            if (!IsInstanceValid(_queueContainer))
            {
                _queueContainer = GetNode<HBoxContainer>(nameof(QueueContainer));
            }

            return _queueContainer;
        }
    }
    public ProducingQueueManager ProducingQueueManager { get; set; }

    public void Expand()
    {
        if (ProducingQueueManager == null)
        {
            return;
        }

        Show();

        var queue = ProducingQueueManager.QueueCapacity.ToList();

        var children = QueueContainer.GetChildren()
            .Select(x => x as UiQuableItem);
        var itemsToShow = children
            .Where(x => ProducingQueueManager.QueueCapacity.ContainsKey(x.Id))
            .OrderBy(x =>
            {
                var pair = queue.First(y => y.Key == x.Id);
                return queue.IndexOf(pair);
            })
            .ToList();
        var itemsToHide = children
            .Where(x => !ProducingQueueManager.QueueCapacity.ContainsKey(x.Id))
            .ToList();

        foreach (var item in itemsToHide)
        {
            item.SetProcess(false);
            item.Hide();
        }

        int count = itemsToShow.Count;

        if (count == 0)
        {
            Hide();
            return;
        }

        int index = 0;

        foreach (var item in itemsToShow)
        {
            QueueContainer.MoveChild(item, index);
            item.SetProcess(true);
            item.Show();
            item.UpdateLabel(ProducingQueueManager.QueueCapacity[item.Id]);
            index++;
        }

        UpdateWidth();
    }

    public void Colapse()
    {
        if (ProducingQueueManager == null)
        {
            return;
        }

        var items = QueueContainer.GetChildren().Select(x => x as UiQuableItem).Where(x => x.Visible).ToList();
        
        foreach (var item in items)
        {
            item.SetProcess(false);
            item.Hide();
        }

        Hide();
    }

    public void ProgressComplete(UnitTypeIds id)
    {
        if (ProducingQueueManager.QueueCapacity.Count == 0)
        {
            Hide();
            return;
        }

        if (!ProducingQueueManager.QueueCapacity.ContainsKey(id))
        {
            var toHide = QueueContainer.GetChildren()
                .Select(x => x as UiQuableItem)
                .First(x => x.Id == id);

            toHide.SetProcess(false);
            toHide.Hide();
        }

        UpdateWidth();
    }

    private void UpdateWidth()
    {
        var count = QueueContainer.GetChildren().Count(x => (x as UiQuableItem).Visible);

        float newWidth = 200 * count;

        Size = new Vector2(newWidth, Size.Y);
        Position = new Vector2(
            count == 1 ? -newWidth + newWidth / 4 : -newWidth / 2,
            Position.Y
        );
    }
}
