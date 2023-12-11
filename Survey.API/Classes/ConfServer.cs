using Genesyslab.Platform.Configuration.Protocols;
using Survey.API.CLasses;

namespace Survey.API.Classes;

using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Events;
using Genesyslab.Platform.Configuration.Protocols.ConfServer.Requests.Objects;
using Genesyslab.Platform.Configuration.Protocols.Types;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Xml;
using System.Xml.Linq;

public class ConfServer
{
    protected ConfServerProtocol _protocol;
    private string _ocServerURI;
    private int _sessionId;
    private string _passKey;
    private string _app_name;

    public ConfServer(IConfiguration configuration)
    {
        var confSettings = configuration.GetSection("configServer");
        _protocol = new ConfServerProtocol(new Endpoint("confserv", (confSettings["ConfigServer_IP"])!.ToString(), Int16.Parse(confSettings["ConfigServer_PORT"]!)));
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
    public bool RequestCampaignAccessXML(string username)
    {
        try
        {
            string DBID = RequestCFGPersonDBID(username);
            Log.Information("RequestCampaignAccessXML");
            //string rolename = ConfigurationManager.AppSettings["RoleName"].ToString();
            /// to do
            string rolename = "API_Auth";
            KeyValueCollection filter = new KeyValueCollection();
            filter.Add("name", rolename);
            RequestReadObjects requestReadPerson = RequestReadObjects.Create((int)CfgObjectType.CFGRole, filter);
            EventObjectsRead response = (EventObjectsRead)_protocol.Request(requestReadPerson);
            XDocument result = (XDocument)response.ConfObject;
            XmlReader reader = result.CreateReader();

            var newXmlDocument = ToXmlDocument(result);
            string json = JsonConvert.SerializeXmlNode(newXmlDocument);
            JObject jsonObj = JObject.Parse(json);
            bool loginSuccessful = false;

            var objectTypeKey = new JObject();
            var objectDBIDKey = new JObject();

            #region JsonReader
            var members = (JObject)JsonConvert.DeserializeObject(jsonObj["ConfData"]["CfgRole"]["members"].ToString());
            if (members["CfgRoleMember"].Type.ToString() == "Array")
            {
                foreach (var member in members["CfgRoleMember"])
                {
                    objectTypeKey = (JObject)JsonConvert.DeserializeObject(member["objectType"].ToString());
                    if (objectTypeKey["@value"].Value<string>().Trim() == "21")
                    {
                        objectDBIDKey = (JObject)JsonConvert.DeserializeObject(member["objectDBID"].ToString());
                        if (RequestAccessGroup(int.Parse(objectDBIDKey["@value"].Value<string>().Trim()), DBID))
                            return true;
                    }
                    else if (objectTypeKey["@value"].Value<string>().Trim() == "3")
                    {
                        objectDBIDKey = (JObject)JsonConvert.DeserializeObject(member["objectDBID"].ToString());
                        if (objectDBIDKey["@value"].Value<string>().Trim().ToString() == DBID)
                            return true;
                    }
                    objectTypeKey = new JObject();
                    objectDBIDKey = new JObject();
                }
            }
            else
            {
                objectTypeKey = (JObject)JsonConvert.DeserializeObject(members["CfgRoleMember"]["objectType"].ToString());
                if (objectTypeKey["@value"].Value<string>().Trim() == "21")
                {
                    objectDBIDKey = (JObject)JsonConvert.DeserializeObject(members["CfgRoleMember"]["objectDBID"].ToString());
                    if (RequestAccessGroup(int.Parse(objectDBIDKey["@value"].Value<string>().Trim()), DBID))
                        return true;
                }
                else if (objectTypeKey["@value"].Value<string>().Trim() == "3")
                {
                    objectDBIDKey = (JObject)JsonConvert.DeserializeObject(members["CfgRoleMember"]["objectDBID"].ToString());
                    if (objectDBIDKey["@value"].Value<string>().Trim().ToString() == DBID)
                        return true;
                }
            }
            #endregion
            Log.Information("UserID: " + DBID + " Not Found In Role: " + rolename);
            return loginSuccessful;
        }
        catch (Exception ex)
        {
            Log.Error("RequestCampaignAccessXML Error: " + ex.ToString());
            return false;
        }
    }
    private string RequestCFGPersonDBID(string username)
    {
        try
        {
            Log.Information("RequestCFGPersonDBID");
            KeyValueCollection filter = new KeyValueCollection();
            filter.Add("user_name", username);
            RequestReadObjects requestReadPerson = RequestReadObjects.Create((int)CfgObjectType.CFGPerson, filter);
            EventObjectsRead response = (EventObjectsRead)_protocol.Request(requestReadPerson);
            XDocument result = (XDocument)response.ConfObject;

            var newXmlDocument = ToXmlDocument(result);
            string json = JsonConvert.SerializeXmlNode(newXmlDocument);
            JObject jsonObj = JObject.Parse(json);
            string DBIDValue = string.Empty;

            var DBID = (JObject)JsonConvert.DeserializeObject(jsonObj["ConfData"]["CfgPerson"]["DBID"].ToString());
            DBIDValue = DBID["@value"].Value<string>().Trim();

            return DBIDValue;
        }
        catch (Exception ex)
        {
            Log.Error("RequestCFGPersonDBID Error: " + ex.ToString());
            return null;
        }
    }
    private XmlDocument ToXmlDocument(XDocument xDocument)
    {
        var xmlDocument = new XmlDocument();
        using (var xmlReader = xDocument.CreateReader())
        {
            xmlDocument.Load(xmlReader);
        }
        return xmlDocument;
    }

    private bool RequestAccessGroup(int GroupDBID, string UserId)
    {
        try
        {
            Log.Information("RequestCFGPersonDBID");
            KeyValueCollection filter = new KeyValueCollection();
            filter.Add("dbid", GroupDBID);
            RequestReadObjects requestReadPerson = RequestReadObjects.Create((int)CfgObjectType.CFGAccessGroup, filter);
            EventObjectsRead response = (EventObjectsRead)_protocol.Request(requestReadPerson);
            XDocument result = (XDocument)response.ConfObject;
            bool userFound = false;

            var newXmlDocument = ToXmlDocument(result);
            string json = JsonConvert.SerializeXmlNode(newXmlDocument);
            JObject jsonObj = JObject.Parse(json);

            var memberIDs = (JObject)JsonConvert.DeserializeObject(jsonObj["ConfData"]["CfgAccessGroup"]["memberIDs"].ToString());
            if (memberIDs["CfgID"].Type.ToString() == "Array")
            {
                foreach (var memberId in memberIDs["CfgID"])
                {
                    var UserIdKey = (JObject)JsonConvert.DeserializeObject(memberId["DBID"].ToString());
                    if (UserIdKey["@value"].Value<string>().Trim().ToString() == UserId)
                        return true;
                }
            }
            else
            {
                var UserIdKey = (JObject)JsonConvert.DeserializeObject(memberIDs["CfgID"]["DBID"].ToString());
                if (UserIdKey["@value"].Value<string>().Trim().ToString() == UserId)
                    return true;
            }
            Log.Information("UserID: " + UserId + " Not Found In any AccessGroup");
            return userFound;
        }
        catch (Exception ex)
        {
            Log.Error("RequestCFGPersonDBID Error: " + ex.ToString());
            return false;
        }
    }

}
