using Chat.TcpServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

            this.tcpServerCore.TcpRequestPipelineFactory = () =>
            {
                var pipelineBuilder = TcpRequestPipelineBuilder.Create();
                pipelineBuilder.SetConnectionsList(this.tcpServerCore.Connections);
                build(pipelineBuilder);
                return pipelineBuilder.Build();
            };

            return this;
        }

        public void Reset()
        {
            this.tcpServerCore = new TcpServerCore();
        }

        public ITcpServer Build()
        {
            var result = this.tcpServerCore;
            this.Reset();
            return result;
        }
    }
}
