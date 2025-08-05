namespace Chat.ChatServer.Endpoint.Attributes;

public class EndpointHttpMethodAttribute : Attribute
{
    public required HttpMethod Method { get; set; }
}