using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core
{
    public static class TcpClientExtensions
    {
        public static async Task SendDataAsync(this TcpClient client, byte[] data)
        {
            var stream = client.GetStream();
            await stream.WriteAsync(data);
        }
    }
}
