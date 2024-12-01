namespace Authentication.Services;

public class AuthResultDto
{

    public string Email { get; set; }
    public string Message { get; set; }
    public bool IsSucceeded { get; set; }
    public string Token { get; set; }
}