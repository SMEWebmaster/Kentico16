using System;
using System.Collections;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Reporting;
using CMS.UIControls;

public partial class CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            txtEmail.Text = InfoToClone.GetStringValue("ReportSubscriptionEmail", "");
        }
    }


    /// <summary>
    /// Returns if input value is valid email address.
    /// </summary>
    public override bool IsValid(CloneSettings settings)
    {
        string value = txtEmail.Text;
        if (String.IsNullOrWhiteSpace(value) || !ValidationHelper.IsEmail(value))
        {
            ShowError(GetString("om.contact.enteremail"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[ReportSubscriptionInfo.OBJECT_TYPE + ".email"] = txtEmail.Text;
        return result;
    }

    #endregion
}