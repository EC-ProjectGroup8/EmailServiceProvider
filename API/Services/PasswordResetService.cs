using Azure;
using Azure.Communication.Email;
using API.Interfaces;
using API.Models;

namespace API.Services;

public class PasswordResetService(EmailClient client, IConfiguration config, ILogger<PasswordResetService> logger) : IPasswordResetService
{
    private readonly EmailClient _client = client;
    private readonly string _senderAddress = config["ACS:SenderAddress"]!;
    private readonly ILogger<PasswordResetService> _logger = logger;


    public async Task<ResponseResult> SendResetEmailAsync(PasswordResetRequest req)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Url))
            {
                _logger.LogInformation("Password reset request was invalid. Email or URL was empty.");
                return new ResponseResult { Succeeded = false, Error = "Invalid request" };
            }

            _logger.LogInformation("Sending password reset email to {Email}", req.Email);

            var subject = "Reset your password";
            var html = $@"
                <p>Hello,</p>
                <p>Click the link below to reset your password:</p>
                <p><a href=""{req.Url}"">Reset password</a></p>
                <p>If you didn't request this, you can safely ignore this email.</p>
                <br>
                <p>Best regards, Core Gym Club Support</p>";

            var message = new EmailMessage(
            _senderAddress,
            new EmailRecipients([new(req.Email)]),
            new EmailContent(subject) { Html = html }
            );
            message.Recipients.To.Add(new EmailAddress(req.Email));

            var operation = await _client.SendAsync(WaitUntil.Completed, message);

            _logger.LogInformation("Password reset email sent successfully to {Email}", req.Email);
            return new ResponseResult { Succeeded = true, Message = "Accepted" };
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error sending password reset email to {Email}", req.Email);
            return new ResponseResult { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<ResponseResult> SendSuccessEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogInformation("Attempted to send password-changed confirmation email but email was empty.");
                return new ResponseResult { Succeeded = false, Error = "Invalid email" };
            }

            _logger.LogInformation("Sending password-changed confirmation email to {Email}", email);

            var subject = "Your password was changed";
            var html = $@" <p>Your password has been successfully changed. If this wasn't you, contact support immediately.</p>
                <br>
                <p>Best regards, Core Gym Club Support</p>";

            var message = new EmailMessage(
                _senderAddress,
                 new EmailRecipients([new(email)]),
                new EmailContent(subject) { Html = html }
            );

            var operation = await _client.SendAsync(WaitUntil.Completed, message);

            _logger.LogInformation("Password-changed confirmation email sent to {Email}", email);
            return new ResponseResult { Succeeded = true, Message = "Accepted" };
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "Error sending password-changed confirmation email to {Email}", email);
            return new ResponseResult { Succeeded = false, Error = ex.Message };
        }
    }
}
