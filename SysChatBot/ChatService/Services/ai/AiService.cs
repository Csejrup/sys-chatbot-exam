using SysChatBot.Shared.Models;

namespace ChatService.Services.ai;

public class AiService : IAiService
{
    public Task<string> GetAIResponseAsync(string userMessage, List<ChatMessage> conversationHistory)
    {
        throw new NotImplementedException();
    }
}