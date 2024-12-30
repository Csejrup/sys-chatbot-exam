using AuthenticationService;
using AuthenticationService.DBContext;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using AuthenticationService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();  // Add Swagger generator

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var config = builder.Configuration.GetSection("Configurations").Get<Configurations>();


// Services
builder.Services.AddSingleton<Configurations>(config ?? new Configurations());

builder.Services.AddScoped<IAuthService, AuthService>();

// Repos
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// Apply any pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.Migrate(); // This will automatically apply any pending migrations
}

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