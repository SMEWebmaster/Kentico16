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

public partial class CMSWebParts_Membership_Registration_RegistrationForm : CMSAbstractWebPart
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
    #region "Layout properties"

    /// <summary>
    /// Full alternative form name ('classname.formname') for usersettingsinfo.
    /// Default value is cms.user.RegistrationForm
    /// </summary>
    public string AlternativeForm
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AlternativeForm"), "cms.user.RegistrationForm");
        }
        set
        {
            SetValue("AlternativeForm", value);
            formUser.AlternativeFormFullName = value;
        }
    }

    #endregion


    #region "Text properties"

    /// <summary>
    /// Gets or sets submit button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Button$}"));
        }

        set
        {
            SetValue("ButtonText", value);
            btnRegister.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets registration approval page URL.
    /// </summary>
    public string ApprovalPage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ApprovalPage"), String.Empty);
        }
        set
        {
            SetValue("ApprovalPage", value);
        }
    }

    #endregion


    #region "Registration properties"

    /// <summary>
    /// Gets or sets the value that indicates whether email to user should be sent.
    /// </summary>
    public bool SendWelcomeEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendWelcomeEmail"), true);
        }
        set
        {
            SetValue("SendWelcomeEmail", value);
        }
    }

    public bool AutoLoginAfterRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AutoLoginAfterRegistration"), true);
        }
        set
        {
            SetValue("AutoLoginAfterRegistration", value);
        }
    }

    public string LoginURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("LoginURL"), String.Empty);
        }
        set
        {
            SetValue("LoginURL", value);
        }
    }


    /// <summary>
    /// Determines whether the captcha image should be displayed.
    /// </summary>
    public bool DisplayCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCaptcha"), false);
        }

        set
        {
            SetValue("DisplayCaptcha", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after registration failed.
    /// </summary>
    public string RegistrationErrorMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RegistrationErrorMessage"), String.Empty);
        }
        set
        {
            SetValue("RegistrationErrorMessage", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether user is enabled after registration.
    /// </summary>
    public bool EnableUserAfterRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableUserAfterRegistration"), true);
        }
        set
        {
            SetValue("EnableUserAfterRegistration", value);
        }
    }


    /// <summary>
    /// Gets or sets the sender email (from).
    /// </summary>
    public string FromAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FromAddress"), SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
        }
        set
        {
            SetValue("FromAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the recipient email (to).
    /// </summary>
    public string ToAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToAddress"), SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress"));
        }
        set
        {
            SetValue("ToAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether after successful registration is 
    /// notification email sent to the administrator 
    /// </summary>
    public bool NotifyAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("NotifyAdministrator"), false);
        }
        set
        {
            SetValue("NotifyAdministrator", value);
        }
    }


    /// <summary>
    /// Gets or sets the roles where is user assigned after successful registration.
    /// </summary>
    public string AssignRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToRoles"), String.Empty);
        }
        set
        {
            SetValue("AssignToRoles", value);
        }
    }


    /// <summary>
    /// Gets or sets the sites where is user assigned after successful registration.
    /// </summary>
    public string AssignToSites
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToSites"), String.Empty);
        }
        set
        {
            SetValue("AssignToSites", value);
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed after successful registration.
    /// </summary>
    public string DisplayMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMessage"), String.Empty);
        }
        set
        {
            SetValue("DisplayMessage", value);
        }
    }


    /// <summary>
    /// Gets or set the url where is user redirected after successful registration.
    /// </summary>
    public string RedirectToURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectToURL"), String.Empty);
        }
        set
        {
            SetValue("RedirectToURL", value);
        }
    }


    /// <summary>
    /// Gets or sets the default starting alias path for newly registered user.
    /// </summary>
    public string StartingAliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StartingAliasPath"), String.Empty);
        }
        set
        {
            SetValue("StartingAliasPath", value);
        }
    }

    #endregion


    #region "Conversion properties"

    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), String.Empty);
        }
        set
        {
            if ((value != null) && (value.Length > 400))
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


    #region "Methods"

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


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
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
            }

            // Set default visibility
            pnlRegForm.Visible = true;
            lblInfo.Visible = false;

            // WAI validation
            lblCaptcha.AssociatedControlClientID = captchaElem.InputClientID;

            // Get alternative form info
            AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeForm);
            if (afi != null)
            {
                formUser.AlternativeFormFullName = AlternativeForm;
                formUser.Info = new UserInfo();
                formUser.Info.ClearData();
                formUser.ClearAfterSave = false;
                formUser.Visible = true;
                formUser.ValidationErrorMessage = RegistrationErrorMessage;
                formUser.IsLiveSite = true;
                // Reload form if not in PortalEngine environment and if post back
                if ((StandAlone) && (RequestHelper.IsPostBack()))
                {
                    formUser.ReloadData();
                }

                captchaElem.Visible = DisplayCaptcha;
                lblCaptcha.Visible = DisplayCaptcha;
                plcCaptcha.Visible = DisplayCaptcha;

                btnRegister.Text = ButtonText;
                btnRegister.Click += btnRegister_Click;

                lblInfo.CssClass = "EditingFormInfoLabel";
                lblError.CssClass = "EditingFormErrorLabel";
                lblError.ForeColor = System.Drawing.Color.Red;

                if (formUser != null)
                {
                    // Set the live site context
                    formUser.ControlContext.ContextName = CMS.ExtendedControls.ControlContext.LIVE_SITE;
                }
            }
            else
            {
                lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeForm);
                lblError.Visible = true;
                pnlRegForm.Visible = false;
            }
        }
    }


    /// <summary>
    /// Page pre-render event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        // Hide default form submit button
        if (formUser != null)
        {
            formUser.SubmitButton.Visible = false;
        }
    }



    /// <summary>
    /// OK click handler (Proceed registration).
    /// </summary>
    private void btnRegister_Click(object sender, EventArgs e)
    {
        string currentSiteName = SiteContext.CurrentSiteName;
        string[] siteList = { currentSiteName };

        // If AssignToSites field set
        if (!String.IsNullOrEmpty(AssignToSites))
        {
            siteList = AssignToSites.Split(';');
        }

        if ((PageManager.ViewMode == ViewModeEnum.Design) || (HideOnCurrentPage) || (!IsVisible))
        {
            // Do not process
        }
        else
        {
            // Ban IP addresses which are blocked for registration
            if (!BannedIPInfoProvider.IsAllowed(currentSiteName, BanControlEnum.Registration))
            {
                lblError.Visible = true;
                lblError.Text = GetString("banip.ipisbannedregistration");
                return;
            }

            // Check if captcha is required and verify captcha text
            if (DisplayCaptcha && !captchaElem.IsValid())
            {
                // Display error message if captcha text is not valid
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.captchaError");
                return;
            }

            string userName = String.Empty;
            string nickName = String.Empty;
            string firstName = String.Empty;
            string lastName = String.Empty;
            string emailValue = String.Empty;
            string pwd = string.Empty;
            string confPassword = string.Empty;
            string educationLevel = String.Empty;
            string interestArea = String.Empty;
            string industry = String.Empty;
            string referralSource = string.Empty;

            // Check duplicate user
            // 1. Find appropriate control and get its value (i.e. user name)
            // 2. Try to find user info
            //FormEngineUserControl txtUserName = formUser.FieldControls["UserName"];
            //if (txtUserName != null)
            //{
            //    userName = ValidationHelper.GetString(txtUserName.Value, String.Empty);
            //}

            FormEngineUserControl txtEmail = formUser.FieldControls["Email"];
            if (txtEmail != null)
            {
                emailValue = ValidationHelper.GetString(txtEmail.Value, String.Empty);
                userName = emailValue;
            }

            // If user name and e-mail aren't filled stop processing and display error.
            if (string.IsNullOrEmpty(userName) && String.IsNullOrEmpty(emailValue))
            {
                formUser.StopProcessing = true;
                lblError.Visible = true;
                lblError.Text = GetString("customregistrationform.usernameandemail");
                return;
            }
            else
            {
                formUser.Data.SetValue("UserName", userName);
            }

            //check if email is valid
            if (!ValidationHelper.IsEmail(txtEmail.Text.ToLowerCSafe()))
            {
                lblError.Visible = true;
                lblError.Text = GetString("Webparts_Membership_RegistrationForm.EmailIsNotValid");
                return;
            }

            FormEngineUserControl txtNickName = formUser.FieldControls["UserNickName"];
            if (txtNickName != null)
            {
                nickName = ValidationHelper.GetString(txtNickName.Value, String.Empty);
            }

            FormEngineUserControl txtFirstName = formUser.FieldControls["FirstName"];
            if (txtFirstName != null)
            {
                firstName = ValidationHelper.GetString(txtFirstName.Value, String.Empty);
            }

            FormEngineUserControl txtLastName = formUser.FieldControls["LastName"];
            if (txtLastName != null)
            {
                lastName = ValidationHelper.GetString(txtLastName.Value, String.Empty);
            }

            FormEngineUserControl txtPwd = formUser.FieldControls["UserPassword"];
            if (txtPwd != null)
            {
                pwd = ValidationHelper.GetString(txtPwd.Value, String.Empty);
            }

            FormEngineUserControl txtConfPassword = formUser.FieldControls["ReenterPassword"];
            if (txtConfPassword != null)
            {
                confPassword = ValidationHelper.GetString(txtConfPassword.Value, String.Empty);
            }

            if (string.IsNullOrEmpty(pwd) || string.IsNullOrEmpty(confPassword))
            {
                lblError.Visible = true;
                lblError.Text = "please enter password with confirmation";
                return;
            }

            if (pwd != confPassword)
            {
                lblError.Visible = true;
                lblError.Text = "Password doesn't match";
                return;
            }


            if (validateFields(formUser.FieldControls["UserPassword"].Value.ToString()))
            {

                // Test if "global" or "site" user exists. 
                SiteInfo si = SiteContext.CurrentSite;
                UserInfo siteui = UserInfoProvider.GetUserInfo(UserInfoProvider.EnsureSitePrefixUserName(userName, si));
                if ((UserInfoProvider.GetUserInfo(userName) != null) || (siteui != null))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserAlreadyExists").Replace("%%name%%", HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, true)));
                    return;
                }

                // Check for reserved user names like administrator, sysadmin, ...
                if (UserInfoProvider.NameIsReserved(currentSiteName, userName))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserNameReserved").Replace("%%name%%", HTMLHelper.HTMLEncode(Functions.GetFormattedUserName(userName, true)));
                    return;
                }

                if (UserInfoProvider.NameIsReserved(currentSiteName, nickName))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("Webparts_Membership_RegistrationForm.UserNameReserved").Replace("%%name%%", HTMLHelper.HTMLEncode(nickName));
                    return;
                }

                // Check limitations for site members
                if (!UserInfoProvider.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.SiteMembers, ObjectActionEnum.Insert, false))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("License.MaxItemsReachedSiteMember");
                    return;
                }

                // Check whether email is unique if it is required
                if (!UserInfoProvider.IsEmailUnique(emailValue, siteList, 0))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("UserInfo.EmailAlreadyExist");
                    return;
                }

                // Validate and save form with new user data
                if (!formUser.Save())
                {
                    // Return if saving failed
                    return;
                }

                // Get user info from form
                UserInfo ui = (UserInfo)formUser.Info;

                // Add user prefix if settings is on
                // Ensure site prefixes
                if (UserInfoProvider.UserNameSitePrefixEnabled(currentSiteName))
                {
                    ui.UserName = UserInfoProvider.EnsureSitePrefixUserName(userName, si);
                }

                ui.Enabled = EnableUserAfterRegistration;
                ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
                ui.UserCampaign = AnalyticsHelper.Campaign;

                ui.SetPrivilegeLevel(UserPrivilegeLevelEnum.None);

                // Fill optionally full user name
                if (String.IsNullOrEmpty(ui.FullName))
                {
                    ui.FullName = UserInfoProvider.GetFullName(ui.FirstName, ui.MiddleName, ui.LastName);
                }

                // Ensure nick name
                if (ui.UserNickName.Trim() == String.Empty)
                {
                    ui.UserNickName = Functions.GetFormattedUserName(ui.UserName, true);
                }

                ui.UserSettings.UserRegistrationInfo.IPAddress = RequestContext.UserHostAddress;
                ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;
                ui.UserSettings.UserLogActivities = true;
                ui.UserSettings.UserShowIntroductionTile = true;

                // Check whether confirmation is required
                bool requiresConfirmation = SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSRegistrationEmailConfirmation");
                bool requiresAdminApprove = SettingsKeyInfoProvider.GetBoolValue(currentSiteName + ".CMSRegistrationAdministratorApproval");
                if (!requiresConfirmation)
                {
                    // If confirmation is not required check whether administration approval is reqiures
                    if (requiresAdminApprove)
                    {
                        ui.Enabled = false;
                        ui.UserSettings.UserWaitingForApproval = true;
                    }
                }
                else
                {
                    // EnableUserAfterRegistration is overrided by requiresConfirmation - user needs to be confirmed before enable
                    ui.Enabled = false;
                }

                // Set user's starting alias path
                if (!String.IsNullOrEmpty(StartingAliasPath))
                {
                    ui.UserStartingAliasPath = MacroResolver.ResolveCurrentPath(StartingAliasPath);
                }

                // Get user password and save it in apropriate format after form save
                string password = ValidationHelper.GetString(ui.GetValue("UserPassword"), String.Empty);
                UserInfoProvider.SetPassword(ui, password);

                var customerToken = PersonifyRegistered(emailValue, password, firstName, lastName);
                if (string.IsNullOrEmpty(customerToken))
                {
                    UserInfoProvider.DeleteUser(ui);
                    return;
                }
                else
                {
                    var roles = GetImsroles(customerToken);
                    string groupslist = "";
                    if (roles.Length > 0)
                    {
                        foreach (string s in roles)
                        {
                            if (s.Length > 0)
                            {
                                groupslist += s + ",";
                            }
                        }
                    }

                    //we need this mispelling.
                    groupslist += "peronifyUser" + ",";

                    new LoginUsertokentico().AddUserToRole(ui, groupslist, true, false);
                }


                // Prepare macro data source for email resolver
                UserInfo userForMail = ui.Clone();
                userForMail.SetValue("UserPassword", string.Empty);

                object[] data = new object[1];
                data[0] = userForMail;

                // Prepare resolver for notification and welcome emails
                MacroResolver resolver = MacroContext.CurrentResolver;
                resolver.SetAnonymousSourceData(data);

                #region "Welcome Emails (confirmation, waiting for approval)"

                bool error = false;
                EmailTemplateInfo template = null;

                // Prepare macro replacements
                string[,] replacements = new string[6, 2];
                replacements[0, 0] = "confirmaddress";
                replacements[0, 1] = AuthenticationHelper.GetRegistrationApprovalUrl(ApprovalPage, ui.UserGUID, currentSiteName, NotifyAdministrator);
                replacements[1, 0] = "username";
                replacements[1, 1] = userName;
                replacements[2, 0] = "password";
                replacements[2, 1] = password;
                replacements[3, 0] = "Email";
                replacements[3, 1] = emailValue;
                replacements[4, 0] = "FirstName";
                replacements[4, 1] = firstName;
                replacements[5, 0] = "LastName";
                replacements[5, 1] = lastName;

                // Set resolver
                resolver.SetNamedSourceData(replacements);

                // Email message
                EmailMessage emailMessage = new EmailMessage();
                emailMessage.EmailFormat = EmailFormatEnum.Default;
                emailMessage.Recipients = ui.Email;

                // Send welcome message with username and password, with confirmation link, user must confirm registration
                if (requiresConfirmation)
                {
                    template = EmailTemplateProvider.GetEmailTemplate("RegistrationConfirmation", currentSiteName);
                    emailMessage.Subject = GetString("RegistrationForm.RegistrationConfirmationEmailSubject");
                }
                // Send welcome message with username and password, with information that user must be approved by administrator
                else if (SendWelcomeEmail)
                {
                    if (requiresAdminApprove)
                    {
                        template = EmailTemplateProvider.GetEmailTemplate("Membership.RegistrationWaitingForApproval", currentSiteName);
                        emailMessage.Subject = GetString("RegistrationForm.RegistrationWaitingForApprovalSubject");
                    }
                    // Send welcome message with username and password, user can logon directly
                    else
                    {
                        template = EmailTemplateProvider.GetEmailTemplate("Membership.Registration", currentSiteName);
                        emailMessage.Subject = GetString("RegistrationForm.RegistrationSubject");
                    }
                }

                if (template != null)
                {
                    emailMessage.From = EmailHelper.GetSender(template, SettingsKeyInfoProvider.GetStringValue(currentSiteName + ".CMSNoreplyEmailAddress"));
                    // Enable macro encoding for body
                    resolver.Settings.EncodeResolvedValues = true;
                    emailMessage.Body = resolver.ResolveMacros(template.TemplateText);
                    // Disable macro encoding for plaintext body and subject
                    resolver.Settings.EncodeResolvedValues = false;
                    emailMessage.PlainTextBody = resolver.ResolveMacros(template.TemplatePlainText);
                    emailMessage.Subject = resolver.ResolveMacros(EmailHelper.GetSubject(template, emailMessage.Subject));

                    emailMessage.CcRecipients = template.TemplateCc;
                    emailMessage.BccRecipients = template.TemplateBcc;

                    try
                    {
                        EmailHelper.ResolveMetaFileImages(emailMessage, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                        // Send the e-mail immediately
                        EmailSender.SendEmail(currentSiteName, emailMessage, true);
                    }
                    catch (Exception ex)
                    {
                        EventLogProvider.LogException("E", "RegistrationForm - SendEmail", ex);
                        error = true;
                    }
                }

                // If there was some error, user must be deleted
                if (error)
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("RegistrationForm.UserWasNotCreated");

                    // Email was not send, user can't be approved - delete it
                    UserInfoProvider.DeleteUser(ui);
                    return;
                }

                #endregion


                #region "Administrator notification email"

                // Notify administrator if enabled and email confirmation is not required
                if (!requiresConfirmation && NotifyAdministrator && (FromAddress != String.Empty) && (ToAddress != String.Empty))
                {
                    EmailTemplateInfo mEmailTemplate = null;

                    if (requiresAdminApprove)
                    {
                        mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.Approve", currentSiteName);
                    }
                    else
                    {
                        mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.New", currentSiteName);
                    }

                    if (mEmailTemplate == null)
                    {
                        EventLogProvider.LogEvent(EventType.ERROR, "RegistrationForm", "GetEmailTemplate", eventUrl: RequestContext.RawURL);
                    }
                    else
                    {
                        // E-mail template ok
                        replacements = new string[4, 2];
                        replacements[0, 0] = "firstname";
                        replacements[0, 1] = ui.FirstName;
                        replacements[1, 0] = "lastname";
                        replacements[1, 1] = ui.LastName;
                        replacements[2, 0] = "email";
                        replacements[2, 1] = ui.Email;
                        replacements[3, 0] = "username";
                        replacements[3, 1] = userName;

                        // Set resolver
                        resolver.SetNamedSourceData(replacements);
                        // Enable macro encoding for body
                        resolver.Settings.EncodeResolvedValues = true;

                        EmailMessage message = new EmailMessage();
                        message.EmailFormat = EmailFormatEnum.Default;
                        message.From = EmailHelper.GetSender(mEmailTemplate, FromAddress);
                        message.Recipients = ToAddress;
                        message.Body = resolver.ResolveMacros(mEmailTemplate.TemplateText);
                        // Disable macro encoding for plaintext body and subject
                        resolver.Settings.EncodeResolvedValues = false;
                        message.Subject = resolver.ResolveMacros(EmailHelper.GetSubject(mEmailTemplate, GetString("RegistrationForm.EmailSubject")));
                        message.PlainTextBody = resolver.ResolveMacros(mEmailTemplate.TemplatePlainText);

                        message.CcRecipients = mEmailTemplate.TemplateCc;
                        message.BccRecipients = mEmailTemplate.TemplateBcc;

                        try
                        {
                            // Attach template meta-files to e-mail
                            EmailHelper.ResolveMetaFileImages(message, mEmailTemplate.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                            EmailSender.SendEmail(currentSiteName, message);
                        }
                        catch
                        {
                            EventLogProvider.LogEvent(EventType.ERROR, "Membership", "RegistrationEmail");
                        }
                    }
                }

                #endregion


                #region "Web analytics"

                // Track successful registration conversion
                if (TrackConversionName != String.Empty)
                {
                    if (AnalyticsHelper.AnalyticsEnabled(currentSiteName) && AnalyticsHelper.TrackConversionsEnabled(currentSiteName) && !AnalyticsHelper.IsIPExcluded(currentSiteName, RequestContext.UserHostAddress))
                    {
                        HitLogProvider.LogConversions(currentSiteName, LocalizationContext.PreferredCultureCode, TrackConversionName, 0, ConversionValue);
                    }
                }

                // Log registered user if confirmation is not required
                if (!requiresConfirmation)
                {
                    AnalyticsHelper.LogRegisteredUser(currentSiteName, ui);
                }

                #endregion


                #region "On-line marketing - activity"

                // Log registered user if confirmation is not required
                if (!requiresConfirmation)
                {
                    Activity activity = new ActivityRegistration(ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                    if (activity.Data != null)
                    {
                        activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                        activity.Log();
                    }

                    // Log login activity
                    if (ui.Enabled)
                    {
                        // Log activity
                        int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                        Activity activityLogin = new ActivityUserLogin(contactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                        activityLogin.Log();
                    }
                }

                #endregion

                #region "Site and roles addition and authentication"

                string[] roleList = AssignRoles.Split(';');

                foreach (string siteName in siteList)
                {
                    // Add new user to the current site
                    UserInfoProvider.AddUserToSite(ui.UserName, siteName);
                    foreach (string roleName in roleList)
                    {
                        if (!String.IsNullOrEmpty(roleName))
                        {
                            String sn = roleName.StartsWithCSafe(".") ? String.Empty : siteName;

                            // Add user to desired roles
                            if (RoleInfoProvider.RoleExists(roleName, sn))
                            {
                                UserInfoProvider.AddUserToRole(ui.UserName, roleName, sn);
                            }
                        }
                    }
                }
                if (ui.Enabled)
                {
                    if (this.AutoLoginAfterRegistration)
                    {
                        Session["UserName"] = userName;
                        Session["Password"] = password;
                        Session["RememberMe"] = true;
                        Session["RetryCount"] = null;

                        if (this.Request.QueryString["ReturnURL"] != null)
                        {
                            var returnURL = this.Request.QueryString["ReturnURL"];

                            Session["ReturnURL"] = returnURL;
                        }
                        else if (!String.IsNullOrEmpty(this.RedirectToURL))
                        {
                            var returnURL = this.Request.QueryString["ReturnURL"];

                            Session["ReturnURL"] = returnURL;
                        }
                        Response.Redirect("~/sso/ssohandler.aspx", true);
                    }
                    else if (!String.IsNullOrEmpty(this.LoginURL))
                    {
                        Response.Redirect(string.Format(this.LoginURL, userName), true);
                    }
                    else if (!String.IsNullOrEmpty(this.RedirectToURL))
                    {
                        Response.Redirect(this.RedirectToURL, true);
                    }
                }
                #endregion

                lblError.Visible = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="masterCustomerId"></param>
    /// <param name="subCustomerId"></param>
    /// <returns></returns>
    private string[] GetImsroles(string customerToken)
    {
        using (var myim = new IMService { Url = PersonifyImsUrl })
        {
            var imsCustomerRoleGetResult = myim.IMSCustomerRoleGet(PersonifyVendorName, PersonifyVendorPassword, customerToken);

            if (imsCustomerRoleGetResult.CustomerRoles == null)
                return new[] { string.Empty };

            return imsCustomerRoleGetResult.CustomerRoles.Select(x => x.Value).ToArray();
        }
    }

    /// <summary>
    /// check if customer alresdy exists in Personify
    /// </summary>
    /// <param name="userName">userName</param>
    /// <param name="password">password</param>
    /// <param name="firstName">firstName</param>
    /// <param name="lastName">lastName</param>
    /// <returns>the customer token</returns>
    private string PersonifyRegistered(string userName, string password, string firstName, string lastName)
    {
        var customerToken = String.Empty;
        try
        {
            var customer = ssoClient.SSOCustomerGetByUsername(PersonifyVendorName, PersonifyVendorPassword, userName);
            if (!customer.UserExists)
            {
                var newPersonifyCustomer = ssoClient.SSOCustomerRegister(PersonifyVendorName, PersonifyVendorPassword, userName, password, userName, firstName, lastName, true);

                if (newPersonifyCustomer.Result == false)
                {
                    //TODO: Need to check all of these errors.
                    lblError.Text = newPersonifyCustomer.Errors.FirstOrDefault();
                    lblError.Visible = true;
                    return String.Empty;
                }

                customerToken = newPersonifyCustomer.NewCustomerToken;

                //creating customer presonify info
                if (!CreateCustomerInf(userName, firstName, lastName))
                {
                    customerToken = String.Empty;
                    //error disable customer account 
                    ssoClient.SSOEnableDisableCustomerAccount(PersonifyVendorName, PersonifyVendorPassword, userName, true);
                }
            }
            else if (customer.UserExists && customer.DisableAccountFlag)// if customer account is disabled
            {
                //enable customer account 
                ssoClient.SSOEnableDisableCustomerAccount(PersonifyVendorName, PersonifyVendorPassword, userName, false);
                //creating customer presonify info
                if (!CreateCustomerInf(userName, firstName, lastName))
                {
                    customerToken = String.Empty;
                    //error disable customer account
                    ssoClient.SSOEnableDisableCustomerAccount(PersonifyVendorName, PersonifyVendorPassword, userName, true);
                }
            }
            else
            {
                customerToken = String.Empty;
                lblError.Visible = true;
                lblError.Text = GetString("UserInfo.EmailAlreadyExist");
            }
        }
        catch (DataServiceClientException dscex)
        {
            var messages = dscex.ParseMessages();

            var errorMessage = messages.ValidationIssues.ValidationMessage.Aggregate(new StringBuilder(), (x, y) => x.AppendLine(y.Message)).ToString();

            customerToken = String.Empty;
            lblError.Visible = true;
            lblError.Text = errorMessage;
            EventLogProvider.LogException(dscex.Source, dscex.Message, dscex);
        }
        catch (Exception ex)
        {
            customerToken = String.Empty;
            lblError.Visible = true;
            lblError.Text = RegistrationErrorMessage;
            EventLogProvider.LogException(ex.Source, ex.Message, ex);
        }

        return customerToken;
    }

    /// <summary>
    /// Create customer personify info object
    /// </summary>
    /// <param name="firstName">firstName</param>
    /// <param name="lastName">lastName</param>
    public bool CreateCustomerInf(string userName, string firstName, string lastName)
    {
        string personifyAddress = string.Empty, address1 = string.Empty, address2 = string.Empty, city = string.Empty, state = string.Empty, zip = string.Empty, country = string.Empty;
        bool custInfCreated = false;
        try
        {
            FormEngineUserControl PersonifyAddress = formUser.FieldControls["PersonifyAddress"];
            if (PersonifyAddress != null)
            {
                personifyAddress = ValidationHelper.GetString(PersonifyAddress.Value, String.Empty);
            }
            var address = personifyAddress.Split('|');
            address1 = address[0];
            address2 = address[1];
            city = address[2];
            state = address[3];
            country = address[4];
            zip = address[5];
            //Create an individual Customer with 1 address linked to employer and another individual address
            var a = new DataServiceCollection<SaveAddressInput>(null, TrackingMode.None);

            //Address linked to a company
            SaveAddressInput a1 = new SaveAddressInput()
            {
                AddressTypeCode = "WORK",
                Address1 = address1,
                Address2 = address2,
                City = city,
                State = state,
                PostalCode = zip,
                CountryCode = country,
                OverrideAddressValidation = false,
                SetRelationshipAsPrimary = true,
                PrimaryAddress = true,
                CreateNewAddressIfOrdersExist = true,
                EndOldPrimaryRelationship = true,
                WebMobileDirectory = true,
            };

            a.Add(a1);

            CusCommunicationInput emailInput = new CusCommunicationInput();
            emailInput.CommLocationCode = "WORK";
            emailInput.CommTypeCode = "EMAIL";
            emailInput.PrimaryFlag = true;
            emailInput.ActiveFlag = true;
            emailInput.CountryCode = country;
            emailInput.FormattedPhoneAddress = userName;

            SaveCustomerInput s = new SaveCustomerInput()
            {
                FirstName = firstName,
                LastName = lastName,
                CustomerClassCode = "INDIV",
                Addresses = a,
                Communication = new DataServiceCollection<CusCommunicationInput>(null, TrackingMode.None)
            };

            s.Communication.Add(emailInput);


            SaveCustomerOutput op = PersonifyDataServiceClient.Post<SaveCustomerOutput>("CreateIndividual", s);

            if (!String.IsNullOrEmpty(op.MasterCustomerId))
            {
                var newIdentifier = op.MasterCustomerId + "|0";
                custInfCreated = true;
                var result = ssoClient.TIMSSCustomerIdentifierSet(PersonifyVendorName, PersonifyVendorPassword, userName, newIdentifier);

                custInfCreated = result.CustomerIdentifier == newIdentifier;

                if (!custInfCreated)
                {
                    throw new Exception(result.Errors.FirstOrDefault());
                }

                FormEngineUserControl txtEducationLevel = formUser.FieldControls["EducationLevel"];
                if (txtEducationLevel != null)
                {
                    var educationLevel = ValidationHelper.GetString(txtEducationLevel.Value, String.Empty);

                    if (!String.IsNullOrEmpty(educationLevel))
                    {
                        var demo1 = PersonifyDataServiceClient.Create<CusDemographicList>();
                        demo1.MasterCustomerId = op.MasterCustomerId;
                        demo1.SubCustomerId = op.SubCustomerId;
                        demo1.DemographicCode = "EDUC_LEVEL";
                        demo1.DemographicSubcode = educationLevel;
                        var result1 = PersonifyDataServiceClient.Save<CusDemographicList>(demo1);
                    }

                }

                FormEngineUserControl txtReferralSource = formUser.FieldControls["ReferralSource"];
                if (txtReferralSource != null)
                {
                    var referralSource = ValidationHelper.GetString(txtReferralSource.Value, String.Empty);

                    if (!String.IsNullOrEmpty(referralSource))
                    {
                        var demo2 = PersonifyDataServiceClient.Create<CusDemographicList>();
                        demo2.MasterCustomerId = op.MasterCustomerId;
                        demo2.SubCustomerId = op.SubCustomerId;
                        demo2.DemographicCode = "REFERRAL_SOURCE";
                        demo2.DemographicSubcode = referralSource;
                        var result2 = PersonifyDataServiceClient.Save<CusDemographicList>(demo2);
                    }
                }

                FormEngineUserControl txtInterestArea = formUser.FieldControls["InterestArea"];
                if (txtInterestArea != null)
                {
                    var interestArea = ValidationHelper.GetString(txtInterestArea.Value, String.Empty);

                    if (!String.IsNullOrEmpty(interestArea))
                    {
                        var demo3 = PersonifyDataServiceClient.Create<CusDemographicList>();
                        demo3.MasterCustomerId = op.MasterCustomerId;
                        demo3.SubCustomerId = op.SubCustomerId;
                        demo3.DemographicCode = "INT_AREA";
                        demo3.DemographicSubcode = interestArea;
                        var result3 = PersonifyDataServiceClient.Save<CusDemographicList>(demo3);
                    }
                }

                FormEngineUserControl txtIndustry = formUser.FieldControls["Industry"];
                if (txtIndustry != null)
                {
                    var industry = ValidationHelper.GetString(txtIndustry.Value, String.Empty);

                    if (!String.IsNullOrEmpty(industry))
                    {
                        var demo4 = PersonifyDataServiceClient.Create<CusDemographicList>();
                        demo4.MasterCustomerId = op.MasterCustomerId;
                        demo4.SubCustomerId = op.SubCustomerId;
                        demo4.DemographicCode = "INDUSTRY";
                        demo4.DemographicSubcode = industry;

                        var result4 = PersonifyDataServiceClient.Save<CusDemographicList>(demo4);
                    }
                }


            }
            else
            {
                throw new Exception("created individual, but master customer id was empty.");
            }
        }
        catch (DataServiceClientException ex)
        {
            //disable customer account
            ssoClient.SSOEnableDisableCustomerAccount(PersonifyVendorName, PersonifyVendorPassword, userName, true);
            custInfCreated = false;
            throw ex;
        }
        return custInfCreated;
    }

    /// <summary>
    /// validate email and password for SSO customer creation
    /// </summary>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <param name="fName"></param>
    /// <param name="lName"></param>
    /// <returns></returns>
    private bool validateFields(string password)
    {
        bool valid = false;
        var service = new service();
        string resPassword = service.SSOCustomerPasswordIsStrong(PersonifyVendorName, PersonifyVendorPassword, password);
        if (string.IsNullOrEmpty(resPassword))
            valid = true;
        else
        {
            valid = false;
            lblError.Visible = true;
            lblError.Text = resPassword;
            return valid;
        }

        return valid;
    }

    #endregion
}

