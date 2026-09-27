using Godot;

public partial class UnitVisualBase : Node2D
{
    protected const int TextureKeyId = 2;

    private ShaderMaterial SpriteShaderMaterial;

    protected Sprite2D Sprite2D;
    protected OutlineVisual Outline;
    protected AnimationPlayer AnimationPlayer;

    [Signal]
    public delegate void OnInteractAnimationFinishedEventHandler();

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        SpriteShaderMaterial = (Sprite2D.Material as ShaderMaterial).Duplicate(true) as ShaderMaterial;
        Sprite2D.Material = SpriteShaderMaterial;
        Outline = GetNode<OutlineVisual>(nameof(Outline));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public virtual void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0
            ? UnitAnimationNames.Run
            : UnitAnimationNames.Idle;

        if (!string.IsNullOrWhiteSpace(animationLibraryName))
        {
            animation = $"{animationLibraryName}/{animation}";
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public virtual void TakeDamage()
    {
        SpriteShaderMaterial.SetShaderParameter("on", true);

        var tween = CreateTween().SetParallel();
        float tweenDuration = 0.15f;


        var tweenColor = CreateTween();
        tweenColor.TweenProperty(SpriteShaderMaterial, "shader_parameter/color", ColorsGlobal.Yellow, tweenDuration * 0.33f);
        tweenColor.TweenProperty(SpriteShaderMaterial, "shader_parameter/color", ColorsGlobal.Error, tweenDuration * 0.33f);
        tweenColor.TweenProperty(SpriteShaderMaterial, "shader_parameter/color", ColorsGlobal.Yellow, tweenDuration * 0.33f);
        tweenColor.Finished += () => { SpriteShaderMaterial.SetShaderParameter("on", false); };

        var tweenScale = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        tweenScale.TweenProperty(this, "scale", new Vector2(0.8f, 1.2f), tweenDuration * 0.5f);
        tweenScale.TweenProperty(this, "scale", Vector2.One, tweenDuration * 0.5f);

        tween.TweenSubtween(tweenColor);
        tween.TweenSubtween(tweenScale);
    }

    public virtual void Stop()
    {
        AnimationPlayer.Stop();
    }

    public void UpdateOutlineVisible(bool value)
    {
        Outline.UpdateVisible(value);
    }
}
