using Godot;

public partial class LancerStateMachine : StateMachineBase<LancerStateIds>
{
    [Export] public Lancer Lancer { get; private set; }
}
