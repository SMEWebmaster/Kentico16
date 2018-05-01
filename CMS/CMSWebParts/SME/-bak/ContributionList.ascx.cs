using System;
using System.Data;
using System.Text;
using System.Web.UI;

using CMS.Controls;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.MacroEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WorkflowEngine;
using CMS.ExtendedControls;

using TimeZoneInfo = CMS.Globalization.TimeZoneInfo;
using CMS.Globalization;
using CMS.Protection;
using CMS.DataEngine;
using CMSAppAppCode.SME;
using CMS.DataEngine;

public partial class CMSWebParts_SME_ContributionList : CMSUserControl
{
    #region "Variables"

    /// <summary>
    /// On after delete event.
    /// </summary>
    public event EventHandler OnAfterDelete = null;


    private bool mAllowInsert = true;
    private bool mAllowDelete = true;
    private bool mAllowEdit = true;
    private string mNewItemPageTemplate = String.Empty;
    private string mAllowedChildClasses = String.Empty;
    private string mAlternativeFormName = null;
    private string mAlternativeFormNameForDetails = null;

    private string mValidationErrorMessage = null;
    private bool mCheckPermissions = false;
    private bool mCheckGroupPermissions = false;
    private bool mCheckDocPermissionsForInsert = true;
    private bool mLogActivity = false;
    private UserContributionAllowUserEnum mAllowUsers = UserContributionAllowUserEnum.DocumentOwner;
    private TreeNode mParentNode = null;
    private string mEventFilter = String.Empty;
    private bool mShowFilter = false;

    /// <summary>
    /// Data properties variable.
    /// </summary>
    protected CMSDataProperties mDataProperties = new CMSDataProperties();

    #endregion


    #region "Document properties"


    /// <summary>
    /// Gets or sets the event category to filter.
    /// </summary>
    public string EventFilter
    {
        get
        {
            return mEventFilter;
        }
        set
        {
            mEventFilter = value;
        }
    }

    /// <summary>
    /// Gets or sets the events dropdown to show.
    /// </summary>
    public bool ShowFilter
    {
        get
        {
            return mShowFilter;
        }
        set
        {
            mShowFilter = value;
        }
    }

