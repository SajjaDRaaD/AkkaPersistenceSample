namespace AkkaPersistenceSample.Events;

public class PlayerHit
{
    public PlayerHit(int damage)
    {
        Damage = damage;
    }

    public int Damage { get; set; }
}