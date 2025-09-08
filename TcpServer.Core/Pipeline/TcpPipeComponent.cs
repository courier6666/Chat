using System.Reflection;

namespace Chat.TcpServer.Core.Pipeline;

internal class TcpPipeComponent
{
    public Type ComponentType { get; set; } =  null!;
    
    public MethodInfo Method { get; set; } = null!;
}