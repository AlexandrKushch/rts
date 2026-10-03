using Godot;

public partial class MonkStateMachine : StateMachineBase<MonkStateIds>
{
    [Export] public Monk Monk { get; private set; }
}
