using System.Net.Sockets;

namespace Chat.TcpServer.Core.Pipeline.Delegates;

public delegate void TcpComponentDelegate(TcpClient client);