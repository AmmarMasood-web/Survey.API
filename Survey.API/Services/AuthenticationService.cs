using Microsoft.IdentityModel.Tokens;
using Survey.API.Interfaces;
using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyAPI.Services;
public class AuthenticationService : IAuthentication
{
    private readonly string tokenSecret;
    private readonly string clientId;
    public AuthenticationService(string tokenSecret, string clientId)
    {
        this.tokenSecret = tokenSecret;
        this.clientId = clientId;
    }

    public AuthenticationDTO generateToken(AuthenticationRequest authenticationRequest)
    {
        try
        {

            Log.Information("Generating Token");
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(tokenSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, clientId)
                }),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationDTO { Token = tokenHandler.WriteToken(token), code = "000", message = "success" };
        }
        catch (Exception ex)
        {
            Log.Error("Error occured in generateToken method ===> " + ex.Message);
            return null!;
        }
    }
}
