using System.Diagnostics.CodeAnalysis;

namespace Chat.ChatServer.Router;

public struct RouteParams
{
    public string Route { get; set; }
    
    public string Method { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not RouteParams routeParams) return false;
        
        return routeParams.Route.Equals(routeParams.Route) && Method.Equals(routeParams.Method);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(this.Route.GetHashCode(), this.Method.GetHashCode());
    }
}