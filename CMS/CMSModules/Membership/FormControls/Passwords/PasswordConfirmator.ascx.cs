using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;

public partial class CMSModules_Membership_FormControls_Passwords_PasswordConfirmator : FormEngineUserControl
{
    #region "Constants"

    private const string hiddenPassword = "********";

    #endregion


    #region "Properties"

    /// <summary>
    /// Crypted password.
    /// </summary>
    private string CryptedPassword
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CryptedPassword"], string.Empty);
        }
        set
        {
            ViewState["CryptedPassword"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtConfirmPassword.Enabled = value;
            passStrength.Enabled = value;
        }
    }


    /// <summary>
    /// Returns encrypted password.
    /// </summary>
    public override object Value
    {
        get
        {
            // Check if text is set
            if (string.IsNullOrEmpty(passStrength.Text))
            {
                passStrength.TextBoxAttributes.Add("value", string.Empty);
                txtConfirmPassword.Attributes.Add("value", string.Empty);
                return string.Empty;
            }

            // Check if password changed
            if (passStrength.Text.EqualsCSafe(hiddenPassword, true))
            {
                return CryptedPassword;
            }

            // Get salt and format
            string salt = null;
            string format = SettingsKeyInfoProvider.GetStringValue("CMSPasswordFormat");
            UserInfo ui = Form.EditedObject as UserInfo;
            if (ui != null)
            {
                salt = ui.UserGUID.ToString();
                format = ui.PasswordFormat;
            }

            passStrength.TextBoxAttributes.Add("value", hiddenPassword);
            txtConfirmPassword.Attributes.Add("value", hiddenPassword);

            CryptedPassword = UserInfoProvider.GetPasswordHash(passStrength.Text, format, salt);
            return CryptedPassword;
        }
        set
        {
            CryptedPassword = ValidationHelper.GetString(value, string.Empty);

            if (!string.IsNullOrEmpty(CryptedPassword))
            {
                passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                txtConfirmPassword.Attributes.Add("value", hiddenPassword);
            }
            else
            {
                passStrength.TextBoxAttributes.Add("value", string.Empty);
                txtConfirmPassword.Attributes.Add("value", string.Empty);
            }
        }
    }


    /// <summary>
    /// Client ID of primary input control.
    /// </summary>
    public override string InputClientID
    {
        get
        {
            return passStrength.ValueElementID;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Page load event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        passStrength.ShowStrengthIndicator = ValidationHelper.GetBoolean(GetValue("showstrength"), true);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (passStrength.Text.EqualsCSafe(hiddenPassword, true))
        {
            return true;
        }

        // Check if passwords match
        if (passStrength.Text != txtConfirmPassword.Text)
        {
            ValidationError = GetString("PassConfirmator.PasswordDoNotMatch");
        }

        // Check regular expresion
        string regularExpression = FieldInfo.RegularExpression;
        if ((!String.IsNullOrEmpty(regularExpression)) && (new Validator().IsRegularExp(passStrength.Text, regularExpression, "error").Result.EqualsCSafe("error", true)))
        {
            ValidationError = GetString("PassConfirmator.InvalidPassword");
        }

        // Check min lenght
        int minLength = ValidationHelper.GetInteger(FieldInfo.MinValue, -1);
        if ((minLength > 0) && (passStrength.Text.Length < minLength))
        {
            ValidationError = string.Format(GetString("PassConfirmator.PasswordLength"), minLength);
        }

        // Check password policy
        if (!passStrength.IsValid())
        {
            ValidationError = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
        }

        if (!String.IsNullOrEmpty(ValidationError))
        {
            Value = string.Empty;
            return false;
        }

        return true;
    }

    #endregion
}