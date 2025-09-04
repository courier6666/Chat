using System.Net;
using System.Net.Sockets;
using System.Text;
using Chat.Message;
using System.Text.Json;
using Chat.DatabaseAccess;
using TcpServer.Core.Interfaces;
using TcpServer.Core;
using Chat.TcpServer.Core.Pipeline;
using TcpServer.Core.Collections;
using TcpServer.Core.Collections.Interfaces;

namespace Chat.TcpServer.Core;

/// <summary>
/// Basic tcp server using tcp client and listener. Used for a chat.
/// </summary>
internal class TcpServerCore : ITcpServer
{
    /// <summary>
    /// Current tcp connections.
    /// </summary>
    private readonly ConnectionList connections = new();

    internal ConnectionList Connections => this.connections;

    private readonly TypeObjectContainer globalServices = new();

    internal TypeObjectContainer GlobalServices => this.globalServices;

    public Func<TcpServerCore, TcpRequestPipeline> TcpRequestPipelineFactory { get; internal set; } = default!;

    public Func<IReadOnlyServices, byte[]> OnClientConnectedMessageSend { get; internal set; } = default!;

    public IPAddress Ip { get; internal set; } = default!;

    public int Port { get; internal set; }
    
    /// <summary>
    /// Sync locker used to allow only one thread to notify users, see <see cref="NotifyAllAsync"/>.
    /// </summary>
    private readonly object locker = new object();
    /// <summary>
    /// Sync locker used for counting current running <see cref="HandleTcpClient"/> methods.
    /// </summary>
    private readonly object proccessLocker = new();
    private int clientProccessCount = 0;

    public void AddGlobalServiceOrConfig<TComponent>(TComponent obj)
    where TComponent : class
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj), "Global service or config cannot be null.");
        }

        this.globalServices.SetGeneric(obj);
    }

    public async Task RunAsync()
    {
        using TcpListener server = new TcpListener(this.Ip, this.Port);
        server.Start();
        _ = Task.Run(() => ClientCountDisplay());
        while (true)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
            lock (locker)
            {
                this.Connections.Add(client);
            }

            _ = Task.Run(() => HandleTcpClient(client));
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

        var pipeline = this.TcpRequestPipelineFactory(this);

        try
        {
            await stream.WriteAsync(OnClientConnectedMessageSend(this.globalServices));

            while (true)
            {
                await pipeline.ExecuteAsync(client);
                await Task.Delay(1);
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
                client.Close();
                this.Connections.Remove(client);
            }
        }

        lock (proccessLocker)
        {
            --this.clientProccessCount;
        }
    }
}