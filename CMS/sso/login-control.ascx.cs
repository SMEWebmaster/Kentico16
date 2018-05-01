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
    bool redirecttomember = false ;
    #endregion

 
 

    protected void Page_Load(object sender, EventArgs e)
    {
        
      
       
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
                string Rurl =fullUrl.Replace("?site=sme&","?");
                Rurl= Rurl.ToLower().Replace("/memberredirect/default.aspx?returnurl=","");
                Rurl= Rurl.Replace("&site=sme","");
                if(Request.QueryString["ReturnUrl"].ToString().ToLower().Contains("personifyebusiness"))
                {

                    if((Request.QueryString["ReturnUrl"].ToString().ToLower().Contains("http://")))///check if user is redirected from Personify Pages
                        {
                            string urlRed = Server.UrlDecode(Request.QueryString["ReturnUrl"].ToString().ToLower());
                             string login = "/memberredirect/default.aspx?rurl=";
                            urlRed.Replace("http://smemi.personifycloud.com/", "");

                            Session["redirectUrl"] = login + (urlRed); 
                           // Session["redirectUrl"] =   Request.QueryString["ReturnUrl"].ToString();//Server.UrlEncode(Rurl) ;//Request.QueryString["ReturnUrl"].ToString();
                        }
                            else
                        {
                            Session["redirectUrl"] =   Server.UrlEncode(Rurl) ;//Request.QueryString["ReturnUrl"].ToString();
                        }
                
                }
                else
                {
                  Session["redirectUrl"] =    Request.QueryString["ReturnUrl"].ToString();//Server.UrlDecode(Rurl);
                }
            }

            else if (Request.QueryString["rurl"] != null)
            { 
                 string Rurl =fullUrl.Replace("?site=sme&","?");
                Rurl= Rurl.Replace("/memberredirect/default.aspx?rurl=","");
                Rurl= Rurl.Replace("&site=sme","");
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
            }            var encryptedVendorToken = RijndaelAlgorithm.GetVendorToken(returnURl, vendorPassword,
               vendorBlock, Username, Password, RememberMember);


            string URL = string.Format("{0}?vi={1}&vt={2}", PersonifyAutoLoginUrl, vendorId, encryptedVendorToken);
            //  var ssoRedirect = "http://smemitst.personifycloud.com/SSO/autologin.aspx" + "?vi=" + vendorId + "&vt=" + encryptedVendorToken;
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
              if(Request.RawUrl.ToLower() =="/cmspages/portaltemplate.aspx?aliaspath=/personifyebusiness/login-join" )
                    {
                        Response.Redirect("/personifyebusiness/login-join");
                    }

                    else  if(Request.RawUrl.ToLower() =="/cmspages/portaltemplate.aspx?aliaspath=/uca-of-sme/login-join-(1)")
                    {
                     Response.Redirect("/uca/login");
                    }
                   // Response.Write(Request.RawUrl.ToLower());
                    MyIframe.Attributes.Add("src", "~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx;layout=button_count&amp;show_faces=true&amp;width=100&amp;action=recommend&amp;colorscheme=light&amp;height=21");
                               redirecttomember = Convert.ToBoolean(Session["redirecttomember"]);
                    if(Request.RawUrl.ToLower() == "/personifyebusiness/login-join" &&  redirecttomember )
                    {
                        Session["redirecttomember"]= false;
                       Response.Redirect("~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx");
                    }
                    else if(redirecttomember)
                    {
                       Session["redirecttomember"]= false;
                        if (Request.RawUrl.ToLower().Contains("login") && Request.RawUrl.ToLower().Contains("uca") )
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
 


}