using ChatService.DBContext;
using ChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Repositories;

public class ConversationRepository(ChatDbContext context) : IConversationRepository
{
    private readonly ChatDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<Conversation?> GetConversationByIdAsync(Guid? conversationId)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.ConversationId.Equals(conversationId));
    }

    public async Task<List<Conversation>> GetAllConversationsByUserIdAsync(Guid userId)
    {
        return await _context.Conversations
            .Where(x => x.UserId.Equals(userId))
            .ToListAsync();
    }

    public async Task<Conversation> AddConversationAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await SaveChangesAsync();
        return conversation;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}