using SysChatBot.Shared.Models;

namespace ChatService.Services.conversations;

public class ConversationService : IConversationService
{
    public Task<List<ChatMessage>> GetConversationHistoryAsync(string userId, string? conversationId)
    {
        throw new NotImplementedException();
    }

    public Task<string> StoreMessageAsync(string userId, string? conversationId, string userMessage, string aiResponse)
    {
        throw new NotImplementedException();
    }
}