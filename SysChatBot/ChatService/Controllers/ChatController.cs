using ChatService.Controllers.Requests;
using ChatService.Controllers.Responses;
using ChatService.Models;
using ChatService.Models.enums;
using ChatService.Services.ai;
using ChatService.Services.conversations;
using ChatService.Services.logs;
using Microsoft.AspNetCore.Mvc;
using SysChatBot.Shared.Events;
using SysChatBot.Shared.Models.Enums;

namespace ChatService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChatController
    : ControllerBase
{

    private readonly IConversationService _conversationService;
    private readonly IAiService _aiService;
    private readonly ILogService _logService;

    public ChatController(IConversationService conversationService, IAiService aiService, ILogService logService)
    {
        _conversationService = conversationService ?? throw new ArgumentNullException(nameof(conversationService));
        _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }







    [HttpPost("chat")]
    public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
    {
        var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

        try
        {
            // Validate request
            if (string.IsNullOrEmpty(userId))
            {

                return Unauthorized(new { message = "User ID not found in the header." });
            }

            if (string.IsNullOrEmpty(request.Message))
            {
                _logService.AddChatLogAsync(new ChatLogEvent()
                {
                    Timestamp = DateTime.UtcNow,
                    ConversationId = !string.IsNullOrEmpty(request.ConversationId) ? Guid.Parse(request.ConversationId) : null,
                    Status = LogStatus.Error,
                    UserId = Guid.Parse(userId),
                    ErrorMessage = "No message provided.",

                });
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


            _logService.AddChatLogAsync(new ChatLogEvent()
            {
                Timestamp = DateTime.UtcNow,
                ConversationId = !string.IsNullOrEmpty(request.ConversationId) ? Guid.Parse(request.ConversationId) : null,
                Status = LogStatus.Error,
                UserId = Guid.Parse(userId),
                UserMessage = request.Message,
                AiResponse = aiResponse
            });

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
            _logService.AddChatLogAsync(new ChatLogEvent()
            {
                Timestamp = DateTime.UtcNow,
                ConversationId = !string.IsNullOrEmpty(request.ConversationId) ? Guid.Parse(request.ConversationId) : null,
                Status = LogStatus.Error,
                UserId = Guid.Parse(userId),
                ErrorMessage = e.Message

            });

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