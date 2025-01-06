using Chatbot;
using ChatService.Models;
using ChatService.Utils;
using Grpc.Net.Client;
using Polly;
using Polly.Wrap;

namespace ChatService.Services.ai;

public class AiService : IAiService
{
    private readonly ChatbotService.ChatbotServiceClient _client;
    private readonly AsyncPolicyWrap _policy; 

    public AiService(string grpcServerUrl, ChatbotService.ChatbotServiceClient? client = null)
    {
        var channel = GrpcChannel.ForAddress(grpcServerUrl);
        _client = client ?? new ChatbotService.ChatbotServiceClient(channel);

        // Create retry and circuit breaker policies and combine them
        var retryPolicy = PollyRetryPolicy.CreateRetryPolicy();
        var circuitBreakerPolicy = PollyCircuitBreaker.CreateCircuitBreakerPolicy();
        _policy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);
    }

    public async Task<string> GetAIResponseAsync(string userMessage, List<ChatMessage> conversationHistory)
    {
        // Create a request and call the gRPC server
        var request = new ChatbotRequest { Message = userMessage };

        // Use the combined policy for resilience
        return await _policy.ExecuteAsync(async () =>
        {
            var response = await _client.GetResponseAsync(request);
            return response.Message;
        });
    }
}