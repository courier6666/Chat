using Chat.Message;

namespace Chat.DatabaseAccess;

public interface IMessageService
{
    Task<int> AddMessageAsync(BasicMessage<string> message);

    Task<ICollection<BasicMessage<string>>> GetAllMessagesAsync();

    Task<ICollection<BasicMessage<string>>> GetPagedMessagedFromBeforeAsync(DateTime before, int size);
    
    Task<ICollection<BasicMessage<string>>> GetPagedMostRecentMessagesAsync(int count);
}