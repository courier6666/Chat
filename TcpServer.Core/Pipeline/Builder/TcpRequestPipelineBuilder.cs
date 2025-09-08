using Chat.TcpServer.Core.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Collections;
using TcpServer.Core.Pipeline.Interfaces;

namespace TcpServer.Core.Pipeline.Builder
{
    internal class TcpRequestPipelineBuilder : ITcpRequestPipelineBuilder
    {
        private readonly List<TcpPipeComponent> components = new();

        private ConnectionList connections = null!;

        private IServiceScope serviceScope;

        private TcpRequestPipelineBuilder()
        {
            
        }

        public static TcpRequestPipelineBuilder Create()
        {
            return new TcpRequestPipelineBuilder();
        }

        public ITcpRequestPipelineBuilder AddComponent<TComponent>()
            where TComponent : class
        {
            var pipeComponent = typeof(TComponent).GetComponentFromClass();
            this.components.Add(pipeComponent);

            return this;
        }

        internal TcpRequestPipelineBuilder SetConnectionsList(ConnectionList connections)
        {
            if (connections == null)
            {
                throw new ArgumentNullException(nameof(connections), "Connections list cannot be null.");
            }
            this.connections = connections;
            return this;
        }

        internal TcpRequestPipelineBuilder SetServiceScope(IServiceScope serviceScope)
        {
            if (serviceScope == null)
            {
                throw new ArgumentNullException(nameof(serviceScope), "Service Scope cannot be null.");
            }

            this.serviceScope = serviceScope;

            return this;
        }

        public void Reset()
        {
            this.components.Clear();
            this.connections = null!;
            this.serviceScope = null!;
        }

        internal TcpRequestPipeline Build()
        {
            if (this.connections == null)
            {
                throw new InvalidOperationException("Connections list must be set before building the pipeline.");
            }

            if (this.components.Count == 0)
            {
                throw new InvalidOperationException("At least one component must be added to the pipeline.");
            }

            if (this.serviceScope == null)
            {
                throw new InvalidOperationException("Service scope must be set before building the pipeline.");
            }

            var pipeline = TcpRequestPipeline.Create(this.components.ToList(), this.connections, this.serviceScope);
            pipeline.ConstructPipeline();
            return pipeline;
        }
    }
}
