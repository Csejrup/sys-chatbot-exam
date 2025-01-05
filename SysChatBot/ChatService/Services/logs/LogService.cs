using SysChatBot.Shared;
using SysChatBot.Shared.Events;

namespace ChatService.Services.logs;

public class LogService(IMessageClient messageClient) : ILogService
{
    public void AddChatLogAsync(ChatLogEvent chatLogEvent)
    {
        messageClient.Send(chatLogEvent, "CreateChatLog");
    }
}