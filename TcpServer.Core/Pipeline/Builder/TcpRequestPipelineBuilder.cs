using Chat.TcpServer.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Pipeline.Interfaces;

namespace TcpServer.Core.Pipeline.Builder
{
    internal class TcpRequestPipelineBuilder : ITcpRequestPipelineBuilder
    {
        private List<TcpPipeComponent> components = new List<TcpPipeComponent>();
        private ConnectionList connections;
        private TcpRequestPipelineBuilder()
        {
            
        }

        public static TcpRequestPipelineBuilder Create()
        {
            return new TcpRequestPipelineBuilder();
        }

        public TcpRequestPipelineBuilder AddComponent<T>(T component)
            where T : class
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component), "Component cannot be null.");
            }

            var pipeComponent = component.GetComponentFromClass();
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

        public void Reset()
        {
            this.components.Clear();
            this.connections = null!;
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

            var pipeline = TcpRequestPipeline.Create(this.components, this.connections);
            pipeline.ConstructPipeline();
            return pipeline;
        }
    }
}
