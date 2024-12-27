
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Requests;
using AuthenticationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{

    private readonly IAuthService _authService = authService;


    [HttpPost("authenticate")]
  public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
  {

      var token = _authService.GenerateToken();
      return Ok(token);
  }
  
  
  

}