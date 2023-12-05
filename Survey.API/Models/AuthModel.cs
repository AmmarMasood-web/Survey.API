namespace Survey.API.Models;

public class AuthModel
{
    public required string accessToken { get; set; }
    public DateTime validTill { get; set; }
}
//Configuration Server Genesys
public class Login
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
