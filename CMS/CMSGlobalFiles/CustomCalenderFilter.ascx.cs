using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using CMS.Controls;
using CMS.Helpers;
using CMS.Ecommerce;
using CMS.DocumentEngine;
using CMS.SiteProvider;

public partial class CMSGlobalFiles_CustomCalenderFilter : CMSAbstractDataFilterControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    /// <summary>
    /// Sets up the inner child controls.
    /// </summary>
    private void SetupControl()
    {
        // Hides the filter if StopProcessing is enabled
        if (this.StopProcessing)
        {
            this.Visible = false;
        }
        // Initializes only if the current request is NOT a postback
        else if (!RequestHelper.IsPostBack())
        {
            // Loads product departments as filtering options
            InitializeDepartments();
        }
    }
    /// <summary>
    /// Loads all existing product departments as filtering options into the department drop-down list.
    /// </summary>
    private void InitializeDepartments()
    {
        // Gets all product departments from the system's database      
        var ClassNames = "SME.Event";
        var siteCode = CultureSiteInfoProvider.GetSiteCultureCodes(SiteContext.CurrentSiteName)[0].ToString();
        var documents = TreeHelper.GetDocuments(SiteContext.CurrentSiteName, string.Empty, siteCode, false, ClassNames, string.Empty, string.Empty, 0, true, 0, "EventType, EventID");

        // Checks that at least one product department exists
        if (!DataHelper.DataSourceIsEmpty(documents))
        {
            // Binds the departments to the drop-down list
            this.drpDepartment.DataSource = documents;
            this.drpDepartment.DataTextField = "EventType";
            this.drpDepartment.DataValueField = "EventID";

            this.drpDepartment.DataBind();

            // Adds the default '(all)' option
            this.drpDepartment.Items.Insert(0, new ListItem("(all)", "##ALL##"));
        }
    }
    /// <summary>
    /// Generates a WHERE condition and ORDER BY clause based on the current filtering selection.
    /// </summary>
    private void SetFilter()
    {
        string where = null;
        // Generates a WHERE condition based on the selected product department
        if (this.drpDepartment.SelectedValue != null)
        {
            // Gets the ID of the selected department
            int departmentId = ValidationHelper.GetInteger(this.drpDepartment.SelectedValue, 0);

            if (departmentId > 0)
            {
                where = "EventID = " + departmentId;
            }
        }
        if (where != null)
        {
            // Sets the Where condition
            this.WhereCondition = where;
        }
        // Raises the filter changed event
        this.RaiseOnFilterChanged();
    }
    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        // Creates the child controls
        SetupControl();
        base.OnInit(e);
    }

    /// <summary>
    /// PreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        // Checks if the current request is a postback
        if (RequestHelper.IsPostBack())
        {
            // Applies the filter to the displayed data
            SetFilter();
        }

        base.OnPreRender(e);
    }
}