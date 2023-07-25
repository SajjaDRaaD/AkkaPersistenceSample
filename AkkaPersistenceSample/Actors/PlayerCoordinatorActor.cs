using Akka.Actor;
using Akka.Persistence;
using AkkaPersistenceSample.Commands;
using AkkaPersistenceSample.Events;

namespace AkkaPersistenceSample.Actors;

public class PlayerCoordinatorActor : ReceivePersistentActor
{
    private int DefaultHealth => 100;
    public override string PersistenceId => "player-coordinator";

    public PlayerCoordinatorActor()
    {
        Command<CreatePlayer>(CreatePlayerCommandHandler);
        Command<HitPlayer>(HitPlayerCommandHandler);
        Command<DisplayPlayerStatus>(DisplayPlayerStatusCommandHandler);
        Command<SimulateError>(CauseErrorCommandHandler);
        Recover<PlayerCreate>(playerCreatedEvent =>
        {
            Console.WriteLine($"Recovering {playerCreatedEvent.PlayerName} ...");
            Context.ActorOf(Props.Create(()=> new PlayerActor(playerCreatedEvent.PlayerName,DefaultHealth)), playerCreatedEvent.PlayerName);
        });
    }
    
    private void HitPlayerCommandHandler(HitPlayer command)
    {
        Context.Child(command.PlayerName).Forward(command);
    }

    private void DisplayPlayerStatusCommandHandler(DisplayPlayerStatus command)
    {
        Context.Child(command.PlayerName).Forward(command);
        
    }
    private void CauseErrorCommandHandler(SimulateError command)
    {
        Context.Child(command.PlayerName).Forward(command);
    }

    private void CreatePlayerCommandHandler(CreatePlayer command)
    {
        var @event = new PlayerCreate(command.PlayerName);
        
        Persist(@event, playerCreatedEvent =>
        {
            Console.WriteLine($"{playerCreatedEvent.PlayerName} event Persisted successfully.");
            Context.ActorOf(Props.Create(()=> new PlayerActor(playerCreatedEvent.PlayerName,DefaultHealth)), playerCreatedEvent.PlayerName);
        });
        
    }

}