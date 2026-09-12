using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class UiSelectedUnitsContainer : HBoxContainer
{
    private float _bgBannerOneItemWidth;

    private Dictionary<UnitTypeIds, int> _selectedUnitsCount = new Dictionary<UnitTypeIds, int>();
    private Dictionary<UnitTypeIds, UiSelectedUnitItem> _items = new Dictionary<UnitTypeIds, UiSelectedUnitItem>();

    [Export] private PackedScene UiSelectedUnitItemScene;

    [Export] private NinePatchRect BgBanner;

    public override void _Ready()
    {
        foreach (var item in GetChildren())
        {
            item.QueueFree();       
        }

        BgBanner.Visible = false;
        _bgBannerOneItemWidth = BgBanner.Size.X;

        UnitsController.Instance.Connect(UnitsController.SignalName.SelectionChanged, Callable.From(UpdateUI));
    }

    public void UpdateUI()
    {
        var newSelectedUnitsCount = UnitsController.Instance.Selections
            .Select(x => x.EffectedOn as UnitBase)
            .Where(x => x != null)
            .GroupBy(x => x.Meta.Id)
            .ToDictionary(x => x.Key, x => x.Count());
        
        var itemsToAdd = newSelectedUnitsCount.Where(x => !_selectedUnitsCount.ContainsKey(x.Key));
        var itemsToRemove = _selectedUnitsCount.Where(x => !newSelectedUnitsCount.ContainsKey(x.Key));

        foreach (var item in itemsToAdd)
        {
            AddItem(item.Key);
        }

        foreach (var item in itemsToRemove)
        {
            RemoveItem(item.Key);
        }

        _selectedUnitsCount = newSelectedUnitsCount;

        UpdateItemsCount();
        UpdateBgBannerWidth();
    }

    private void UpdateItemsCount()
    {
        foreach (var selectedUnitCount in _selectedUnitsCount)
        {
            var item = _items[selectedUnitCount.Key];
            int count = _selectedUnitsCount[selectedUnitCount.Key];

            if (count > 1)
            {
                item.Count.Visible = true;
                item.Count.Text = count.ToString();
            }
            else
            {
                item.Count.Visible = false;
            }
        }
    }

    private void UpdateBgBannerWidth()
    {
        bool oldVisible = BgBanner.Visible;
        bool newVisible = _items.Count != 0;

        if (!newVisible)
        {
            var tween = CreateTween()
                .SetTrans(Tween.TransitionType.Bounce);
            float tweenDuration = 0.2f;
            tween.TweenProperty(BgBanner, "size", BgBanner.Size + Vector2.Right * 50, tweenDuration / 2);
            tween.TweenProperty(BgBanner, "size", BgBanner.Size, tweenDuration / 2);
            tween.Finished += () => { BgBanner.Visible = false; };
            return;
        }

        BgBanner.Visible = newVisible;

        float oldWidth = BgBanner.Size.X;
        float newWidth = _items.Count > 1
            ? _bgBannerOneItemWidth + (156 * (_items.Count - 1))
            : _bgBannerOneItemWidth;
        BgBanner.Size = new Vector2(newWidth, BgBanner.Size.Y);

        if (newWidth != oldWidth || BgBanner.Visible != oldVisible)
        {
            var tween = CreateTween()
                .SetTrans(Tween.TransitionType.Bounce);
            float tweenDuration = 0.2f;
            tween.TweenProperty(BgBanner, "size", BgBanner.Size + Vector2.Right * 50, tweenDuration / 2);
            tween.TweenProperty(BgBanner, "size", BgBanner.Size, tweenDuration / 2);
        }
    }

    private void AddItem(UnitTypeIds unitId)
    {
        var item = UiSelectedUnitItemScene.Instantiate<UiSelectedUnitItem>();
        AddChild(item);

        item.Id = unitId;
        item.Icon.Texture = GlobalResources.Instance.Units[unitId].Icon;

        _items.Add(unitId, item);
        
        var tweenObject = item.Icon;

        var tweenPosition = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenPositionDuration = 0.2f;
        tweenPosition.TweenProperty(tweenObject, "position", tweenObject.Position + Vector2.Up * 25, tweenPositionDuration / 2);
        tweenPosition.TweenProperty(tweenObject, "position", tweenObject.Position, tweenPositionDuration / 2);

        var tweenSize = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        float tweenSizeDuration = 0.2f;
        tweenSize.TweenProperty(tweenObject, "scale", new Vector2(0.9f, 1.1f), tweenSizeDuration / 2);
        tweenSize.TweenProperty(tweenObject, "scale", Vector2.One, tweenSizeDuration / 2);
        
        var tween = CreateTween().SetParallel();
        tween.TweenSubtween(tweenSize);
        tween.TweenSubtween(tweenSize);
    }

    private void RemoveItem(UnitTypeIds unitId)
    {
        var item = _items[unitId];
        _items.Remove(unitId);
        item.QueueFree();
    }
}
