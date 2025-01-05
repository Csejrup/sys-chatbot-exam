using ChatService.Services.conversations;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationController(IConversationService conversationService) : ControllerBase
{

    private readonly IConversationService _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetConversationById(string conversationId)
    {
        // Validate request
        var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

        if (string.IsNullOrEmpty(userId))
        {

            return Unauthorized(new { message = "User ID not found in the header." });
        }
        
        var conversation = await _conversationService.GetConversationByIdAsync(Guid.Parse(userId),
            Guid.Parse(conversationId));
        return Ok(conversation);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConversations()
    {
        // Validate request
        var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

        if (string.IsNullOrEmpty(userId))
        {

            return Unauthorized(new { message = "User ID not found in the header." });
        }

        var conversations = await _conversationService.GetAllConversationsByUserId(Guid.Parse(userId)); 
        return Ok(conversations);
    }
}