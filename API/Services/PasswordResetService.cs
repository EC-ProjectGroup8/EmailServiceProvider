using Azure;
using Azure.Communication.Email;
using API.Interfaces;
using API.Models;

namespace API.Services;

public class PasswordResetService(EmailClient client, IConfiguration config) : IPasswordResetService
{
    private readonly EmailClient _client = client;
    private readonly string _senderAddress = config["ACS:SenderAddress"]!;

    public async Task<ResponseResult> SendResetEmailAsync(PasswordResetRequest req)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Url))
                return new ResponseResult { Succeeded = false, Error = "Invalid request" };

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

            return new ResponseResult { Succeeded = true, Message = "Accepted" };
        }
        catch (Exception ex)
        {
            return new ResponseResult { Succeeded = false, Error = ex.Message };
        }
    }

    public async Task<ResponseResult> SendSuccessEmailAsync(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
                return new ResponseResult { Succeeded = false, Error = "Invalid email" };

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
            return new ResponseResult { Succeeded = true, Message = "Accepted" };
        }
        catch (Exception ex)
        {
            return new ResponseResult { Succeeded = false, Error = ex.Message };
        }
    }
}
