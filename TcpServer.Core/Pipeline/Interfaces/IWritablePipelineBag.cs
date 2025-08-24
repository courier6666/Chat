using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Pipeline.Interfaces
{
    public interface IWritablePipelineBag : IReadablePipelineBag
    {
        void Set(object value);
    }
}
