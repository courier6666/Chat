namespace Chat.Message;

public class BasicMessage<T> : Message
{
    public override MessageType MessageType { get; set; } = MessageType.Basic;

    public T Data { get; set; } = default!;

    public int Id { get; set; }
    
    public Guid AuthorId  { get; set; }
}