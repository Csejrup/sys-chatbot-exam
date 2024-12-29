using System.ComponentModel.DataAnnotations;
using SysChatBot.Shared.Models.enums;

namespace SysChatBot.Shared.Models;


public class ChatMessage
{
    [Key]
    public int MessageId { get; set; } 

    [Required]
    public string ConversationId { get; set; } 

    [Required]
    public string UserId { get; set; }

    [Required]
    public MessageRole Role { get; set; } 

    [Required]
    public string Content { get; set; } 

    [Required]
    public DateTime Timestamp { get; set; } 
}