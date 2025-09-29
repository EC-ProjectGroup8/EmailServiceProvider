namespace API.Models;

public class ResponseResult
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public string? Error { get; set; }
}   