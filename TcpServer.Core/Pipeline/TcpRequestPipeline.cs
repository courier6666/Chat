using System.Net.Sockets;
using Chat.TcpServer.Core.Pipeline.Delegates;

namespace Chat.TcpServer.Core.Pipeline;

public class TcpRequestPipeline
{
    private List<TcpPipeComponent> pipelineComponents;
    private Dictionary<Type, object> objectsPipelineBag = [];
    private TcpComponentDelegate? head = null!;

    public TcpRequestPipeline(IEnumerable<TcpPipeComponent> components)
    {
        pipelineComponents = components.ToList();
    }

    public bool IsConstructed => head != null;
    
    public void ConstructPipeline()
    {
        if (IsConstructed)
            return;
        
        var queue = new Queue<TcpPipeComponent>(this.pipelineComponents);
        this.head = BuildNextComponent(queue);
    }
    
    private TcpComponentDelegate? BuildNextComponent(Queue<TcpPipeComponent> currentComponents)
    {
        if(currentComponents.Count == 0)
            return null;
        
        var currentComponent = currentComponents.Dequeue();
        var nextComponent = BuildNextComponent(currentComponents);
        return async (client) =>
        {
            
        };
    }
    
    public async Task ExecuteAsync(TcpClient client)
    {
        if(!IsConstructed)
            this.ConstructPipeline();

        await this.head!(client);
    }
}