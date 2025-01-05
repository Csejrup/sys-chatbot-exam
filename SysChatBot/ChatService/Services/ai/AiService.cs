using Chatbot;
using ChatService.Models;
using ChatService.Utils;
using Grpc.Net.Client;
using Polly.Retry;
using SysChatBot.Shared.Models;

namespace ChatService.Services.ai;

public class AiService : IAiService
{
    
    private readonly ChatbotService.ChatbotServiceClient _client;
    private readonly AsyncRetryPolicy _retryPolicy;

    public AiService(string grpcServerUrl, ChatbotService.ChatbotServiceClient? client = null )
    {
        var channel = GrpcChannel.ForAddress(grpcServerUrl);
        _client = client ?? new ChatbotService.ChatbotServiceClient(channel);
        _retryPolicy = PollyRetryPolicy.CreateRetryPolicy();

    }
    
    public async Task<string> GetAIResponseAsync(string userMessage, List<ChatMessage> conversationHistory)
    {
        // Create a request and call the gRPC server
        var request = new ChatbotRequest { Message = userMessage };
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            var response = await _client.GetResponseAsync(request);
            return response.Message;
        });
    }
}


