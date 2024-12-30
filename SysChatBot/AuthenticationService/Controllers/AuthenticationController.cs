
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

    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
      return BadRequest("Invalid data.");
    }

    try
    {
      var token = await _authService.LoginAsync(request.Email, request.Password);
      return Ok(new { Token = token });
    }
    catch (Exception ex)
    {
      return Unauthorized(ex.Message);
    }

  }

  [HttpPost("signup")]
  public async Task<IActionResult> Signup([FromBody] SignupRequest request)
  {
    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
      return BadRequest("Invalid data.");
    }

    try
    {
      var token = await _authService.SignupAsync(request.Email, request.Password);
      return Ok(new
      {
        Token = token
      });
    }
    catch (Exception ex)
    {
      return BadRequest(ex.Message);
    }
  }





}