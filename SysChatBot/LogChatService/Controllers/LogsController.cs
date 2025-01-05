using LogChatService.Services.Logs;
using Microsoft.AspNetCore.Mvc;

namespace LogChatService.Controllers;



[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{

    private readonly ILogService _logService;

    public LogsController(ILogService logService)
    {
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
    }

    

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetLogsByUserId()
    {
        var userId = Request.Headers["userId"].ToString(); // Get the userId from the headers

        var logs = await _logService.GetAllChatLogsByUserIdAsync(Guid.Parse(userId));
        return Ok(logs);
    }


}