    /// <summary>
    /// Component name
    /// </summary>
    public override string ComponentName
    {
        get
        {
            return base.ComponentName;
        }
        set
        {
            base.ComponentName = value;
            DocumentManager.ComponentName = value;
            editDoc.ComponentName = value;
        }
    }


    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            return editDoc.DocumentManager;
        }
    }


    /// <summary>
    /// Class names.
    /// </summary>
    public string ClassNames
    {
        get
        {
            return mDataProperties.ClassNames;
        }
        set
        {
            mDataProperties.ClassNames = value;
        }
    }


    /// <summary>
    /// Combine with default culture.
    /// </summary>
    public bool CombineWithDefaultCulture
    {
        get
        {
            return mDataProperties.CombineWithDefaultCulture;
        }
        set
        {
            mDataProperties.CombineWithDefaultCulture = value;
        }
    }


    /// <summary>
    /// Culture code.
    /// </summary>
    public string CultureCode
    {
        get
        {
            return mDataProperties.CultureCode;
        }
        set
        {
            mDataProperties.CultureCode = value;
        }
    }


    /// <summary>
    /// Maximal relative level.
    /// </summary>
    public int MaxRelativeLevel
    {
        get
        {
            return mDataProperties.MaxRelativeLevel;
        }
        set
        {
            mDataProperties.MaxRelativeLevel = value;
        }
    }


    /// <summary>
    /// Order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mDataProperties.OrderBy;
        }
        set
        {
            mDataProperties.OrderBy = value;
        }
    }


    /// <summary>
    /// Nodes path.
    /// </summary>
    public string Path
    {
        get
        {
            return mDataProperties.Path;
        }
        set
        {
            mDataProperties.Path = value;
        }
    }


    /// <summary>
    /// Select only published nodes.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return mDataProperties.SelectOnlyPublished;
        }
        set
        {
            mDataProperties.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return mDataProperties.SiteName;
        }
        set
        {
            mDataProperties.SiteName = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public string WhereCondition
    {
        get
        {
            return mDataProperties.WhereCondition;
        }
        set
        {
            mDataProperties.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets the columns to retrieve from DB.
    /// </summary>
    public string Columns
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Columns"), null);
        }
        set
        {
            SetValue("Columns", value);
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets Edit form control.
    /// </summary>
    public CMSWebParts_SME_EditForm EditForm
    {
        get
        {
            return editDoc;
        }
    }


    /// <summary>
    /// Indicates whether the list of documents should be displayed.
    /// </summary>
    public bool DisplayList
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DisplayList"], true);
        }
        set
        {
            ViewState["DisplayList"] = value;
        }
    }


    /// <summary>
    /// Path for new created documents.
    /// </summary>
    public string NewDocumentPath
    {
        get
        {
            string newDocPath = ValidationHelper.GetString(ViewState["NewDocumentPath"], "");
            // If new document path is not set, use source path
            if (newDocPath.Trim() == String.Empty)
            {
                newDocPath = Path;
            }

            // Ensure correct format of the path
            if (newDocPath.EndsWithCSafe("/%"))
            {
                newDocPath = newDocPath.Remove(newDocPath.Length - 2);
            }

            if (String.IsNullOrEmpty(newDocPath))
            {
                newDocPath = "/";
            }

            return newDocPath;
        }
        set
        {
            ViewState["NewDocumentPath"] = value;
        }
    }


    /// <summary>
    /// Indicates whether inserting new document is allowed.
    /// </summary>
    public bool AllowInsert
    {
        get
        {
            return mAllowInsert;
        }
        set
        {
            mAllowInsert = value;
        }
    }


    /// <summary>
    /// Indicates whether editing document is allowed.
    /// </summary>
    public bool AllowEdit
    {
        get
        {
            return mAllowEdit;
        }
        set
        {
            mAllowEdit = value;
        }
    }


    /// <summary>
    /// Indicates whether deleting document is allowed.
    /// </summary>
    public bool AllowDelete
    {
        get
        {
            return mAllowDelete;
        }
        set
        {
            mAllowDelete = (value && CheckGroupPermission("deletepages"));
            editDoc.AllowDelete = mAllowDelete;
        }
    }


    /// <summary>
    /// Page template the new items are assigned to.
    /// </summary>
    public string NewItemPageTemplate
    {
        get
        {
            return mNewItemPageTemplate;
        }
        set
        {
            mNewItemPageTemplate = value;
        }
    }


    /// <summary>
    /// Type of the child documents that are allowed to be created.
    /// </summary>
    public string AllowedChildClasses
    {
        get
        {
            return mAllowedChildClasses;
        }
        set
        {
            mAllowedChildClasses = value;
        }
    }


    /// <summary>
    /// Alternative form name.
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return mAlternativeFormName;
        }
        set
        {
            mAlternativeFormName = value;
        }
    }

    /// <summary>
    /// Alternative form name.
    /// </summary>
    public string AlternativeFormNameForDetails
    {
        get
        {
            return mAlternativeFormNameForDetails;
        }
        set
        {
            mAlternativeFormNameForDetails = value;
        }
    }

    /// <summary>
    /// Form validation error message.
    /// </summary>
    public string ValidationErrorMessage
    {
        get
        {
            return mValidationErrorMessage;
        }
        set
        {
            mValidationErrorMessage = value;
        }
    }


    /// <summary>
    /// Indicates whether permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            return mCheckPermissions;
        }
        set
        {
            mCheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates whether group permissions should be checked.
    /// </summary>
    public bool CheckGroupPermissions
    {
        get
        {
            return mCheckGroupPermissions;
        }
        set
        {
            mCheckGroupPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if document type permissions are required to create new document.
    /// </summary>
    public bool CheckDocPermissionsForInsert
    {
        get
        {
            return mCheckDocPermissionsForInsert;
        }
        set
        {
            mCheckDocPermissionsForInsert = value;
        }
    }


    /// <summary>
    /// Which group of users can work with the documents.
    /// </summary>
    public UserContributionAllowUserEnum AllowUsers
    {
        get
        {
            return mAllowUsers;
        }
        set
        {
            mAllowUsers = value;
        }
    }


    /// <summary>
    /// Gets or sets New item button label.
    /// </summary>
    public string NewItemButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("NewItemButtonText"), "ContributionList.lnkNewDoc");
        }
        set
        {
            SetValue("NewItemButtonText", value);
            btnNewDoc.ResourceString = value;
        }
    }


    /// <summary>
    /// Gets or sets List button label.
    /// </summary>
    public string ListButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ListButtonText"), "general.pages");
        }
        set
        {
            SetValue("ListButtonText", value);
            btnList.ResourceString = value;
        }
    }


    /// <summary>
    /// Indicates whether activity logging is enabled.
    /// </summary>
    public bool LogActivity
    {
        get
        {
            return mLogActivity;
        }
        set
        {
            mLogActivity = value;
            editDoc.LogActivity = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Parent node.
    /// </summary>
    private TreeNode ParentNode
    {
        get
        {
            return mParentNode ?? (mParentNode = GetParentNode());
        }
    }


    /// <summary>
    /// Parent node id.
    /// </summary>
    private int ParentNodeID
    {
        get
        {
            return (ParentNode == null ? 0 : ParentNode.NodeID);
        }
    }

    #endregion


    #region "Methods"

    string query = "SELECT ROW_NUMBER() OVER (PARTITION BY EventName ORDER BY CMS_DocumentCategory.CategoryID) AS RowNumber, EventName,EndDate,NodeID,NodeAliasPath,StartDate,datediff(minute,'1990-1-1',StartDate) AS DateTicks,EventDetails,EventCategory,Location,CategoryName,CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 AND NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND StartDate>=getdate()";
    string queryForApprovalList = "SELECT EventName,EndDate,NodeID,NodeAliasPath,StartDate,datediff(minute,'1990-1-1',StartDate) AS DateTicks,EventDetails,EventCategory,Location,CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND DocumentWorkflowStepID='9191' AND ClassName='SME.Event' AND StartDate>=getdate()";

    //string query = "select EventName,EndDate,NodeID,NodeAliasPath,CONVERT(DateTime,StartDate) as StartDate,DocumentName,EventDetails,EventCategory,Location,SiteName,ClassName,NodeACLID,NodeSiteID,NodeOwner,DocumentName,Published,DocumentModifiedWhen,DocumentWorkflowStepID  from [dbo].[View_SME_Content_Event_Joined] where published=1 and NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event'AND CONVERT(Date,StartDate) >= (GetDate()) order by CONVERT(Date,StartDate) ASC";
    //string queryForApprovalList = "select EventName,EndDate,NodeID,NodeAliasPath,CONVERT(DateTime,StartDate) as StartDate,DocumentName,EventDetails,EventCategory,Location,SiteName,ClassName,NodeACLID,NodeSiteID,NodeOwner,DocumentName,Published,DocumentModifiedWhen,DocumentWorkflowStepID  from [dbo].[View_SME_Content_Event_Joined] where NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND DocumentWorkflowStepID='9191' AND CONVERT(Date,StartDate)>= (GetDate()) order by CONVERT(Date,StartDate) ASC";


    DataSet ds = new DataSet();

    protected void Page_Load(object sender, EventArgs e)
    {
        addEventCategory();
        string[] eventFilters = mEventFilter.ToString().ToLower().Split('|');

        string where = string.Empty;
       
		if (eventFilters[0] != "" && eventFilters[0] == "events_webinar" )
        {
            //string queryForWebinars = "SELECT EventName,EndDate,NodeID,NodeAliasPath,StartDate,datediff(minute,'1990-1-1',StartDate) AS DateTicks,EventDetails,EventCategory,Location,CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 AND NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND CategoryName='" + eventFilters[0]+"' ORDER BY StartDate DESC";
            string queryForWebinars = "SELECT  * FROM (SELECT ROW_NUMBER() OVER (PARTITION BY EventName ORDER BY CMS_DocumentCategory.CategoryID) AS RowNumber, EventName,EndDate,NodeID,NodeAliasPath,StartDate,datediff(minute,'1990-1-1',StartDate) AS DateTicks,EventDetails,EventCategory,Location,CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 AND NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND CategoryName='" + eventFilters[0]+"') AS EventsList where EventsList.RowNumber=1 ORDER BY StartDate DESC";
			IsApprovalListVisible();
            loadCalendarEvents(queryForWebinars, "events");
        }    
		else if(eventFilters[0] != "" && eventFilters[0] == "uca_sponsored_event")
        {
            IsApprovalListVisible();
            //query = query + " ORDER BY StartDate ASC";
			query = "SELECT  * FROM ("+query +") AS EventsList where EventsList.RowNumber=1 AND CategoryName='" + eventFilters[0] + "' ORDER BY StartDate ASC";
            loadCalendarEvents(query, "events");
        }
        else if (eventFilters[0] != "" && !QueryHelper.Contains("categoryname") && !QueryHelper.Contains("approvalevents"))
        {
            if (eventFilters.Length > 1)
            {
                query = query + " AND (";

                for (int i = 0; i < eventFilters.Length; i++)
                {
                    where = where + "CategoryName='" + eventFilters[i] + "' OR ";
                }
                //query = query + where.Substring(0, where.Length - 3) + " )  ORDER BY StartDate ASC";
				query = "SELECT  * FROM ("+query + where.Substring(0, where.Length - 3) + " ) AS EventsList where EventsList.RowNumber=1 ORDER BY StartDate ASC";
			}
            else
            {
                //query = query + " AND CategoryName='" + eventFilters[0] + "'  ORDER BY StartDate ASC";
				query = "SELECT  * FROM ("+query + " AND CategoryName='" + eventFilters[0] + "' ) AS EventsList where EventsList.RowNumber=1 ORDER BY StartDate ASC";
            }

            loadCalendarEvents(query, "events");
        }
        else if (eventFilters[0] != "" && !QueryHelper.Contains("categoryname") && QueryHelper.Contains("approvalevents"))
        {
            if (eventFilters.Length > 1)
            {
                query = query + " AND (";

                for (int i = 0; i < eventFilters.Length; i++)
                {
                    where = where + "CategoryName='" + eventFilters[i] + "' OR ";
                }

                queryForApprovalList = queryForApprovalList + where.Substring(0, where.Length - 3) + " )  ORDER BY StartDate ASC";

            }
            else
            {
                queryForApprovalList = queryForApprovalList + " AND CategoryName='" + eventFilters[0] + "'  ORDER BY StartDate ASC";
            }
            loadCalendarEvents(queryForApprovalList, "approvalevents");
        }

        else if (QueryHelper.Contains("categoryname") && !QueryHelper.Contains("approvalevents"))
        {
            IsApprovalListVisible();
            hdnWhere.Value = QueryHelper.GetString("categoryname", "");
            //query = query + " AND CategoryName='" + QueryHelper.GetString("categoryname", "") + "'  ORDER BY StartDate ASC";
			query = "SELECT  * FROM ("+query + " AND CategoryName='" + QueryHelper.GetString("categoryname", "") + "') AS EventsList where EventsList.RowNumber=1 ORDER BY StartDate ASC";
			loadCalendarEvents(query, "events");
        }
        else if (QueryHelper.Contains("categoryname") && QueryHelper.Contains("approvalevents"))
        {
            btnUsersToBeApproved.Visible = false;
            btnCalendar.Visible = true;
            if (Utility.IsUserInAdminRole())
            {
                rptApprovalEvents.Visible = true;
            }
            else
            {
                Response.Redirect(Request.RawUrl.Split('?')[0]);
            }

            rptEvents.Visible = false;
            queryForApprovalList = queryForApprovalList + " AND CategoryName='" + QueryHelper.GetString("categoryname", "") + "'  ORDER BY StartDate ASC";
            loadCalendarEvents(queryForApprovalList, "approvalevents");
        }
        else if ((eventFilters[0] == "" || eventFilters.Length == 0) && QueryHelper.Contains("approvalevents") && !QueryHelper.Contains("categoryname"))
        {
            btnUsersToBeApproved.Visible = false;
            btnCalendar.Visible = true;
            if (Utility.IsUserInAdminRole())
            {
                rptApprovalEvents.Visible = true;
            }
            else
            {
                Response.Redirect(Request.RawUrl.Split('?')[0]);
            }
            rptEvents.Visible = false;
            queryForApprovalList = queryForApprovalList + " ORDER BY StartDate ASC";
            loadCalendarEvents(queryForApprovalList, "approvalevents");
        }
        else
        {
            IsApprovalListVisible();
            //query = query + " ORDER BY StartDate ASC";
			query = "SELECT  * FROM ("+query +") AS EventsList where EventsList.RowNumber=1 ORDER BY StartDate ASC";
            loadCalendarEvents(query, "events");
        }

        if (!mShowFilter)
        {
            btnNewDoc.Visible = false;
            categoryDropdown.Visible = false;
            btnUsersToBeApproved.Visible = false;
        }
		
		 if (QueryHelper.Contains("approvalevents"))
        {
            categoryDropdown.Visible = false;
        }

        if (!IsPostBack && QueryHelper.Contains("approvalevents"))
        {
            loadCategoryDropDown("approvalevents");
        }
        else if (!IsPostBack && !QueryHelper.Contains("approvalevents"))
        {
            loadCategoryDropDown("categoryname");
        }

        // Check license
        LicenseHelper.CheckFeatureAndRedirect(RequestContext.CurrentDomain, FeatureEnum.UserContributions);

        mDataProperties.ParentControl = this;

        btnNewDoc.ResourceString = HTMLHelper.HTMLEncode(NewItemButtonText);
        btnList.ResourceString = HTMLHelper.HTMLEncode(ListButtonText);

        // Hide/Show edit document form
        bool editVisible = (editDoc.NodeID > 0);

        pnlEdit.Visible = editVisible;

        editDoc.SiteName = SiteName;
        editDoc.CultureCode = CultureCode;
        editDoc.LogActivity = LogActivity;
    }

    private void addEventCategory()
    {
        string where = "EventCategory='Non-SME_Industry_Conference' AND DocumentWorkflowStepID='9191'";
        DataSet eventsList = DocumentHelper.GetDocuments("SME.EVENT")
                                                         .OnSite(SiteContext.CurrentSiteName)
                                                         .Culture("en-us")
                                                         .CombineWithDefaultCulture(false)
                                                         .All()
                                                         .Where(where);

        DataTable events = new DataTable();
        events = eventsList.Tables[0];

        foreach (DataRow dr in events.Rows)
        {
            CMS.Taxonomy.CategoryInfo ci = CMS.Taxonomy.CategoryInfoProvider.GetCategoryInfo("Non-SME_Industry_Conference", SiteContext.CurrentSiteName);
            CMS.DocumentEngine.DocumentCategoryInfo dci = new CMS.DocumentEngine.DocumentCategoryInfo(dr);
            if (dci.CategoryID != 138)
            {
                dci.CategoryID = ci.CategoryID;
                dci.DocumentID = (int)dr["DocumentID"];
                dci.Update();
            }
        }
    }

    private void loadCategoryDropDown(string eventtype)
    {
        string eventTypeList = string.Empty;
        if (eventtype.ToLower() == "approvalevents")
        {
            eventTypeList = "SELECT DISTINCT CategoryName, CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE  NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND StartDate>=getdate()";
        }
		/*else if(mEventFilter.ToString().ToLower().Split('|')[0] == "uca_sponsored_event")
		{
			eventTypeList = "SELECT DISTINCT CategoryName, CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 and NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND StartDate>=getdate()";	
		}*/
        else
        {
           // eventTypeList = "SELECT DISTINCT CategoryName, CategoryDisplayName FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 and NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND StartDate>=getdate()";
		  eventTypeList = "SELECT  DISTINCT CategoryName, CategoryDisplayName FROM (SELECT ROW_NUMBER() OVER (PARTITION BY EventName ORDER BY CMS_DocumentCategory.CategoryID) AS RowNumber,EventName,EndDate,NodeID,NodeAliasPath,StartDate,datediff(minute,'1990-1-1',StartDate) AS DateTicks,EventDetails,EventCategory,Location,CategoryName,CategoryDisplayName,CMS_DocumentCategory.CategoryID FROM View_SME_Content_Event_Joined INNER JOIN CMS_DocumentCategory ON View_SME_Content_Event_Joined.DocumentID=CMS_DocumentCategory.DocumentID INNER JOIN CMS_Category ON CMS_Category.CategoryID=CMS_DocumentCategory.CategoryID WHERE published=1 AND NodeAliasPath LIKE '/Events-Professional-Development/Events/%' AND ClassName='SME.Event' AND StartDate>=getdate()) AS EventsList where EventsList.RowNumber=1";
		}
        //"Select * FROM CMS_Category where CategoryNamePath LIKE '%Events/%' ";

        var queryForEventTypes = new QueryParameters(eventTypeList, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);
        DataSet dsForEventTypes = ConnectionHelper.ExecuteQuery(queryForEventTypes);

        ddlEventType.DataSource = dsForEventTypes;
        ddlEventType.DataTextField = "CategoryDisplayName";
        ddlEventType.DataValueField = "CategoryName";
        ddlEventType.DataBind();
        if (!QueryHelper.Contains("categoryname"))
        {
            ddlEventType.Items.Insert(0, "Select One");
            //Response.Write(1);
        }
         else if (QueryHelper.Contains("categoryname") && (mEventFilter.ToString() == ""))
        {
            ddlEventType.Items.Insert(0, "View All Events");
            ddlEventType.SelectedValue = QueryHelper.GetString("categoryname", "View All Events");
        }
		else if (QueryHelper.Contains("categoryname") && (mEventFilter.ToString() != ""))
        {
            ddlEventType.Items.Insert(0, "View All UCA Events");
            ddlEventType.SelectedValue = QueryHelper.GetString("categoryname", "View All Events");
        }
        else if (mEventFilter.ToString().ToLower().Split('|')[0] == "uca_sponsored_event" && ddlEventType.Items.FindByValue("uca_sponsored_event") != null)
        {
            ddlEventType.SelectedValue = "uca_sponsored_event";
        }

    }

    private void IsApprovalListVisible()
    {
        if (Utility.IsUserInAdminRole())
        {
            btnUsersToBeApproved.Visible = true;
        }
    }

    private void loadCalendarEvents(string query, string eventtype)
    {
        var queryForEvents = new QueryParameters(query, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);
        DataSet dsForEvent = ConnectionHelper.ExecuteQuery(queryForEvents);
        if (dsForEvent != null && dsForEvent.Tables[0].Rows.Count > 0)
        {
            if (eventtype == "approvalevents")
            {
                rptApprovalEvents.DataSource = dsForEvent;
                rptApprovalEvents.DataBind();
            }
            else
            {
                rptEvents.DataSource = dsForEvent;
                rptEvents.DataBind();
            }

        }
        else
        {
            info.Visible = true;
            info.Text = ValidationHelper.GetString(GetString("<span class='no-events'>Oops...<br>There are currently no events of this type scheduled.<br> Please select another event type from the menu.</span>"), "<h5>No Events</h5>");
        }

    }

    protected override void OnPreRender(EventArgs e)
    {
        // Register the scripts
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ContributionList_" + ClientID, ltlScript.Text);
        ltlScript.Text = String.Empty;

        base.OnPreRender(e);
    }


    private void ReloadData()
    {
        if (StopProcessing)
        {
            // Do nothing
            editDoc.StopProcessing = true;
        }
        else
        {
            SetContext();
            bool isAuthorizedToCreateDoc = false;
            if (ParentNode != null)
            {
                // Check if single class name is set
                string className = (!string.IsNullOrEmpty(AllowedChildClasses) && !AllowedChildClasses.Contains(";")) ? AllowedChildClasses : null;

                // Check user's permission to create new document if allowed
                isAuthorizedToCreateDoc = !CheckPermissions || MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(ParentNodeID, className);
                // Check group's permission to create new document if allowed
                isAuthorizedToCreateDoc &= CheckGroupPermission("createpages");

                if (!CheckDocPermissionsForInsert && CheckPermissions)
                {
                    // If document permissions are not required check create permission on parent document
                    isAuthorizedToCreateDoc = MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(ParentNode, NodePermissionsEnum.Create) == AuthorizationResultEnum.Allowed;
                }

                if (AllowUsers == UserContributionAllowUserEnum.DocumentOwner)
                {
                    // Do not allow documents creation under virtual user
                    if (MembershipContext.AuthenticatedUser.IsVirtual)
                    {
                        isAuthorizedToCreateDoc = false;
                    }
                    else
                    {
                        // Check if user is document owner (or global admin)
                        isAuthorizedToCreateDoc = isAuthorizedToCreateDoc && ((ParentNode.NodeOwner == MembershipContext.AuthenticatedUser.UserID) || MembershipContext.AuthenticatedUser.IsGlobalAdministrator);
                    }
                }
            }

            // Enable/disable inserting new document
            if (pnlList.Visible)
            {
                // Not authenticated to create new docs and grid is hidden
                StopProcessing = true;
            }

            ReleaseContext();
        }
    }

    /// <summary>
    /// Initializes and shows edit form with available documents.
    /// </summary>
    protected void btnNewDoc_Click(object sender, EventArgs e)
    {
        // Initialize EditForm control
        editDoc.EnableViewState = true;
        editDoc.AllowedChildClasses = AllowedChildClasses;
        editDoc.AllowDelete = false;// AllowDelete && CheckGroupPermission("deletepages");
        editDoc.NewItemPageTemplate = NewItemPageTemplate;
        editDoc.CheckPermissions = CheckPermissions;
        editDoc.CheckDocPermissionsForInsert = CheckDocPermissionsForInsert;
        editDoc.Action = "new";
        // Set parent nodeId
        editDoc.NodeID = ParentNodeID;
        editDoc.ClassID = 0;
        editDoc.AlternativeFormName = AlternativeFormName;
        editDoc.ValidationErrorMessage = ValidationErrorMessage;
        editDoc.LogActivity = LogActivity;

        pnlEdit.Visible = true;
        pnlList.Visible = false;
    }


    /// <summary>
    /// Gets parent node ID.
    /// </summary>
    private TreeNode GetParentNode()
    {
        TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        return tree.SelectSingleNode(SiteName, MacroResolver.ResolveCurrentPath(NewDocumentPath), TreeProvider.ALL_CULTURES);
    }


    /// <summary>
    /// Displays document list and hides edit form.
    /// </summary>
    protected void btnList_Click(object sender, EventArgs e)
    {
        // EditDoc control registers progress script on PreRender event - but when btnList button is clicked, EditDoc control gets hidden, so 
        // progress script needs to be registered again here to hide progress layout properly.
        if (editDoc.UseProgressScript)
        {
            ScriptHelper.RegisterLoader(Page);
        }
        pnlList.Visible = true;
        pnlEdit.Visible = false;
        editDoc.Action = "";
        editDoc.NodeID = 0;
        editDoc.EnableViewState = false;
    }


    /// <summary>
    /// Checks whether the user is authorized to delete document.
    /// </summary>
    /// <param name="node">Document node</param>
    protected bool IsUserAuthorizedToDeleteDocument(TreeNode node)
    {
        // Check delete permission
        bool isAuthorized = MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, new NodePermissionsEnum[] { NodePermissionsEnum.Delete, NodePermissionsEnum.Read }) == AuthorizationResultEnum.Allowed;

        return isAuthorized;
    }


    /// <summary>
    /// Adds the alert message to the output request window.
    /// </summary>
    /// <param name="message">Message to display</param>
    private void AddAlert(string message)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), message.GetHashCode().ToString(), ScriptHelper.GetAlertScript(message));
    }


    /// <summary>
    /// Returns true if group permissions should be checked and specified permission is allowed in current group.
    /// Also returns true if group permissions should not be checked.
    /// </summary>
    /// <param name="permissionName">Permission to check (createpages, editpages, deletepages)</param>
    protected bool CheckGroupPermission(string permissionName)
    {
        if (CheckGroupPermissions && !MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            // Get current group ID
            int groupId = ModuleCommands.CommunityGetCurrentGroupID();
            if (groupId > 0)
            {
                // Returns true if current user is authorized for specified action in current group
                return ModuleCommands.CommunityCheckGroupPermission(permissionName, groupId) || MembershipContext.AuthenticatedUser.IsGroupAdministrator(groupId);
            }

            return false;
        }

        return true;
    }


    /// <summary>
    /// Raises the OnAfterDelete event.
    /// </summary>
    private void RaiseOnAfterDelete()
    {
        if (OnAfterDelete != null)
        {
            OnAfterDelete(this, null);
        }
    }

    protected void btnUsersToBeApproved_Click(object sender, EventArgs e)
    {
        string url = Request.RawUrl.Split('?')[0];
        url = URLHelper.UpdateParameterInUrl(url, "approvalevents", "true");
        URLHelper.Redirect(url.Trim());
    }
    protected void btnCalendar_Click(object sender, EventArgs e)
    {
        string url = Request.RawUrl.Split('?')[0];
        ViewState["PendingForApproval"] = false;
        URLHelper.Redirect(url.Trim());
    }

    #endregion
    protected void ddlEventType_SelectedIndexChanged(object sender, EventArgs e)
    {
        string url = Request.RawUrl.Split('?')[0];
        string urlUpdated = string.Empty;
        if (QueryHelper.Contains("approvalevents"))
        {
            url = URLHelper.UpdateParameterInUrl(url, "categoryname", System.Web.HttpUtility.UrlEncode(ddlEventType.SelectedValue));
            url = URLHelper.UpdateParameterInUrl(url, "approvalevents", System.Web.HttpUtility.UrlEncode(ddlEventType.SelectedValue));
            URLHelper.Redirect(url.Trim());
        }
        else if (ddlEventType.SelectedValue.ToString().ToLower() == "select one" || ddlEventType.SelectedValue.ToString().ToLower() == "view all events" || ddlEventType.SelectedValue.ToString().ToLower() == "view all uca events")
        {
            URLHelper.Redirect(url.Trim());
        }
        else
        {
            url = URLHelper.UpdateParameterInUrl(url, "categoryname", System.Web.HttpUtility.UrlEncode(ddlEventType.SelectedValue));
            URLHelper.Redirect(url.Trim());
        }
    }

    protected void view_Click(object sender, EventArgs e)
    {
        if (CheckGroupPermission("editpages"))
        {
            //Get the reference of the clicked button.
            System.Web.UI.WebControls.LinkButton button = (sender as System.Web.UI.WebControls.LinkButton);

            //Get the command argument
            string commandArgument = button.CommandArgument;

            editDoc.NodeID = ValidationHelper.GetInteger(commandArgument, 0);
            editDoc.Action = "edit";
            editDoc.CheckPermissions = CheckPermissions;
            editDoc.AllowDelete = AllowDelete && CheckGroupPermission("deletepages");
            editDoc.AlternativeFormName = AlternativeFormName;

            pnlEdit.Visible = true;
            pnlList.Visible = false;
        }
    }
}