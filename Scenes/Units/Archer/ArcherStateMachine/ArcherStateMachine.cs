using Godot;

public partial class ArcherStateMachine : StateMachineBase<ArcherStateIds>
{
    [Export] public Archer Archer { get; private set; }
}
