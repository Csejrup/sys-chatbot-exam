using SysChatBot.Shared.Events;

namespace ChatService.Services.logs;

public interface ILogService
{
    public void AddChatLogAsync(ChatLogEvent chatLogEvent);
}