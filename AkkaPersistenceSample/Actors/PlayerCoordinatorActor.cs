using Akka.Actor;
using Akka.Persistence;

namespace AkkaPersistenceSample.Actors;

public class PlayerCoordinatorActor : ReceivePersistentActor
{
    private int DefaultHealth => 100;
    public override string PersistenceId => "player-coordinator";

    public PlayerCoordinatorActor()
    {
        Command<CreatePlayerMessage>(CreatePlayerMessageHandler);
        Command<HitMessage>(HitMessageHandler);
        Command<DisplayPlayerStatusMessage>(DisplayPlayerStatusMessageHandler);
        Command<CauseErrorMessage>(CauseErrorMessageHandler);
        Recover<CreatePlayerMessage>(message =>
        {
            Console.WriteLine($"Recovering {message.PlayerName} ...");
            Context.ActorOf(Props.Create(()=> new PlayerActor(message.PlayerName,DefaultHealth)), message.PlayerName);
        });
    }
    
    private void HitMessageHandler(HitMessage message)
    {
        var child= Context.Child(message.PlayerName);
        Console.WriteLine($"{child.Path.Name} selected to receive hit");
        child.Tell(new HitMessage {Damage = message.Damage});
    }

    private void DisplayPlayerStatusMessageHandler(DisplayPlayerStatusMessage message)
    {
        Context.Child(message.PlayerName)
            .Tell(new DisplayPlayerStatusMessage());
        
    }
    private void CauseErrorMessageHandler(CauseErrorMessage message)
    {
        Context.Child(message.PlayerName)
            .Tell(new CauseErrorMessage());
    }

    private void CreatePlayerMessageHandler(CreatePlayerMessage message)
    {
        Persist(message, createMessage =>
        {
            Console.WriteLine($"{message.PlayerName} Persisted successfully.");
            Context.ActorOf(Props.Create(()=> new PlayerActor(message.PlayerName,DefaultHealth)), message.PlayerName);
        });
        var children = Context.GetChildren();

        foreach (var child in children)
        {
            Console.WriteLine(child.Path.Name);
        }
    }

}