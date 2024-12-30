using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using SysChatBot.Shared;

namespace AuthenticationService.Services;

public class AuthService : IAuthService
{

    private readonly IUserRepository _userRepository;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    public AuthService(IUserRepository userRepository, Configurations config)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _jwtKey = config?.Authentication?.JwtKey ?? throw new ArgumentNullException(nameof(config.Authentication.JwtKey));
        _jwtIssuer = config?.Authentication?.JwtIssuer ?? throw new ArgumentNullException(nameof(config.Authentication.JwtIssuer));
    }





    public async Task<string> SignupAsync(string email, string password)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser != null)
        {
            throw new Exception("User already exists.");
        }

        // TODO: Hash the password before storing

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Email = email,
            Password = password
        };

        // Save the user to the database
        await _userRepository.CreateUserAsync(user);

        // Return the JWT token
        return GenerateJwtToken(user.UserId);
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(email);
        if (user == null || !VerifyPassword(password, user.Password))
        {
            throw new Exception("Invalid credentials.");
        }

        // Return the JWT token
        return GenerateJwtToken(user.UserId);

    }


    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        // You need to use the same salt and hashing method used during signup.
        return enteredPassword == storedPassword;
    }

    private string GenerateJwtToken(Guid userId)
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