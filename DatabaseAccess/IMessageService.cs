using Chat.Message;

namespace Chat.DatabaseAccess;

public interface IMessageService
{
    public Task<int> AddMessageAsync(Message<string> message);

    public Task<ICollection<Message<string>>> GetAllMessagesAsync();
}