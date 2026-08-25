using Godot;

public partial class PawnVisual : Node
{
    private Sprite2D Sprite2D;
    private AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
    }

    public void UpdateMovement(Vector2 velocity)
    {
        Sprite2D.FlipH = velocity.Length() > 0 ? velocity.X < 0 : Sprite2D.FlipH;

        string animation = velocity.Length() > 0 ? PawnAnimationNames.Run : PawnAnimationNames.Idle;

        if (AnimationPlayer.CurrentAnimation.Equals(animation))
        {
            return;
        }

        AnimationPlayer.Play(animation);
    }
}
