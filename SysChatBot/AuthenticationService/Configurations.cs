namespace AuthenticationService;

public class Configurations
{
 
        public Authentication Authentication { get; set; }
}



public class Authentication
{
        public string JwtIssuer { get; set; }
        public string JwtKey { get; set; } 
}