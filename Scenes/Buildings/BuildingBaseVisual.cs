using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class BuildingBaseVisual : Node2D
{
    private const int MaxFireParticlesCount = 7;

    private BuildingBase _buildingBase;
    private ShaderMaterial _shader;
    private Vector2 _fireBoundaryPosition;
    private RectangleShape2D _fireBoundary;

    private List<Node2D> _fireParticles;

    public Sprite2D Sprite2D { get; private set; }
    public GpuParticles2D DestroyParticles01 { get; set; }
    public GpuParticles2D DestroyParticles02 { get; set; }
    public GpuParticles2D BuildProgressParticles { get; private set; }

    public override void _Ready()
    {
        _buildingBase = GetParent<BuildingBase>();
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        BuildProgressParticles = GetNode<GpuParticles2D>(nameof(BuildProgressParticles));
        DestroyParticles01 = GetNode<GpuParticles2D>(nameof(DestroyParticles01));
        DestroyParticles02 = GetNode<GpuParticles2D>(nameof(DestroyParticles02));

        _shader = (Sprite2D.Material as ShaderMaterial).Duplicate(true) as ShaderMaterial;
        Sprite2D.Material = _shader;
        UpdateProgress(1.0f);

        _fireParticles = new List<Node2D>();
        var hurtBoxCollision = _buildingBase.GetNode<HurtBox>(nameof(HurtBox))
            .GetNode<CollisionShape2D>(nameof(CollisionShape2D));
        _fireBoundaryPosition = hurtBoxCollision.Position;
        _fireBoundary = hurtBoxCollision
            .Shape as RectangleShape2D;

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

    public void Destroy()
    {
        var tween = CreateTween();
        float tweenDuration = 0.2f;

        float positionShake = 2;
        var origPosition = Sprite2D.Position;
        var tweenPosition = CreateTween().SetTrans(Tween.TransitionType.Bounce);
        tweenPosition.TweenProperty(Sprite2D, "position", origPosition + Vector2.Right * +positionShake, tweenDuration * 0.2f);
        tweenPosition.TweenProperty(Sprite2D, "position", origPosition + Vector2.Right * -positionShake, tweenDuration * 0.2f);
        tweenPosition.TweenProperty(Sprite2D, "position", origPosition + Vector2.Right * +positionShake, tweenDuration * 0.2f);
        tweenPosition.TweenProperty(Sprite2D, "position", origPosition + Vector2.Right * -positionShake, tweenDuration * 0.2f);
        tweenPosition.TweenProperty(Sprite2D, "position", origPosition, tweenDuration * 0.2f);

        var tweenAlpha = CreateTween();
        // float tweenAlphaDuration = 0.3f;
        var zeroAlhpaColor = new Color(0, 0, 0, 0);
        tweenAlpha.TweenProperty(Sprite2D, "modulate", zeroAlhpaColor, tweenDuration);

        tween.TweenSubtween(tweenPosition);
        tween.TweenSubtween(tweenAlpha);

        DestroyParticles01.Emitting = true;
        DestroyParticles02.Emitting = true;

        foreach (var fire in _fireParticles)
        {
            fire.Visible = false;
        }
    }

    public void UpdateFireParticles(int hp, int maxHp)
    {
        var newParticlesCount = (int)Mathf.Remap(hp, 0, maxHp, MaxFireParticlesCount, 0);
        var currentParticlesCount = _fireParticles.Count(IsInstanceValid);

        if (newParticlesCount > MaxFireParticlesCount)
        {
            return;
        }

        if (newParticlesCount > currentParticlesCount)
        {
            int fireId = (int)Mathf.Ceil(currentParticlesCount / 3);
            var fireScene = GlobalResources.Instance.EffectScenes[(EffectType)fireId + 3].Instantiate<Node2D>();
            fireScene.Position = RandomExtension.GetRandomPointInRect(_fireBoundary) + _fireBoundaryPosition + Vector2.Up * 40;
            AddChild(fireScene);
            _fireParticles.Add(fireScene);
        }
        else if (newParticlesCount < currentParticlesCount)
        {
            var toRemove = _fireParticles.LastOrDefault(IsInstanceValid);
            toRemove.QueueFree();
            _fireParticles.Remove(toRemove);
        }
    }
}
