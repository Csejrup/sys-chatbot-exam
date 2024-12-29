using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



// Services


var app = builder.Build();

// Add Swagger UI middleware only in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
 
}

app.UseHttpsRedirection();
app.MapControllers();  // Ensure controllers are mapped

app.Run();