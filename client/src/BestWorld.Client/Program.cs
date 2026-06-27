namespace BestWorld.Client;

internal static class Program
{
    private static void Main()
    {
        using var game = new GameClient();
        game.Run();
    }
}
