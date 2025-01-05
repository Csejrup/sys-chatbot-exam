using EasyNetQ;
using LogChatService.DBContext;
using LogChatService.Repositories;
using LogChatService.Services.Logs;
using Microsoft.EntityFrameworkCore;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddDbContext<LogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Service bus
builder.Services.AddEasyNetQ("host=rabbitmq;username=admin;password=securepassword");
builder.Services.AddScoped<IMessageClient, MessageClient>();

// Repositories
builder.Services.AddScoped<ILogRepository, LogRepository>();

// Services
builder.Services.AddScoped<ILogService, LogService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
