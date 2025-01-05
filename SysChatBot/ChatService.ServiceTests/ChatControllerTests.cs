using ChatService.Controllers;
using ChatService.Controllers.Requests;
using ChatService.Controllers.Responses;
using ChatService.Models;
using ChatService.Services.ai;
using ChatService.Services.conversations;
using ChatService.Services.logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ChatService.ServiceTests;


public class ChatControllerTests
{
    private readonly Mock<IConversationService> _conversationServiceMock;
    private readonly Mock<IAiService> _aiServiceMock;
    private readonly Mock<ILogService> _logServiceMock;
    private readonly ChatController _chatController;

    public ChatControllerTests()
    {
        _conversationServiceMock = new Mock<IConversationService>();
        _aiServiceMock = new Mock<IAiService>();
        _logServiceMock = new Mock<ILogService>();
        _chatController = new ChatController(
            _conversationServiceMock.Object,
            _aiServiceMock.Object,
            _logServiceMock.Object
        );
    }

    [Fact]
    public async Task SendMessage_ShouldReturnUnauthorized_WhenUserIdIsMissing()
    {
        // Arrange
        var request = new ChatRequest { Message = "Hello" };
        _chatController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext() // No userId in headers
        };

        // Act
        var result = await _chatController.SendMessage(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("User ID not found in the header.", unauthorizedResult.Value?.ToString());
    }

    [Fact]
    public async Task SendMessage_ShouldReturnFailedResponse_WhenMessageIsEmpty()
    {
        // Arrange
        var request = new ChatRequest { Message = "" };
        _chatController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _chatController.Request.Headers["userId"] = Guid.NewGuid().ToString();

        // Act
        var result = await _chatController.SendMessage(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ChatResponse>(okResult.Value);
        Assert.Equal("You did not provide any message.", response.Content);
        Assert.Equal(ResponseStatus.Failed, response.Status);
    }

    [Fact]
    public async Task SendMessage_ShouldReturnSuccessfulResponse_WhenValidRequestProvided()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var request = new ChatRequest { Message = "Hello" };
        _chatController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _chatController.Request.Headers["userId"] = userId;

        _aiServiceMock.Setup(ai => ai.GetAIResponseAsync(request.Message, It.IsAny<List<ChatMessage>>()))
            .ReturnsAsync("AI Response");

        // Act
        var result = await _chatController.SendMessage(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ChatResponse>(okResult.Value);
        Assert.Equal("AI Response", response.Content);
        Assert.Equal(ResponseStatus.Success, response.Status);
    }

    [Fact]
    public async Task SendMessage_ShouldLogError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var request = new ChatRequest { Message = "Hello" };
        _chatController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _chatController.Request.Headers["userId"] = userId;

        _aiServiceMock.Setup(ai => ai.GetAIResponseAsync(request.Message, It.IsAny<List<ChatMessage>>()))
            .ThrowsAsync(new Exception("AI Service error"));

        // Act
        var result = await _chatController.SendMessage(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ChatResponse>(okResult.Value);
        Assert.Equal("Sorry, something went wrong. Please try again later.", response.Content);
        Assert.Equal(ResponseStatus.Error, response.Status);
    }
}
