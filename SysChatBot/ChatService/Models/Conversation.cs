using System.ComponentModel.DataAnnotations;

namespace ChatService.Models;

public class Conversation
{
    [Key]
    public Guid ConversationId { get; set; } // Matches the type in ChatMessage
    
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Title { get; set; } 

    [Required]
    public DateTime CreatedAt { get; set; } // When the conversation was created


    // Navigation property
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}