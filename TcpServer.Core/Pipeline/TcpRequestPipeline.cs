using System.Net.Sockets;
using System.Reflection;
using Chat.TcpServer.Core.Pipeline.Delegates;
using Microsoft.Extensions.DependencyInjection;
using TcpServer.Core;
using TcpServer.Core.Collections;
using TcpServer.Core.Interfaces;
using TcpServer.Core.Pipeline;

namespace Chat.TcpServer.Core.Pipeline;

internal class TcpRequestPipeline : IDisposable
{
    private bool isDisposed = false;

    private readonly List<TcpPipeComponent> pipelineComponents;
    private readonly ConnectionList connections;
    private readonly IServiceScope serviceScope;
    private readonly IPipelineBag pipelineBag = new PipelineTypeObjectContainer();
    private TcpComponentDelegate? head = null!;

    private TcpRequestPipeline(IEnumerable<TcpPipeComponent> components, ConnectionList connections, IServiceScope serviceScope)
    {
        this.pipelineComponents = components.ToList();
        this.connections = connections;
        this.serviceScope = serviceScope;
    }

    public static TcpRequestPipeline Create(IEnumerable<TcpPipeComponent> components, ConnectionList connections, IServiceScope serviceScope)
    {
        return new TcpRequestPipeline(components, connections, serviceScope);
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

        var componentInstance = CreateComponentInstance(currentComponent, this.connections, this.serviceScope);

        return async (client) =>
        {
            var methodParams = currentComponent.Method.
                GetParameters().
                Select(p => ResolveParameterValue(client, nextComponent, this.pipelineBag, this.connections, this.serviceScope, p));

            var methodResult = currentComponent.Method.Invoke(componentInstance, methodParams.ToArray());

            if (methodResult is Task task)
            {
                await task.ConfigureAwait(false);
            }
        };
    }

    private static object CreateComponentInstance(
        TcpPipeComponent component,
        ConnectionList connections,
        IServiceScope serviceScope)
    {
        var constructor = component.ComponentType.GetConstructors()[0];
        var constructorParam = constructor.GetParameters().Select(p => ResolveConstructorParameterValue(connections, serviceScope, p));

        return constructor.Invoke(constructorParam.ToArray());
    }

    private static object ResolveParameterValue(
        TcpClient client,
        TcpComponentDelegate? nextComponent,
        IPipelineBag pipelineBag,
        ConnectionList connections,
        IServiceScope serviceScope,
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

        var foundServiceOrConfig = serviceScope.ServiceProvider.GetService(parameterType);
        if (foundServiceOrConfig != null)
        {
            return foundServiceOrConfig;
        }

        var foundObject = pipelineBag.Get(parameterType);

        if (foundObject != null)
        {
            return foundObject;
        }


        throw new InvalidOperationException($"Cannot resolve parameter of type {parameterType.FullName} in method {parameter.Member.Name}.");
    }

    private static object ResolveConstructorParameterValue(
        ConnectionList connections,
        IServiceScope serviceScope,
        ParameterInfo parameter)
    {
        var connectionListType = typeof(ConnectionList);

        var parameterType = parameter.ParameterType;

        if (parameterType == connectionListType)
        {
            return connections;
        }

        var foundServiceOrConfig = serviceScope.ServiceProvider.GetService(parameterType);
        if (foundServiceOrConfig != null)
        {
            return foundServiceOrConfig;
        }

        throw new InvalidOperationException($"Cannot resolve parameter of type {parameterType.FullName} in method {parameter.Member.Name}.");
    }

    public async Task ExecuteAsync(TcpClient client)
    {
        if (this.isDisposed)
            throw new ObjectDisposedException(nameof(TcpRequestPipeline), "Cannot execute a disposed pipeline.");

        if (!IsConstructed)
            this.ConstructPipeline();

        await this.head!(client);

        this.pipelineBag.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            this.isDisposed = true;

            if (disposing)
            {
                // Dispose managed state.
                this.serviceScope.Dispose();
            }
        }
    }

    ~TcpRequestPipeline()
    {
        this.Dispose(false);
    }
}