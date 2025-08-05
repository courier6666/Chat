using System.Net;
using Chat.ChatServer.Router;

namespace Chat.ChatServer.Extensions;

public static class HttpContextExtensions
{
    public static RouteParams GetRouteParamsFromHttpContext(this HttpListenerContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.Request.Url);
        
        return new RouteParams()
        {
            Route = context.Request.Url.AbsolutePath,
            Method = context.Request.HttpMethod,
        };
    }
    
}