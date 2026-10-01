using Godot;

public partial class WarriorStateMachine : StateMachineBase<WarriorStateIds>
{
    [Export] public Warrior Warrior { get; private set; }
}
