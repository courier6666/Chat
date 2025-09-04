using Chat.TcpServer.Core.Pipeline.Delegates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Interfaces;

namespace TcpServer.Core.Pipeline.Components
{
    public class BytesReadFromStreamComponent
    {
        public async Task HandleAsync(TcpClient tcpClient, TcpComponentDelegate next, IWritablePipelineBag pipelineBag)
        {
            var stream = tcpClient.GetStream();
            byte[] data = new byte[8192];

            int bytesRead = await stream.ReadAsync(data, 0, data.Length);
            if (bytesRead == 0)
            {
                throw new InvalidOperationException("Connection lost");
            }

            Console.WriteLine($"Data has been received at {DateTime.Now}. Data length: {bytesRead} bytes");
            pipelineBag.Set(data[..bytesRead].ToArray());

            await next(tcpClient);
        }
    }
}
