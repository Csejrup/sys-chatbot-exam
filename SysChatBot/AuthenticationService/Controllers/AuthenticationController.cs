
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Requests;
using AuthenticationService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class AuthenticationController(IAuthService authService) : ControllerBase
{

  private readonly IAuthService _authService = authService;


  [HttpPost("authenticate")]
  public async Task<IActionResult> Authenticate([FromBody] AuthenticationRequest request)
  {

    // TODO: Validate user via db and get UserID 
    var userId = Guid.NewGuid();
    var token = _authService.GenerateToken(userId);
    return Ok(token);
  }




}