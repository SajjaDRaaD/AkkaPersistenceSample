using Akka.Actor;
using Akka.Dispatch.SysMsg;
using AkkaPersistenceSample;
using AkkaPersistenceSample.Actors;
using AkkaPersistenceSample.Commands;

var sys = ActorSystem.Create("Game");
var playerCoordinator = sys.ActorOf(Props.Create<PlayerCoordinatorActor>(), "PlayerCoordinator");

Console.WriteLine("Commands Helper : ");
Console.WriteLine("<Player Name> create : Create a new player");
Console.WriteLine("<Player Name> display : Display player health");
Console.WriteLine("<Player Name> hit <Damage> : Damage player by given Damage value");
Console.WriteLine("<Player Name> error : Simulate a error for player actor");

while (true)
{
    var command = Console.ReadLine();
    var playerName = command?.Split(" ")[0] ?? throw new ArgumentNullException();

    if (command.Contains("create"))
    {
        CreatePlayer(playerName);
    }

    if (command.Contains("hit"))
    {
        var damage = command.Split(" ")[2];
        playerCoordinator.Tell(new HitPlayer()
        {
            PlayerName = playerName,
            Damage = Convert.ToInt32(damage)
        });
    }

    if (command.Contains("display"))
    {
        playerCoordinator.Tell(new DisplayPlayerStatus()
        {
            PlayerName = playerName
        });
    }

    if (command.Contains("error"))
    {
        playerCoordinator.Tell(new SimulateError()
        {
            PlayerName = playerName
        });
    }
}

void CreatePlayer(string playerName)
{
    Console.WriteLine($"System Message : {playerName} created successfully");
    playerCoordinator.Tell(new CreatePlayer
    {
        PlayerName = playerName
    });
}