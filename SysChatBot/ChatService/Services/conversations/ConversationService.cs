using ChatService.DBContext;
using ChatService.Models;
using ChatService.Models.enums;
using ChatService.Repositories;
using ChatService.Services.logs;
using Microsoft.EntityFrameworkCore;
using SysChatBot.Shared.Models;

namespace ChatService.Services.conversations;
public class ConversationService(IConversationRepository conversationRepository, ILogService logService)
    : IConversationService
{
    private readonly IConversationRepository _conversationRepository = conversationRepository ?? throw new ArgumentNullException(nameof(conversationRepository));
    private readonly ILogService _logService = logService ?? throw new ArgumentNullException(nameof(logService));

    public async Task<List<ChatMessage>?> GetConversationHistoryAsync(Guid userId, Guid? conversationId)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(conversationId);
        return conversation?.Messages.ToList();
    }

    public async Task StoreMessageAsync(Guid userId, Guid? conversationId, string userMessage, string aiResponse)
    {
        // TODO: Add some retry or Polly logic

        // Ensure the conversation ID exists or create a new one
        var conversation = conversationId.HasValue
            ? await _conversationRepository.GetConversationByIdAsync(conversationId)
            : null;

        if (conversation == null)
        {
            // Create a new conversation if it doesn't exist
            conversation = new Conversation
            {
                ConversationId = Guid.NewGuid(),
                Title = userMessage,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Messages = new List<ChatMessage>()
            };

            await _conversationRepository.AddConversationAsync(conversation);
        }

        // Add the user's message
        conversation.Messages.Add(new ChatMessage
        {
            MessageId = Guid.NewGuid(),
            ConversationId = conversation.ConversationId,
            Role = MessageRole.User,
            Content = userMessage,
            Timestamp = DateTime.UtcNow
        });

        // Add the AI's response
        conversation.Messages.Add(new ChatMessage
        {
            MessageId = Guid.NewGuid(),
            ConversationId = conversation.ConversationId,
            Role = MessageRole.AI, 
            Content = aiResponse,
            Timestamp = DateTime.UtcNow
        });

        // Save changes to the database
        await _conversationRepository.SaveChangesAsync();
    }

    public async Task<Conversation?> GetConversationByIdAsync(Guid userId, Guid? conversationId)
    {
       return await _conversationRepository.GetConversationByIdAsync(conversationId);
    }

    public async Task<List<Conversation>> GetAllConversationsByUserId(Guid userId)
    {
        return await _conversationRepository.GetAllConversationsByUserIdAsync(userId);
    }
}