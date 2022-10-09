namespace HttpServer;

internal static class Program
{
    private static bool _isRunning;
    
    public static void Main(string[] args)
    {
        using var server = new HttpServer();
        server.Start();
        _isRunning = true;
        while (_isRunning)
            Handle(Console.ReadLine()?.ToLower() ?? string.Empty, server);
        Console.WriteLine("Exiting gracefully...");
    }

    private static void Handle(string command, HttpServer server)
    {
        switch (command)
        {
            case "status":
                Console.WriteLine(server.Status);
                break;
            
            case "start":
                server.Start();
                break;
            
            case "stop":
                server.Stop();
                break;
            
            case "restart":
                server.Stop();
                server.Start();
                break;
            
            case "exit":
                _isRunning = false;
                break;
            
            default:
                Console.WriteLine("Can't recognize. Try again.");
                break;
        }
    }
}