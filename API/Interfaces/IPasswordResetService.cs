using API.Models;

namespace API.Interfaces;

public interface IPasswordResetService
{
    Task<ResponseResult> SendResetEmailAsync(PasswordResetRequest req);
    Task<ResponseResult> SendSuccessEmailAsync(string email);
}