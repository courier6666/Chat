using Chat.Message;
using Chat.TcpServer.Core.Pipeline.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TcpServer.Core.MessageComponents
{
    public class NotifyAllChatComponent
    {
        public static readonly object locker = new();
        public async Task HandleAsync(TcpClient client,
            ConnectionList connections,
            JsonSerializerOptions jsonOptions,
            BasicMessage<string> message)
        {
            List<Task> tasks = [];
            List<TcpClient> toRemove = new List<TcpClient>();
            lock (locker)
            {
                Console.WriteLine($"Notifying start---");
                Console.WriteLine("Connections count - " + connections.Count + ".");
                foreach (var connection in connections)
                {
                    Console.WriteLine(connection.Connected);
                    Console.WriteLine(connection.Client.RemoteEndPoint);
                    tasks.Add(Task.Factory.StartNew(async () =>
                    {
                        try
                        {
                            await connection.SendMessageAsync(message, jsonOptions);
                        }
                        catch (Exception ex)
                        {
                            toRemove.Add(connection);
                        }
                    }));
                }

                Console.WriteLine($"Notifying end---");

                Task.WaitAll(tasks.ToArray());
                foreach (var connection in toRemove)
                {
                    connection.Dispose();
                    connections.Remove(connection);
                }
            }
        }
    }
}
