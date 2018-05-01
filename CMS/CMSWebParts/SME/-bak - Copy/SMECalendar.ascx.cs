using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.CustomTables;
using CMS.DocumentEngine;
using CMS.SiteProvider;

public partial class CMSWebParts_SME_SMECalendar : CMSAbstractWebPart
{
    #region "Variables"

    protected string mCustomTableName;
    protected DataClassInfo mCustomtableInfo = null;

    #endregion

    #region "Properties"

    public string CSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CSSClass"), "");
        }
        set
        {
            SetValue("CSSClass", value);
        }
    }

    public string ZeroRowsText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ZeroRowsText"), "");
        }
        set
        {
            SetValue("ZeroRowsText", value);
        }
    }


    public string ClassName
    {

        get
        {
            return ValidationHelper.GetString(GetValue("ClassName"), "");
        }
        set
        {
            SetValue("ClassName", value);
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (QueryHelper.Contains("categoryname"))
        {
            hdnWhere.Value = QueryHelper.GetString("categoryname", "");
        }
        if (!IsPostBack)
        {
            string eventTypeList = "Select * FROM CMS_Category where CategoryNamePath LIKE '%Events/%' ";

            var queryForEventTypes = new QueryParameters(eventTypeList, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);
            DataSet dsForEventTypes = ConnectionHelper.ExecuteQuery(queryForEventTypes);

            ddlEventType.DataSource = dsForEventTypes;
            ddlEventType.DataTextField = "CategoryDisplayName".Replace('&', ' ');
            ddlEventType.DataValueField = "CategoryName";
            ddlEventType.DataBind();
            ddlEventType.Items.Insert(0, "All");
            ddlEventType.SelectedValue = QueryHelper.GetString("categoryname", "");

        }

        // Control initialization           
        gridCalendar.GridView.CssClass = CSSClass;
        gridCalendar.ZeroRowsText = ZeroRowsText;
        gridCalendar.IsLiveSite = false;

        loadGrid(hdnWhere.Value.ToString());

    }

    /// <summary>
    /// Reload data.
    /// </summary>
    private void loadGrid(string whereCondition)
    {
        try
        {
			string where="EventCategory='"+whereCondition+"'";
            DataSet dsForEventsList;
            if (whereCondition != "" && whereCondition != "All")
            {
                dsForEventsList = DocumentHelper.GetDocuments("SME.Event")
                                                   .OnSite(SiteContext.CurrentSiteName)
                                                   .Culture("en-us")
                                                   .CombineWithDefaultCulture(false)
                                                   .Where(where, null)
                                                   .NestingLevel(-1)
                                                   .Published(true);
            }
            else
            {
                dsForEventsList = DocumentHelper.GetDocuments("SME.Event")
                                                   .OnSite(SiteContext.CurrentSiteName)
                                                   .Culture("en-us")
                                                   .CombineWithDefaultCulture(false)
                                                   .NestingLevel(-1)
                                                   .Published(true);
            }

            gridCalendar.DataSource = dsForEventsList;
            gridCalendar.DataBind();

        }

        catch (Exception)
        {

        }
    }

    #endregion
    protected void ddlEventType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = "/full-calendar";
        url = URLHelper.UpdateParameterInUrl(url, "categoryname", HttpUtility.UrlEncode(ddlEventType.SelectedValue));
        URLHelper.Redirect(url.Trim());
    }
}



