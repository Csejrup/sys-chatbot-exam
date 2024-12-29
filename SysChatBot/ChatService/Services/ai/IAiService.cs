using SysChatBot.Shared.Models;

namespace ChatService.Services.ai;

public interface IAiService
{
    public Task<string> GetAIResponseAsync(string userMessage, List<ChatMessage>? conversationHistory);

}