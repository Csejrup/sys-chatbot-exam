using Chatbot;
using ChatService.Models;
using ChatService.Services.ai;
using Grpc.Core;
using Moq;

namespace ChatService.ServiceTests;

public class AiServiceTests
{
    [Fact]
    public async Task GetAIResponseAsync_ShouldReturnResponse_WhenCalledSuccessfully()
    {
        // Arrange
        var mockClient = new Mock<ChatbotService.ChatbotServiceClient>();
        var expectedMessage = "Mocked AI response";
        var request = new ChatbotRequest { Message = "Hello" };
        var response = new ChatbotResponse { Message = expectedMessage };

        mockClient
            .Setup(client => client.GetResponseAsync(request, null, null, default))
            .Returns(new AsyncUnaryCall<ChatbotResponse>(
                Task.FromResult(response), 
                Task.FromResult(new Metadata()), 
                () => Status.DefaultSuccess, 
                () => new Metadata(), 
                () => { }));

        var aiService = new AiService("http://localhost:5000", mockClient.Object);

        // Act
        var result = await aiService.GetAIResponseAsync("Hello", new List<ChatMessage>());

        // Assert
        Assert.Equal(expectedMessage, result);
        mockClient.Verify(c => c.GetResponseAsync(It.IsAny<ChatbotRequest>(), null, null, default), Times.Once);
    }

    [Fact]
    public async Task GetAIResponseAsync_ShouldRetryOnTransientError_AndSucceed()
    {
        // Arrange
        var mockClient = new Mock<ChatbotService.ChatbotServiceClient>();
        var expectedMessage = "Successful response after retry";
        var request = new ChatbotRequest { Message = "Hello" };
        var response = new ChatbotResponse { Message = expectedMessage };

        // Simulate transient failures followed by a successful response
        int callCount = 0;
        mockClient
            .Setup(client => client.GetResponseAsync(request, null, null, default))
            .Returns(() =>
            {
                if (callCount++ < 2) // Fail the first two times
                    throw new RpcException(new Status(StatusCode.Unavailable, "Transient error"));
                return new AsyncUnaryCall<ChatbotResponse>(
                    Task.FromResult(response), 
                    Task.FromResult(new Metadata()), 
                    () => Status.DefaultSuccess, 
                    () => new Metadata(), 
                    () => { });
            });

        var aiService = new AiService("http://localhost:5000", mockClient.Object);

        // Act
        var result = await aiService.GetAIResponseAsync("Hello", new List<ChatMessage>());

        // Assert
        Assert.Equal(expectedMessage, result);
        mockClient.Verify(c => c.GetResponseAsync(It.IsAny<ChatbotRequest>(), null, null, default), Times.Exactly(3));
    }

    [Fact]
    public async Task GetAIResponseAsync_ShouldFailAfterRetriesExceeded()
    {
        // Arrange
        var mockClient = new Mock<ChatbotService.ChatbotServiceClient>();
        mockClient
            .Setup(client => client.GetResponseAsync(It.IsAny<ChatbotRequest>(), null, null, default))
            .Throws(new RpcException(new Status(StatusCode.Unavailable, "Persistent error")));

        var aiService = new AiService("http://localhost:5000", mockClient.Object);

        // Act & Assert
        await Assert.ThrowsAsync<RpcException>(() => aiService.GetAIResponseAsync("Hello", new List<ChatMessage>()));
        mockClient.Verify(c => c.GetResponseAsync(It.IsAny<ChatbotRequest>(), null, null, default), Times.AtLeast(3));
    }
}