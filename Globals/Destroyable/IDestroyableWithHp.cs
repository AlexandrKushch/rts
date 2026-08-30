public interface IDestroyableWithHp : IDestroyable
{
    public int HP { get; set; }

    void TakeDamage(int value);
}
