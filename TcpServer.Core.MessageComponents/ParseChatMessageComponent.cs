using System.Net.Sockets;
using System.Text.Json;
using System.Text;
using Chat.TcpServer.Core.Pipeline.Delegates;
using TcpServer.Core.Interfaces;
using Chat.Message;

namespace TcpServer.Core.MessageComponents
{
    public class ParseChatMessageComponent
    {
        public async Task HandleAsync(TcpClient tcpClient, TcpComponentDelegate next, byte[] data, IWritablePipelineBag pipelineBag, JsonSerializerOptions jsonOptions)
        {
            if (data == null || data.Length == 0)
            {
                throw new InvalidOperationException("No data to parse");
            }

            var message = data.MessageFromJsonBytes<BasicMessage<string>>(jsonOptions);
            if (message == null)
            {
                throw new InvalidOperationException("Failed to parse message");
            }

            message.TimeUtc = DateTime.UtcNow;
            pipelineBag.Set(message);
            await next(tcpClient);
        }
    }
}
