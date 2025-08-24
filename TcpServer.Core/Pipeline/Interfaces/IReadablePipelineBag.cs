using Chat.TcpServer.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Pipeline.Interfaces
{
    public interface IReadablePipelineBag
    {
        public object? Get(Type type);

        public T? Get<T>() where T : class;
    }
}
