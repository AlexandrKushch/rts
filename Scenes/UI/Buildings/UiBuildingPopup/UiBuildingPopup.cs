using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class UiBuildingPopup : Control
{
    private const float MaxProgress = 100;
    private float _currentProgress = 0;
    private UiBuildingPopupItem _currentProgressItem;

    private Vector2 _itemOrigin;
    private Vector2 _itemScaleOrigin;

    private List<UiBuildingPopupItem> _popupItems = new List<UiBuildingPopupItem>();
    private Dictionary<UnitTypeIds, int> QueueCapacity = new Dictionary<UnitTypeIds, int>();

    [Export] private PackedScene UiBuildingPopupItem;
    [Export] private float Radius;
    [Export] private Vector2 OffsetCenter;
    [Export] private float DefaultOffset = 30;

    public bool Selected { get; set; } = false;
    public BuildingBase Building { get; set; }
    private UnitType[] Produces { get; set; }

    public override void _Ready()
    {
        Building = GetParent<BuildingBase>();

        Produces = Building.Resource.Produces;

        foreach (var produce in Building.Resource.Produces)
        {
            var item = UiBuildingPopupItem.Instantiate<UiBuildingPopupItem>();
            AddChild(item);

            item.Icon.Texture = produce.Icon;
            item.Id = produce.Id;
            item.AddToQueue += AddToQueue;
            item.RemoveFromQueue += RemoveFromQueue;

            _popupItems.Add(item);
        }

        _itemOrigin = _popupItems[0].Position;
        _itemScaleOrigin = _popupItems[0].Scale;

        Colapse();
    }

    public void Expand()
    {
        int count = _popupItems.Count;

        float offset = Mathf.Min(360, DefaultOffset * count) / count;
        int halfCount = count / 2;

        for (int i = -halfCount, index = 0; i <= halfCount; i++)
        {
            if (count % 2 == 0 && i == 0)
            {
                continue;
            }

            float offsetIndex = count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            float angle = Mathf.DegToRad((offset * offsetIndex) - 90);
            var item = _popupItems[index];

            item.Visible = true;
            item.Scale = _itemScaleOrigin;
            item.Position = _itemOrigin + OffsetCenter + new Vector2(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle));
            item.SetExpanded(true);
            if (QueueCapacity.ContainsKey(item.Id))
            {
                UpdateLabel(item.Id, QueueCapacity[item.Id]);
            }
            else
            {
                item.RemoveQueue.Visible = false;
            }
            index++;
        }
    }

    public void Colapse()
    {
        var remainVisible = _popupItems.Where(x => QueueCapacity.ContainsKey(x.Id)).ToArray();
        var toHide = _popupItems.Where(x => !QueueCapacity.ContainsKey(x.Id)).ToArray();

        foreach (var item in toHide)
        {
            item.Position = _itemOrigin;
            item.SetExpanded(false);
            item.Visible = false;
        }

        int count = remainVisible.Length;
        int halfCount = count / 2;
        float offset = 30;

        for (int i = -halfCount, index = 0; i <= halfCount; i++)
        {
            if (count % 2 == 0 && i == 0)
            {
                continue;
            }

            float offsetIndex = count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            var item = remainVisible[index];

            item.Scale = _itemScaleOrigin * 0.5f;
            item.Position = _itemOrigin + new Vector2(offset * offsetIndex, 0);
            item.SetExpanded(false);
            index++;
        }
    }

    public override void _Process(double delta)
    {
        Scale = Vector2.One * (2.5f - (float)Mathf.Remap(GetViewport().GetCamera2D().Zoom.X, 0.25, 2, 0, 1.5));
        Visible = Selected || QueueCapacity.Count > 0;
        if (QueueCapacity.Count == 0) return;

        if (_currentProgressItem == null)
        {
            var queueItem = QueueCapacity.FirstOrDefault();
            _currentProgressItem = _popupItems.FirstOrDefault(x => x.Id == queueItem.Key);
            _currentProgress = 0;
        }

        _currentProgress += 100 * (float)delta;

        _currentProgressItem.ProgressBar.Value = _currentProgress;

        if (_currentProgress >= MaxProgress)
        {
            _currentProgress = 0;

            _currentProgressItem.ProgressBar.Value = _currentProgress;

            RemoveFromQueue(_currentProgressItem.Id);
            _currentProgressItem = null;

            if (!Selected)
            {
                Colapse();
            }
        }
    }

    public void AddToQueue(UnitTypeIds id)
    {
        GD.Print($"Add: {id}");

        if (!QueueCapacity.ContainsKey(id))
        {
            QueueCapacity = QueueCapacity.Append(new KeyValuePair<UnitTypeIds, int>(id, 1)).ToDictionary();
        }
        else
        {
            QueueCapacity[id] += 1;
        }

        UpdateLabel(id, QueueCapacity[id]);
    }

    public void RemoveFromQueue(UnitTypeIds id)
    {
        GD.Print($"Remove: {id}");

        if (QueueCapacity.ContainsKey(id))
        {
            QueueCapacity[id] -= 1;

            UpdateLabel(id, QueueCapacity[id]);

            if (QueueCapacity[id] <= 0)
            {
                _currentProgress = 0;
                _currentProgressItem.ProgressBar.Value = _currentProgress;
                _currentProgressItem = null;
                QueueCapacity.Remove(id);
            }
        }
    }

    private void UpdateLabel(UnitTypeIds id, int value)
    {
        var popupItem = _popupItems.FirstOrDefault(x => x.Id == id);

        if (popupItem != null)
        {
            popupItem.InQueue.Visible = value != 0;
            popupItem.RemoveQueue.Visible = popupItem.Expanded && value != 0;
            popupItem.InQueue.Text = value.ToString();
        }
    }
}
