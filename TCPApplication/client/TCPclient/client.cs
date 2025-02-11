using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client
{
    private string serverIp;
    private int serverPort;

    public Client(string serverIp, int serverPort)
    {
        this.serverIp = serverIp;
        this.serverPort = serverPort;
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            using (TcpClient client = new TcpClient())
            {
                await client.ConnectAsync(serverIp, serverPort);
                using (NetworkStream stream = client.GetStream())
                {
                    string encryptedMessage = Encrypt(message);
                    byte[] data = Encoding.UTF8.GetBytes(encryptedMessage);
                    await stream.WriteAsync(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0) break;

                        string encryptedResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        string response = Decrypt(encryptedResponse);
                        Console.WriteLine($"Received: {response}");

                        if (response == "EMPTY") break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private string Encrypt(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }

    private string Decrypt(string base64Text)
    {
        byte[] bytes = Convert.FromBase64String(base64Text);
        return Encoding.UTF8.GetString(bytes);
    }
}