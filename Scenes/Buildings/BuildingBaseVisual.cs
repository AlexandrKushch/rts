using Godot;

public partial class BuildingBaseVisual : Node2D
{
    private BuildingBase _buildingBase;
    private ShaderMaterial _shader;

    public Sprite2D Sprite2D { get; private set; }
    public GpuParticles2D BuildProgressParticles { get; private set; }

    public override void _Ready()
    {
        _buildingBase = GetParent<BuildingBase>();
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        BuildProgressParticles = GetNode<GpuParticles2D>(nameof(BuildProgressParticles));

        _shader = (Sprite2D.Material as ShaderMaterial).Duplicate(true) as ShaderMaterial;
        Sprite2D.Material = _shader;
        UpdateProgress(1.0f);
        
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        BuildProgressParticles.Emitting = _buildingBase.BuildersCount > 0;
        if (_buildingBase.BuildersCount > 0)
        {
            BuildProgressParticles.Amount = _buildingBase.BuildersCount;
        }
    }

    public void Deploy()
    {
        var tween = CreateTween().SetParallel();
        float tweenDuration = 0.3f;

        var tweenScale = CreateTween().SetTrans(Tween.TransitionType.Quad);
        tweenScale.TweenProperty(this, "scale", new Vector2(0.9f, 1.1f), tweenDuration * 0.5);
        tweenScale.TweenProperty(this, "scale", new Vector2(1.05f, 0.95f), tweenDuration * 0.2);
        tweenScale.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.1);
        tweenScale.TweenProperty(this, "scale", new Vector2(1.0f, 1.0f), tweenDuration * 0.2);

        var tweenPosition = CreateTween();
        var origPos = Position;
        tweenPosition.TweenProperty(this, "position", origPos + Vector2.Up * 25, tweenDuration * 0.5);
        tweenPosition.TweenProperty(this, "position", origPos, tweenDuration * 0.2);
        tweenPosition.TweenProperty(this, "position", origPos, tweenDuration * 0.3);

        tween.TweenSubtween(tweenPosition);
        tween.TweenSubtween(tweenScale);

        SetProcess(true);
    }

    public void UpdateProgress(float value)
    {
        _shader.SetShaderParameter("progress", value);        
    }

    public void Built()
    {
        SetProcess(false);
        BuildProgressParticles.Emitting = false;
    }
}
