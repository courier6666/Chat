using System.Text;
using System.Text.Json;

namespace Chat.Message;

public abstract class Message
{
    public abstract MessageType MessageType { get; set; }
    
    public DateTime TimeUtc { get; set; }
}