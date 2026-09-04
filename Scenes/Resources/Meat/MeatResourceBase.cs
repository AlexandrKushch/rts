using Godot;

public partial class MeatResourceBase : ResourceBase
{
    [Export] private Node2D Visual;
    
    public override void CollectOne()
    {
        base.CollectOne();

        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Bounce);
        float tweenDuration = 0.25f;
        tween.TweenProperty(Visual, "scale", new Vector2(1.1f, 0.9f), tweenDuration * 0.5f);
        tween.TweenProperty(Visual, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.5f);
    }
}
