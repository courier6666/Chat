using Chat.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.MessageComponents.Services.Interfaces
{
    public interface IBroadcastService
    {
        void AddMessageToQueue<TMessage>(TcpClient client, TMessage message, bool alsoSendToSender = true)
            where TMessage : Message;

        void Start();
    }
}
