using Akka.Persistence;

namespace AkkaPersistenceSample.Actors;

public class PlayerActor : ReceivePersistentActor
{

    private readonly string _playerName;
    private int _health;
    public override string PersistenceId => $"Player-{_playerName}";

    public PlayerActor(string playerName, int startingHealth)
    {
        _playerName = playerName;
        _health = startingHealth;
        
        Command<HitMessage>(HitMessageHandler);
        Command<DisplayPlayerStatusMessage>(DisplayPlayerStatusMessageHandler);
        Command<CauseErrorMessage>(CauseErrorMessageHandler);

        Recover<HitMessage>(message =>
        {
            Console.WriteLine($"Recover Hit Message from Journal Store ...");
            _health -= message.Damage;
        });
    }

    private void CauseErrorMessageHandler(CauseErrorMessage message)
    {
        Console.WriteLine("System Message : Simulating a error.");
        throw new NotImplementedException();
    }

    private void DisplayPlayerStatusMessageHandler(DisplayPlayerStatusMessage message)
    {
        Console.WriteLine($"System Message : {_playerName} has {_health} hp.");
    }

    private void HitMessageHandler(HitMessage message)
    {
        Persist(message, _ =>
        {
            Console.WriteLine($"System Message : Hit Message Persisted in {Self.Path.Name}.");
            _health -= message.Damage;
        });
    }
}