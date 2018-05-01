using Personify.DataServices.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SMETests
{
    /// <summary>
    /// https://resource1.personifycorp.com/PersonifyOnlineHelp/7.6.0/mergedProjects/Data%20Services/Executing_Service_Operations_(using_.NET_client).htm 
    /// Test client code pulled from personify online documentation.  Refactor and place inside main solution.
    /// </summary>
    public static class SvcClient
    {

        static string sUri = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];
        static string EnableBasicAuthentication = System.Configuration.ConfigurationManager.AppSettings["EnableBasicAuthentication"];
        static string UserName = System.Configuration.ConfigurationManager.AppSettings["svcLogin"];
        static string Password = System.Configuration.ConfigurationManager.AppSettings["svcPassword"];
        static Uri svcUri = new Uri(sUri);

        #region Helpers


        private static PersonifyData.PersonifyEntitiesBase ctxt;
        public static PersonifyData.PersonifyEntitiesBase Ctxt
        {
            get
            {
                if (ctxt == null)
                {
                    ctxt = new PersonifyData.PersonifyEntitiesBase(svcUri);

                    //enable authentication if necessary
                    if (Convert.ToBoolean(EnableBasicAuthentication) == true)
                    {
                        var serviceCreds = new NetworkCredential(UserName, Password);
                        var cache = new CredentialCache();
                        cache.Add(svcUri, "Basic", serviceCreds);
                        ctxt.Credentials = cache;
                    }
                    ctxt.IgnoreResourceNotFoundException = true;
                }
                return ctxt;
            }
        }

        private static PersonifyData.PersonifyEntitiesBase ctxtAnonymous;
        public static PersonifyData.PersonifyEntitiesBase CtxtAnonymous
        {
            get
            {
                if (ctxtAnonymous == null)
                {
                    ctxtAnonymous = new PersonifyData.PersonifyEntitiesBase(svcUri);
                    ctxtAnonymous.IgnoreResourceNotFoundException = true;
                }
                return ctxtAnonymous;
            }
        }

        public static ReturnType Post<ReturnType>(string SvcOperName, object o)
        {
            return DoPost<ReturnType>(SvcOperName, o, true);
        }

        public static ReturnType PostAnonymous<ReturnType>(string SvcOperName, object o)
        {
            return DoPost<ReturnType>(SvcOperName, o, false);
        }

        private static ReturnType DoPost<ReturnType>(string SvcOperName, object o, bool enableAuthentication)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUri.TrimEnd('/') + "/" + SvcOperName);
            if (enableAuthentication)
            {
                NetworkCredential serviceCreds = new NetworkCredential(UserName, Password);
                CredentialCache cache = new CredentialCache();
                Uri uri = new Uri(sUri);
                cache.Add(uri, "Basic", serviceCreds);
                req.Credentials = cache;
            }

            req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.ContentType = "application/xml";
            req.Timeout = 1000 * 60 * 15; // 15 minutes

            byte[] arr = o.ToSerializedByteArrayUTF8();
            req.ContentLength = arr.Length;
            Stream reqStrm = req.GetRequestStream();
            reqStrm.Write(arr, 0, arr.Length);
            reqStrm.Close();

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                var stream = resp.GetResponseStream();

                if (stream.CanRead)
                {
                    XDocument doc = XDocument.Load(stream);
                    resp.Close();
                    ReturnType oResp = doc.Root.ToString().ToBusinessEntity<ReturnType>(SourceFormatEnum.XML);
                    return oResp;
                }
                else
                {
                    return default(ReturnType);
                }
            }
            catch (WebException wex)
            {
                throw DataServiceExceptionUtil.ParseException(wex);
            }
        }

        public static ReturnType Create<ReturnType>()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                string.Format("{0}/Create?EntityName='{1}'",
                    sUri.TrimEnd('/'),
                    typeof(ReturnType).Name)
                    );
            NetworkCredential serviceCreds = new NetworkCredential(UserName, Password);
            CredentialCache cache = new CredentialCache();
            cache.Add(new Uri(sUri), "Basic", serviceCreds);

            req.Credentials = cache;
            req.Method = "GET";
            req.ContentType = "application/xml";
            req.Timeout = 1000 * 60 * 15; // 15 minutes

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                XDocument doc = XDocument.Load(resp.GetResponseStream());
                resp.Close();
                ReturnType oEntity = doc.Root.ToString().ToBusinessEntity<ReturnType>(SourceFormatEnum.XML);
                return oEntity;
            }
            catch (WebException wex)
            {
                throw Personify.DataServices.Serialization.DataServiceExceptionUtil.ParseException(wex);
            }
        }

        public static ReturnType Save<ReturnType>(object entityToSave)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                string.Format("{0}/Save?EntityName='{1}'",
                    sUri.TrimEnd('/'),
                    typeof(ReturnType).Name)
                    );
            NetworkCredential serviceCreds = new NetworkCredential(UserName, Password);
            CredentialCache cache = new CredentialCache();
            Uri uri = new Uri(sUri);
            cache.Add(uri, "Basic", serviceCreds);

            req.Credentials = cache;
            req.Method = "POST";
            req.ContentType = "application/xml";
            req.Timeout = 1000 * 60 * 15; // 15 minutes

            byte[] arr = entityToSave.ToSerializedByteArrayUTF8();
            req.ContentLength = arr.Length;
            Stream reqStrm = req.GetRequestStream();
            reqStrm.Write(arr, 0, arr.Length);
            reqStrm.Close();

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                XDocument doc = XDocument.Load(resp.GetResponseStream());
                resp.Close();
                ReturnType oResp = doc.Root.ToString().ToBusinessEntity<ReturnType>(SourceFormatEnum.XML);
                return oResp;
            }
            catch (WebException wex)
            {
                throw Personify.DataServices.Serialization.DataServiceExceptionUtil.ParseException(wex);
            }
        }

        public static ReturnType Delete<ReturnType>(object entityToDelete)
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
                string.Format("{0}/Delete?EntityName='{1}'",
                    sUri.TrimEnd('/'),
                    typeof(ReturnType).Name)
                    );
            NetworkCredential serviceCreds = new NetworkCredential(UserName, Password);
            CredentialCache cache = new CredentialCache();
            Uri uri = new Uri(sUri);
            cache.Add(uri, "Basic", serviceCreds);

            req.Credentials = cache;
            req.Method = "POST";
            req.ContentType = "application/xml";
            req.Timeout = 1000 * 60 * 15; // 15 minutes

            byte[] arr = entityToDelete.ToSerializedByteArrayUTF8();
            req.ContentLength = arr.Length;
            Stream reqStrm = req.GetRequestStream();
            reqStrm.Write(arr, 0, arr.Length);
            reqStrm.Close();

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                XDocument doc = XDocument.Load(resp.GetResponseStream());
                resp.Close();
                ReturnType oResp = doc.Root.ToString().ToBusinessEntity<ReturnType>(SourceFormatEnum.XML);
                return oResp;
            }
            catch (WebException wex)
            {
                throw Personify.DataServices.Serialization.DataServiceExceptionUtil.ParseException(wex);
            }
        }

        public static string FileUpload(byte[] fileContent, string TargetFileName)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sUri.TrimEnd('/') + "/FileUpload?fileName='" + TargetFileName + "'");
            NetworkCredential serviceCreds = new NetworkCredential(UserName, Password);
            CredentialCache cache = new CredentialCache();
            Uri uri = new Uri(sUri);
            cache.Add(uri, "Basic", serviceCreds);
            req.Credentials = cache;
            req.Method = "POST";
            req.Timeout = 1000 * 60 * 20; // 20 minutes
            req.SendChunked = true;

            req.ContentLength = fileContent.Length;
            Stream reqStrm = req.GetRequestStream();
            reqStrm.Write(fileContent, 0, fileContent.Length);
            reqStrm.Close();

            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                XDocument doc = XDocument.Load(resp.GetResponseStream());
                resp.Close();
                return doc.Root.Value;
            }
            catch (WebException wex)
            {
                throw Personify.DataServices.Serialization.DataServiceExceptionUtil.ParseException(wex);
            }


        }

        #endregion

    }






    public static class SerializationUtils
    {
        public static TargetType ToBusinessEntity<TargetType>(this string source)
        {
            XmlSerializer x = new XmlSerializer(typeof(TargetType));
            return (TargetType)x.Deserialize(new StringReader(source));
        }

        public static byte[] ToSerializedByteArrayUTF8(this object o)
        {
            if (o == null)
            {
                return null;
            }
            else
            {
                return Encoding.UTF8.GetBytes(o.ToSerializedXml());
            }
        }

        public static string ToSerializedXml(this object o)
        {
            if (o == null)
            {
                return null;
            }
            XmlSerializer x = new XmlSerializer(o.GetType());
            StringWriter sw = new StringWriter();
            x.Serialize(sw, o);
            return sw.ToString();
        }
    }

    public class DataServiceClientException : Exception
    {

        public DataServiceClientException(string Message, string StackTrace, Exception InternalException)
            : base(Message, InternalException)
        {

            _stackTrace = StackTrace;

        }
        private string _stackTrace;
        public override string StackTrace
        {
            get
            {
                return _stackTrace;
            }
        }
    }

    public static class DataServiceExceptionUtil
    {
        public static DataServiceClientException ParseException(WebException wex)
        {
            HttpWebResponse respEx = (HttpWebResponse)wex.Response;
            XDocument doc = XDocument.Load(respEx.GetResponseStream());
            return ParseExceptionRecursive(doc.Root);
        }
        private static DataServiceClientException ParseExceptionRecursive(XElement errorElement)
        {

            string DataServicesMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            XName xnCode = XName.Get("code", DataServicesMetadataNamespace);
            XName xnType = XName.Get("type", DataServicesMetadataNamespace);
            XName xnMessage = XName.Get("message", DataServicesMetadataNamespace);
            XName xnStackTrace = XName.Get("stacktrace", DataServicesMetadataNamespace);
            XName xnInternalException = XName.Get("internalexception", DataServicesMetadataNamespace);
            XName xnInnerError = XName.Get("innererror", DataServicesMetadataNamespace);

            switch (errorElement.Name.LocalName)
            {
                case "error":
                case "innererror":
                case "internalexception":
                    DataServiceClientException internalException2 =
                                errorElement.Element(xnInternalException) != null ?
                                            ParseExceptionRecursive(errorElement.Element(xnInternalException)) : null;
                    if (internalException2 != null) return internalException2;
                    DataServiceClientException internalException =
                                errorElement.Element(xnInnerError) != null ?
                                            ParseExceptionRecursive(errorElement.Element(xnInnerError)) : null;
                    if (internalException != null) return internalException;
                    string code = errorElement.Element(xnCode) != null ?
                                            errorElement.Element(xnCode).Value.ToString() : String.Empty;
                    string message = errorElement.Element(xnMessage) != null ?
                                            errorElement.Element(xnMessage).Value.ToString() : String.Empty;
                    string stackTrace = errorElement.Element(xnStackTrace) != null ?
                                            errorElement.Element(xnStackTrace).Value.ToString() : String.Empty;
                    return new DataServiceClientException(
                        message,
                        stackTrace,
                        (internalException == null ? internalException2 : internalException)
                        );

                default:
                    throw new InvalidOperationException("Could not parse WebException to DataServiceClientException");
            }
        }
    }
}
