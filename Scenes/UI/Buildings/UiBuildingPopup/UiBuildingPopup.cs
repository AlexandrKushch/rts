using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class UiBuildingPopup : Control
{
    private Vector2 _itemOrigin;
    private Vector2 _itemScaleOrigin;

    private Dictionary<UnitTypeIds, UiBuildingPopupItem> _popupItems = new Dictionary<UnitTypeIds, UiBuildingPopupItem>();

    [Export] private PackedScene UiBuildingPopupItem;
    [Export] private float Radius;
    [Export] private Vector2 OffsetCenter;
    [Export] private float DefaultOffset = 30;

    [Export] private ProducingQueueManager ProducingQueueManager;

    public bool Selected { get; set; } = false;
    public BuildingBase Building { get; set; }
    public UnitType[] Produces { get; set; }

    public override void _Ready()
    {
        Building = GetParent<BuildingBase>();

        Produces = Building.Resource.Produces;

        SetProcess(Produces.Length != 0);

        if (Produces.Length == 0) return;

        foreach (var produce in Building.Resource.Produces)
        {
            var item = UiBuildingPopupItem.Instantiate<UiBuildingPopupItem>();
            AddChild(item);

            item.Icon.Texture = produce.Icon;
            item.Id = produce.Id;
            item.AddToQueue += AddToQueue;
            item.RemoveFromQueue += (id) => { RemoveFromQueue(id, true); };

            _popupItems.Add(produce.Id, item);
        }

        _itemOrigin = _popupItems.First().Value.Position;
        _itemScaleOrigin = _popupItems.First().Value.Scale;

        ProducingQueueManager.ProgressComplete += ProgressComplete;
        Colapse();
    }

    public void Expand(bool useTween)
    {
        var popupItemsList = _popupItems.Select(x => x.Value).ToArray();
        int count = popupItemsList.Length;

        if (count == 0) return;

        float offset = Mathf.Min(360, DefaultOffset * count) / count;
        int halfCount = count / 2;

        Tween tween = null;
        if (useTween)
        {
            tween = CreateTween().SetParallel();
        }

        for (int i = -halfCount, index = 0; i <= halfCount; i++)
        {
            if (count % 2 == 0 && i == 0)
            {
                continue;
            }

            float offsetIndex = count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            float angle = Mathf.DegToRad((offset * offsetIndex) - 90);
            var item = popupItemsList[index];

            item.Visible = true;
            item.SetExpanded(true);
            if (ProducingQueueManager.QueueCapacity.ContainsKey(item.Id))
            {
                UpdateLabel(item.Id, ProducingQueueManager.QueueCapacity[item.Id]);
            }
            else
            {
                item.RemoveQueue.Visible = false;
            }

            if (useTween && tween != null)
            {
                item.Scale = _itemScaleOrigin * 0.5f;
                item.Position = _itemOrigin;
                var itemTween = CreateTween()
                    .SetTrans(Tween.TransitionType.Sine)
                    .SetParallel();
                float itemTweenDuration = 0.2f;
                var tweenToPos = _itemOrigin + OffsetCenter + new Vector2(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle));

                tween.TweenSubtween(itemTween).SetDelay(index * itemTweenDuration / 10);
                itemTween.TweenProperty(item, "scale", _itemScaleOrigin, itemTweenDuration);
                itemTween.TweenProperty(item, "position", tweenToPos, itemTweenDuration);
            }

            index++;
        }
    }

    public void Colapse()
    {
        if (_popupItems.Count == 0) return;

        var remainVisible = _popupItems.Where(x => ProducingQueueManager.QueueCapacity.ContainsKey(x.Key)).ToArray();
        var toHide = _popupItems.Where(x => !ProducingQueueManager.QueueCapacity.ContainsKey(x.Key)).ToArray();

        foreach (var item in toHide)
        {
            item.Value.SetExpanded(false);
            item.Value.Position = _itemOrigin;
            item.Value.Visible = false;
        }

        int count = remainVisible.Length;
        int halfCount = count / 2;
        float offset = 30;

        Tween tween = null;

        if (count > 0)
        {
            tween = CreateTween().SetParallel();
        }

        for (int i = -halfCount, index = 0; i <= halfCount; i++)
        {
            if (count % 2 == 0 && i == 0)
            {
                continue;
            }

            float offsetIndex = count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            var item = remainVisible[index];
            item.Value.SetExpanded(false);

            var itemTween = CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetParallel();
            float itemTweenDuration = 0.2f;
            tween.TweenSubtween(itemTween).SetDelay(index * itemTweenDuration / 10);
            itemTween.TweenProperty(item.Value, "scale", _itemScaleOrigin * 0.5f, itemTweenDuration);
            itemTween.TweenProperty(item.Value, "position", _itemOrigin + new Vector2(offset * offsetIndex, 0), itemTweenDuration);

            index++;
        }
    }

    public override void _Process(double delta)
    {
        Scale = Vector2.One * (2.5f - (float)Mathf.Remap(GetViewport().GetCamera2D().Zoom.X, 0.25, 2, 0, 1.5));
        Visible = Selected || ProducingQueueManager.QueueCapacity.Count > 0;

        if (_popupItems.ContainsKey(ProducingQueueManager.CurrentProgressItem))
        {
            _popupItems[ProducingQueueManager.CurrentProgressItem].ProgressBar.Value = ProducingQueueManager.CurrentProgress;
        }
    }

    public void ProgressComplete(UnitTypeIds id)
    {
        if (!Selected)
        {
            Colapse();
        }

        if (_popupItems.TryGetValue(id, out var popupItem))
        {
            popupItem.ProgressBar.Value = 0;
        }

        if (ProducingQueueManager.QueueCapacity.ContainsKey(id))
        {
            UpdateLabel(id, ProducingQueueManager.QueueCapacity[id]);
        }
        else
        {
            UpdateLabel(id, 0);
        }
    }

    public void AddToQueue(UnitTypeIds id)
    {
        ProducingQueueManager.AddToQueue(id);
        UpdateLabel(id, ProducingQueueManager.QueueCapacity[id]);
    }

    public void RemoveFromQueue(UnitTypeIds id, bool cashback = false)
    {
        if (!ProducingQueueManager.QueueCapacity.ContainsKey(id))
        {
            return;
        }
        UpdateLabel(id, ProducingQueueManager.QueueCapacity[id] - 1);
        ProducingQueueManager.RemoveFromQueue(id, cashback);
    }

    private void UpdateLabel(UnitTypeIds id, int value)
    {
        if (_popupItems.TryGetValue(id, out var popupItem))
        {
            popupItem.InQueue.Visible = value != 0;
            popupItem.RemoveQueue.Visible = popupItem.Expanded && value != 0;
            popupItem.InQueue.Text = value.ToString();
        }
    }
}
