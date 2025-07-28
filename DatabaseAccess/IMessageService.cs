using Chat.Message;

namespace Chat.DatabaseAccess;

public interface IMessageService
{
    public Task<int> AddMessageAsync(BasicMessage<string> message);

    public Task<ICollection<BasicMessage<string>>> GetAllMessagesAsync();
}