using Godot;

public partial class MonkVisual : UnitVisualBase
{
    private AnimationTree AnimationTree;

    public override void _Ready()
    {
        base._Ready();

        AnimationTree = GetNode<AnimationTree>(nameof(AnimationTree));
    }

    public override void UpdateMovement(Vector2 velocity, string animationLibraryName)
    {
        bool moving = velocity.Length() > 0;

        Sprite2D.FlipH = moving ? velocity.X < 0 : Sprite2D.FlipH;

        float movementParam = moving ? 1 : 0;

        AnimationTree.Set("parameters/Movement/blend_position", movementParam);
    }

    public override void Stop()
    {
        AnimationTree.Set("parameters/Heal/request", (int)AnimationNodeOneShot.OneShotRequest.Abort);   
    }

    public void UpdateHealing(bool value)
    {
        if (value)
        {
            AnimationTree.Set("parameters/Heal/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);            
        }
        else
        {
            AnimationTree.Set("parameters/SM_Healing/conditions/cancel", true);
        }
    }

    public void ResetCancelCondition()
    {
        AnimationTree.Set("parameters/SM_Healing/conditions/cancel", false);        
    }
}
