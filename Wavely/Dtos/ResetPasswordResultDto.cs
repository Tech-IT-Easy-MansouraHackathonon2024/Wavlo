using Microsoft.AspNetCore.Http.HttpResults;

namespace Authentication.Services;

public class ResetPasswordResultDto
{
    public string Message { get; set; }
    public bool IsSucceeded { get; set; }
}