using LogChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace LogChatService.DBContext;


public class LogDbContext(DbContextOptions<LogDbContext> options) : DbContext(options)
{
    public DbSet<ChatLog> Logs { get; set; }
    
}
