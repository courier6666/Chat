using System.Text;
using System.Text.Json;

namespace Chat.Message;

public class Message<T>
{
    public long Id { get; set; }
    public MessageType MessageType { get; set; }
    
    public T Data { get; set; }

    public Guid? AuthorId { get; set; }
    
    public DateTime TimeUtc { get; set; }
    
    public static Message<T> CreateIdentificationMessage(T data)
    {
        return new Message<T>()
        {
            MessageType = MessageType.Identification,
            Data = data,
        };
    }

    public static Message<T> CreateBasicMessage(T data, Guid? authorId = null)
    {
        return new Message<T>()
        {
            MessageType = MessageType.Basic,
            Data = data,
            AuthorId = authorId,
        };
    }

    public static Message<string> CreateErrorMessage(string errorMessage)
    {
        return new Message<string>()
        {
            MessageType = MessageType.Error,
            Data = errorMessage,
        };
    }
}