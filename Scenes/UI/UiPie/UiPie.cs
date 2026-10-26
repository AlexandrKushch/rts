using Godot;
using System;
using System.Linq;

[Tool]
public partial class UiPie : Control
{
    public bool Selected { get; set; } = false;

    [ExportToolButton("Expand")] public Callable ExpandButton => Callable.From(Expand);
    [ExportToolButton("Colapse")] public Callable ColapseButton => Callable.From(Colapse);

    [Export] private float Radius;
    [Export] private float ButtonScale = 1.0f;
    [Export] private float DefaultOffset = 30;

    public void Expand()
    {
        Show();
        var items = GetChildren().Select(x => x as Control).ToList();
        int count = items.Count;

        if (count == 0)
        {
            Hide();
            return;
        }

        float offset = Mathf.Min(360, DefaultOffset * count) / count;
        int halfCount = count / 2;

        Tween tween = CreateTween().SetParallel();

        for (int i = -halfCount, index = 0; i <= halfCount; i++)
        {
            if (count % 2 == 0 && i == 0)
            {
                continue;
            }

            float offsetIndex = count % 2 == 0 ? i - 0.5f * Math.Sign(i) : i;
            float angle = Mathf.DegToRad((offset * offsetIndex) - 90);
            var item = items[index];

            item.Visible = true;

            item.Scale = Vector2.One * ButtonScale * 0.5f;
            item.Position = Vector2.Zero;
            var itemTween = CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetParallel();
            float itemTweenDuration = 0.2f;
            var tweenToPos = new Vector2(Radius * Mathf.Cos(angle), Radius * Mathf.Sin(angle));

            tween.TweenSubtween(itemTween).SetDelay(index * itemTweenDuration / 10);
            itemTween.TweenProperty(item, "scale", Vector2.One * ButtonScale, itemTweenDuration);
            itemTween.TweenProperty(item, "position", tweenToPos, itemTweenDuration);

            index++;
        }
    }

    public void Colapse()
    {
        var items = GetChildren().Select(x => x as Control).ToList();
        int count = items.Count;

        if (count == 0)
        {
            Hide();
            return;
        }

        Tween tween = CreateTween().SetParallel();

        for (int i = 0; i < count; i++)
        {
            var item = items[i];

            var itemTween = CreateTween()
                .SetTrans(Tween.TransitionType.Sine)
                .SetParallel();
            float itemTweenDuration = 0.2f;
            tween.TweenSubtween(itemTween).SetDelay(i * itemTweenDuration / 10);
            itemTween.TweenProperty(item, "scale", Vector2.One * ButtonScale * 0.5f, itemTweenDuration);
            itemTween.TweenProperty(item, "position", Vector2.Zero, itemTweenDuration);
        }

        tween.Finished += Hide;
    }
}
