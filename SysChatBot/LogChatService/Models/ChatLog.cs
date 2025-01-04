using System.ComponentModel.DataAnnotations;
using LogChatService.Models.Enums;

namespace LogChatService.Models;

public class ChatLog
{
    [Key]
    public Guid Id { get; set; } 
    
    [Required]
    public Guid? ConversationId { get; set; } 
    
    [Required]
    public Guid MessageId { get; set; } 
    
    [Required]
    public Guid UserId { get; set; } 

    [Required]
    public DateTime Timestamp { get; set; } 
    
    [Required]
    public string? UserMessage { get; set; } 
    
    [Required]
    public string? AiResponse { get; set; } 
    
    [Required]
    public LogStatus Status { get; set; } 
    
    [Required]
    public string? ErrorMessage { get; set; } 

}