using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordResetController(IPasswordResetService service) : ControllerBase
{
    private readonly IPasswordResetService _service = service;

    [HttpPost("send-reset-email")]
    public async Task<IActionResult> SendResetEmail([FromBody] PasswordResetRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResponseResult { Succeeded = false, Error = "Invalid model" });

        var result = await _service.SendResetEmailAsync(request);
        return result.Succeeded 
            ? Accepted(result) 
            : BadRequest(result);
    }


    [HttpPost("send-confirmation-email")]
    public async Task<IActionResult> SendConfirmationEmail([FromBody] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new ResponseResult { Succeeded = false, Error = "Invalid email" });

        var result = await _service.SendSuccessEmailAsync(email);
        return result.Succeeded 
            ? Accepted(result) 
            : BadRequest(result);
    }
}
