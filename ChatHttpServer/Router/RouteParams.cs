using System.Diagnostics.CodeAnalysis;

namespace Chat.ChatServer.Router;

public struct RouteParams
{
    public string Route { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not RouteParams routeParams) return false;
        
        return routeParams.Route.Equals(routeParams.Route);
    }
    
    public override int GetHashCode()
    {
        return this.Route.GetHashCode();
    }
}