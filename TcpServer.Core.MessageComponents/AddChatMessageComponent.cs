using Chat.DatabaseAccess;
using Chat.Message;
using Chat.TcpServer.Core.Pipeline.Delegates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Interfaces;

namespace TcpServer.Core.MessageComponents
{
    public class AddChatMessageComponent
    {

        public async Task HandleAsync(TcpClient tcpClient, TcpComponentDelegate next, IMessageService messageService, BasicMessage<string> message)
        {
            if (message == null)
            {
                throw new InvalidOperationException("No message to add");
            }
            try
            {
                await messageService.AddMessageAsync(message);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to add message", ex);
            }

            await next(tcpClient);
        }
    }
}
