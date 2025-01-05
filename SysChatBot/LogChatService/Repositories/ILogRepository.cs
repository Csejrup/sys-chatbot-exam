using LogChatService.Models;

namespace LogChatService.Repositories;

public interface ILogRepository
{
    Task<List<ChatLog>> GetAllChatLogsByUserIdAsync(Guid userId);
    Task<List<string>> GetTopNErrorMessages(int numberOfMessages);
    
    Task AddChatLogAsync(ChatLog conversation);

}