using ChatService.Services.ai;
using ChatService.Services.conversations;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator


// Services
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IConversationService, ConversationService>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

// Add Swagger UI middleware only in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
 
}

app.UseHttpsRedirection();
app.MapControllers();  // Ensure controllers are mapped

app.Run();