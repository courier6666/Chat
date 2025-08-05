using System.Net;

namespace Chat.ChatServer.Result;

public interface IResult
{
    public byte[] Data  { get; set; }
    
    public string ContentType  { get; set; }
    
    public HttpStatusCode StatusCode { get; set; }
}