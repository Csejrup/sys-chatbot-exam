using LogChatService.Models;
using LogChatService.Repositories;
using SysChatBot.Shared;
using SysChatBot.Shared.Events;

namespace LogChatService.Services.Logs;

public class LogService : ILogService
{
    
    
    private readonly ILogRepository _logRepository;
    private readonly IMessageClient _messageClient;
    public LogService(ILogRepository logRepository, IMessageClient messageClient)
    {
        _logRepository = logRepository;
        _messageClient = messageClient;

        // Listen for requests to fetch tweets for a user
        _messageClient.Listen<ChatLogEvent>(HandleChatLogged, "CreateChatLog");

    }
    private async void HandleChatLogged(ChatLogEvent logEvent)
    {
        
        // TODO: ADD POLLY
        try
        {
            await _logRepository.AddChatLogAsync(new ChatLog()
            {
                Id = Guid.NewGuid(),
                UserId = logEvent.UserId,
                MessageId= logEvent.MessageId,
                ConversationId = logEvent.ConversationId,
                Status = logEvent.Status,
                Timestamp = logEvent.Timestamp,
                ErrorMessage = logEvent.ErrorMessage,
                AiResponse = logEvent.AiResponse,
                UserMessage = logEvent.UserMessage
            });

        }
        catch (Exception ex)
        {
            // DO sometihng? 
        }
    }

    
    public Task<List<ChatLog>> GetAllChatLogsByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetTopNErrorMessagesAsync(int numberOfMessages)
    {
        throw new NotImplementedException();
    }

    public Task AddChatLogAsync(ChatLog conversation)
    {
        throw new NotImplementedException();
    }
}