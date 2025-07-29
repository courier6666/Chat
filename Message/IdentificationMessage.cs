namespace Chat.Message;

public class IdentificationMessage : Message
{
    public override MessageType MessageType { get; set; }  = MessageType.Identification;
    public Guid ProvidedId { get; set; }
}