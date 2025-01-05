using ChatService.Models;
using ChatService.Models.enums;
using ChatService.Repositories;
using ChatService.Services.conversations;
using ChatService.Services.logs;
using Moq;

namespace ChatService.ServiceTests;

public class ConversationServiceTests
{
    private readonly Mock<IConversationRepository> _mockRepository;
    private readonly Mock<ILogService> _mockLogService;
    private readonly ConversationService _conversationService;

    public ConversationServiceTests()
    {
        _mockRepository = new Mock<IConversationRepository>();
        _mockLogService = new Mock<ILogService>();
        _conversationService = new ConversationService(_mockRepository.Object, _mockLogService.Object);
    }

    [Fact]
    public async Task GetConversationHistoryAsync_ShouldReturnMessages_WhenConversationExists()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var conversation = new Conversation
        {
            ConversationId = conversationId,
            Messages = new List<ChatMessage>
            {
                new ChatMessage { Content = "Hello", Role = MessageRole.User },
                new ChatMessage { Content = "Hi there!", Role = MessageRole.AI }
            }
        };
        _mockRepository.Setup(repo => repo.GetConversationByIdAsync(conversationId)).ReturnsAsync(conversation);

        // Act
        var result = await _conversationService.GetConversationHistoryAsync(userId, conversationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task StoreMessageAsync_ShouldCreateNewConversation_WhenConversationIdIsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userMessage = "Hello AI";
        var aiResponse = "Hello, how can I help you?";

        _mockRepository.Setup(repo => repo.AddConversationAsync(It.IsAny<Conversation>()))
                       .Returns(() => Task.CompletedTask);
        _mockRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _conversationService.StoreMessageAsync(userId, null, userMessage, aiResponse);

        // Assert
        _mockRepository.Verify(repo => repo.AddConversationAsync(It.Is<Conversation>(c =>
            c.UserId == userId &&
            c.Messages.Any(m => m.Content == userMessage && m.Role == MessageRole.User) &&
            c.Messages.Any(m => m.Content == aiResponse && m.Role == MessageRole.AI))), Times.Once);
        _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task StoreMessageAsync_ShouldAddMessagesToExistingConversation_WhenConversationExists()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userMessage = "New user message";
        var aiResponse = "New AI response";
        var conversation = new Conversation
        {
            ConversationId = conversationId,
            UserId = userId,
            Messages = new List<ChatMessage>()
        };

        _mockRepository.Setup(repo => repo.GetConversationByIdAsync(conversationId)).ReturnsAsync(conversation);
        _mockRepository.Setup(repo => repo.SaveChangesAsync()).Returns(Task.CompletedTask);

        // Act
        await _conversationService.StoreMessageAsync(userId, conversationId, userMessage, aiResponse);

        // Assert
        Assert.Equal(2, conversation.Messages.Count);
        Assert.Contains(conversation.Messages, m => m.Content == userMessage && m.Role == MessageRole.User);
        Assert.Contains(conversation.Messages, m => m.Content == aiResponse && m.Role == MessageRole.AI);
        _mockRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }
}
