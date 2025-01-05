using ChatService.Models;

namespace ChatService.Repositories;

public interface IConversationRepository
{
    Task<Conversation?> GetConversationByIdAsync(Guid? conversationId);
    Task<List<Conversation>> GetAllConversationsByUserIdAsync(Guid userId);
    Task<Conversation> AddConversationAsync(Conversation conversation);
    Task SaveChangesAsync();
}