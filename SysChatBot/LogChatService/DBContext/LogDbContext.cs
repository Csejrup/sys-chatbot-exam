using LogChatService.Models;
using Microsoft.EntityFrameworkCore;

namespace LogChatService.DBContext;


public class LogDbContext : DbContext
{
    public LogDbContext(DbContextOptions<LogDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatLog> Logs { get; set; }
    
}
