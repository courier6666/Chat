namespace Chat.ChatServer.Endpoint.Attributes;

public class EndpointRouteAttribute : Attribute
{
    public string Route { get; set; }
}