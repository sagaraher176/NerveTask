using System;
using System.Threading.Tasks;

class Program1
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, TCP Server Application!");
        Console.WriteLine("==============================");

        int port = GetServerPort();

        try
        {
            Server server = new Server(port);

            Console.WriteLine("\n[SERVER] Starting on port {port}");
            Console.WriteLine("[SERVER] Waiting for client connections...");

            Console.WriteLine("[INFO] Press Ctrl+C to stop the server");

            await server.StartAsync();
        }
        catch (Exception ex)
        {
            
            Console.WriteLine($"[ERROR] Server startup failed: {ex.Message}");
        }
    }

    static int GetServerPort()
    {
        while (true)
        {
            Console.Write("Enter server port number: ");
            
            string? portInput = Console.ReadLine();

            if (int.TryParse(portInput, out int port))
            {
                if (port >= 1024 && port <= 65535)
                {
                    return port;
                }
                else
                {
                    Console.WriteLine("Port must be between 1024 and 65535.");
                }
            }
            else
            {
                Console.WriteLine("Invalid port number. Please enter a valid integer.");
            }
        }
    }
}