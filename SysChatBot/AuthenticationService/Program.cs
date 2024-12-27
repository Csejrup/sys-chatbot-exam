using AuthenticationService;
using AuthenticationService.Services;
using AuthenticationService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  

var config = builder.Configuration.GetSection("Configurations").Get<Configurations>();


// Services
builder.Services.AddSingleton<Configurations>(config ?? new Configurations());

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Add Swagger UI middleware only in development environment
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "API V1");  
        c.RoutePrefix = string.Empty;  
    });
}

app.UseHttpsRedirection();
app.MapControllers();  // Ensure controllers are mapped

app.Run();