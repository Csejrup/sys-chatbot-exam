using AuthenticationService.Models;

namespace AuthenticationService.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<Guid> CreateUserAsync(User user);
    Task DeleteUserAsync(Guid userId);
}