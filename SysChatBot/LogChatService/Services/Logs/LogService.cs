using LogChatService.Models;
using LogChatService.Repositories;
using SysChatBot.Shared;
using SysChatBot.Shared.Events;
using Polly.Retry;
using Shared.Events.Saga;
using Shared.Utils;

namespace LogChatService.Services.Logs;

public class LogService : ILogService
{
    private readonly ILogRepository _logRepository;
    private readonly IMessageClient _messageClient;
    private readonly AsyncRetryPolicy _retryPolicy;

    public LogService(ILogRepository logRepository, IMessageClient messageClient)
    {
        _logRepository = logRepository;
        _messageClient = messageClient;

        // Initialize retry policy
        _retryPolicy = PollyRetryPolicy.CreateRetryPolicy();

        // Listen for requests to fetch chat logs
        _messageClient.Listen<ChatLogEvent>(HandleChatLogged, "CreateChatLog");
    }

    private async void HandleChatLogged(ChatLogEvent logEvent)
    {
        try
        {
           
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _logRepository.AddChatLogAsync(new ChatLog
                {
                    Id = Guid.NewGuid(),
                    UserId = logEvent.UserId,
                    MessageId = logEvent.MessageId,
                    ConversationId = logEvent.ConversationId,
                    Status = logEvent.Status,
                    Timestamp = logEvent.Timestamp,
                    ErrorMessage = logEvent.ErrorMessage,
                    AiResponse = logEvent.AiResponse,
                    UserMessage = logEvent.UserMessage
                });
            });

           
            var successEvent = new LogEvent
            {
                UserId = logEvent.UserId,
                ConversationId = logEvent.ConversationId,
                EventType = "ChatLogCreated",
                Timestamp = DateTime.UtcNow
            };

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await Task.Run(() => _messageClient.Send(successEvent, "ChatLogCreated"));
            });
        }
        catch (Exception ex)
        {
            
            var failureEvent = new LogEvent
            {
                UserId = logEvent.UserId,
                ConversationId = logEvent.ConversationId,
                EventType = "ChatLogFailed",
                Timestamp = DateTime.UtcNow,
                Reason = ex.Message
            };

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await Task.Run(() => _messageClient.Send(failureEvent, "ChatLogFailed"));
            });

            throw; 
        }
    }

    public async Task<List<ChatLog>> GetAllChatLogsByUserIdAsync(Guid userId)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            return await Task.Run(() => _logRepository.GetAllChatLogsByUserIdAsync(userId));
        });
    }

    public async Task<List<string>> GetTopNErrorMessagesAsync(int numberOfMessages)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            return await Task.Run(() => _logRepository.GetTopNErrorMessagesAsync(numberOfMessages));
        });
    }

    public async Task AddChatLogAsync(ChatLog conversation)
    {
        await _retryPolicy.ExecuteAsync(async () =>
        {
            await Task.Run(() => _logRepository.AddChatLogAsync(conversation));
        });
    }
}
