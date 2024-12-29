using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using SysChatBot.Shared;

namespace AuthenticationService.Services;

public class AuthService : IAuthService
{
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    public AuthService(Configurations config)
    {
        _jwtKey = config?.Authentication?.JwtKey ?? throw new ArgumentNullException(nameof(config.Authentication.JwtKey));
        _jwtIssuer = config?.Authentication?.JwtIssuer ?? throw new ArgumentNullException(nameof(config.Authentication.JwtIssuer));
    }


    public string GenerateToken(Guid userId)
    {

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create the claims for the token
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),

            new Claim("userId", userId.ToString()) // Add the userId as a custom claim
        };

        var securityToken = new JwtSecurityToken(
            _jwtIssuer,
            _jwtIssuer,
            claims,
            expires: DateTime.Now.AddHours(12),
            signingCredentials: credentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return token;
    }



}