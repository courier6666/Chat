using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chat.Message;

public abstract class Message
{
    public abstract MessageType MessageType { get; set; }
    
    public DateTime TimeUtc { get; set; }
}