using SysChatBot.Shared;
using SysChatBot.Shared.Events;

namespace ChatService.Services.logs;

public class LogService : ILogService
{
    private readonly IMessageClient _messageClient;
    public LogService( IMessageClient messageClient)
    {
        _messageClient = messageClient;
        
    }
    public void AddChatLogAsync(ChatLogEvent chatLogEvent)
    {
        _messageClient.Send(chatLogEvent, "CreateChatLog");
    }
}