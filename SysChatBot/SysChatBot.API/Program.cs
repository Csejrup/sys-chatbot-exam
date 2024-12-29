using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using SysChatBot.Shared;

var builder = WebApplication.CreateBuilder(args);
//load config
var config = builder.Configuration.GetSection("Configurations").Get<Configurations>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var jwtIssuer = config?.Authentication?.JwtIssuer
    ?? throw new NullReferenceException(nameof(config.Authentication.JwtIssuer));
var jwtKey = config?.Authentication?.JwtKey ?? throw new NullReferenceException(nameof(config.Authentication.JwtKey));

// Add authetincation
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };


    }
);

// Add Ocelot config
builder.Configuration.AddJsonFile("ocelot.json");

// Add Ocelot services
builder.Services.AddOcelot();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Use Ocelot middleware
app.UseOcelot().Wait();

app.Run();