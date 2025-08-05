using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Endpoint.Attributes;
using Chat.ChatServer.Router;

namespace Chat.ChatServer.Extensions;

public static class HttpServerEndpointExtensions
{
    public static RouteParams GetRouteParamsFromHttpServerEndpoint(this IHttpServerEndpoint endpoint)
    {
        var type = endpoint.GetType();
        var customAttributes = Attribute.GetCustomAttributes(type, inherit: true);
        var routeAttribute = customAttributes.FirstOrDefault(a => a is EndpointRouteAttribute) as EndpointRouteAttribute;
        var httpMethodAttributes = customAttributes.FirstOrDefault(a => a is EndpointHttpMethodAttribute) as EndpointHttpMethodAttribute;
        
        if (routeAttribute == null)
            throw new InvalidOperationException("Endpoint route not defined!");

        return new RouteParams()
        {
            Route = routeAttribute.Route,
            Method = httpMethodAttributes?.Method.ToString() ?? HttpMethod.Get.ToString(),
        };
    }
}