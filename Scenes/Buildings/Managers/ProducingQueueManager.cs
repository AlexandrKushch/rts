using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ProducingQueueManager : Node
{
    private const float MaxProgress = 100;

    public bool Progressing { get; set; } = false;
    public float CurrentProgress { get; set; } = 0;
    public UnitTypeIds CurrentProgressItem { get; set; }

    public Dictionary<UnitTypeIds, int> QueueCapacity { get; private set; } = new Dictionary<UnitTypeIds, int>();

    [Signal]
    public delegate void ProgressCompleteEventHandler(UnitTypeIds id);

    public override void _Process(double delta)
    {
        if (QueueCapacity.Count == 0) return;

        if (!Progressing)
        {
            var queueItem = QueueCapacity.FirstOrDefault();
            CurrentProgressItem = queueItem.Key;
            CurrentProgress = 0;
            Progressing = true;
        }

        CurrentProgress += 100 * (float)delta;

        if (CurrentProgress >= MaxProgress)
        {
            CurrentProgress = 0;

            RemoveFromQueue(CurrentProgressItem);
            EmitSignal(SignalName.ProgressComplete, (int)CurrentProgressItem);
        }
    }

    public void AddToQueue(UnitTypeIds id)
    {
        var costs = GlobalResources.Instance.Units[id].Cost.ToDictionary();

        if (!ResourceController.Instance.TrySpentCost(costs))
        {
            return;
        }

        if (!QueueCapacity.ContainsKey(id))
        {
            QueueCapacity = QueueCapacity.Append(new KeyValuePair<UnitTypeIds, int>(id, 1)).ToDictionary();
        }
        else
        {
            QueueCapacity[id] += 1;
        }
    }

    public void RemoveFromQueue(UnitTypeIds id, bool cashback = false)
    {
        SetProcess(false);
        if (QueueCapacity.ContainsKey(id))
        {
            QueueCapacity[id] -= 1;

            if (cashback)
            {
                var costs = GlobalResources.Instance.Units[id].Cost.ToDictionary();
                ResourceController.Instance.CollectCost(costs);
            }

            if (QueueCapacity[id] <= 0)
            {
                CurrentProgress = 0;
                Progressing = false;
                QueueCapacity.Remove(id);
            }
        }
        SetProcess(true);
    }
}
