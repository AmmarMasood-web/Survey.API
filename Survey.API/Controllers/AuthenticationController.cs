using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Survey.API.Interfaces;
using Survey.API.RequestDTOs;

namespace SurveyAPI.API.Controllers;

[AllowAnonymous]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthentication authentication;
    private readonly INanoIdInitializer nanoId;
    public AuthenticationController(IAuthentication authentication, INanoIdInitializer nanoId)
    {
        this.nanoId = nanoId;
        this.authentication = authentication;
    }

    //[HttpPost]
    //public IActionResult Login([FromBody] Login model)
    //{
    //    try
    //    {
    //        ConfigurationServer confServer = new ConfigurationServer();
    //        string passwordKey = StaticConfigurationManager.AppSettings["EnKey"]!.ToString();
    //        model.Password = DESEncryption.Encrypt(model.Password, passwordKey);
    //        if (confServer.connectionOpen(model.Username, model.Password))
    //        {
    //        }
    //        return Ok();
    //    }
    //    catch (Exception ex)
    //    {
    //        Log.Error("{0} :", nanoId.ApplicationId + "Error occured in authentication controller ===> " + ex.Message);
    //        return null!;
    //    }
    //}

    [HttpPost]
    public IActionResult generateToken([FromBody] AuthenticationRequest request)
    {
        try
        {
            Log.Debug("Generate Token Method with nano id: " + nanoId.ApplicationId);
            Log.Information("{0} :", nanoId.ApplicationId + "Token Request body : " + JsonConvert.SerializeObject(request));
            var token = this.authentication.generateToken(request);
            if (token != null)
            {
                Log.Information("{0} :", nanoId.ApplicationId + "Token Response body : " + JsonConvert.SerializeObject(token));
                return Ok(token);
            }
            else
            {
                return Unauthorized();
            }
        }
        catch (Exception ex)
        {
            Log.Error("{0} :", nanoId.ApplicationId + "Error occured in authentication controller ===> " + ex.Message);
            return null!;
        }

    }
}
