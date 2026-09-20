using Godot;

public partial class UiButton : TextureButton
{
    [Export] private Control TweenWith;

    public override void _Ready()
    {
        base._Ready();

        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
    }

    public void OnButtonDown()
    {
        var tween = CreateTween().SetParallel();
        tween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.1f);

        if (TweenWith != null)
        {
            tween.TweenProperty(TweenWith, "scale", new Vector2(0.9f, 0.9f), 0.1f);            
        }
    }

    public void OnButtonUp()
    {
        var tween = CreateTween().SetParallel();
        tween.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), 0.1f);

        if (TweenWith != null)
        {
            tween.TweenProperty(TweenWith, "scale", new Vector2(1.0f, 1.0f), 0.1f);          
        }
    }
}
