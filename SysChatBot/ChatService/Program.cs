using ChatService.DBContext;
using ChatService.Repositories;
using ChatService.Services.ai;
using ChatService.Services.conversations;
using Microsoft.EntityFrameworkCore;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator


// Repos
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

// Services
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddSingleton<IAiService>(provider =>
    new AiService("http://localhost:50051"));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContext<ChatDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

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