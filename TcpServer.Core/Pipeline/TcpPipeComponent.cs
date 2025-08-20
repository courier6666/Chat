using System.Reflection;

namespace Chat.TcpServer.Core.Pipeline;

public class TcpPipeComponent
{
    public object Component { get; set; } =  null!;
    
    public MethodInfo Method { get; set; } = null!;
}