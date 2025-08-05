using System.Net;
using System.Text.Json;
using Chat.ChatServer;
using Chat.ChatServer.Endpoint;
using Chat.ChatServer.Endpoint.Attributes;
using Chat.ChatServer.Result;
using Chat.DatabaseAccess;

namespace ChatServer.Endpoints;

[EndpointRoute(Route = "/messages")]
public class GetAllMessagesEndpoint : HttpServerEndpoint
{
    private readonly IMessageService messageService;
    
    public GetAllMessagesEndpoint(IMessageService messageService)
    {
        this.messageService  = messageService;
    }

    public override async Task<IResult> Execute(HttpListenerContext context)
    {
        var query = context.Request.QueryString;

        DateTime? timeBefore = null;
        try
        {
            var timeParam = query.Get("timeBefore");
            
            if(timeParam != null)
                timeBefore = DateTime.Parse(timeParam!);
        }
        catch (FormatException e)
        {
            return StatusCode(HttpStatusCode.BadRequest, "Failed to parse query parameters!");
        }
        
        if (timeBefore == null)
            return Ok(await this.messageService.GetAllMessagesAsync());

        return Ok(await this.messageService.GetPagedMessagedFromBeforeAsync(timeBefore.Value, 10));
    }
}