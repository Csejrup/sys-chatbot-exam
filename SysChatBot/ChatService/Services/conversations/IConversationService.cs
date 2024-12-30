using SysChatBot.Shared.Models;

namespace ChatService.Services.conversations;

public interface IConversationService
{
    Task<List<ChatMessage>?> GetConversationHistoryAsync(string userId, string? conversationId);
    Task StoreMessageAsync(string userId, string? conversationId, string userMessage, string aiResponse);
    
}