using System.Net.Sockets;

namespace Chat.TcpServer.Core.Pipeline.Delegates;

public delegate Task TcpComponentDelegate(TcpClient client);