using Chat.TcpServer.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Pipeline
{
    internal static class TcpComponentExtensions
    {
        public static bool IsMethodCorrectForPipeComponent(this MethodInfo methodInfo)
        {
            throw new NotImplementedException("This method should be implemented to check if the method is suitable for a pipe component.");
        }

        public static TcpPipeComponent GetComponentFromClass<T>(this T obj)
            where T : class
        {
            var type = typeof(T);
            var handleMethods = type.GetMethods().
                Where(m => m.IsMethodCorrectForPipeComponent());

            if(handleMethods == null || !handleMethods.Any())
            {
                throw new InvalidOperationException($"Class {type.Name} does not contain a public method named 'Handle' or 'HandleAsync'.");
            }

            if(handleMethods.Count() > 1)
            {
                throw new InvalidOperationException($"Class {type.Name} contains more than one public method named 'Handle' or 'HandleAsync'.");
            }

            return new TcpPipeComponent
            {
                Component = obj,
                Method = handleMethods.First()
            };
        }
    }
}
