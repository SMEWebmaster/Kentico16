using System;
using System.Web;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Protection;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Personify.DataServices.Serialization;
using System.Xml.Linq;
using System.Data.Services.Client;
using personifyDataservice;
using System.Linq;
using System.Configuration;
using SSO;
using System.Collections.Generic;
using IMS;

public partial class SSOHandler : System.Web.UI.Page
{

    public bool DoLogout
    {
        get
        {
            var doLogout = false;

            if (Request.QueryString[DoLogoutQSKey] != null)
            {
                bool.TryParse(Request.QueryString[DoLogoutQSKey], out doLogout);
            }

            return doLogout;
        }
    }

    private const string DoLogoutQSKey = "logout";
    private const string PersonifySessionKey = "PersonifyToken";
    private const string SSOTokenCookie = "SSOToken";
    private const string UserNameSessionKey = "UserName";
    private const string PasswordSessionKey = "Password";
    private const string RetryCountSessionKey = "RetryCount";
    private const string RememberMeSessionKey = "RememberMe";
    private const string ReturnURLSessionKey = "ReturnURL";

    private readonly string _personifyBaseURN = ConfigurationManager.AppSettings["PersonifyBaseURN"];
    private readonly string _loginURL = ConfigurationManager.AppSettings["LoginURL"];
    private readonly string _loginErrorUrl = ConfigurationManager.AppSettings["LoginErrorURL"];
    private readonly string _personifyLogoutURL = ConfigurationManager.AppSettings["Personify_LogoutURL"];
    private readonly string _personifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
    private readonly string _personifyVendorID = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
    private readonly string _personifyImsUrl = ConfigurationManager.AppSettings["IMSWebReferenceURL"];
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];

    private SSOClient ssoClient = new SSOClient();


    /// <summary>
    /// Logic to handle SSO sign in/out and user reauthorization
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Did user log out?
        CheckForLogout();

        // Do we need to reauthorize user?
        ReAuthorizeCheck();

        Session[PersonifySessionKey] = null;

        // Check for empty session values
        if (Session[UserNameSessionKey] == null || (string)Session[PasswordSessionKey] == null)
        {
            ReAuthorizeFail();
        }

        var username = (string)Session[UserNameSessionKey];
        var password = (string)Session[PasswordSessionKey];

        // Logic to handle retry
        if (Session[RetryCountSessionKey] == null)
        {
            Session[RetryCountSessionKey] = 0;
        }
        else if ((int)Session[RetryCountSessionKey] == 5)
        {
            Session[RetryCountSessionKey] = null;
            EventLogProvider.LogException("SSOHandler", "Retry", new Exception("Cannot resolve username and password with autologin.  Exceeded retry limit."), SiteContext.CurrentSiteID);
            URLHelper.Redirect(_loginErrorUrl);
        }
        else
        {
            var current = (int)Session[RetryCountSessionKey] + 1;

            Session[RetryCountSessionKey] = current;
        }

        // Get customer from Personify
        var ssoCustomer = ssoClient.SSOCustomerGetByUsername(_personifySsoVendorName, _personifySsoVendorPassword, username);

        // Check if Customer exists in Personify
        if (ssoCustomer == null)
        {
            EventLogProvider.LogException("SSOHandler", "LookupCustomer", new Exception("ssoCustomer does not exist for given username."), SiteContext.CurrentSiteID);
            URLHelper.Redirect(_loginErrorUrl);
            //throw new Exception("ssoCustomer does not exist for given username.");
        }

        // Get Token from Personify Request
        var customerToken = Request.QueryString["ct"];
        var decryptedToken = String.Empty;

        var rememberMe = Session[RememberMeSessionKey] != null ? (bool)Session[RememberMeSessionKey] : false;

        // If decrypted token is not empty and valid, then proceed to log in        
        if (!string.IsNullOrEmpty(customerToken) && isValidToken(decryptedToken = DecryptCustomerToken(customerToken)))
        {
            Session[PersonifySessionKey] = decryptedToken;

            // Verify Kentico User
            VerifyKenticoUser(decryptedToken, username);

            // Log in to Kentico
            AuthenticationHelper.AuthenticateUser(username, rememberMe);

            // Set SSO Token cookie
            var ssoToken = new HttpCookie(SSOTokenCookie, decryptedToken);
            ssoToken.Expires = DateTime.Now.AddDays(90);
            Response.Cookies.Add(ssoToken);
            
            SessionHelper.Remove("VendorToken");

            RedirectToDesiredURL();
        }
        else
        {
            //we don't have a valid token, initiate Retry
            String returnURL = HttpContext.Current.Request.Url.AbsoluteUri;

            if(!String.IsNullOrEmpty(HttpContext.Current.Request.Url.Query))
                returnURL = returnURL.Replace(HttpContext.Current.Request.Url.Query, "");
                        
            var encryptedVendorToken = RijndaelAlgorithm.GetVendorToken(returnURL, _personifySsoVendorPassword, _personifySsoVendorBlock, username, password, rememberMe);
            SessionHelper.SetValue("VendorToken", encryptedVendorToken);
            var url = string.Format("{0}?vi={1}&vt={2}", _personifyAutoLoginUrl, _personifyVendorID, encryptedVendorToken);
            Response.Redirect(url);
        }
    }

    /// <summary>
    /// User has logged out, call ReAuthorizeFail
    /// </summary>
    private void CheckForLogout()
    {
        if (DoLogout)
        {
            ReAuthorizeFail(_personifyLogoutURL);
        }
    }

    /// <summary>
    /// Attempt Reauthorization against Personify
    /// </summary>
    private void ReAuthorizeCheck()
    {
        if (CMS.Helpers.RequestContext.IsUserAuthenticated)
        {
            // Check for existing cookie
            var ssoToken = Request.Cookies[SSOTokenCookie];
            if (ssoToken == null || String.IsNullOrEmpty(ssoToken.Value))
            {
                ReAuthorizeFail();
            }

            var ct = ssoToken.Value;

            // Validate existing cookie
            var result = ssoClient.SSOCustomerTokenIsValid(_personifySsoVendorName, _personifySsoVendorPassword, ct);

            if (result.Valid)
            {
                ct = result.NewCustomerToken;

                var user = ssoClient.SSOCustomerGetByCustomerToken(_personifySsoVendorName, _personifySsoVendorPassword, ct);

                if (user.UserExists)
                {
                    Session[PersonifySessionKey] = ct;
                    VerifyKenticoUser(ct, user.UserName);
                }
                else
                {
                    ReAuthorizeFail();
                }
            }
            else
            {
                ReAuthorizeFail();
            }

            ReAuthorizeSuccess(ct);
        }
    }

    /// <summary>
    /// Reauthorization was successful, update SSO Token
    /// </summary>
    /// <param name="ct">Decrypted Customer Token</param>
    private void ReAuthorizeSuccess(string ct)
    {
        // Update SSO Token Cookie
        HttpCookie ssoToken = new HttpCookie(SSOTokenCookie, ct);
        ssoToken.Expires = DateTime.Now.AddDays(90);
        Response.Cookies.Add(ssoToken);

        // Redirect to success URL
        RedirectToDesiredURL();
    }

    /// <summary>
    /// Redirect user if user successfully logged in.
    /// </summary>
    private void RedirectToDesiredURL()
    {
        var redirectURL = "~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx";

        if (Session[ReturnURLSessionKey] != null)
        {
            var sessionURL = (string)Session[ReturnURLSessionKey];

            if (!String.IsNullOrEmpty(sessionURL))
            {
                redirectURL = sessionURL;

                if (redirectURL.ToLower().Contains(_personifyBaseURN))
                {
                    redirectURL += (redirectURL.Contains("?") ? "&SSOForce=Y" : "?SSOForce=Y");
                }
            }

            Session[ReturnURLSessionKey] = null;
        }
        try
        {
            Response.Redirect(redirectURL, true);
        }
        catch (System.Threading.ThreadAbortException tae)
        {
            // do nothing
        }
        catch (Exception excp)
        {
            EventLogProvider.LogException("SSOHandler", "Redirect", excp);
        }
    }

    /// <summary>
    /// Force logout due to error during reauthorization or user log out.
    /// </summary>
    /// <param name="targetUrl">URL where user should be redirected to.</param>
    private void ReAuthorizeFail(string targetUrl = "")
    {

        if (String.IsNullOrEmpty(targetUrl))
        {
            targetUrl = _loginURL;
        }

        // Log user out of Kentico
        AuthenticationHelper.LogoutUser();



        var ssoToken = Request.Cookies[SSOTokenCookie];
        if (!(ssoToken == null || String.IsNullOrEmpty(ssoToken.Value)))
        {
            try
            {
                var ct = ssoToken.Value;
                ssoClient.SSOCustomerLogout(_personifySsoVendorName, _personifySsoVendorPassword, ct);
            }    
            catch(Exception excp)
            {
                EventLogProvider.LogException("Personify", "LOGOUT", excp);
            }
        }

        

        // Clear Session values
        Session[UserNameSessionKey] = null;
        Session[PasswordSessionKey] = null;
        Session[RememberMeSessionKey] = null;

        // Expire cookie
        var cookie = new HttpCookie(SSOTokenCookie);
        cookie.Expires = DateTime.Now.AddDays(-1);
        Response.Cookies.Add(cookie);

        // Redirect User
        try
        {
            Response.Redirect(targetUrl, true);
        }
        catch (System.Threading.ThreadAbortException tae)
        {
            // do nothing
        }
        catch (Exception excp)
        {
            EventLogProvider.LogException("SSOHandler", "Redirect", excp);
        }
    }
    /// <summary>
    /// Decrypt Customer Token via Personify API
    /// </summary>
    /// <param name="customerToken">Encrypted Customer Token</param>
    /// <returns>Decrypted Personify Token</returns>
    private string DecryptCustomerToken(string customerToken)
    {
        // Double check for an empty token
        if (String.IsNullOrEmpty(customerToken))
            return null;

        var res = ssoClient.CustomerTokenDecrypt(_personifySsoVendorName, _personifySsoVendorPassword, _personifySsoVendorBlock, customerToken);

        // log any errors reported from personify
        if (res.Errors != null)
        {
            if (res.Errors.Length > 0)
            {

                var encryptedVendorToken = SessionHelper.GetValue("VendorToken");
                EventLogProvider.LogException("SSOHandler", "Decrypt", new Exception(res.Errors.Join("\r\n<br />") + "\r\n<br />User: " + ValidationHelper.GetString(Session[UserNameSessionKey],"") + "\r\n<br />Decrypted Token: " + ValidationHelper.GetString(res.CustomerToken,"No Value") + "\r\n<br />Customer Token: " + Request.QueryString["ct"].ToString() + "\r\n<br />Vendor Token:" + encryptedVendorToken.ToString()));
            }
        }
        
        if (!string.IsNullOrEmpty(res.CustomerToken))
        {
            return res.CustomerToken;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Verify User in Kentico
    /// </summary>
    /// <param name="customerToken">Decrypted Customer Token</param>
    /// <param name="userName">Username of Customer</param>
    private void VerifyKenticoUser(string customerToken, string userName)
    {
        // Get CustomerID
        var res = ssoClient.TIMSSCustomerIdentifierGet(_personifySsoVendorName, _personifySsoVendorPassword, customerToken);

        if (res != null && !String.IsNullOrEmpty(res.CustomerIdentifier))
        {
            var aIdentifiers = res.CustomerIdentifier.Split('|');
            string sMasterCustomerId = aIdentifiers[0];
            int subCustomerId = int.Parse(aIdentifiers[1]);

            Uri ServiceUri = new Uri(svcUri_Base);

            LoginUsertokentico.WriteError("AuthenticateCustomer ID ==>", sMasterCustomerId.ToString());

            // Get User details from Personify
            PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
            DataAccessLayer.Credentials = new NetworkCredential(svcLogin, svcPassword);

            var userdetails =
                DataAccessLayer.CusNameDemographics.Where(p => p.MasterCustomerId == sMasterCustomerId)
                    .Select(o => o)
                    .ToList()
                    .FirstOrDefault();

            string pfirstname = null;
            string plastname = null;

            if (userdetails == null)
            {
                pfirstname = @"&nbsp;";
                plastname = @"&nbsp;";
            }
            if (userdetails != null && string.IsNullOrWhiteSpace(userdetails.FirstName))
            {
                pfirstname = @"&nbsp;";
            }
            else
            {
                if (userdetails != null) pfirstname = userdetails.FirstName;
            }
            if (userdetails != null && string.IsNullOrWhiteSpace(userdetails.LastName))
            {
                plastname = @"&nbsp;";

            }
            else
            {
                if (userdetails != null) plastname = userdetails.LastName;
            }
            //*******End Custom Dataservice code to get Firstname, Lastname***********//

            string[] memberGroups = GetImsroles(customerToken);

            string groupslist = String.Empty;

            if (memberGroups.Length > 0)
            {
                foreach (string s in memberGroups)
                {
                    if (s.Length > 0)
                    {
                        groupslist += s + ",";
                    }
                }
            }

            groupslist += "peronifyUser" + ",";



            string login = new LoginUsertokentico().CreateUpdateLoginUserinKentico(
                userName,
                pfirstname,
                plastname,
                userName,
                groupslist,
                true,
                false, res.CustomerIdentifier, customerToken);
            userinfo uInfo = new userinfo
            {
                ID = sMasterCustomerId,
                Token = Session[PersonifySessionKey].ToString(),
                email = userName,
                firstname = pfirstname,
                lastname = plastname,
                username = userName,
                groupNames = groupslist
            };

            // Add info to Session variable
            Session["userClass"] = uInfo;
        }
        else
        {
            EventLogProvider.LogException("SSOHandler", "Verify", new Exception("Unable to retrieve personify ID. Customer Token: " + Request.QueryString["ct"].ToString() + " <br />Decrypted Token: " + customerToken + "  <br />Username: " + userName));
            // should not get here
            URLHelper.Redirect(_loginErrorUrl);
        }
    }
    /// <summary>
    /// Check format of Decrypted token
    /// <param name="Token">Decrypted User Token</paramref>
    /// </summary>
    private bool isValidToken(String Token)
    {
        // Check for Empty Value
        if (String.IsNullOrEmpty(Token))
            return false;

        // Tokens appear to be GUID format, attempt to parse as GUID
        Guid tmpGuid = Guid.Empty;
        bool isValid = Guid.TryParse(Token, out tmpGuid);


       
       
        if (!isValid)
        {
            var encryptedVendorToken = SessionHelper.GetValue("VendorToken");
            var bufferDateTime = SessionHelper.GetValue("BufferDateTime");
            String bufferTimeStamp = ValidationHelper.GetString(SessionHelper.GetValue("BufferDateTimeString"),"");

            String bufferDTString = "";            

            try
            {
                bufferDTString = Convert.ToDateTime(bufferDateTime).ToString();                
            }
            catch(Exception excp)
            {
                EventLogProvider.LogException("SSOHandler", "VerifyDate", excp);
            }


            EventLogProvider.LogException("SSOHandler", "Verify", new Exception("Bad Token.\r\n<br />User: " 
                + ValidationHelper.GetString(Session[UserNameSessionKey], "") 
                + "\r\n<br />Decrypted Token: " + Token 
                + "\r\n<br />Customer Token: " + Request.QueryString["ct"].ToString() 
                + "\r\n<br />Vendor Token: " + encryptedVendorToken.ToString() 
                + "\r\n<br />Vendor Token Decrypted: " + ValidationHelper.GetString(SessionHelper.GetValue("VendorTokenText"), "") 
                + "\r\n<br />Time Stamp: " + bufferTimeStamp.ToString() 
                + "\r\n<br />Date Time: " + bufferDTString 
                + "\r\n<br />Original Vender Token: " + ValidationHelper.GetString(SessionHelper.GetValue("OriginalVendorToken"),"") 
                + "\r\n<br />Original Bytes: " + ValidationHelper.GetString(SessionHelper.GetValue("OriginalBytes"),"")
                + "\r\n<br />Vendor Block: " + ValidationHelper.GetString(SessionHelper.GetValue("VendorBlock"), "")
                + "\r\n<br />Vendor Pass: " + ValidationHelper.GetString(SessionHelper.GetValue("VendorPW"), "")));


        }

        SessionHelper.Remove("VendorPW");
        SessionHelper.Remove("VendorBlock");

        SessionHelper.Remove("OriginalVendorToken");
        SessionHelper.Remove("VendorTokenText");
        SessionHelper.Remove("BufferDateTime");
        SessionHelper.Remove("BufferDateTimeString");
        SessionHelper.Remove("OriginalBytes");
        return isValid;
    }

    // Get Roles for user
    private string[] GetImsroles(string customerToken)
    {
        using (var myim = new IMService { Url = _personifyImsUrl })
        {
            var roleList = new List<string>();

            var imsCustomerRoleGetResult = myim.IMSCustomerRoleGet(_personifySsoVendorName, _personifySsoVendorPassword, customerToken);

            if (imsCustomerRoleGetResult.CustomerRoles == null)
                return new[] { string.Empty };

            foreach (var roledetail in imsCustomerRoleGetResult.CustomerRoles)
            {

                roleList.Add(roledetail.Value);

            }

            return roleList.ToArray();
        }
    }

    private string[] GetImsroles(string masterCustomerId, int subCustomerId)
    {
        var timssId = masterCustomerId + "|" + subCustomerId;

        var roleList = new List<string>();

        using (var myim = new IMService { Url = _personifyImsUrl })
        {
            IMSCustomerRoleGetResult imsCustomerRoleGetResult = myim.IMSCustomerRoleGetByTimssCustomerId(_personifySsoVendorName, _personifySsoVendorPassword, timssId);

            if (imsCustomerRoleGetResult.CustomerRoles == null)
                return new[] { string.Empty };

            foreach (var roledetail in imsCustomerRoleGetResult.CustomerRoles)
            {

                roleList.Add(roledetail.Value);

            }

            return roleList.ToArray();
        }

    }
}