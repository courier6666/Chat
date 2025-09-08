using Chat.TcpServer.Core.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer.Core.Pipeline
{
    internal static class TcpComponentExtensions
    {
        public static bool IsMethodCorrectForPipeComponent(this MethodInfo methodInfo)
        {
            if (methodInfo.Name != "Handle" && methodInfo.Name != "HandleAsync")
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.Name == "HandleAsync" && methodInfo.ReturnType != typeof(Task))
            {
                return false;
            }

            if (methodInfo.Name == "Handle" && methodInfo.ReturnType != typeof(void))
            {
                return false;
            }

            var parameters = methodInfo.GetParameters();

            if (!parameters.Any() || parameters[0].ParameterType != typeof(TcpClient))
            {
                return false;
            }

            return true;
        }

        public static TcpPipeComponent GetComponentFromClass(this Type type)
        {
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

            var constructors = type.GetConstructors();

            if (constructors.Count() > 1)
            {
                throw new InvalidOperationException($"Class {type.Name} contains more than one public constructor.");
            }

            return new TcpPipeComponent
            {
                ComponentType = type,
                Method = handleMethods.First()
            };
        }
    }
}
