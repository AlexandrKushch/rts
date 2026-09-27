using Godot;

public interface IDestroyableWithHp : IDestroyable
{
    public int HP { get; set; }

    void TakeDamage(int value);

    void TakeDamageWithDash(Vector2 from, int value, float power);
}
