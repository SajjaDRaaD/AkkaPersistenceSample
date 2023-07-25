namespace AkkaPersistenceSample.Events;

public class PlayerCreate
{
    public PlayerCreate(string playerName)
    {
        PlayerName = playerName;
    }

    public string PlayerName { get; set; }
}