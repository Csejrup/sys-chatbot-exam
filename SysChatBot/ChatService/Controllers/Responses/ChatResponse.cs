using SysChatBot.Shared.Models.enums;

namespace ChatService.Controllers.Responses;

public class ChatResponse
{
    
    
    public string? Content { get; set; }

    
    public MessageRole Role { get; set; }
    
    public DateTime Timestamp { get; set; }

   
    public ResponseStatus Status { get; set; }


    public string? Metadata { get; set; }
}

public enum ResponseStatus
{

    Success = 1,
    Error = 2,
    Processing = 3,
    Failed = 4,
    Cancelled = 5
}