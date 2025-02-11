using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

public class Server
{
    private TcpListener listener;
    private Dictionary<string, List<Dictionary<string, int>>> serverCollection;
    private readonly object lockObject = new object();

    public Server(int port)
    {
        InitializeCollection();
        listener = new TcpListener(IPAddress.Any, port);
    }

    private void InitializeCollection()
    {
        string json = @"{""SetA"":[{""One"":1,""Two"":2}],""SetB"":[{""Three"":3,""Four"":4}],""SetC"":[{""Five"":5,""Six"":6}],""SetD"":[{""Seven"":7,""Eight"":8}],""SetE"":[{""Nine"":9,""Ten"":10}]}";
        serverCollection = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, int>>>>(json);
    }

    public async Task StartAsync()
    {
        try
        {
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Server error: {ex.Message}");
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string encryptedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                string message = Decrypt(encryptedMessage);
                Console.WriteLine($"Received: {message}");

                string[] parts = message.Split('-');
                if (parts.Length != 2)
                {
                    await SendEncryptedMessageAsync(stream, "EMPTY");
                    return;
                }

                string setKey = parts[0];
                string valueKey = parts[1];

                if (serverCollection.TryGetValue(setKey, out var subset))
                {
                    foreach (var dict in subset)
                    {
                        if (dict.TryGetValue(valueKey, out int repeatCount))
                        {
                            for (int i = 0; i < repeatCount; i++)
                            {
                                string timeMessage = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                                await SendEncryptedMessageAsync(stream, timeMessage);
                                await Task.Delay(1000);
                            }
                            return;
                        }
                    }
                }

                await SendEncryptedMessageAsync(stream, "EMPTY");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }

    private async Task SendEncryptedMessageAsync(NetworkStream stream, string message)
    {
        string encryptedMessage = Encrypt(message);
        byte[] response = Encoding.UTF8.GetBytes(encryptedMessage);
        await stream.WriteAsync(response, 0, response.Length);
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

    public void Stop()
    {
        listener.Stop();
    }
}