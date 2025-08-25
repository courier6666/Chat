using System.Net.Sockets;
using System.Reflection;
using Chat.TcpServer.Core.Pipeline.Delegates;
using TcpServer.Core;
using TcpServer.Core.Pipeline;
using TcpServer.Core.Pipeline.Interfaces;

namespace Chat.TcpServer.Core.Pipeline;

internal class TcpRequestPipeline
{
    private List<TcpPipeComponent> pipelineComponents;
    private ConnectionList connections;
    private IPipelineBag pipelineBag = new PipelineBag();
    private TcpComponentDelegate? head = null!;

    private TcpRequestPipeline(IEnumerable<TcpPipeComponent> components, ConnectionList connections)
    {
        this.pipelineComponents = components.ToList();
        this.connections = connections;
    }

    public static TcpRequestPipeline Create(IEnumerable<TcpPipeComponent> components, ConnectionList connections)
    {
        return new TcpRequestPipeline(components, connections);
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
        if (currentComponents.Count == 0)
            return null;

        var currentComponent = currentComponents.Dequeue();
        var nextComponent = BuildNextComponent(currentComponents);
        return (client) =>
        {
            var methodParams = currentComponent.Method.
                GetParameters().
                Select(p => SelectParameterValue(client, nextComponent, this.pipelineBag, this.connections, p));

            var methodResult = currentComponent.Method.Invoke(currentComponent.Component, methodParams.ToArray());
            
            if(methodResult is Task task)
            {
                task.GetAwaiter().GetResult();
            }
        };
    }

    private static object SelectParameterValue(
        TcpClient client,
        TcpComponentDelegate? nextComponent,
        IPipelineBag pipelineBag,
        ConnectionList connections,
        ParameterInfo parameter)
    {
        var tcpClientType = typeof(TcpClient);
        var tcpComponentType = typeof(TcpComponentDelegate);
        var writablePipelineBag = typeof(IWritablePipelineBag);
        var readablePipelineBag = typeof(IReadablePipelineBag);
        var connectionListType = typeof(ConnectionList);

        var parameterType = parameter.ParameterType;

        if (parameterType == tcpClientType)
        {
            return client;
        }

        if (parameterType == tcpComponentType)
        {
            if (nextComponent == null)
                throw new InvalidOperationException("Next component is not defined in the pipeline.");

            return (TcpComponentDelegate)nextComponent!;
        }

        if (parameterType.IsGenericMethodParameter
            && parameterType == typeof(Nullable<>)
            && Nullable.GetUnderlyingType(parameterType) == tcpComponentType)
        {
            return nextComponent!;
        }

        if (parameterType == writablePipelineBag || parameterType == readablePipelineBag)
        {
            return pipelineBag;
        }

        if (parameterType == connectionListType)
        {
            return connections;
        }


        var foundObject = pipelineBag.Get(parameterType);

        if (foundObject != null)
        {
            return foundObject;
        }
        else
        {
            throw new InvalidOperationException(
                $"Parameter of type {parameterType.Name} is not found in the pipeline bag.");
        }
    }

    public Task ExecuteAsync(TcpClient client)
    {
        if (!IsConstructed)
            this.ConstructPipeline();

        this.head!(client);

        this.pipelineBag.Clear();

        return Task.CompletedTask;
    }
}