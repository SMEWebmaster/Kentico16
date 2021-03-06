using System;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_User_New : CMSUsersPage
{
    #region "Variables"

    private String userName = String.Empty;
    private bool error;

    #endregion


    private bool AllowAssignToWebsite
    {
        get
        {
            return (SiteID <= 0) && (SiteContext.CurrentSiteID > 0) && CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin);
        }
    }


    #region "Public methods"

    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        ucUserName.UseDefaultValidationGroup = false;

        LabelConfirmPassword.Text = GetString("Administration-User_New.ConfirmPassword");
        LabelPassword.Text = GetString("Administration-User_New.Password");
        RequiredFieldValidatorFullName.ErrorMessage = GetString("Administration-User_New.RequiresFullName");

        if (!RequestHelper.IsPostBack())
        {
            CheckBoxEnabled.Checked = true;

            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                // Remove global and site admin options for non global admins.
                drpPrivilegeLevel.ExcludedValues = (int)UserPrivilegeLevelEnum.GlobalAdmin + ";" + (int)UserPrivilegeLevelEnum.Admin;
            }

            drpPrivilegeLevel.Value = (int)UserPrivilegeLevelEnum.Editor;
        }

        if (AllowAssignToWebsite)
        {
            chkAssignToSite.Text = String.Format("{0} {1}", GetString("general.assignwithwebsite"), HTMLHelper.HTMLEncode(SiteContext.CurrentSiteName));
            plcAssignToSite.Visible = true;
        }

        string users = GetString("general.users");
        string currentUser = GetString("Administration-User_New.CurrentUser");

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = users,
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("ParentElem", "")), "displaytitle=false"),
            Target = "_parent"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem
        {
            Text = currentUser
        });
    }


    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        // Email format validation
        string emailAddress = TextBoxEmail.Text.Trim();
        if (!String.IsNullOrEmpty(emailAddress) && !ValidationHelper.IsEmail(emailAddress))
        {
            ShowError(GetString("Administration-User_New.WrongEmailFormat"));
            return;
        }

        // Find whether user name is valid
        string result = null;
        if (!ucUserName.IsValid())
        {
            result = ucUserName.ValidationError;
        }

        // Additional validation
        if (String.IsNullOrEmpty(result))
        {
            result = new Validator().NotEmpty(TextBoxFullName.Text, GetString("Administration-User_New.RequiresFullName")).Result;
        }

        userName = ValidationHelper.GetString(ucUserName.Value, String.Empty).Trim();

        // Check if user with the same user name exists 
        if (UserInfoProvider.GetUserInfo(userName) != null)
        {
            ShowError(GetString("Administration-User_New.UserExists"));
            return;
        }

        // Check if username with site prefix exists on current site  
        var userNameWithPrefix = UserInfoProvider.GetUserInfo(UserInfoProvider.EnsureSitePrefixUserName(userName, SiteContext.CurrentSite));
        if (userNameWithPrefix != null)
        {
            ShowError(GetString("Administration-User_New.siteprefixeduserexists"));
            return;
        }
      
        // If site prefixed allowed - add site prefix
        if (((SiteID != 0) || (chkAssignToSite.Checked && AllowAssignToWebsite)) && UserInfoProvider.UserNameSitePrefixEnabled(SiteContext.CurrentSiteName))
        {
            if (!UserInfoProvider.IsSitePrefixedUser(userName))
            {
                userName = UserInfoProvider.EnsureSitePrefixUserName(userName, SiteContext.CurrentSite);
            }
        }
        // User without site prefix is going to be created -> check if site prefixed user does not exist in solution
        else if (!UserInfoProvider.IsUserNamePrefixUnique(userName, 0))
        {
            ShowError(GetString("Administration-User_New.siteprefixeduserexists"));
            return;
        }

        if (result == "")
        {
            if (TextBoxConfirmPassword.Text == passStrength.Text)
            {
                // Check whether password is valid according to policy
                if (passStrength.IsValid())
                {
                    int userId = SaveNewUser();
                    if (userId != -1)
                    {
                        var uiElementUrl = UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("editelem", ""), false);
                        var url = URLHelper.AppendQuery(uiElementUrl, "siteid=" + SiteID + "&objectid=" + userId);
                        URLHelper.Redirect(url);
                    }
                }
                else
                {
                    ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
                }
            }
            else
            {
                ShowError(GetString("Administration-User_Edit_Password.PasswordsDoNotMatch"));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Saves new user's data into DB.
    /// </summary>
    /// <returns>Returns ID of created user</returns>
    protected int SaveNewUser()
    {
        UserInfo ui = new UserInfo();

        // Load default values
        FormHelper.LoadDefaultValues("cms.user", ui);

        string emailAddress = TextBoxEmail.Text.Trim();
        ui.PreferredCultureCode = "";
        ui.Email = emailAddress;
        ui.FirstName = "";
        ui.FullName = TextBoxFullName.Text;
        ui.LastName = "";
        ui.MiddleName = "";
        ui.UserName = userName;
        ui.Enabled = CheckBoxEnabled.Checked;
        ui.IsExternal = false;

        // Set privilege level, global admin may set all levels, rest only editor.
        UserPrivilegeLevelEnum privilegeLevel = (UserPrivilegeLevelEnum)drpPrivilegeLevel.Value.ToInteger(0);
        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin)
            || (privilegeLevel == UserPrivilegeLevelEnum.None) || (privilegeLevel == UserPrivilegeLevelEnum.Editor))
        {
            ui.SetPrivilegeLevel(privilegeLevel);
        }

        // Check license limitations only in cmsdesk
        if (SiteID > 0)
        {
            string errorMessage = String.Empty;
            UserInfoProvider.CheckLicenseLimitation(ui, ref errorMessage);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
        }

        // Check whether email is unique if it is required
        string siteName = SiteName;
        bool assignUserToSite = chkAssignToSite.Checked && AllowAssignToWebsite;
        if (assignUserToSite)
        {
            siteName = SiteContext.CurrentSiteName;
        }

        if (!UserInfoProvider.IsEmailUnique(emailAddress, siteName, 0))
        {
            ShowError(GetString("UserInfo.EmailAlreadyExist"));
            return -1;
        }

        if (!error)
        {
            // Set password and save object
            UserInfoProvider.SetPassword(ui, passStrength.Text);

            // Add user to current site
            if ((SiteID > 0) || assignUserToSite)
            {
                UserInfoProvider.AddUserToSite(ui.UserName, siteName);
            }

            return ui.UserID;
        }

        return -1;
    }
}