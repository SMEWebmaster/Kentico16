using System;
using System.Data;
using System.Text;

using CMS.CMSImportExport;
using CMS.Helpers;
using CMS.ImportExport;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSInstall_Controls_WizardSteps_SiteCreationDialog : CMSUserControl
{
    /// <summary>
    /// Type of the site creation.
    /// </summary>
    public enum CreationTypeEnum
    {
        /// <summary>
        /// Use template.
        /// </summary>
        Template = 0,

        /// <summary>
        /// Continue to the new site wizard.
        /// </summary>
        NewSiteWizard = 1,

        /// <summary>
        /// Import existing site.
        /// </summary>
        ExistingSite = 2
    }


    private bool mStopProcess = true;
    private string mCurrentEdition = null;
    private string mCurrentPackages = null;

    /// <summary>
    /// Template name to be created.
    /// </summary>
    public string TemplateName
    {
        get
        {
            return hdnName.Value;
        }
    }


    /// <summary>
    /// Stop processing.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return mStopProcess;
        }
        set
        {
            mStopProcess = value;
        }
    }


    /// <summary>
    /// Current edition.
    /// </summary>
    protected string CurrentEdition
    {
        get
        {
            if (mCurrentEdition == null)
            {
                // Get current domain and license
                string domain = RequestContext.CurrentDomain;
                LicenseKeyInfo li = LicenseKeyInfoProvider.GetLicenseKeyInfo(domain);
                if (li != null)
                {
                    mCurrentEdition = li.Edition.ToStringRepresentation<ProductEditionEnum>();
                }
            }
            return mCurrentEdition;
        }
    }


    /// <summary>
    /// Current edition.
    /// </summary>
    protected string CurrentPackages
    {
        get
        {
            if (mCurrentPackages == null)
            {
                // Get current domain and license
                string domain = RequestContext.CurrentDomain;
                LicenseKeyInfo li = LicenseKeyInfoProvider.GetLicenseKeyInfo(domain);
                if (li != null)
                {
                    mCurrentPackages = li.LicensePackages;
                }
            }
            return mCurrentPackages;
        }
    }


    /// <summary>
    /// Template name to be created.
    /// </summary>
    public CreationTypeEnum CreationType
    {
        get
        {
            if (radExisting.Checked)
            {
                return CreationTypeEnum.ExistingSite;
            }
            else if (radWizard.Checked)
            {
                return CreationTypeEnum.NewSiteWizard;
            }
            else
            {
                return CreationTypeEnum.Template;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("var hdnField=document.getElementById('" + hdnName.ClientID + "');var hdnLastSelected=document.getElementById('" + hdnLastSelected.ClientID + "');");

        if (RequestHelper.IsPostBack())
        {
            ltlScriptAfter.Text = ScriptHelper.GetScript("SelectTemplate(hdnLastSelected.value,hdnField.value);");
        }

        radWizard.Text = ResHelper.GetFileString("Install.radWizard");
        radTemplate.Text = ResHelper.GetFileString("Install.radTemplate");
        radExisting.Text = ResHelper.GetFileString("Install.radExisting");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Load web templates
        if (!StopProcessing)
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Gets metafile preview.
    /// </summary>
    /// <param name="objTemplateId">Template ID</param>
    protected string GetPreviewImage(object objTemplateId)
    {
        int templateId = ValidationHelper.GetInteger(objTemplateId, 0);

        DataSet dsPreview = MetaFileInfoProvider.GetMetaFiles(templateId, WebTemplateInfo.OBJECT_TYPE);
        if (!DataHelper.DataSourceIsEmpty(dsPreview))
        {
            string guid = ValidationHelper.GetString(dsPreview.Tables[0].Rows[0]["MetaFileGUID"], "");
            return ResolveUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + guid);
        }
        else
        {
            return GetImageUrl("Others/Install/no_image.png");
        }
    }


    /// <summary>
    /// Data bind.
    /// </summary>
    public void ReloadData()
    {
        if (WebTemplateInfoProvider.IsAnyTemplatePresent())
        {
            DataSet dsTemplates = WebTemplateInfoProvider.GetWebTemplates(null, "WebTemplateOrder", 0, null, true);
            if (!DataHelper.DataSourceIsEmpty(dsTemplates))
            {
                string firstTemplateName = "";
                int firstTemplateId = 0;

                // Find first allowed template
                foreach (DataRow row in dsTemplates.Tables[0].Rows)
                {
                    string templateEditions = ValidationHelper.GetString(row["WebTemplateLicenses"], "");
                    string templatePackages = ValidationHelper.GetString(row["WebTemplatePackages"], "");
                    if (IsTemplateAllowed(templateEditions, templatePackages))
                    {
                        firstTemplateName = ValidationHelper.GetString(row["WebTemplateName"], "");
                        firstTemplateId = ValidationHelper.GetInteger(row["WebTemplateID"], 0);
                        break;
                    }
                }

                // Bind the list
                rptSites.DataSource = dsTemplates;
                rptSites.DataBind();

                // Select Corporate site by default
                if (string.IsNullOrEmpty(hdnLastSelected.Value) || (hdnLastSelected.Value == "0"))
                {
                    ltlScriptAfter.Text += ScriptHelper.GetScript("SelectTemplate('tpl" + firstTemplateId + "','" + firstTemplateName + "');");
                }
            }
        }
        else
        {
            plcInfo.Visible = true;
            lblInfo.Text = ResHelper.GetFileString("Install.TemplatesWarning");
            radWizard.Enabled = false;
            radTemplate.Enabled = false;
            plcTemplates.Visible = false;
            radExisting.Checked = true;
        }
    }


    protected string GetItemHTML(object dataItem)
    {
        DataRowView view = (DataRowView)dataItem;
        string templateId = ValidationHelper.GetString(view["WebTemplateID"], "");
        string templateName = ValidationHelper.GetString(view["WebTemplateName"], "");
        string templateDisplayName = ValidationHelper.GetString(view["WebTemplateDisplayName"], "");
        string templateDescription = ValidationHelper.GetString(view["WebTemplateDescription"], "");
        string templateEditions = ValidationHelper.GetString(view["WebTemplateLicenses"], "");
        string templatePackages = ValidationHelper.GetString(view["WebTemplatePackages"], "");
        bool isAllowed = false;

        // Check if the current license is suitable for webtemplate
        isAllowed = IsTemplateAllowed(templateEditions, templatePackages);

        // Generate HTML code
        StringBuilder builder = new StringBuilder();

        string cssClass = isAllowed ? "install-item" : "install-disabled-item";
        string textColor = isAllowed ? "" : "color:Silver;";
        string errorMessage = "";

        if (!isAllowed)
        {
            errorMessage = "<div style=\"color:Red;\"><br />";
            errorMessage += string.Format(ResHelper.GetFileString("Install.WebTemplateRequiresLicense"), EditionsToString(templateEditions));

            string templatePackageNames = PackagesToString(templatePackages);
            if (!String.IsNullOrEmpty(templatePackageNames))
            {
                errorMessage += " " + string.Format(ResHelper.GetFileString("Install.WebTemplateRequiresLicenseOrPackages"), templatePackageNames);
            }

            errorMessage += ". " + string.Format(ResHelper.GetFileString("Install.WebTemplateCurrentLicense"), GetEditionName(CurrentEdition));

            string currentPackagesNames = PackagesToString(CurrentPackages);
            if (!String.IsNullOrEmpty(currentPackagesNames))
            {
                errorMessage += " " + string.Format(ResHelper.GetFileString("Install.WebTemplateCurrentPackages"), currentPackagesNames);
            }

            // Finish the sentence
            errorMessage += ".</div>";
        }

        builder.Append("<div class=\"" + cssClass + "\" id=\"tpl" + templateId + "\"");
        if (isAllowed)
        {
            builder.Append(" onclick=\"SelectTemplate('tpl" + templateId + "','" + templateName + "')\"");
        }
        builder.Append(">\n");
        builder.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">\n");
        builder.Append("<tr>\n<td style=\"vertical-align:top;\">\n");
        builder.Append("<img style=\"margin: 3px;\" src=\"" + GetPreviewImage(templateId) + "\" width=\"140\" height=\"108\" alt=\"Preview\" />\n");
        builder.Append("</td>\n<td style=\"vertical-align:top;\">\n");
        builder.Append("<div style=\"margin: 3px;" + textColor + "\">\n<div>\n<strong>" + templateDisplayName + "</strong></div>\n");
        builder.Append("<br /><div>" + templateDescription + "</div>" + errorMessage + "</div></td></tr>\n</table>\n</div>\n");
        return builder.ToString();
    }


    private bool IsTemplateAllowed(string templateEditions, string templatePackages)
    {
        if (CurrentEdition != null)
        {
            // Check if the current license is suitable for webtemplate
            if (templateEditions.Contains(CurrentEdition))
            {
                return true;
            }
        }

        // Check license packages
        if ((!String.IsNullOrEmpty(CurrentPackages))
            && (!String.IsNullOrEmpty(templatePackages)))
        {
            string[] tempCurrentPackages = CurrentPackages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tempTemplatePackages = templatePackages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            // Check if the current license (package) is suitable for webtemplate
            foreach (string templateP in tempTemplatePackages)
            {
                foreach (string currentP in tempCurrentPackages)
                {
                    if (templateP.EqualsCSafe(currentP, true))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    private static string EditionsToString(string templateEditions)
    {
        string names = "";
        string[] editions = templateEditions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        bool isProfInNames = false;
        foreach (string editionChar in editions)
        {
            try
            {
                // Add all editions to names
                // Prof and Pro3x has same resource string, ensure that this string will be only once in list
                ProductEditionEnum edition = editionChar.ToEnum<ProductEditionEnum>();
                if ((edition != ProductEditionEnum.Professional) && (edition != ProductEditionEnum.Professional3x))
                {
                    names += LicenseHelper.GetEditionName(edition) + ", ";
                }
                else if (!isProfInNames)
                {
                    names += LicenseHelper.GetEditionName(edition) + ", ";
                    isProfInNames = true;
                }
            }
            catch
            {
                return "#UNKNOWN#";
            }
        }

        if (names == "")
        {
            return names;
        }
        else
        {
            return names.Substring(0, names.Length - 2);
        }
    }


    private static string GetEditionName(string editionChar)
    {
        try
        {
            ProductEditionEnum edition = editionChar.ToEnum<ProductEditionEnum>();
            return LicenseHelper.GetEditionName(edition);
        }
        catch
        {
            return "#UNKNOWN#";
        }
    }


    /// <summary>
    /// Convert package codes to their names.
    /// </summary>
    /// <param name="packages">The packages in the format "ECM;SCN"</param>
    private static string PackagesToString(string packages)
    {
        string names = "";

        if (!String.IsNullOrEmpty(packages))
        {
            string[] packageCodes = packages.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string package in packageCodes)
            {
                try
                {
                    if (!String.IsNullOrEmpty(names))
                    {
                        names += ", ";
                    }

                    PackagesEnum packageEnum = package.ToEnum<PackagesEnum>();
                    names += LicenseHelper.GetPackageName(packageEnum);
                }
                catch
                {
                }
            }
        }

        return names;
    }
}