using System.Net.Sockets;
using System.Reflection;
using Chat.TcpServer.Core.Pipeline.Delegates;
using TcpServer.Core;
using TcpServer.Core.Collections;
using TcpServer.Core.Interfaces;
using TcpServer.Core.Pipeline;

namespace Chat.TcpServer.Core.Pipeline;

internal class TcpRequestPipeline
{
    private readonly List<TcpPipeComponent> pipelineComponents;
    private readonly ConnectionList connections;
    private readonly TypeObjectContainer globalServices;
    private readonly IPipelineBag pipelineBag = new PipelineTypeObjectContainer();
    private TcpComponentDelegate? head = null!;

    private TcpRequestPipeline(IEnumerable<TcpPipeComponent> components, ConnectionList connections, TypeObjectContainer globalServices)
    {
        this.pipelineComponents = components.ToList();
        this.connections = connections;
        this.globalServices = globalServices;
    }

    public static TcpRequestPipeline Create(IEnumerable<TcpPipeComponent> components, ConnectionList connections, TypeObjectContainer globalServices)
    {
        return new TcpRequestPipeline(components, connections, globalServices);
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
                Select(p => ResolveParameterValue(client, nextComponent, this.pipelineBag, this.connections, this.globalServices, p));

            var methodResult = currentComponent.Method.Invoke(currentComponent.Component, methodParams.ToArray());
            
            if(methodResult is Task task)
            {
                task.GetAwaiter().GetResult();
            }
        };
    }

    private static object ResolveParameterValue(
        TcpClient client,
        TcpComponentDelegate? nextComponent,
        IPipelineBag pipelineBag,
        ConnectionList connections,
        TypeObjectContainer globalServices,
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

        var foundServiceOrConfig = globalServices.Get(parameterType);
        if (foundServiceOrConfig != null)
        {
            return foundServiceOrConfig;
        }

        var foundObject = pipelineBag.Get(parameterType);

        if (foundObject != null)
        {
            return foundObject;
        }

        throw new InvalidOperationException($"Cannot resolve parameter of type {parameterType.FullName} in pipeline component method {parameter.Member.Name}.");
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