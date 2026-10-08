using Godot;
using System;
using System.Collections.Generic;

public partial class LancerVisual : UnitVisualBase
{
    private Vector2 _direction;

    private Dictionary<Vector2, string> _defenceDirections = new Dictionary<Vector2, string>
    {
        [Vector2.Zero] = UnitAnimationNames.Idle,
        [Vector2.Down] = UnitAnimationNames.Lancer.UpDefence,
        [new Vector2(1, 1)] = UnitAnimationNames.Lancer.UpRightDefence,
        [Vector2.Up] = UnitAnimationNames.Lancer.DownDefence,
        [new Vector2(1, -1)] = UnitAnimationNames.Lancer.DownRightDefence,
        [Vector2.Right] = UnitAnimationNames.Lancer.RightDefence,
    };

    private Dictionary<Vector2, string> _attackDirections = new Dictionary<Vector2, string>
    {
        [Vector2.Zero] = UnitAnimationNames.Idle,
        [Vector2.Down] = UnitAnimationNames.Lancer.UpAttack,
        [new Vector2(1, 1)] = UnitAnimationNames.Lancer.UpRightAttack,
        [Vector2.Up] = UnitAnimationNames.Lancer.DownAttack,
        [new Vector2(1, -1)] = UnitAnimationNames.Lancer.DownRightAttack,
        [Vector2.Right] = UnitAnimationNames.Lancer.RightAttack,
    };

    [Signal]
    public delegate void OnAttackAnimationFinishedEventHandler();

    public override void _Ready()
    {
        base._Ready();

        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains("attack", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        base.UpdateMovement(velocity, animationLibraryName);
    }


    public void UpdateDefence(Vector2? attackTarget)
    {
        if (AnimationPlayer.CurrentAnimation.ToString().Contains("attack", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        string animation;

        if (attackTarget == null)
        {
            animation = _defenceDirections[Vector2.Zero];
        }
        else
        {
            _direction = GlobalPosition.DirectionTo(attackTarget.Value) * new Vector2(1, -1);

            Sprite2D.FlipH = _direction.X < 0;
            _direction.X = Mathf.Abs(_direction.X);
            _direction.X = (float)Math.Round(_direction.X, MidpointRounding.AwayFromZero);
            _direction.Y = (float)Math.Round(_direction.Y, MidpointRounding.AwayFromZero);

            animation = _defenceDirections[_direction];
        }

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    public void Attack()
    {
        string animation = _attackDirections[_direction];

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }

    private void OnAnimationFinished(StringName animation)
    {
        if (animation.ToString().Contains("attack", StringComparison.OrdinalIgnoreCase))
        {
            EmitSignal(SignalName.OnAttackAnimationFinished);
        }
    }
}
