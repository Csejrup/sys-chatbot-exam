using Chatbot;
using ChatService.Models;
using ChatService.Services.ai;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Grpc.Net.Client;
using Xunit;
using System.Threading.Tasks;
using Chatbot;
using ChatService.ServiceTests.Utils;
using Grpc.Core;

namespace ChatService.ServiceTests;

public class AiServiceTests : IAsyncLifetime
{
    private AiService _aiService;

    private readonly string _grpcServerUrl = "http://localhost:5000"; // Local test server URL
    
    
    public async Task InitializeAsync()
    {
        
        var stubClient = new StubChatbotServiceClient();
        _aiService = new AiService(_grpcServerUrl, stubClient);
    }
    

    [Fact]
    public async Task GetAIResponseAsync_ReturnsCorrectResponse()
    {
        // Arrange
        string userMessage = "Hello!";
        var conversationHistory = new List<ChatMessage>(); 

        // Act
        var result = await _aiService.GetAIResponseAsync(userMessage, conversationHistory);
        var expectedResponse = "Hello from the AI!";
        
        
        // Assert
        double similarity = Similarity.GetSimilarity(expectedResponse, result);
        Assert.True(similarity >= 60, $"The similarity was {similarity}% but expected at least 60%");
    }
    
    
    
    public async Task DisposeAsync()
    {
    }


}


   




public class StubChatbotServiceClient : ChatbotService.ChatbotServiceClient
{
    public override AsyncUnaryCall<ChatbotResponse> GetResponseAsync(ChatbotRequest request, CallOptions options = default)
    {
        // Create a mock response
        
        var response = new ChatbotResponse
        {
            Message =  request.Message switch {
            "Hello!" => "Hello from the AI chatbot!",
            "Who are you?" => "I am an ai chatbot designed to help you",
            _ => "I could not understand this message!"
        }
        };

        // Create a Task that wraps the response
        var task = Task.FromResult(response);

        // Return the task as an AsyncUnaryCall
        return new AsyncUnaryCall<ChatbotResponse>(
        task, null,null,null,null
        );
    }
}

