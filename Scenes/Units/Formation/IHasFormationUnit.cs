using Godot;

public interface IHasFormationUnit
{
    public Formation Formation { get; set; }

    void AttachTo(Node2D targetObject);

    void Detach();

    void Destroy();
}
