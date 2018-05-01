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
    private const string DoLogoutQSKey = "logout";
    private const string PersonifySessionKey = "PersonifyToken";
    private const string SSOTokenCookie = "SSOToken";
    private const string UserNameSessionKey = "UserName";
    private const string PasswordSessionKey = "Password";
    private const string RetryCountSessionKey = "RetryCount";
    private const string RememberMeSessionKey = "RememberMe";
    private const string ReturnURLSessionKey = "ReturnURL";

    #region "Global Variables"

    private readonly string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
    private SSOClient _wsSso = new SSOClient();
    bool redirecttomember = false;
    #endregion

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }

    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (this.ViewMode == ViewModeEnum.LiveSite)
            {
                if (CMS.Helpers.RequestContext.IsUserAuthenticated)
                {
                    Response.Redirect("~/sso/ssohandler.aspx", true);
                }
                cvUsernamePassword.ServerValidate += CvUsernamePassword_ServerValidate;
                LoginButton.Click += btnLogin_Click;
            }
        }
    }

    private void CvUsernamePassword_ServerValidate(object source, ServerValidateEventArgs args)
    {           
        args.IsValid = validateUserNamePassword(txtUsername.Text, txtPassword.Text);
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {           
        if (this.Page.IsValid)
        {
            string Username = txtUsername.Text;
            string Password = txtPassword.Text;
            bool RememberMember = chkRememberMe.Checked;
                        
            try
            {
                Session["UserName"] = Username;
                Session["Password"] = Password;
                Session["RememberMe"] = RememberMember;
                Session["RetryCount"] = null;

                if (this.Request.QueryString[ReturnURLSessionKey] != null)
                {
                    var returnURL = this.Request.QueryString[ReturnURLSessionKey];

                    Session[ReturnURLSessionKey] = returnURL;
                }

                Response.Redirect("~/sso/ssohandler.aspx", true);
            }
            catch (Exception exception)
            {
                EventLogProvider.LogException("Login Screen", "E", exception, CurrentSite.SiteID, String.Format("Login Failed for user {0}", Username));
            }
        }
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
                        EventLogProvider.LogWarning("Login", "Login", new Exception(x.Errors.Join(" | ")), SiteContext.CurrentSiteID, Username);
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
            //log exception into eventlog
            EventLogProvider.LogException(ex.Source, ex.Message, ex);
            flag = false;
        }
        return flag;
    }

    #endregion
}