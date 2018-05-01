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
using Ektron.Cms;
using System.Text;
using Ektron.Newtonsoft.Json;
using Ektron.Newtonsoft.Json.Linq;

using CMS.Protection;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

using CMS.EmailEngine;
using CMS.MacroEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMSAppAppCode.SME;

public partial class pages_controls_hLdiscussions : System.Web.UI.UserControl
{
    public Boolean loggegedin = false;
    public int _maxresults = 4;
    public bool _aslist = false;
    string HLIAMKey = "2eb17c85-84d1-4888-b1b6-f5edf1a4396d";
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
       /* try
        {
            Ektron.Cms.UserAPI userApi = new UserAPI();
            if (userApi.IsLoggedIn && userApi.UserId > 0)
            {
                loggegedin = true;
            }

            ////Cache Implementation
 
            IEnumerable<RootObject> data = null;
            string CacheKey = "Aspen::Pages:Discussions";
           

                        string url = "https://api.connectedcommunity.org/api/v1.0/Discussions/GetLatestDiscussionPosts?maxToRetrieve={5}";
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Headers.Add("HLIAMKey", HLIAMKey);
                        httpWebRequest.Headers.Add("HLAuthToken", getToken());

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            var responseText = streamReader.ReadToEnd();
                            var j = JsonConvert.DeserializeObject<List<RootObject>>(responseText);
                            var top_three = (from person in j
                                             select person).Take(Maxresults);
                            data = top_three.OrderByDescending(x => x.DatePosted).ToList();
                         
                        }
                    
                
            #region "binding"
            if (AsList)
            {
                rptasList.DataSource = data;
                rptasList.DataBind();
                dataRepeater.Visible = false;
            }
            else
            {

                rptasList.Visible = false;
                dataRepeater.DataSource = data;
                dataRepeater.DataBind();
            }

            #endregion
        }
        catch (Exception ex)
        {
        }
*/
	// create a new instnace of the resolver
				MacroResolver resolver = new MacroResolver();
			
				// set all the available values within the template
				resolver.SetNamedSourceData("company", "sr2@gmail.com");
				resolver.SetNamedSourceData("contact", "Test");
				//resolver.SetNamedSourceData("eventstartdate", eventDateTime);
				//resolver.SetNamedSourceData("eventsubmissiontime", eventSubmissionDateTime);
				resolver.SetNamedSourceData("eventname", "testEvent Name");
        
        SendEmail("EventSubmission",resolver,"srayaprolu@networkats.com");
      }
    private void SendEmail(string templateName,MacroResolver resolver,string emailTo)
	{
				CMS.EmailEngine.EmailMessage msg = new CMS.EmailEngine.EmailMessage();
				EmailTemplateInfo emailTemplate = EmailTemplateProvider.GetEmailTemplate(templateName, SiteContext.CurrentSiteID);

				msg.EmailFormat = EmailFormatEnum.Both;
				msg.From = emailTemplate.TemplateFrom;
				msg.Recipients = emailTo;
							if(resolver!=null)
							{
				msg.Body = resolver.ResolveMacros(emailTemplate.TemplateText);
							}

				EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, msg, emailTemplate, null,true);
	}

    public string getToken()
    {

        string s = "";

        var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.connectedcommunity.org/api/v1.0/Authentication/Login");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";
        httpWebRequest.Headers.Add("HLIAMKey", HLIAMKey);
        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
         
            string json = "{\"username\":\"kral@smenet.org\"," +
            "\"password\":\"Password1\" , \"HLIAMKey\":\"2eb17c85-84d1-4888-b1b6-f5edf1a4396d\" }";

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

    public string GetURL(string NavigationUrl)
    {
        string url = NavigationUrl;//.Replace("http:","");
        if (!loggegedin)
        {
            url = "/memberlogin.aspx?returnurl=" + url;
        }
        else
        {
            if (url.Contains("?"))
                {
                    url = url + "&" + Session["usertoken"];
                }
                else
                {
                    url  = url + "?" + Session["usertoken"];
                }
              
        }
        return url;
    }
}

public class AuthorWrap
{
    public List<Author> Observations { get; set; }
}

public class Author
{
    public string LinkToProfile { get; set; }
    public string PictureUrl { get; set; }
    public string ContactKey { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public string ContactStatusCode { get; set; }
    public string PrefixCode { get; set; }
    public string UpdatedOn { get; set; }
    public string UpdatedByContactKey { get; set; }
    public string CompanyName { get; set; }
    public string CompanyTitle { get; set; }
    public string SuffixCode { get; set; }
    public string Designation { get; set; }
    public string LegacyContactKey { get; set; }
    public string InformalName { get; set; }
    public string MiddleName { get; set; }
    public bool IsCompany { get; set; }
    public string CompanyLegacyContactKey { get; set; }
    public bool AMSDirectoryOptOut { get; set; }
    public bool HLDirectoryOptOut { get; set; }
    public bool HLContactMeOptOut { get; set; }
    public string LargePictureUrl { get; set; }
}

public class RootObject
{
    public Author Author { get; set; }
    public string Body { get; set; }
    public string BodyWithoutMarkup { get; set; }
    public string ContactKey { get; set; }
    public string DatePosted { get; set; }
    public string LinkToDiscussion { get; set; }
    public string DiscussionName { get; set; }
    public string EmailAddress { get; set; }
    public string LinkToMessage { get; set; }
    public string DiscussionPostKey { get; set; }
    public string DiscussionKey { get; set; }
    public string MessageStatus { get; set; }
    public string ModerationType { get; set; }
    public bool Pinned { get; set; }
    public int Status { get; set; }
    public string Subject { get; set; }
}

//public class HLToken
//{

//    public string token { get; set; }


//}