using LogChatService.Models;

namespace LogChatService.Services.Logs;

public interface ILogService
{
    Task<List<ChatLog>> GetAllChatLogsByUserIdAsync(Guid userId);
    Task<List<string>> GetTopNErrorMessagesAsync(int numberOfMessages);
    
    Task AddChatLogAsync(ChatLog conversation);
}