using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Interfaces
{
    internal interface IPipelineBag : IWritablePipelineBag
    {
        public void Clear();
    }
}
