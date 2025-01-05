using LogChatService.Models;

namespace LogChatService.Repositories;

public class LogRepository : ILogRepository
{
    public Task<List<ChatLog>> GetAllChatLogsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetTopNErrorMessages(int numberOfMessages)
    {
        throw new NotImplementedException();
    }

    public Task AddChatLogAsync(ChatLog conversation)
    {
        throw new NotImplementedException();
    }

   
}