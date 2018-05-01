using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Reporting;
using CMS.Base;
using CMS.DataEngine;

public partial class CMSModules_Reporting_FormControls_ReportItemSelector : FormEngineUserControl
{
    #region "Private variables"

    private bool mDisplay = true;
    private DataSet mCurrentDataSet = null;
    private bool mKeepDataInWindowsHelper = false;
    private string mFirstItemText = String.Empty;
    private ReportInfo mReportInfo = null;
    private bool mSetValues = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether id value should be used in selector
    /// </summary>
    public bool UseIDValue
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the current data set.
    /// </summary>
    private DataSet CurrentDataSet
    {
        get
        {
            DataSet ds = WindowHelper.GetItem(CurrentGuid()) as DataSet;
            if (DataHelper.DataSourceIsEmpty(ds))
            {
                if (DataHelper.DataSourceIsEmpty(mCurrentDataSet))
                {
                    mCurrentDataSet = LoadFromXML(Convert.ToString(ViewState["ParametersXmlData"]), Convert.ToString(ViewState["ParametersXmlSchema"]));
                }
                ds = mCurrentDataSet;
            }
            return ds;
        }
    }


    /// <summary>
    /// If false control shows only report selector.
    /// </summary>
    public bool ShowItemSelector
    {
        get
        {
            return GetValue("ShowItemSelector", false);
        }
        set
        {
            SetValue("ShowItemSelector", value);
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();

            string usReportsValue = GetString(usReports.Value.ToString());
            string usItemsValue = GetString(usItems.Value.ToString());

            if (!ShowItemSelector)
            {
                if (usReportsValue == "0")
                {
                    return String.Empty;
                }
                return usReportsValue;
            }
            else
            {
                if ((usReportsValue == "0") || (usItemsValue == "0"))
                {
                    return String.Empty;
                }
                return String.Format("{0};{1}", usReportsValue, usItemsValue);
            }
        }
        set
        {
            EnsureChildControls();

            // Convert input value to string
            string values = Convert.ToString(value);

            // Check whether value is defined
            if (!String.IsNullOrEmpty(values))
            {
                if (ShowItemSelector)
                {
                    // Get report name and item name
                    string[] items = values.Split(';');
                    // Check whether all required items are defined
                    if ((items != null) && (items.Length == 2))
                    {
                        // Set report and item values
                        usReports.Value = items[0];
                        usItems.Value = items[1];
                    }
                }
                else
                {
                    usReports.Value = values;
                }

                if ((Form != null) && (Form.Data != null) && !RequestHelper.IsPostBack())
                {
                    // Check if the schema information is available
                    IDataContainer data = Form.Data;
                    if (data.ContainsColumn("ParametersXmlSchema") && data.ContainsColumn("ParametersXmlData"))
                    {
                        // Get xml schema and data                    
                        string schema = Convert.ToString(Form.GetFieldValue("ParametersXmlSchema"));
                        string xml = Convert.ToString(Form.GetFieldValue("ParametersXmlData"));

                        LoadFromXML(xml, schema);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Type of report (graph,table,value).
    /// </summary>
    public ReportItemType ReportType
    {
        get
        {
            return (ReportItemType)GetValue("ReportType", (int)ReportItemType.All);
        }
        set
        {
            SetValue("ReportType", (int)value);
        }
    }


    /// <summary>
    /// Report Id.
    /// </summary>
    public int ReportID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the item uniselector drop down list client id for javascript use.
    /// </summary>
    public string UniSelectorClientID
    {
        get
        {
            EnsureChildControls();
            return usItems.UseUniSelectorAutocomplete ? usItems.AutocompleteValueClientID : usItems.DropDownSingleSelect.ClientID;
        }
    }


    /// <summary>
    /// Indicates weather display report drop down list.
    /// </summary>
    public bool Display
    {
        get
        {
            return mDisplay;
        }
        set
        {
            mDisplay = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns an array of values of any other fields returned by the control.
    /// </summary>
    /// <remarks>It returns an array where first dimension is attribute name and the second dimension is its value.</remarks>
    public override object[,] GetOtherValues()
    {
        // Get current dataset
        DataSet ds = CurrentDataSet;

        // Set properties names
        object[,] values = new object[2, 2];
        values[0, 0] = "ParametersXmlSchema";
        values[1, 0] = "ParametersXmlData";

        // Check whether dataset is defined
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            // Set dataset values
            values[0, 1] = ds.GetXmlSchema();
            values[1, 1] = ds.GetXml();
        }

        return values;
    }


    /// <summary>
    /// Loads dataset to windows helper from schema and data xml definitions.
    /// </summary>
    /// <param name="xml">XML data</param>
    /// <param name="schema">XML schema</param>
    protected DataSet LoadFromXML(string xml, string schema)
    {
        // Check whether schema and data are defined
        if (!String.IsNullOrEmpty(schema) && !String.IsNullOrEmpty(xml))
        {
            //Create data set from xml
            DataSet ds = new DataSet();

            // Load schema
            StringReader sr = StringReader.New(schema);
            ds.ReadXmlSchema(sr);

            // Load data
            ds.TryReadXml(xml);

            // Set current dataset
            WindowHelper.Add(CurrentGuid(), ds);

            return ds;
        }

        return null;
    }


    /// <summary>
    /// Builds condition for report selector
    /// </summary>
    private void BuildReportCondition()
    {
        switch (ReportType)
        {
            case ReportItemType.Graph:
                usReports.WhereCondition = "EXISTS (SELECT GraphID FROM Reporting_ReportGraph as graph WHERE (graph.GraphIsHtml IS NULL OR graph.GraphIsHtml = 0) AND graph.GraphReportID = ReportID)";
                break;

            case ReportItemType.HtmlGraph:
                usReports.WhereCondition = "EXISTS (SELECT GraphID FROM Reporting_ReportGraph as graph WHERE (graph.GraphIsHtml = 1) AND graph.GraphReportID = ReportID)";
                break;

            case ReportItemType.Table:
                usReports.WhereCondition = "EXISTS (SELECT TableID FROM Reporting_ReportTable as reporttable WHERE reporttable.TableReportID = ReportID)";
                break;

            case ReportItemType.Value:
                usReports.WhereCondition = "EXISTS (SELECT ValueID FROM Reporting_ReportValue as value WHERE value.ValueReportID = ReportID)";
                break;

            // By default do nothing
            default:
                break;
        }
    }


    /// <summary>
    /// Builds conditions for particular type of selector.
    /// </summary>
    protected void BuildConditions()
    {
        switch (ReportType)
        {
            case ReportItemType.Graph:
                usItems.WhereCondition = "GraphReportID = " + ReportID + " AND (GraphIsHtml IS NULL OR GraphIsHtml = 0)";
                usItems.DisplayNameFormat = "{%GraphDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "GraphID" : "GraphName";
                usItems.ObjectType = "reporting.reportgraph";
                mFirstItemText = GetString("rep.graph.pleaseselect");
                break;

            case ReportItemType.HtmlGraph:
                usItems.WhereCondition = "GraphReportID = " + ReportID + " AND (GraphIsHtml = 1)";
                usItems.DisplayNameFormat = "{%GraphDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "GraphID" : "GraphName";
                usItems.ObjectType = "reporting.reportgraph";
                mFirstItemText = GetString("rep.graph.pleaseselect");
                break;

            case ReportItemType.Table:
                usItems.WhereCondition = "TableReportID = " + ReportID;
                usItems.ObjectType = "reporting.reporttable";
                usItems.DisplayNameFormat = "{%TableDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "TableID" : "TableName";
                mFirstItemText = GetString("rep.table.pleaseselect");
                break;

            case ReportItemType.Value:
                usItems.WhereCondition = "ValueReportID = " + ReportID;
                usItems.ObjectType = "reporting.reportvalue";
                usItems.DisplayNameFormat = "{%ValueDisplayName%}";
                usItems.ReturnColumnName = UseIDValue ? "ValueID" : "ValueName";
                mFirstItemText = GetString("rep.value.pleaseselect");
                break;

            // By default do nothing
            default:
                break;
        }
    }


    /// <summary>
    /// Forces Items Uni select to reload.
    /// </summary>
    public void ReloadItems()
    {
        string selected = usItems.DropDownSingleSelect.SelectedValue;

        usItems.Reload(true);

        try
        {
            usItems.DropDownSingleSelect.SelectedValue = selected;
        }
        catch
        {
        }
    }


    /// <summary>
    /// Gets GUID from hidden field .. if not there create new one.
    /// </summary>
    private String CurrentGuid()
    {
        // For reloaddata (f.e. webpart save) store guid also in request helper, because hidden is empty after control is reloaded.
        Guid guid = ValidationHelper.GetGuid(RequestStockHelper.GetItem("wppreportselector"), Guid.Empty);
        if (hdnGuid.Value == String.Empty)
        {
            hdnGuid.Value = (guid == Guid.Empty) ? Guid.NewGuid().ToString() : guid.ToString();
        }

        if (guid == Guid.Empty)
        {
            RequestStockHelper.Add("wppreportselector", hdnGuid.Value);
        }

        return hdnGuid.Value;
    }


    protected override void OnLoad(EventArgs e)
    {
        // First item as "please select .." - not default "none"
        usItems.AllowEmpty = false;
        usReports.AllowEmpty = false;

        usReports.SpecialFields.Add(new SpecialField() { Text = "(" + GetString("rep.pleaseselect") + ")", Value = "0" });

        // Disable 'please select' for items selector
        usItems.MaxDisplayedItems = usItems.MaxDisplayedTotalItems = 1000;

        BuildReportCondition();
        usReports.OnSelectionChanged += usReports_OnSelectionChanged;
        base.OnLoad(e);
    }


    protected override void OnPreRender(EventArgs e)
    {
        pnlReports.Attributes.Add("style", "margin-bottom:3px");

        string reportName = ValidationHelper.GetString(usReports.Value, String.Empty);
        mReportInfo = ReportInfoProvider.GetReportInfo(reportName);
        if (mReportInfo != null)
        {
            usItems.Enabled = true;

            // Test if there is any item visible in report parameters
            FormInfo fi = new FormInfo(mReportInfo.ReportParameters);

            // Get dataset from cache
            DataSet ds = (DataSet)WindowHelper.GetItem(CurrentGuid());
            DataRow dr = fi.GetDataRow(false);
            fi.LoadDefaultValues(dr, true);
            bool itemVisible = false;
            List<IField> items = fi.ItemsList;
            foreach (IField item in items)
            {
                FormFieldInfo ffi = item as FormFieldInfo;
                if (ffi != null)
                {
                    if (ffi.Visible)
                    {
                        itemVisible = true;
                        break;
                    }
                }
            }

            ReportID = mReportInfo.ReportID;

            if (!itemVisible)
            {
                plcParametersButtons.Visible = false;
            }
            else
            {
                plcParametersButtons.Visible = true;
            }
        }
        else
        {
            if (ReportID == 0)
            {
                plcParametersButtons.Visible = false;
                usItems.Enabled = false;
            }
        }

        ltlScript.Text = ScriptHelper.GetScript("function refresh () {" + ControlsHelper.GetPostBackEventReference(pnlUpdate, String.Empty) + "}");
        usReports.DropDownSingleSelect.AutoPostBack = true;

        if (!mDisplay)
        {
            pnlReports.Visible = false;
            plcParametersButtons.Visible = false;
            usItems.Enabled = true;
        }

        if (!ShowItemSelector)
        {
            pnlItems.Visible = false;
        }

        usItems.IsLiveSite = IsLiveSite;
        usReports.IsLiveSite = IsLiveSite;
        ugParameters.GridName = "~/CMSModules/Reporting/FormControls/ReportParametersList.xml";
        ugParameters.ZeroRowsText = String.Empty;
        ugParameters.PageSize = "##ALL##";
        ugParameters.Pager.DefaultPageSize = -1;

        BuildConditions();

        if (mReportInfo == null)
        {
            usItems.SpecialFields.Add(new SpecialField() { Text = "(" + mFirstItemText + ")", Value = "0" });
        }

        if (ShowItemSelector)
        {
            ReloadItems();
        }

        if (mSetValues)
        {
            WindowHelper.Add(CurrentGuid(), CurrentDataSet);
	        ScriptHelper.RegisterDialogScript(Page);
            ScriptHelper.RegisterStartupScript(Page, typeof(Page), "OpenModalWindowReportItem", ScriptHelper.GetScript("modalDialog('" + ResolveUrl("~/CMSModules/Reporting/Dialogs/ReportParametersSelector.aspx?ReportID=" + ReportID + "&guid=" + CurrentGuid().ToString()) + "','ReportParametersDialog', 700, 500);"));
            mKeepDataInWindowsHelper = true;
        }

        // Apply reportid condition if report was selected via uniselector
        if (mReportInfo != null)
        {
            DataSet ds = CurrentDataSet;

            ViewState["ParametersXmlData"] = null;
            ViewState["ParametersXmlSchema"] = null;

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ViewState["ParametersXmlData"] = ds.GetXml();
                ViewState["ParametersXmlSchema"] = ds.GetXmlSchema();
            }

            if (!mKeepDataInWindowsHelper)
            {
                WindowHelper.Remove(CurrentGuid().ToString());
            }

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ds = DataHelper.DataSetPivot(ds, new string[] { "ParameterName", "ParameterValue" });
                ugParameters.DataSource = ds;
                ugParameters.ReloadData();
                pnlParameters.Visible = true;
            }
            else
            {
                pnlParameters.Visible = false;
            }
        }
        else
        {
            pnlParameters.Visible = false;
        }

        base.OnPreRender(e);
    }


    protected void btnSet_Click(object sender, EventArgs e)
    {
        mSetValues = true;
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        WindowHelper.Remove(CurrentGuid());
        ViewState["ParametersXmlData"] = null;
        ViewState["ParametersXmlSchema"] = null;
    }


    protected void usReports_OnSelectionChanged(object sender, EventArgs ea)
    {
        WindowHelper.Remove(CurrentGuid());
        ViewState["ParametersXmlData"] = null;
        ViewState["ParametersXmlSchema"] = null;

        // Try to set first item
        if (usItems.DropDownSingleSelect.Items.Count > 0)
        {
            usItems.DropDownSingleSelect.SelectedIndex = 0;
        }
    }

    #endregion
}