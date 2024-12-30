using ChatService.Models;
using SysChatBot.Shared.Models;

namespace ChatService.Services.conversations;

public interface IConversationService
{
    Task<List<ChatMessage>?> GetConversationHistoryAsync(Guid userId, Guid? conversationId);

    Task StoreMessageAsync(Guid userId, Guid? conversationId, string userMessage, string aiResponse);

    Task<Conversation?> GetConversationByIdAsync(Guid userId, Guid? conversationId);

    Task<List<Conversation>> GetAllConversationsByUserId(Guid userId);

}