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
using Personify.ErrorHandling;
using System.Collections.Generic;
using IMS;
using CMS.ExtendedControls;

public partial class CMSWebParts_Personify_RecoverPassword : CMSAbstractWebPart
{
    private static readonly string PersonifyImsUrl = ConfigurationManager.AppSettings["IMSWebReferenceURL"];
    private static readonly string PersonifyVendorID = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
    private static readonly string PersonifyVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private static readonly string PersonifyVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private static readonly string PersonifyVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private static readonly string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private static string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private static string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
    static string sUri = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];

    private SSOClient ssoClient = new SSOClient();

    /// <summary>
    /// Gets or sets reset password url
    /// </summary>
    public string ResetPasswordURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ResetPasswordURL"),
                AuthenticationHelper.GetResetPasswordUrl(SiteContext.CurrentSiteName));
        }
        set { SetValue("ResetPasswordURL", value); }
    }

    public string SuccessLandingPage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SuccessLandingPage"), "~/");
        }
        set { SetValue("SuccessLandingPage", value); }
    }

    public string FromEmailAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FromEmailAddress"), EmailHelper.DEFAULT_EMAIL_SENDER);
        }
        set { SetValue("FromEmailAddress", value); }
    }

    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ViewMode != ViewModeEnum.LiveSite)
        {
            return;
        }

        lbSubmit.Click += LbSubmit_Click;
    }

    private void LbSubmit_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            if (!String.IsNullOrWhiteSpace(txtEmailAddress.Text))
            {
                var ssoCustomer = ssoClient.SSOCustomerGetByEmail(PersonifyVendorName, PersonifyVendorPassword, txtEmailAddress.Text);

                if (ssoCustomer != null && ssoCustomer.UserExists && !String.IsNullOrEmpty(ssoCustomer.UserName) && (ssoCustomer.Errors == null || !ssoCustomer.Errors.Any()))
                {
                    var userName = ssoCustomer.UserName;

                    var userInfo = VerifyKenticoUser(userName, txtEmailAddress.Text);

                    TrySendForgotPasswordEmail(userInfo);
                }
                else if (ssoCustomer.Errors != null && ssoCustomer.Errors.Any())
                {
                    plcMess.Clear();
                    plcMess.AddInformation(ssoCustomer.Errors.First());
                    plcMess.Visible = true;
                }
                else
                {
                    plcMess.Clear();
                    plcMess.AddInformation("&nbsp;Account was not found.&nbsp;");
                    plcMess.Visible = true;
                }
            }
        }
    }

    private UserInfo VerifyKenticoUser(string userName, string emailAddress)
    {
        UserInfo user = CMS.Membership.UserInfoProvider.GetUserInfo(userName);

        if (user == null)
        {
            user = new UserInfo();
            user.UserName = userName;
            user.FirstName = "Temp";
            user.LastName = "Temp";
            user.FullName = "Temp";
            user.Email = emailAddress;

            user.PreferredCultureCode = "en-us";
            user.PasswordFormat = "SHA1";
            user.Enabled = true;

            CMS.Membership.UserInfoProvider.SetUserInfo(user);
            CMS.Membership.UserInfoProvider.SetPassword(userName, userName);
            UserInfoProvider.AddUserToSite(userName, CMS.SiteProvider.SiteContext.CurrentSiteName);
        }

        return user;
    }

    private void TrySendForgotPasswordEmail(UserInfo userInfo)
    {
        if (userInfo != null)
        {
            if (TrySendPasswordResetEmail(userInfo))
            {
                Response.Redirect(SuccessLandingPage, true);
            }

            plcMess.Clear();
            plcMess.Visible = false;
        }
        else
        {
            plcMess.Clear();
            plcMess.AddInformation("&nbsp;Account was not found.&nbsp;");
            plcMess.Visible = true;
        }
    }

    private bool TrySendPasswordResetEmail(UserInfo userInfo)
    {
        string returnUrl = RequestContext.CurrentURL;
        returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", userInfo.UserName);

        bool success;

        MacroResolver macros = MacroResolver.GetInstance();
        macros.SetNamedSourceData("Username", userInfo.UserName);
        var resolvedURL =  Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + ResolveUrl(ResetPasswordURL);
        var result = AuthenticationHelper.ForgottenEmailRequest(userInfo.UserName, SiteContext.CurrentSiteName, "RECOVERPASSWORDFORM", FromEmailAddress, macros, resolvedURL, out success);

        return success;
    }

    private UserInfo GetUserInfoFromRecoveryText(string emailAddress)
    {
        var recoveryText = emailAddress;

        var users = UserInfoProvider.GetUsers().Where("Email", QueryOperator.Equals, recoveryText);

        if (!DataHelper.DataSourceIsEmpty(users))
        {
            return new UserInfo(users.Tables[0].Rows[0]);
        }

        return null;
    }
}