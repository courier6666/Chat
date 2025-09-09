using Azure.Core;
using Chat.Message;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TcpServer.Core.MessageComponents.Services.Interfaces;

namespace TcpServer.Core.MessageComponents.Services
{
    public class BroadcastService : IBroadcastService
    {
        private readonly ConcurrentQueue<(TcpClient Client, byte[] Message, bool SendToSender)> messageQueue;
        private readonly ConnectionList connections;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly CancellationTokenSource cancellationTokenSource = new();

        public BroadcastService(JsonSerializerOptions options, ConnectionList connections)
        {
            messageQueue = new ConcurrentQueue<(TcpClient, byte[], bool)>();
            this.connections = connections;
            this.jsonSerializerOptions = options;
        }

        public void AddMessageToQueue<TMessage>(TcpClient client, TMessage message, bool alsoSendToSender = true) where TMessage : Message
        {
            this.messageQueue.Enqueue((client, message.ToBytes(jsonSerializerOptions), alsoSendToSender));
        }

        private async Task RunAsync(CancellationToken token)
        {

            while(true)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                var hasMessage = this.messageQueue.TryDequeue(out var item);
                if (hasMessage)
                {
                    await Parallel.ForEachAsync(this.connections, async (tcpClient, ct) =>
                    {
                        if (tcpClient == item.Client && !item.SendToSender)
                        {
                            return;
                        }
                        try
                        {
                            await tcpClient.SendDataAsync(item.Message);
                        }
                        catch (Exception)
                        {
                            // Ignore exceptions here, as the client handling will take care of disconnecting faulty clients.
                        }
                    });
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1);
            }
        }

        public void Start()
        {
            _ = Task.Run(() => RunAsync(this.cancellationTokenSource.Token));
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
        }
    }
}
