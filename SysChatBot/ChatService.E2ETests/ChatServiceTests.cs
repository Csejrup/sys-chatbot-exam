using System.ComponentModel;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChatService.Controllers.Responses;
using Xunit;
using Xunit.Abstractions;
using IContainer = DotNet.Testcontainers.Containers.IContainer;

namespace ChatService.E2ETests;

public class ChatServiceTests : IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private HttpClient _client;
    private INetwork _network;
    private IContainer _aiServiceContainer;
    private IContainer chatServiceContainer;
    
    public ChatServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task SendMessage_ShouldReturnAiResponse()
    {
        // Arrange
        var chatRequest = new
        {
            Message = "Hello, AI!",
            ConversationId = "123e4567-e89b-12d3-a456-426614174000"
        };

        var content = new StringContent(JsonSerializer.Serialize(chatRequest), Encoding.UTF8, "application/json");
        _client.DefaultRequestHeaders.Add("userId", "123e4567-e89b-12d3-a456-426614174000");

        // Act
        var response = await _client.PostAsync("/chat", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var chatResponse = JsonSerializer.Deserialize<ChatResponse>((string) responseBody);

        // Assert
        Assert.NotNull(chatResponse);
        Assert.Equal("Hello from ai!", chatResponse.Content); // Mocked response in the AI service
        Assert.Equal(ResponseStatus.Success, chatResponse.Status);
    }

    public async Task InitializeAsync()
    {
        _output.WriteLine("Starting testcontainers...");

        _network = new NetworkBuilder().Build();

        _aiServiceContainer = new ContainerBuilder()
            .WithName("aiservice")
            .WithImage("sys-chatbot-exam/grpcservice")  // Replace with actual image
            .WithNetwork(_network)
            .WithNetworkAliases("aiservice")
            .WithPortBinding(5000, 5000)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5000))
            .Build();

        await _aiServiceContainer.StartAsync();
        _output.WriteLine("AI service started.");

        chatServiceContainer = new ContainerBuilder()
            .WithName("chatservice")
            .WithImage("sys-chatbot-exam/chatservice")  // Replace with actual image
            .WithNetwork(_network)
            .WithNetworkAliases("chatapi")
            .WithPortBinding(8080, 80)
            .WithEnvironment("AI_SERVICE_URL", "http://aiservice:5000")
            .DependsOn(_aiServiceContainer)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
            .Build();

        await chatServiceContainer.StartAsync();
        _output.WriteLine("Chat Service started.");

        _client = new HttpClient { BaseAddress = new Uri("http://localhost:8080") };
    }

    public async Task DisposeAsync()
    {
        await _aiServiceContainer.DisposeAsync();
        await chatServiceContainer.DisposeAsync();
    }
}
