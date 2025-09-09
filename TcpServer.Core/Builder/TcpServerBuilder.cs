using Chat.TcpServer.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Collections;
using TcpServer.Core.Collections.Interfaces;
using TcpServer.Core.Interfaces;
using TcpServer.Core.Pipeline.Builder;
using TcpServer.Core.Pipeline.Interfaces;

namespace TcpServer.Core.Builder
{
    public class TcpServerBuilder
    {
        private TcpServerCore tcpServerCore = new TcpServerCore();

        private TcpServerBuilder() { }

        public static TcpServerBuilder Create()
        {
            return new TcpServerBuilder();
        }

        public TcpServerBuilder Port(int port)
        {
            this.tcpServerCore.Port = port;
            return this;
        }

        public TcpServerBuilder IpAddress(IPAddress address)
        {
            this.tcpServerCore.Ip = address;
            return this;
        }

        public TcpServerBuilder Pipeline(Action<ITcpRequestPipelineBuilder> build)
        {
            if (build == null)
            {
                throw new ArgumentNullException(nameof(build), "Build action cannot be null.");
            }

            this.tcpServerCore.TcpRequestPipelineFactory = (server) =>
            {
                var pipelineBuilder = TcpRequestPipelineBuilder.Create();
                pipelineBuilder.SetConnectionsList(server.Connections).
                    SetServiceScope(server.ServiceProvider.CreateScope());

                build(pipelineBuilder);
                return pipelineBuilder.Build();
            };

            return this;
        }

        public TcpServerBuilder OnClientConnectedMessageSend(Func<IServiceProvider, byte[]> messageFactory)
        {
            if (messageFactory == null)
            {
                throw new ArgumentNullException(nameof(messageFactory), "Message factory cannot be null.");
            }

            this.tcpServerCore.OnClientConnectedMessageSend = messageFactory;
            return this;
        }

        public TcpServerBuilder OnServerStart(Action<IServiceProvider> onServerStart)
        {
            if (onServerStart == null)
            {
                throw new ArgumentNullException(nameof(onServerStart), "On server start action cannot be null.");
            }

            this.tcpServerCore.OnServerStart = onServerStart;
            return this;
        }

        public IServiceCollection Services => this.tcpServerCore.Services;

        public void Reset()
        {
            this.tcpServerCore = new TcpServerCore();
            this.tcpServerCore.TcpRequestPipelineFactory = null!;
        }

        public ITcpServer Build()
        {
            if (this.tcpServerCore.TcpRequestPipelineFactory == null)
            {
                throw new InvalidOperationException("Pipeline is not configured. Please configure the pipeline before building the server.");
            }

            if (this.tcpServerCore.Port == 0)
            {
                throw new InvalidOperationException("Port is not configured. Please configure the port before building the server.");
            }

            if (this.tcpServerCore.Ip == null)
            {
                throw new InvalidOperationException("IP address is not configured. Please configure the IP address before building the server.");
            }

            if (this.tcpServerCore.OnClientConnectedMessageSend == null)
            {
                throw new InvalidOperationException("OnClientConnectedMessageSend is not configured. Please configure the OnClientConnectedMessageSend before building the server.");
            }

            if (this.tcpServerCore.OnServerStart == null)
            {
                throw new InvalidOperationException("OnServerStart is not configured. Please configure the OnServerStart before building the server.");
            }

            var result = this.tcpServerCore;
            this.Reset();
            return result;
        }
    }
}
