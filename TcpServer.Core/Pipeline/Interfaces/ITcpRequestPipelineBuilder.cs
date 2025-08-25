using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpServer.Core.Pipeline.Builder;

namespace TcpServer.Core.Pipeline.Interfaces
{
    public interface ITcpRequestPipelineBuilder
    {
        public ITcpRequestPipelineBuilder AddComponent<T>(T component)
            where T : class;
    }
}
