using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Extensions;

namespace Chat.ChatServer.Router;

public class Router
{
    private readonly Dictionary<RouteParams, IHttpServerEndpoint> routes = [];

    public void AddRoute(IHttpServerEndpoint endpoint)
    {
        var routeParams = endpoint.GetRouteParamsFromHttpServerEndpoint();
        if(routes.ContainsKey(routeParams))
            throw new InvalidOperationException($"Route {routeParams} already exists!");
        
        routes.Add(routeParams, endpoint);
    }

    public IHttpServerEndpoint? GetRoute(RouteParams routeParams)
    {
        return routes.ContainsKey(routeParams) ? routes[routeParams] : null;
    }
}