using Godot;

public partial class SheepStateMachine : StateMachineBase<SheepStateIds>
{
    [Export] public Sheep Sheep { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        
        ChangeState(SheepStateIds.Idle);
        SetProcess(true);
    }
}
