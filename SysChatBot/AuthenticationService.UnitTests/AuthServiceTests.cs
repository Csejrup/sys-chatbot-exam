using AuthenticationService.Models;
using AuthenticationService.Repositories;
using AuthenticationService.Services;
using SysChatBot.Shared.config;

namespace AuthenticationService.UnitTests;


   using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<Configurations> _configMock;
    private readonly AuthService _authService;
    private readonly string _jwtKey = "mpc.corp.org";
    private readonly string _jwtIssuer = "my-super-secure-jwt-key-12345678";
    
    public AuthServiceTests()
    {
        // Setup mock configurations
        _configMock = new Mock<Configurations>();
        _configMock.Setup(c => c.Authentication.JwtKey).Returns(_jwtKey);
        _configMock.Setup(c => c.Authentication.JwtIssuer).Returns(_jwtIssuer);

        // Setup mock user repository
        _userRepositoryMock = new Mock<IUserRepository>();

        // Initialize AuthService with mocks
        _authService = new AuthService(_userRepositoryMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task SignupAsync_ShouldReturnToken_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var newUser = new User { UserId = Guid.NewGuid(), Email = email, Password = password };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);
        _userRepositoryMock.Setup(repo => repo.CreateUserAsync(It.IsAny<User>())).Returns(Task.FromResult(new Guid()));

        // Act
        var result = await _authService.SignupAsync(email, password);

        // Assert
        Assert.NotNull(result);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadToken(result) as JwtSecurityToken;
        Assert.Equal(_jwtIssuer, jsonToken.Issuer);
        Assert.Equal(_jwtIssuer, jsonToken.Audiences.First());
    }

    [Fact]
    public void SignupAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var existingUser = new User { UserId = Guid.NewGuid(), Email = email, Password = password };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(existingUser);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.SignupAsync(email, password));
        Assert.Equal("User already exists.", ex.Result.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        var user = new User { UserId = Guid.NewGuid(), Email = email, Password = password };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        Assert.NotNull(result);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jsonToken = tokenHandler.ReadToken(result) as JwtSecurityToken;
        Assert.Equal(_jwtIssuer, jsonToken.Issuer);
        Assert.Equal(_jwtIssuer, jsonToken.Audiences.First());
    }

    [Fact]
    public void LoginAsync_ShouldThrowException_WhenCredentialsAreInvalid()
    {
        // Arrange
        var email = "test@example.com";
        var password = "password123";
        
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.LoginAsync(email, password));
        Assert.Equal("Invalid credentials.", ex.Result.Message);
    }

    [Fact]
    public void LoginAsync_ShouldThrowException_WhenPasswordIsIncorrect()
    {
        // Arrange
        var email = "test@example.com";
        var correctPassword = "password123";
        var wrongPassword = "wrongPassword";
        var user = new User { UserId = Guid.NewGuid(), Email = email, Password = correctPassword };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _authService.LoginAsync(email, wrongPassword));
        Assert.Equal("Invalid credentials.", ex.Result.Message);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenUserRepositoryIsNull()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new AuthService(null, _configMock.Object));
        Assert.Equal("userRepository", ex.ParamName);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenConfigIsNull()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new AuthService(_userRepositoryMock.Object, null));
        Assert.Equal("config", ex.ParamName);
    }
}

