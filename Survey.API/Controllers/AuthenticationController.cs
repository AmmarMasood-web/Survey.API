using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Survey.API.Classes;
using Survey.API.CLasses;
using Survey.API.Interfaces;
using Survey.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyAPI.API.Controllers;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/Authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthentication authentication;
    private readonly INanoIdInitializer nanoId;
    private readonly IConfiguration _configuration;
    public AuthenticationController(IAuthentication authentication, INanoIdInitializer nanoId, IConfiguration configuration)
    {
        this.nanoId = nanoId;
        this.authentication = authentication;
        _configuration = configuration;
    }

    [HttpPost]
    [Route("Login")]
    public IActionResult Login([FromBody] Login request)
    {
        try
        {
            Log.Debug("Generating Login Token with confServer with nano id: " + nanoId.ApplicationId);
            ConfServer confServer = new ConfServer(this._configuration);
            var configSettingSection = _configuration.GetSection("configServer");
            string PasswordKey = configSettingSection["EnKey"]!.ToString();
            request.Password = DESEncryption.Encrypt(request.Password, PasswordKey);
            if (confServer.ConnectionOpen(request.Username, request.Password))
            {
                if (confServer.RequestCampaignAccessXML(request.Username))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, request.Username),
                        // Add other claims as needed
                    };
                    var tokenConfig = _configuration.GetSection("tokenConfiguration");
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfig.GetValue<string>("tokenSecret")!));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        tokenConfig.GetValue<string>("Issuer"),
                        tokenConfig.GetValue<string>("Audience"),
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: creds
                    );
                    Log.Debug("{0} :", nanoId.ApplicationId + "Successfully Signed in ConfServer Sending token... ");
                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
            }
            Log.Debug("Failure ...");
            return StatusCode(500, "Error Username or Password is invalid");
        }
        catch (Exception ex)
        {
            Log.Error("{0} :", nanoId.ApplicationId + "Error occured in authentication controller ===> " + ex.Message);
            return null!;
        }
    }

    //[HttpPost]
    //public IActionResult generateToken([FromBody] AuthenticationRequest request)
    //{
    //    try
    //    {
    //        Log.Debug("Generate Token Method with nano id: " + nanoId.ApplicationId);
    //        Log.Information("{0} :", nanoId.ApplicationId + "Token Request body : " + JsonConvert.SerializeObject(request));
    //        var token = this.authentication.generateToken(request);
    //        if (token != null)
    //        {
    //            Log.Information("{0} :", nanoId.ApplicationId + "Token Response body : " + JsonConvert.SerializeObject(token));
    //            return Ok(token);
    //        }
    //        else
    //        {
    //            return Unauthorized();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Log.Error("{0} :", nanoId.ApplicationId + "Error occured in authentication controller ===> " + ex.Message);
    //        return null!;
    //    }
    //}
}
