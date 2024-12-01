namespace Authentication;

public class JwtSettings
{
    public string key { get; set; }
    public string issuer { get; set; }
    public string audience { get; set; }
    public int durationInMinutes { get; set; }
}