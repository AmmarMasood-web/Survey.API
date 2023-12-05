using Genesyslab.Platform.Configuration.Protocols;
using Survey.API.CLasses;
using Endpoint = Genesyslab.Platform.Commons.Protocols.Endpoint;

namespace Survey.API.Classes;

using Microsoft.Extensions.Configuration;
using System;

public class ConfServer
{
    private readonly ConfServerProtocol _protocol;
    private readonly string _ocServerURI;
    private readonly int _sessionId;
    private readonly string _passKey;
    private readonly string _app_name;

    public ConfServer(IConfiguration configuration)
    {
        var confSettings = configuration.GetSection("configServer");
        _protocol = new ConfServerProtocol(new Endpoint("confserv", confSettings["ConfigServer_IP"], Int16.Parse(confSettings["ConfigServer_PORT"]!)));
        _ocServerURI = confSettings["OCServerURI"]!;
        _sessionId = 0;
        _passKey = confSettings["EnKey"]!;
        _app_name = confSettings["Application_Name"]!;
    }

    public bool ConnectionOpen(string username, string password)
    {
        Log.Information("Confconnection Open Request");
        try
        {
            password = DESEncryption.Decrypt(password, _passKey);
            _protocol.ClientName = _app_name;
            _protocol.UserName = username;
            _protocol.UserPassword = password;
            _protocol.Open();
            Log.Information("Confconnection Open Successfully");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error($"Error In Confconnection Open Method: {ex}");
            return false;
        }
    }
}
