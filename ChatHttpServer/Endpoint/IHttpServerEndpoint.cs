using System.Net;
using Chat.ChatServer.Result;

namespace Chat.ChatServer.Endpoint;

public interface IHttpServerEndpoint
{
    Task<IResult> Execute(HttpListenerContext context);
}