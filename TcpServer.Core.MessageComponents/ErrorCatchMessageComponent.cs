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
    public class ErrorCatchMessageComponent
    {
        public async Task HandleAsync(TcpClient client, TcpComponentDelegate next, JsonSerializerOptions jsonOptions)
        {
            if (next is null)
            {
                throw new ArgumentNullException(nameof(next), "Next component delegate cannot be null.");
            }

            try
            {
                await next(client);
            }
            catch (Exception ex)
            {
                await client.SendMessageAsync(MessageFactory.CreateErrorMessage(ex.Message), jsonOptions);
            }
        }
    }
}
