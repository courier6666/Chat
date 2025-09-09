using Chat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.MessageComponents.Services.Interfaces;

namespace TcpServer.Core.MessageComponents
{
    public class NotifyAllChatBroadcastComponent
    {
        private readonly IBroadcastService broadcastService;

        public NotifyAllChatBroadcastComponent(IBroadcastService broadcastService)
        {
            this.broadcastService = broadcastService;
        }

        public void Handle(TcpClient client, BasicMessage<string> message)
        {
            this.broadcastService.AddMessageToQueue(client, message);
        }
    }
}
