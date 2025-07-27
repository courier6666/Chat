using System.Net;
using System.Net.Sockets;
using System.Text;
using Chat.Message;
using System.Text.Json;
using Chat.DatabaseAccess;

namespace Chat.TcpServer.Core;

/// <summary>
/// Basic tcp server using tcp client and listener. Used for a chat.
/// </summary>
public class TcpServerCore
{
    /// <summary>
    /// Current tcp connections.
    /// </summary>
    private List<TcpClient> connections = new();
    private IPAddress ip;
    private JsonSerializerOptions jsonSerializerOptions;
    /// <summary>
    /// Database message service.
    /// </summary>
    private IMessageService messageService;
    private int port;
    
    /// <summary>
    /// Sync locker used to allow only one thread to notify users, see <see cref="NotifyAllAsync"/>.
    /// </summary>
    private object locker = new object();
    /// <summary>
    /// Sync locker used for counting current running <see cref="HandleTcpClient"/> methods.
    /// </summary>
    private object proccessLocker = new();
    private int clientProccessCount = 0;

    public TcpServerCore(IPAddress ip, int port, IMessageService messageService,
        JsonSerializerOptions jsonSerializerOptions = null!)
    {
        ArgumentNullException.ThrowIfNull(ip);
        ArgumentNullException.ThrowIfNull(messageService);

        this.ip = ip;
        this.port = port;
        this.jsonSerializerOptions = jsonSerializerOptions;
        this.messageService = messageService;
    }

    public async Task RunAsync()
    {
        using TcpListener server = new TcpListener(this.ip, port);
        server.Start();
        ClientCountDisplay();
        while (true)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
            lock (locker)
            {
                this.connections.Add(client);
            }

            Task.Run(async () => { await HandleTcpClient(client); });
        }
    }

    public async Task ClientCountDisplay()
    {
        while (true)
        {
            Console.WriteLine("Clients - " + this.clientProccessCount);
            await Task.Delay(1000);
        }
    }

    public async Task HandleTcpClient(TcpClient client)
    {
        using var stream = client.GetStream();
        lock (proccessLocker)
        {
            ++this.clientProccessCount;
        }

        try
        {
            var idMessage = Message<Guid>.CreateIdentificationMessage(Guid.NewGuid());
            await stream.WriteAsync(idMessage.ToBytes(this.jsonSerializerOptions));

            while (true)
            {
                byte[] data = new byte[8192];

                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                if (bytesRead == 0)
                {
                    throw new InvalidOperationException("Connection lost");
                }

                Console.WriteLine($"Data has been received at {DateTime.Now}. Data length: {bytesRead} bytes");

                try
                {
                    var message = data[..bytesRead].MessageFromJsonBytes<string>(this.jsonSerializerOptions);
                    message.TimeUtc = DateTime.UtcNow;
                    var id = await this.messageService.AddMessageAsync(message);
                    message.Id = id;
                    NotifyAllAsync(message);
                }
                catch (Exception e)
                {
                    await this.SendMessageAsync(client,
                        Message<string>.CreateErrorMessage(
                            "Failed to read and save message! Make sure it's in correct format"));
                }

                await Task.Delay(50);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Connection closed or lost.");
        }
        finally
        {
            lock (locker)
            {
                client.Dispose();
                this.connections.Remove(client);
            }
        }

        lock (proccessLocker)
        {
            --this.clientProccessCount;
        }
    }

    /// <summary>
    /// Sends a message to the specified tcp client.
    /// </summary>
    /// <remarks>
    /// See exceptions thrown by methods: <see cref="TcpClient.GetStream"/>, <see cref="NetworkStream.WriteAsync(ReadOnlyMemory{byte},CancellationToken)"/>
    /// </remarks>
    /// <param name="client">Tcp client connection.</param>
    /// <param name="message">Message to send.</param>
    /// <typeparam name="TMessageData">Message data type.</typeparam>
    public async Task SendMessageAsync<TMessageData>(TcpClient client, Message<TMessageData> message)
    {
        var stream = client.GetStream();
        await stream.WriteAsync(message.ToBytes(this.jsonSerializerOptions));
    }

    /// <summary>
    /// Sends a message to all connected tcp clients. If a message cannot be sent through a connection, a connection is closed and removed.
    /// </summary>
    /// <param name="message">Message with string data to send <see cref="Message{T}"/>.</param>
    public async Task NotifyAllAsync(Message<string> message)
    {
        List<Task> tasks = [];
        List<TcpClient> toRemove = new List<TcpClient>();
        lock (locker)
        {
            Console.WriteLine($"Notifying start---");
            Console.WriteLine("Connections count - " + connections.Count + ".");
            foreach (var connection in this.connections)
            {
                Console.WriteLine(connection.Connected);
                Console.WriteLine(connection.Client.RemoteEndPoint);
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        await this.SendMessageAsync(connection, message);
                    }
                    catch (Exception ex)
                    {
                        toRemove.Add(connection);
                    }
                }));
            }

            Console.WriteLine($"Notifying end---");
        }

        await Task.WhenAll(tasks);
        lock (locker)
        {
            foreach (var connection in toRemove)
            {
                connection.Dispose();
                connections.Remove(connection);
            }
        }
    }
}