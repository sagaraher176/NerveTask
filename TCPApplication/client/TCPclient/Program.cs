class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Enter server IP: ");
        string serverIp = Console.ReadLine();

        Console.Write("Enter server port: ");
        int serverPort = int.Parse(Console.ReadLine());

        Client client = new Client(serverIp, serverPort);

        while (true)
        {
            Console.Write("Enter message or 'exit' to quit: ");
            string message = Console.ReadLine();
            
            if (message.ToLower() == "exit") break;
            
            await client.SendMessageAsync(message);
        }
    }
}