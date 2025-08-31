using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Collections.Interfaces
{
    public interface IReadOnlyServices
    {
        public object? Get(Type type);

        public T? Get<T>() where T : class;
    }
}
