using System.Net;

namespace Chat.ChatServer.Result;

public interface IResultFactory
{
    IResult Ok();
    
    IResult Ok<TObject>(TObject obj);
    
    IResult StatusCode(HttpStatusCode statusCode);
    
    IResult StatusCode(HttpStatusCode statusCode, string message);
    
    IResult StatusCode<TObject>(HttpStatusCode statusCode, TObject obj);
}