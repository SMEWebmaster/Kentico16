using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.DocumentEngine;
using CMS.SettingsProvider;
using System.Data;
using CMS.SiteProvider;
using CMS.CMSHelper;
using CMS.Localization;
using CMS.PortalControls;
using CMS.Helpers;

public partial class service_Default :  TemplatePage
{
    private int? mSiteCulturesCount = null;
    private int SiteCulturesCount
    {
        get
        {
            if (mSiteCulturesCount == null)
            {
                DataSet dsCultures = CultureInfoProvider.GetSiteCultures(CMSContext.CurrentSiteName);
                mSiteCulturesCount = !CMS.Helpers.DataHelper.DataSourceIsEmpty(dsCultures) ? dsCultures.Tables[0].Rows.Count : 0;
            }
            return mSiteCulturesCount.Value;
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

       //Response.Write(GetWhere());
        //repSearchSQL.Visible = true;
        //repSearchSQL.StopProcessing = false;
        //repSmartSearch.Visible = false;
        //repSmartSearch.StopProcessing = true;

        //repSearchSQL.SelectOnlyPublished = QueryHelper.GetBoolean("searchpublished", true);

        //string culture = QueryHelper.GetString("searchculture", "##ANY##");
        //string mode = QueryHelper.GetString("searchlanguage", null);
        //if ((culture == "##ANY##") || (mode == "<>"))
        //{
        //    culture = TreeProvider.ALL_CULTURES;
        //}
        //else
        //{
        //    repSearchSQL.CombineWithDefaultCulture = false;
        //}
        //repSearchSQL.WhereCondition = searchDialog.WhereCondition;
        //repSearchSQL.CultureCode = culture;
        //repSearchSQL.TransformationName = "CMS.Root.CMSDeskSQLSearchResults";

        //Response.Write("<br/>@@-->" +searchDialog.WhereCondition + "<--<br/>@@");
        string siteName = CMSContent.CurrentSiteName;
        string queryCMSMenuItems=   "SELECT View_CMS_Tree_Joined.DocumentContent,View_CMS_Tree_Joined.NodeAliasPath,View_CMS_Tree_Joined.NodeName,View_CMS_Tree_Joined.NodeOwnerFullName, SearchResultName = CASE View_CMS_Tree_Joined.DocumentName WHEN '' THEN '/' else View_CMS_Tree_Joined.DocumentName END" +
                    "FROM View_CMS_Tree_Joined WHERE SiteName= '" + siteName + "' " +
                    "AND (Published = 1)  AND ((DocumentSearchExcluded IS NULL) OR (DocumentSearchExcluded = 0))" +
                    "and ClassName in('CMS.MenuItem') ORDER BY   ClassName"   ;

        string queryNonContent = "SELECT DISTINCT ClassName FROM View_CMS_Tree_Joined" +
                                "WHERE ((((SiteName = N'" + siteName + "') AND (Published = 1)) AND ((DocumentSearchExcluded IS NULL) OR (DocumentSearchExcluded = 0)))" +
                                 "And ClassName not in ( 'CMS.MenuItem','CMS.Root','CMS.Folder'))";
                        
    
    }
    private string GetWhere()
    {
     
        string where = null;
        string val = QueryHelper.GetString("searchculture", "##ANY##");
        string mode = QueryHelper.GetString("searchlanguage", null);
        const string query = " (NodeID IN (SELECT NodeID FROM View_CMS_Tree_Joined GROUP BY NodeID HAVING (COUNT(NodeID) {0} {1}))) ";
     
        if (val == "")
        {
            val = "##ANY##";
        }

        // If culture IS
        if (mode == "=")
        {
            // document IS in ALL cultures
            if (val == "##ALL##")
            {
                where = String.Format(query, mode, SiteCulturesCount);
            }
        }
        // If culture IS NOT
        else if (mode == "<>")
        {
            switch (val)
            {
                // document IS NOT in ALL cultures
                case "##ALL##":
                    where = String.Format(query, mode, SiteCulturesCount);
                    break;

                // document IS NOT in ANY culture is always empty result
                case "##ANY##":
                    where = SqlHelperClass.NO_DATA_WHERE;
                    break;

                // document IS NOT in one specific culture
                default:
                    where = " (DocumentCulture <> '" + SqlHelperClass.GetSafeQueryString(val, false) + "')";
                    break;
            }
        }
      //  Response.Write(query+ "--" +where );
        return where;
    }
}