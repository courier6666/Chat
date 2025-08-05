using System.Net;
using System.Text.Json;
using Chat.ChatServer.Result;

namespace Chat.ChatServer.Endpoint;

public abstract class HttpServerEndpoint : IHttpServerEndpoint, IResultFactory
{
    internal ChatHttpServer ChatHttpServer { get; set; } = null!;
    public abstract Task<IResult> Execute(HttpListenerContext context);
    
    public IResult Ok()
    {
        return new Result.Result()
        {
            StatusCode = HttpStatusCode.OK,
            ContentType = "application/json",
            Data = System.Text.Encoding.UTF8.GetBytes(string.Empty),
        };
    }

    public IResult Ok<TObject>(TObject obj)
    {
        return new Result.Result()
        {
            StatusCode = HttpStatusCode.OK,
            ContentType = "application/json",
            Data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj, this.ChatHttpServer.JsonSerializerOptions)),
        };
    }

    public IResult StatusCode(HttpStatusCode statusCode)
    {
        return new Result.Result()
        {
            StatusCode = statusCode,
            ContentType = "application/json",
            Data = System.Text.Encoding.UTF8.GetBytes(string.Empty),
        };
    }

    public IResult StatusCode(HttpStatusCode statusCode, string message)
    {
        return new Result.Result()
        {
            StatusCode = statusCode,
            ContentType = "application/json",
            Data = System.Text.Encoding.UTF8.GetBytes(message),
        };
    }

    public IResult StatusCode<TObject>(HttpStatusCode statusCode, TObject obj)
    {
        return new Result.Result()
        {
            StatusCode = statusCode,
            ContentType = "application/json",
            Data = System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj, this.ChatHttpServer.JsonSerializerOptions)),
        };
    }
}