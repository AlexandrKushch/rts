using Godot;

public partial class Dead : Node2D
{
    private const string BurriedAnimationName = "Burried";

    private Sprite2D Sprite2D;
    private AnimationPlayer AnimationPlayer;
    private Timer Timer;

    public override void _Ready()
    {
        Sprite2D = GetNode<Sprite2D>(nameof(Sprite2D));
        AnimationPlayer = GetNode<AnimationPlayer>(nameof(AnimationPlayer));
        Timer = GetNode<Timer>(nameof(Timer));

        Timer.Timeout += TimerTimeout;
    }

    private void TimerTimeout()
    {
        AnimationPlayer.Play(BurriedAnimationName);
    }
}
