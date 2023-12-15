namespace Survey.API.ResponseDTOs;
public class AuthenticationDTO
{
    public string Token { get; set; } = string.Empty;
    public string code { get; set; } = "001";
    public string message { get; set; } = "failure";
}
public class Logout
{
    public required string message { get; set; } = "failure";
}
