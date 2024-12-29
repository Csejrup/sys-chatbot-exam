using Microsoft.AspNetCore.Mvc;

namespace ChatService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ChatController() : ControllerBase
{



    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        
        return Ok("WELCOME TO THE CHAT");
    }




}