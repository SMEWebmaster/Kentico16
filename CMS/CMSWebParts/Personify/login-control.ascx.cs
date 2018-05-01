using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Xml;
using System.Xml.Linq;
using CMS.UIControls;
using CMS.TreeEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.CMSHelper;
using CMS.FileManager;
using CMS.WorkflowEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using System.Data;
using System.Data.SqlClient;

using CMS.PortalControls;
using CMS.PortalEngine;
using System.Security.Cryptography;

using System.Text;
using CMS.SettingsProvider;


using CMS.Helpers;
using System.Configuration;
using System.Net;
using System.IO;
using SSO;


public partial class LoginControl : CMSAbstractWebPart
{
    #region "Global Variables"

    DataSet ds = new DataSet();
    DataSet groupDs = new DataSet();
    string ID, FirstName, LastName, Email, web_login, IsDisabled, MemberType = "", lastpage;

    string DOMAIN = ".networkats.com";
    string RolePrefix = "ats_";
    string tokenid = "x";
    private const string PersonifySessionKey = "PersonifyToken";

    private readonly string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
    private service _wsSso = new service();
    bool redirecttomember = false;
    #endregion

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["LoginAttempted"] != null)
        {
            bool loginAttempted;
            bool.TryParse(Session["LoginAttempted"].ToString(), out loginAttempted);
            if (loginAttempted)
            {
                phErrorMessage.Visible = true;
                litError.Text = "The User ID/ Password you entered is not valid. Please try again.";
            }
        }

        if (Request.RawUrl.ToLower().Contains("/login-join"))
        {
            divSubmit.Visible = true;
        }
        else
        {
            divSubmit.Visible = false;
        }


    }
    protected void Page_LoadComplete(object sender, EventArgs e)
    {
        if (Request.QueryString["option"] == "logout")
        {
            //  logoutEvent();
            // Response.Write("/logout/logout.aspx");

            if (Request.RawUrl.ToLower().Contains("uca"))
            {
                Response.Redirect("~/logout/logout.aspx?site=uca");
            }
            else
            {
                Response.Redirect("~/logout/logout.aspx?site=sme");
            }
        }


    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (Request.Url.AbsoluteUri.ToLower().Contains("login-join"))
        {
            //  pclogin.Visible = true;
            //  pclogin2.Visible = true;

            litHead.Text = "<div class='landing-page-content'><div class='body clear'><div class='grid_4'><div class='bucket'>";
            litFoot.Text = "</div></div></div></div>";
            // Response.Write(Request.Url.AbsoluteUri.ToLower());
        }
        else
        {
            litHead.Text = "<div class='personify-login-bucket'>";
            litFoot.Text = "</div>";
        }
        // LoginMember("kral@smenet.org", "Password1", true);
        if (!Page.IsPostBack)
        {

            if (Request.UrlReferrer != null)
            {
                if (!Request.UrlReferrer.ToString().Contains("login"))
                {
                    Session["lastpage"] = Request.UrlReferrer.ToString();
                }
            }
            if (CMS.Helpers.RequestContext.IsUserAuthenticated)
            {
                if (Request.QueryString["option"] == "logout")
                {
                    //  logoutEvent();
                    // Response.Redirect("/logout/logout.aspx");
                    if (Request.RawUrl.ToLower().Contains("uca"))
                    {
                        Response.Redirect("~/logout/logout.aspx?site=uca");
                    }
                    else
                    {
                        Response.Redirect("~/logout/logout.aspx?site=sme");
                    }
                }
                else if (Request.QueryString["link"] != null && Request.QueryString["link"] != "")
                {
                    Response.Redirect(Request.QueryString["link"].ToString(), false);
                }
                // T20140130.0023
                else
                {
                    pannelLoggedin();
                }
            }
            else
            {
                //  logegdOut();
                if (!(CMS.Membership.AuthenticationHelper.IsAuthenticated()))
                {
                    // if (chkRememberMe.) 
                    if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
                    {
                        chkRememberMe.Checked = true;
                        txtUsername.Text = encryppter.StringCipher.Decrypt(
                            Request.Cookies["UserName"].Value.ToString(),
                            "password");
                        txtPassword.Text = encryppter.StringCipher.Decrypt(
                           Request.Cookies["Password"].Value.ToString(),
                           "password");
                    }
                }
            }

            if (Request.QueryString["rurl"] != null)
            {
                Session["rurlValues"] = Request.QueryString["rurl"];
            }

            /////divGC  guestCheckout=N

            if (Request.QueryString["guestCheckout"] != "" && Request.QueryString["guestCheckout"] != null)
            {
                if (Request.QueryString["ReturnUrl"].ToString().ToLower().Contains("y"))
                {
                    divGC.Visible = true;
                }
            }
        }
    }
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            string Username = txtUsername.Text;
            string Password = txtPassword.Text;
            bool RememberMember = rememberMe_check();

            if (validateUserNamePassword(Username, Password))
            {
                LoginMember(Username, Password, RememberMember);
            }
            else
            {
                Session["LoginAttempted"] = true;
                Response.Redirect(Request.RawUrl);
            }
        }
        catch (Exception exception)
        {
            //  ApplicationEngine.HandleException(exception);
        }
        //  LoginMember(txtUsername.Text, txtPassword.Text, true);

    }

    #region login

    //Returns bool value for Username and Password Validation
    private bool validateUserNamePassword(string Username, string Password)
    {
        bool flag = false;
        try
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                flag = false;
            }
            else
            {

                var x = _wsSso.SSOCustomerAuthenticate(
                    _personifySsoVendorName,
                    _personifySsoVendorPassword,
                    Username,
                    Password);


                if (x != null)
                {
                    if (x.Result == false)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }

            }
        }
        catch (Exception ex)
        {
            flag = false;

        }
        return flag;
    }


    //Returns bool value for generating VendorToken.
    protected bool rememberMe_check()
    {
        bool ret = false;
        if (chkRememberMe.Checked)
        {
            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(90);
            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(90);
            ret = true;
        }
        else
        {
            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
            ret = false;
        }


        string EncryptedUsername = encryppter.StringCipher.Encrypt(txtUsername.Text.Trim(), "password");//EncryptString(txtUsername.Text.Trim(), ApplicationEngine.DataEncryptionKey);
        string EncryptedPassword = encryppter.StringCipher.Encrypt(txtPassword.Text.Trim(), "password");//CryptographyUtility.EncryptString(txtPassword.Text.Trim(), ApplicationEngine.DataEncryptionKey);
        Response.Cookies["UserName"].Value = EncryptedUsername;
        Response.Cookies["Password"].Value = EncryptedPassword;
        return ret;
    }


    private void LoginMember(string Username, string Password, bool RememberMember)
    {
        try
        {
            var vendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"].ToString();
            var vendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"].ToString();
            var vendorId = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
            string returnURl = Request.Url.AbsoluteUri;
            string fullUrl = Request.RawUrl;
            if (Request.QueryString["ReturnUrl"] != null)
            {
                //  string url = Request.Url.ToString();
                //  Uri originalUrl = new Uri(url); // Request.Url
                //  string domainUrl = String.Concat(originalUrl.Scheme, Uri.SchemeDelimiter, originalUrl.Host); // http://www.mydomain.com
                ////  UrlParameterHelper.ConstructURLWithExistingQueryString(Step2Url, Request.QueryString, QueryStringParametersToPreserve);
                //  returnURl = domainUrl + "/CMSPages/PortalTemplate.aspx?aliaspath=" + Request.QueryString["ReturnUrl"].ToString();
                string Rurl = fullUrl.Replace("?site=sme&", "?");
                Rurl = Rurl.ToLower().Replace("/memberredirect/default.aspx?returnurl=", "");
                Rurl = Rurl.Replace("&site=sme", "");
                if (Request.QueryString["ReturnUrl"].ToString().ToLower().Contains("personifyebusiness"))
                {

                    if ((Request.QueryString["ReturnUrl"].ToString().ToLower().Contains("http://")))///check if user is redirected from Personify Pages
                    {
                        string urlRed = Server.UrlDecode(Request.QueryString["ReturnUrl"].ToString().ToLower());
                        string login = "/memberredirect/default.aspx?rurl=";
                        urlRed.Replace("http://smemi.personifycloud.com/", "");

                        Session["redirectUrl"] = login + (urlRed);
                        // Session["redirectUrl"] =   Request.QueryString["ReturnUrl"].ToString();//Server.UrlEncode(Rurl) ;//Request.QueryString["ReturnUrl"].ToString();
                    }
                    else
                    {
                        Session["redirectUrl"] = Server.UrlEncode(Rurl);//Request.QueryString["ReturnUrl"].ToString();
                    }

                }
                else
                {
                    Session["redirectUrl"] = Request.QueryString["ReturnUrl"].ToString();//Server.UrlDecode(Rurl);
                }
            }

            else if (Request.QueryString["rurl"] != null)
            {
                string Rurl = fullUrl.Replace("?site=sme&", "?");
                Rurl = Rurl.Replace("/memberredirect/default.aspx?rurl=", "");
                Rurl = Rurl.Replace("&site=sme", "");
                string login = "/memberredirect/default.aspx?rurl=";
                Session["redirectUrl"] = login + (Session["rurlValues"]);
                //Response.Write(Request.QueryString["rurl"] +" <br/>") ;
            }
            else if (Request.QueryString["loginurl"] != null)
            {
                Session["redirectUrl"] = Request.QueryString["loginurl"];
            }
            else
            {
                Session["redirectUrl"] = null;
            }
            var encryptedVendorToken = RijndaelAlgorithm.GetVendorToken(returnURl, vendorPassword,
  vendorBlock, Username, Password, RememberMember);


            string URL = string.Format("{0}?vi={1}&vt={2}", PersonifyAutoLoginUrl, vendorId, encryptedVendorToken);
            //  var ssoRedirect = "http://smemitst.personifycloud.com/SSO/autologin.aspx" + "?vi=" + vendorId + "&vt=" + encryptedVendorToken;
            LoginUsertokentico.WriteError("URL which is sent via Autologin  --->", URL);
            Response.Redirect(URL);
        }
        catch (Exception exception)
        {
            // ApplicationEngine.HandleException(exception);
            Response.Write(exception.ToString());
            // LoginUsertokentico.WriteError("LoginMember", exception.ToString());

            EventLogProvider.LogException("LoginMember", "Get", exception);
        }
    }
    #endregion

    protected void btnLogout_Click(object sender, EventArgs e)
    {
        if (Request.RawUrl.ToLower().Contains("uca"))
        {
            Response.Redirect("~/logout/logout.aspx?site=uca");
        }
        else
        {
            Response.Redirect("~/logout/logout.aspx?site=sme");
        }
    }



    public void logoutEvent()
    {
        /* try
         {

             if (Session["userClass"] != null && Session["userClass"] != "")
             {
                 //KenticoService.Logout(ui.username);
                 //  MemService.SSOCustomerLogout(vendor,vendorpw,tokenid);
                 logegdOut();
             }
             else
             {
                 //  MemService.SSOCustomerLogout(vendor, vendorpw, tokenid);
                 logegdOut();
                 //  Response.Redirect("here:");
             }

             /////
             if (Session[PersonifySessionKey] != null)
             {

                 _wsSso.SSOCustomerLogout(_personifySsoVendorName, _personifySsoVendorPassword, Session[PersonifySessionKey].ToString());
             }
         }
         catch (Exception ex)
         {
             //EventLogProvider evp = new EventLogProvider();
             EventLogProvider.LogException("Logout Issue", "Get", ex);
         }
         // HttpResponse.Cache.SetNoStore();
         Session["userClass"] = null;
         Session["cuctomerReturnToken"] = null;
         System.Web.Security.FormsAuthentication.SignOut();
         //CMS.CMSHelper.CMSContext.ClearShoppingCart();
         //CMS.CMSHelper.CMSContext.CurrentUser = null;
 */

        Response.Redirect("~/logout/logout.aspx", false);
    }



    private void pannelLoggedin()
    {
        pnlSiteLogin.Visible = false;
        pnlLoggedInMember.Visible = true;

        if (CMS.Helpers.RequestContext.IsUserAuthenticated)
        {
            lblLoggedin.Text = "Welcome " + MembershipContext.AuthenticatedUser.FirstName + " " + MembershipContext.AuthenticatedUser.LastName + "!";

            if (Session["redirectUrl"] != null)
            {

                string val = Session["redirectUrl"].ToString();
                Session["redirectUrl"] = null;
                //Response.Redirect(val, true);
                Response.Redirect(val);
            }
            //Response.Write(Session["redirecttomember"]);
            if (Request.RawUrl.ToLower() == "/cmspages/portaltemplate.aspx?aliaspath=/personifyebusiness/login-join" || Request.RawUrl.ToLower() == "/cmspages/portaltemplate.aspx?aliaspath=/login-join")
            {
                Response.Redirect("/personifyebusiness/login-join");
            }
            // Response.Write(Request.RawUrl.ToLower());
            MyIframe.Attributes.Add("src", "~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx");
            redirecttomember = Convert.ToBoolean(Session["redirecttomember"]);
            if (Request.RawUrl.ToLower() == "/personifyebusiness/login-join" && redirecttomember)
            {
                Session["redirecttomember"] = false;
                Response.Redirect("~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx");
            }
            else if (redirecttomember)
            {
                Session["redirecttomember"] = false;
                if (Request.RawUrl.ToLower().Contains("login") && Request.RawUrl.ToLower().Contains("uca"))
                {
                    Response.Redirect("~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx");
                }
                else
                {
                    // Response.Redirect("/personifyebusiness/login-join");
                }

            }
        }

    }
    private void pannelLoggedinAsNonMember()
    {
        pnlSiteLogin.Visible = false;
        pnlLoggedInMember.Visible = false;
    }
    private void logegdOut()
    {
        pnlSiteLogin.Visible = true;
        pnlLoggedInMember.Visible = false;
    }

    protected string getMemberData(string sID)
    {
        //http://ats75.personifycorp.com:106/PersonifyDataServices/PersonifyData.svc/CustomerInfos(MasterCustomerId='000000001475',SubCustomerId=0)

        string posturl = "http://ats75.personifycorp.com:106/PersonifyDataServices/PersonifyData.svc/CustomerInfos(MasterCustomerId='" + sID + "',SubCustomerId=0)";

        string sReturn = "";
        var httpWebRequest = (HttpWebRequest)WebRequest.Create(posturl);

        String username = "admin";
        String password = "@ts751n3w";
        //string postBody = "grant_type=client_credentials";
        string authHeaderFormat = "Basic {0}";
        String encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(username + ":" + password));
        string authHeader = string.Format(authHeaderFormat, Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(username) + ":" + Uri.EscapeDataString((password)))));
        httpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
        httpWebRequest.Method = "GET";
        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password)));
        //   httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, authHeader);
        httpWebRequest.Headers.Add("Accept-Encoding", "gzip");
        WebResponse authResponse = httpWebRequest.GetResponse();

        using (authResponse)
        {
            //using (var reader = new StreamReader(authResponse.GetResponseStream()))
            //{

            //    sReturn = reader.ReadToEnd().ToString();

            //}
            XmlTextReader reader = new XmlTextReader(authResponse.GetResponseStream());

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                    //    Console.WriteLine("{0}", reader.Value.Trim());
                    Response.Write(reader.Name.ToString() + " " + reader.Value.Trim().ToString());
            }

        }

        return sReturn;
    }


}