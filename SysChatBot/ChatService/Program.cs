using ChatService.DBContext;
using ChatService.Repositories;
using ChatService.Services.ai;
using ChatService.Services.conversations;
using ChatService.Services.logs;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator



builder.Services.AddDbContext<ChatDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Service bus
builder.Services.AddEasyNetQ("host=rabbitmq;username=admin;password=securepassword");
builder.Services.AddScoped<IMessageClient, MessageClient>();

// Repos
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

// Services
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddSingleton<IAiService>(provider =>
    new AiService("http://grpcservice:50051"));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Apply any pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
    dbContext.Database.Migrate(); // This will automatically apply any pending migrations
}
// Add Swagger UI middleware only in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseHttpsRedirection();
app.MapControllers();  // Ensure controllers are mapped

app.Run();