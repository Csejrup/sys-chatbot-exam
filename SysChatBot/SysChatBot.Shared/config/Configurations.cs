namespace SysChatBot.Shared.config;

public class Configurations
{ 
        public Authentication? Authentication { get; set; }
        public string DbConnectionString { get; set; }
}

public class Authentication
{
        public string? JwtIssuer { get; set; }
        public string? JwtKey { get; set; }
}