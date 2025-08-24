using Chat.TcpServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Builder
{
    public class TcpServerBuilder
    {
        private TcpServerCore? tcpServerCore;

        private TcpServerBuilder() { }

        public static TcpServerBuilder Create()
        {
            return new TcpServerBuilder();
        }

        public TcpServerBuilder Port(int port)
        {
            // Set the port for the TCP server
            // Implementation here...
            return this;
        }

        public TcpServerBuilder IpAddress(IPAddress address)
        {
            
        }

        public TcpServerBuilder
    }
}
