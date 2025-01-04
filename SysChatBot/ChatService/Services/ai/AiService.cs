using Chatbot;
using ChatService.Models;
using Grpc.Net.Client;
using SysChatBot.Shared.Models;

namespace ChatService.Services.ai;

public class AiService : IAiService
{
    
    private readonly ChatbotService.ChatbotServiceClient _client;

    public AiService(string grpcServerUrl)
    {
        var channel = GrpcChannel.ForAddress(grpcServerUrl);
        _client = new ChatbotService.ChatbotServiceClient(channel);
    }
    
    public async Task<string> GetAIResponseAsync(string userMessage, List<ChatMessage> conversationHistory)
    {
        // Create a request and call the gRPC server
        var request = new ChatbotRequest { Message = userMessage };
        var response = await _client.GetResponseAsync(request);
        return response.Message;
    }
}