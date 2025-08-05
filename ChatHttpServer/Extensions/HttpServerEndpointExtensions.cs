using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Endpoint.Attributes;
using Chat.ChatServer.Router;

namespace Chat.ChatServer.Extensions;

public static class HttpServerEndpointExtensions
{
    public static RouteParams GetRouteParamsFromHttpServerEndpoint(this IHttpServerEndpoint endpoint)
    {
        var type = endpoint.GetType();
        var attribute = Attribute.GetCustomAttributes(type, inherit: true).FirstOrDefault(a => a is EndpointRouteAttribute) as EndpointRouteAttribute;
        
        if (attribute == null)
            throw new InvalidOperationException("Endpoint route not defined!");

        return new RouteParams()
        {
            Route = attribute.Route,
        };
    }
}