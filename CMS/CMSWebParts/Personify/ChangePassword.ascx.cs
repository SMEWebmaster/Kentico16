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

public partial class CMSWebParts_Personify_ChangePassword : CMSAbstractWebPart
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

    #region "Constants"

    /// <summary>
    /// Identifies key used to store reset password request identifier
    /// </summary>
    private const string RESET_REQUEST_ID = "UserPasswordRequestID";

    #endregion


    #region "Variables"

    private string siteName = string.Empty;
    private double interval = 0;
    private string hash = string.Empty;
    private string time = string.Empty;
    private int policyReq = 0;
    private int pwdExp = 0;
    private string returnUrl = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Text shown if request hash isn't found.
    /// </summary>
    public string InvalidRequestText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("InvalidRequestText"), "");
        }
        set { SetValue("InvalidRequestText", value); }
    }


    /// <summary>
    /// Text shown when request time was exceeded
    /// </summary>
    public string ExceededIntervalText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ExceededIntervalText"), "");
        }
        set { SetValue("ExceededIntervalText", value); }
    }


    /// <summary>
    /// Url on which is user redirected after successful password reset.
    /// </summary>
    public string RedirectUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RedirectUrl"), "");
        }
        set { SetValue("RedirectUrl", value); }
    }

    /// <summary>
    /// E-mail address from which e-mail is sent.
    /// </summary>
    public string SendEmailFrom
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SendEmailFrom"), "");
        }
        set { SetValue("SendEmailFrom", value); }
    }

    /// <summary>
    /// E-mail address from which e-mail is sent.
    /// </summary>
    public string RequestPasswordResetURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("RequestPasswordResetURL"), "~/cmspages/logon.aspx?forgottenpassword=1");
        }
        set { SetValue("RequestPasswordResetURL", value); }
    }


    /// <summary>
    /// Text shown when password reset was successful.
    /// </summary>
    public string SuccessText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SuccessText"), "");
        }
        set { SetValue("SuccessText", value); }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    protected void Page_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Clear session value
            ClearResetRequestID();
        }
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
        cVNewPassword.ServerValidate += CVNewPassword_ServerValidate;

        hash = QueryHelper.GetString("hash", string.Empty);
        time = QueryHelper.GetString("datetime", string.Empty);
        policyReq = QueryHelper.GetInteger("policyreq", 0);
        pwdExp = QueryHelper.GetInteger("exp", 0);
        returnUrl = QueryHelper.GetString("returnurl", null);

        siteName = SiteContext.CurrentSiteName;

        // Get interval from settings
        interval = SettingsKeyInfoProvider.GetDoubleValue(siteName + ".CMSResetPasswordInterval");

        // Prepare failed message
        string invalidRequestMessage = DataHelper.GetNotEmpty(InvalidRequestText, String.Format(ResHelper.GetString("membership.passwresetfailed"), ResolveUrl(RequestPasswordResetURL)));

        // Reset password cancelation
        if (QueryHelper.GetBoolean("cancel", false))
        {
            // Get user info
            UserInfo ui = UserInfoProvider.GetUsersDataWithSettings()
                   .WhereEquals("UserPasswordRequestHash", hash)
                   .FirstObject;

            if (ui != null)
            {
                ui.UserPasswordRequestHash = null;
                UserInfoProvider.SetUserInfo(ui);
                ClearResetRequestID();

                ShowInformation(GetString("membership.passwresetcancelled"));
            }
            else
            {
                ShowError(invalidRequestMessage);
            }

            pnlReset.Visible = false;
            return;
        }

        // Reset password request
        if (!URLHelper.IsPostback())
        {
            if (policyReq > 0)
            {
                ShowInformation(GetString("passwordpolicy.policynotmet"));
            }

            // Get user info
            var uiData = UserInfoProvider.GetUsersDataWithSettings()
                .WhereEquals("UserPasswordRequestHash", hash);

            int userId = GetResetRequestID();
            if (userId > 0)
            {
                uiData
                    .Or()
                    .WhereEquals("UserID", userId);
            }


            UserInfo ui = uiData.FirstObject;

            // Validate request
            ResetPasswordResultEnum result = AuthenticationHelper.ValidateResetPassword(ui, hash, time, interval, "Reset password control");

            // Prepare messages
            string timeExceededMessage = DataHelper.GetNotEmpty(ExceededIntervalText, String.Format(ResHelper.GetString("membership.passwreqinterval"), ResolveUrl(RequestPasswordResetURL)));
            string resultMessage = string.Empty;

            // Check result
            switch (result)
            {
                case ResetPasswordResultEnum.Success:
                    // Save user to session                    
                    SetResetRequestID(ui.UserID);

                    // Delete it from user info
                    ui.UserPasswordRequestHash = null;
                    UserInfoProvider.SetUserInfo(ui);

                    break;

                case ResetPasswordResultEnum.TimeExceeded:
                    resultMessage = timeExceededMessage;
                    break;

                default:
                    resultMessage = invalidRequestMessage;
                    break;
            }

            if (!string.IsNullOrEmpty(resultMessage))
            {
                // Show error message
                ShowError(resultMessage);

                pnlReset.Visible = false;
            }
        }
    }

    private void CVNewPassword_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
    {
        var message = String.Empty;
        args.IsValid = PasswordIsStrong(txtNewPassword.Text, out message);

        cVNewPassword.ErrorMessage = message;
    }

    private void LbSubmit_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            int userId = GetResetRequestID();

            // Check if password expired
            if (pwdExp > 0)
            {
                UserInfo ui = UserInfoProvider.GetUserInfo(userId);
                if (!UserInfoProvider.IsUserPasswordDifferent(ui, txtNewPassword.Text))
                {
                    ShowError(GetString("passreset.newpasswordrequired"));
                    return;
                }
            }

            // Get e-mail address of sender
            string emailFrom = DataHelper.GetNotEmpty(SendEmailFrom, SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"));
            // Try to reset password and show result to user
            bool success;
            string resultText = AuthenticationHelper.ResetPassword(hash, time, userId, interval, txtNewPassword.Text, "Reset password control", emailFrom, siteName, null, out success, InvalidRequestText, ExceededIntervalText);

            if (success)
            {
                try
                {
                    var userInfo = UserInfoProvider.GetFullUserInfo(userId);

                    var userName = userInfo.UserName;

                    var result = ssoClient.SSOCustomerUpdatePasswordByUserName(PersonifyVendorName, PersonifyVendorPassword, userName, txtNewPassword.Text);

                    if (!result.Result)
                    {
                        success = false;

                        if (result.Errors != null && result.Errors.Any())
                        {
                            resultText = result.Errors.First();
                        }
                        else
                        {
                            resultText = "Unknown SSO Error Updating Password.";
                        }

                        EventLogProvider.LogEvent(EventType.WARNING, "CMSWebParts_Personify_ChangePassword", resultText,
                                eventDescription: string.Format("Failed to update user credentials in personify SSO with username: {0}", userName),
                                userId: userId,
                                userName: userName,
                                siteId: SiteContext.CurrentSiteID,
                                eventTime: DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    success = false;
                    resultText = "Program Error Updating SSO Password";
                    EventLogProvider.LogException("CMSWebParts_Personify_ChangePassword", resultText, ex);
                }
            }

            // If password reset was successful
            if (success)
            {


                ClearResetRequestID();

                // Redirect to specified URL 
                if (!string.IsNullOrEmpty(RedirectUrl))
                {
                    URLHelper.Redirect(RedirectUrl);
                }

                // Get proper text
                ShowConfirmation(DataHelper.GetNotEmpty(SuccessText, resultText));
                pnlReset.Visible = false;
            }
            else
            {
                ShowError(resultText);
            }
        }
    }
    public bool PasswordIsStrong(string password, out string message)
    {
        message = String.Empty;

        message = ssoClient.SSOCustomerPasswordIsStrong(PersonifyVendorName, PersonifyVendorPassword, password);

        var isStrong = String.IsNullOrEmpty(message);

        return isStrong;
    }

    public bool UpdateCredentials(string username, string password )
    {
        var ssoService = new SSOClient();

        var setCredentialsResult = ssoService.SSOCustomerUpdatePasswordByUserName(PersonifyVendorName, PersonifyVendorPassword, username, password);

        if (!setCredentialsResult.Result)
        {
            return false;
        }

        return true;
    }

    #region "Helper methods"

    /// <summary>
    /// Gets reset password request identifier from session
    /// </summary>
    private int GetResetRequestID()
    {
        return ValidationHelper.GetInteger(SessionHelper.GetValue(RESET_REQUEST_ID), 0);
    }


    /// <summary>
    /// Stores reset password request identifier to session
    /// </summary>
    /// <param name="requestId">Request ID to be stored</param>
    private void SetResetRequestID(int requestId)
    {
        SessionHelper.SetValue(RESET_REQUEST_ID, requestId);
    }


    /// <summary>
    /// Removes reset password request identifier from session
    /// </summary>
    private void ClearResetRequestID()
    {
        SessionHelper.Remove(RESET_REQUEST_ID);
    }

    #endregion
}