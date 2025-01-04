using LogChatService.Models.Enums;

namespace SysChatBot.Shared.Events;

public class ChatLogEvent
{
    public Guid? ConversationId { get; set; } 
    
    public Guid MessageId { get; set; } 
    
    public Guid UserId { get; set; } 

    public DateTime Timestamp { get; set; } 
    
    public string? UserMessage { get; set; } 
    
    public string? AiResponse { get; set; } 
    
    public LogStatus Status { get; set; } 
    
    public string? ErrorMessage { get; set; } 
}