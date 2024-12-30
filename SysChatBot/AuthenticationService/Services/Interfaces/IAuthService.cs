namespace AuthenticationService.Services.Interfaces;

public interface IAuthService
{
    
    Task<string> SignupAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}