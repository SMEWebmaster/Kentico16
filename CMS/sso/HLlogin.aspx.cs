using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
public partial class sso_HLlogin : System.Web.UI.Page
{
    public Boolean loggegedin = false;
    public int _maxresults = 4;
    public bool _aslist = false;
    string HLIAMKey = "194e4c0b-1dae-45c5-8d6e-749f12b21e8f";

    string username = "kral@smenet.org";
    string password = "Password1";
    #region"properties"
    public int Maxresults
    {
        get { return _maxresults; }
        set { _maxresults = value; }
    }
    public bool AsList
    {
        get { return _aslist; }
        set { _aslist = value; }
    }
    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write(getToken());
    }

    public string getToken()
    {

        string s = "";
        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.connectedcommunity.org/api/v2.0/Authentication/Login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("HLIAMKey", "194e4c0b-1dae-45c5-8d6e-749f12b21e8f");
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                // string json = "{\"username\":\"abarroc@tulane.edu\"," +
                //           "\"password\":\"1000168\" , \"HLIAMKey\": \"67EE6ECE-2247-47E3-8994-F85BCF62AB2F\"}";

                string json = "{\"username\":\"01786850\"," +
                                   "\"password\":\"Password1\" , \"HLIAMKey\": \"194e4c0b-1dae-45c5-8d6e-749f12b21e8f\"}";
                streamWriter.Write(json);
            }
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {

                s = streamReader.ReadToEnd();
            }
            var serializer2 = new JavaScriptSerializer();

            var persons = serializer2.Deserialize<List<HLToken>>(s);
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            var jsonObject12 = serializer.DeserializeObject(s);
            dynamic jsonObject = serializer.Deserialize<dynamic>(s);
            s = jsonObject["Token"];
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString());
        }
        return s;
    }

    public string getToken2()
    {

        string s = "";

        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.connectedcommunity.org/api/v1.0/Authentication/Login");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        httpWebRequest.Headers.Add("HLIAMKey", HLIAMKey);
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            /* string json = "{\"username\":"+ username  +"," +
                         "\"password\":"+ password +" , \"HLIAMKey\":"+HLIAMKey+"}";*/
            string json = "{\"username\":\"01786850\"," +
            "\"password\":\"Password1\" , \"HLIAMKey\":\"194e4c0b-1dae-45c5-8d6e-749f12b21e8f\" }";

          streamWriter.Write(json);
        }
        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {

            s = streamReader.ReadToEnd();
        }
        var serializer2 = new JavaScriptSerializer();

        var persons = serializer2.Deserialize<List<HLToken>>(s);
        var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
        var jsonObject12 = serializer.DeserializeObject(s);
        dynamic jsonObject = serializer.Deserialize<dynamic>(s);
        s = jsonObject["Token"];

        return s;
    }
}

public class HLToken
{

    public string token { get; set; }


}