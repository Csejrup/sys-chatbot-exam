using ChatService.Controllers.Requests;
using ChatService.Controllers.Responses;
using ChatService.Models;
using ChatService.Services.ai;
using ChatService.Services.conversations;
using Microsoft.AspNetCore.Mvc;
using SysChatBot.Shared.Models;
using SysChatBot.Shared.Models.enums;

namespace ChatService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{

    private readonly IConversationService _conversationService;
    private readonly IAiService _aiService;

    public ChatController(IConversationService conversationService, IAiService aiService)
    {
        _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
    }




    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

        return Ok("WELCOME TO THE CHAT. Your userId is: " + userId);
    }




    [HttpPost("chat")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {

        try
        {
            // Validate request
            var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

            if (string.IsNullOrEmpty(userId))
            {
                // TODO: Send log error event 

                return Unauthorized(new { message = "User ID not found in the header." });
            }



            if (string.IsNullOrEmpty(request.Message))
            {
                // TODO: Send log error event 

                return Ok(new ChatResponse()
                {
                    Content = "You did not provide any message.",
                    Timestamp = DateTime.UtcNow,
                    Role = MessageRole.AI,
                    Status = ResponseStatus.Failed,
                    Metadata = "Message was empty"
                });
            }

            // Retrieve conversation history (if applicable)
            List<ChatMessage>? history = new List<ChatMessage>();
            if (!string.IsNullOrEmpty(request.ConversationId))
            {
                history = await _conversationService.GetConversationHistoryAsync(Guid.Parse(userId), Guid.Parse(request.ConversationId));
            }

            // Forward user query and history to AI Service
            var aiResponse = await _aiService.GetAIResponseAsync(request.Message, history);

            // Store user message and AI response in conversation
            _ = _conversationService.StoreMessageAsync(Guid.Parse(userId),
                string.IsNullOrEmpty(request.ConversationId) ? null : Guid.Parse(request.ConversationId),
                request.Message, aiResponse);


            // TODO: Send log success event 

            // Return response
            return Ok(new ChatResponse()
            {
                Content = aiResponse,
                Timestamp = DateTime.UtcNow,
                Role = MessageRole.AI,
                Status = ResponseStatus.Success,

            });

        }
        catch (Exception e)
        {
            // TODO: Send log error event 

            return Ok(new ChatResponse()
            {

                Timestamp = DateTime.UtcNow,
                Content = "Sorry, something went wrong. Please try again later.",
                Role = MessageRole.AI,
                Status = ResponseStatus.Error,
                Metadata = e.Message
            });

        }

    }




}