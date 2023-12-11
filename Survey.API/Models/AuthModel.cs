namespace Survey.API.Models;

public class AuthModel
{
    public required string accessToken { get; set; }
    public DateTime validTill { get; set; }
}

public class Login
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
