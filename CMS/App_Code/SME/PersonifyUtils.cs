using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Data.Services.Client;
using System.Net;
using System.Text;
using Personify.WebControls.Base.PersonifyDataServicesBase;
using Personify.WebControls;

using System.Collections.Generic;
public class PersonifyUtils
{
    private static readonly object ParametersSyncLock = new object();
    private static readonly string SUri = ConfigurationManager.AppSettings["svcUri_Base"];
    private static readonly string UserName = ConfigurationManager.AppSettings["svcLogin"];
    private static readonly string Password = ConfigurationManager.AppSettings["svcPassword"];
    private static List<WebControlParameter> _webControlParameters = null;

    public static List<WebControlParameter> WebControlParameters
    {
        get
        {
            lock (ParametersSyncLock)
            {
                if (_webControlParameters == null)
                {
                    Uri ServiceUri = new Uri(SUri);
                    PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                    DataAccessLayer.Credentials = new NetworkCredential(UserName, Password);
                    _webControlParameters = DataAccessLayer.WebControlParameters.ToList();
                    //IQueryable<WebControlParameter> webControlParameters = DataAccessLayer.WebControlParameters.Select(o => o);
                    //IQueryable<WebControlParameter> webControlParameters = Personify.WebControls.Base.Utilities.SvcClient.Client.Context.WebControlParameters.Select(o => o);
                    // IQueryable<WebControlParameter> webControlParameters = Personify.WebControls.Base.Utilities.SvcClient.Client.Context.WebControlParameters.Select(o => o);
                    //var parameters = new DataServiceCollection<WebControlParameter>(webControlParameters);
                    //_webControlParameters = from w in parameters orderby w.ParameterName select w;
                }
            }
            return _webControlParameters;
        }
    }

    public static string DoPost(string serviceOperation, string content)
    {
        string result = null;
        var req = (HttpWebRequest)WebRequest.Create(SUri.TrimEnd('/') + "/" + serviceOperation);
        var serviceCreds = new NetworkCredential(UserName, Password);
        var cache = new CredentialCache();
        var svcUri = new Uri(SUri);
        cache.Add(svcUri, "Basic", serviceCreds);
        req.Credentials = cache;
        req.Method = "POST";
        req.ContentType = "application/xml;charset=utf-8";
        req.Timeout = 1000 * 60 * 15; // 15 minutes

        if (!string.IsNullOrEmpty(content))
        {
            byte[] arr = Encoding.ASCII.GetBytes(content);
            req.ContentLength = arr.Length;
            Stream reqStrm = req.GetRequestStream();
            reqStrm.Write(arr, 0, arr.Length);
            reqStrm.Close();
        }

        try
        {
            var resp = (HttpWebResponse)req.GetResponse();
            if (resp.CharacterSet != null)
            {
                Encoding responseEncoding = Encoding.GetEncoding(resp.CharacterSet);
                var respStr = resp.GetResponseStream();
                if (respStr != null)
                {
                    using (var sr = new StreamReader(respStr, responseEncoding))
                    {
                        result = sr.ReadToEnd();
                    }
                }
            }

            return result;
        }
        catch (WebException wex)
        {
            throw wex;
        }
    }
}