using System.Net;
using System.Text.Json;
using Chat.ChatServer;
using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Endpoint.Attributes;
using Chat.ChatServer.Result;
using Chat.DatabaseAccess;

namespace ChatServer.Endpoints;

[EndpointRoute(Route = "/messages/all")]
public class GetAllMessagesEndpoint : HttpServerEndpoint
{
    private readonly IMessageService messageService;
    
    public GetAllMessagesEndpoint(IMessageService messageService)
    {
        this.messageService  = messageService;
    }

    public override async Task<IResult> Execute(HttpListenerContext context)
    {
        return Ok(await this.messageService.GetAllMessagesAsync());
    }
}