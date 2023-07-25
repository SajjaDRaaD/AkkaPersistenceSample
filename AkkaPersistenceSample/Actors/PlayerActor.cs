using Akka.Persistence;
using AkkaPersistenceSample.Commands;
using AkkaPersistenceSample.Events;

namespace AkkaPersistenceSample.Actors;


public class PlayerActorState
{
    public string PlayerName { get; set; }
    public int Health { get; set; }

    public PlayerActorState(string playerName, int health)
    {
        PlayerName = playerName ?? throw new ArgumentNullException(nameof(playerName));
        Health = health;
    }

    public override string ToString()
    {
        return $"[Player Actor State : {PlayerName},{Health}]";
    }
}

public class PlayerActor : ReceivePersistentActor
{

    private PlayerActorState _state;
    private int _eventCount;
    public override string PersistenceId => $"Player-{_state.PlayerName}";

    public PlayerActor(string playerName, int startingHealth)
    {
        _state = new PlayerActorState(playerName, startingHealth);
        
        Command<HitPlayer>(HitPlayerCommandHandler);
        Command<DisplayPlayerStatus>(DisplayPlayerStatusCommandHandler);
        Command<SimulateError>(CauseErrorCommandHandler);

        Recover<PlayerHit>(playerHitEvent =>
        {
            Console.WriteLine($"Recover Hit Event from Journal Store ...");
            _state.Health -= playerHitEvent.Damage;
        });

        Recover<SnapshotOffer>(offer =>
        {
            Console.WriteLine($"{_state.PlayerName} Received a snapshot offer from snapshot store.");
            _state = (PlayerActorState)offer.Snapshot;
            Console.WriteLine($"{_state.PlayerName} state {_state} set from snapshot offer.");
        });
    }

    private void CauseErrorCommandHandler(SimulateError command)
    {
        Console.WriteLine("System Message : Simulating a error.");
        throw new Exception();
    }

    private void DisplayPlayerStatusCommandHandler(DisplayPlayerStatus command)
    {
        Console.WriteLine($"System Message : {_state.PlayerName} has {_state.Health} hp.");
    }

    private void HitPlayerCommandHandler(HitPlayer command)
    {
        var @event = new PlayerHit(command.Damage);
        
        Persist(@event, playerHitEvent =>
        {
            Console.WriteLine($"System Message : Hit Event Persisted in {_state.PlayerName}.");
            _state.Health -= playerHitEvent.Damage;

            _eventCount++;

            if (_eventCount != 5) return;
            Console.WriteLine($"System Message : Saving Snapshot for player {_state.PlayerName}");
            SaveSnapshot(_state);
            Console.WriteLine($"System Message : Snapshot saved for player {_state.PlayerName}");

            Console.WriteLine($"System Message : Resetting event count to zero.");
            _eventCount = 0;
        });
    }
}