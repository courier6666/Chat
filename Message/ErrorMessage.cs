namespace Chat.Message;

public class ErrorMessage : Message
{
    public override MessageType MessageType { get; set; } = MessageType.Error;
    
    public string Error { get; set; }
}