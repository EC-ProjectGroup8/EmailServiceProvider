using API.Interfaces;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordResetController(IPasswordResetService service, ILogger<PasswordResetController> logger) : ControllerBase
{
    private readonly IPasswordResetService _service = service;
    private readonly ILogger<PasswordResetController> _logger = logger;

    [HttpPost("send-reset-email")]
    public async Task<IActionResult> SendResetEmail([FromBody] PasswordResetRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Password reset request was invalid. Email or URL was empty.");
            return BadRequest(new ResponseResult { Succeeded = false, Error = "Invalid model" });
        }


        _logger.LogInformation("Sending password reset email.");
        var result = await _service.SendResetEmailAsync(request);
        return result.Succeeded 
            ? Accepted(result) 
            : BadRequest(result);
    }


    [HttpPost("send-confirmation-email")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            _logger.LogInformation("Controller SendConfirmationEmail failed, email IsNullOrWhiteSpace.");
            return BadRequest(new ResponseResult { Succeeded = false, Error = "Invalid email" });
        }

        _logger.LogInformation("Sending password-changed confirmation email.");
        var result = await _service.SendSuccessEmailAsync(email);
        return result.Succeeded 
            ? Accepted(result) 
            : BadRequest(result);
    }
}
