using Chat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TcpServer.Core.MessageComponents
{
    public static class MessageTcpClientExtensions
    {
        /// <summary>
        /// Sends a message to the specified tcp client.
        /// </summary>
        /// <remarks>
        /// See exceptions thrown by methods: <see cref="TcpClient.GetStream"/>, <see cref="NetworkStream.WriteAsync(ReadOnlyMemory{byte},CancellationToken)"/>
        /// </remarks>
        /// <param name="client">Tcp client connection.</param>
        /// <param name="message">Message to send.</param>
        /// <typeparam name="TMessageData">Message data type.</typeparam>
        public static async Task SendMessageAsync<TMessage>(this TcpClient client, TMessage message, JsonSerializerOptions jsonOptions)
            where TMessage : Message
        {
            var stream = client.GetStream();
            await stream.WriteAsync(message.ToBytes(jsonOptions));
        }
    }
}
