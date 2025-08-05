namespace Chat.ChatServer.Endpoint.Attributes;

public class EndpointRouteAttribute : Attribute
{
    public required string Route { get; set; }

}