using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SysChatBot.Shared.Models.enums;

namespace ChatService.Models;


public class ChatMessage
{
    [Key]
    public Guid MessageId { get; set; } 
    
    [Required]
    public Guid ConversationId { get; set; } 
    
    [Required]
    public MessageRole Role { get; set; } 

    [Required]
    public string Content { get; set; } 

    [Required]
    public DateTime Timestamp { get; set; } 
    
    
    [ForeignKey(nameof(ConversationId))]
    public Conversation Conversation { get; set; } 
}