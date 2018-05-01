using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using CMS.SettingsProvider;
using CMS.CMSImportExport;
using CMS.DocumentEngine;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using System.Data;
using CMS.SiteProvider;
using CMS.MediaLibrary;
using CMS.DataEngine;

/// <summary>
/// Summary description for KenticoSearch
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class KenticoSearch : System.Web.Services.WebService
{

    public KenticoSearch()
    {
    }


    [WebMethod]
    public string GetDocuments(string date)
    {
        try
        {
            var query =
@"
SELECT  DocumentLastPublished,DocumentContent, [Description], case when ISNULL(DocumentMenuJavascript, '') = '' then NodeAliasPath  else Replace(Replace(Replace(Replace( Replace(Documentmenujavascript,'window.open(',''),')','') ,'''_blank'';return false;','') ,''',',''),'''','') end as NodeAliasPath ,View_SME_CONTENT_MenuItem_Joined.NodeName,View_SME_CONTENT_MenuItem_Joined.IsSecuredNode ,View_SME_CONTENT_MenuItem_Joined.NodeOwnerFullName,View_SME_CONTENT_MenuItem_Joined.DocumentPageDescription ,View_SME_CONTENT_MenuItem_Joined.DocumentPageKeyWords ,View_SME_CONTENT_MenuItem_Joined.DocumentPageTitle , SearchResultName = CASE View_SME_CONTENT_MenuItem_Joined.DocumentName WHEN '' THEN '/' else DocumentName END
FROM View_SME_CONTENT_MenuItem_Joined 
WHERE 
    SiteName= @SiteName
    AND (Published = 1)  AND ((DocumentSearchExcluded IS NULL) OR (DocumentSearchExcluded = 0))
    AND ClassName in ('CMS.MenuItem','SME.MenuItem')  AND (@IgnoreDate = 1 OR DocumentModifiedWhen   >= @CreatedWhen) 
ORDER BY   DocumentLastPublished,DocumentName ";

            var parameters = new QueryDataParameters();
            parameters.Add("@SiteName", CMS.SiteProvider.SiteContext.CurrentSiteName);

            var createdWhen = DateTime.Now;
            parameters.Add("@IgnoreDate", !DateTime.TryParse(date, out createdWhen), typeof(bool));
            parameters.AddDateTime("@CreatedWhen", createdWhen);

            var ds = CMS.DataEngine.ConnectionHelper.ExecuteQuery(query, parameters, CMS.DataEngine.QueryTypeEnum.SQLQuery);

            return ds == null ? String.Empty : ds.GetXml();

        }
        catch (Exception ex)
        {
            return String.Empty;
        }
    }

   

    [WebMethod]
    public string GetStructuredDocumentsData(string date)
    {
        try
        {
            var query =
@"
WITH CTE AS (SELECT DocumentID,
	   DocumentModifiedWhen,
       DocumentLastPublished,
       EventDate AS DocumentCreatedWhen,
       DocumentPageKeyWords,
       DocumentPageDescription,
       DocumentPageTitle,
       IsSecuredNode,
	   Published,
	   NodeAliasPath,
       NodeName,
       (EventName + ' ' + EventSummary + ' ' + EventDetails + ' ' + EventLocation) AS ContentData,
       (EventSummary + '  ' + EventLocation) AS Summary,
       ClassName
FROM View_SME_CONTENT_Event_Joined va
UNION
SELECT DocumentID,
	   DocumentModifiedWhen,
       DocumentLastPublished,
       EventDate AS DocumentCreatedWhen,
       DocumentPageKeyWords,
       DocumentPageDescription,
       DocumentPageTitle,
       IsSecuredNode,
	   Published,
	   NodeAliasPath,
       NodeName,
       (EventName + ' ' + EventSummary + ' ' + EventDetails + ' ' + EventLocation) AS ContentData,
       (EventSummary + '  ' + EventLocation) AS Summary,
       ClassName
FROM View_UCA_CONTENT_Event_Joined va
UNION
SELECT DocumentID,
	   DocumentModifiedWhen,
       DocumentLastPublished,
       DocumentCreatedWhen,
       DocumentPageKeyWords,
       DocumentPageDescription,
       DocumentPageTitle,
       IsSecuredNode,
	   Published,
	   NodeAliasPath,
       NodeName,
       (DocumentContent + ' ' + Title + ' ' + Author) AS ContentData,
       '' AS Summary,
       ClassName
FROM View_SME_Publication_Joined va)
SELECT 
		STUFF(
			(SELECT ', ' + CategoryDisplayName
				FROM CMS_Category C
				WHERE categoryID IN (SELECT
				categoryID
				FROM cms_documentcategory
				WHERE documentid = CTE.DocumentID)
				FOR xml PATH (''))
			, 1, 1, '') AS Categoryname,
		STUFF(
			(SELECT ', ' + convert(nvarchar(4), CategoryID)
				FROM CMS_Category C
				WHERE categoryID IN (SELECT
				categoryID
				FROM cms_documentcategory
				WHERE documentid = CTE.DocumentID)
				FOR xml PATH (''))
			, 1, 1, '') AS CategoryIds,
	DocumentModifiedWhen,
	DocumentLastPublished,
	DocumentCreatedWhen,
	DocumentPageKeyWords,
	DocumentPageDescription,
	DocumentPageTitle,
	IsSecuredNode,
	Published,
	NodeAliasPath,
	NodeName,
	ContentData,
	Summary,
	ClassName
FROM CTE WHERE Published = 1
AND (@IgnoreDate = 1 OR DocumentModifiedWhen >= @CreatedWhen)
ORDER BY DocumentModifiedWhen desc";

            var parameters = new QueryDataParameters();

            var createdWhen = DateTime.Now;
            parameters.Add("@IgnoreDate", !DateTime.TryParse(date, out createdWhen), typeof(bool));
            parameters.AddDateTime("@CreatedWhen", createdWhen);

            var ds = CMS.DataEngine.ConnectionHelper.ExecuteQuery(query, parameters, CMS.DataEngine.QueryTypeEnum.SQLQuery);

            return ds == null ? String.Empty : ds.GetXml();

        }
        catch (Exception ex)
        {
            return String.Empty;
        }
    }

    [WebMethod]
    public string GetNewsCategories()
    {
        try
        {

            var query = "select CategoryParentID,categoryid,CategoryDisplayName,categoryname  from CMS_Category";
            var ds = CMS.DataEngine.ConnectionHelper.ExecuteQuery(query, null, CMS.DataEngine.QueryTypeEnum.SQLQuery);

            return ds == null ? String.Empty : ds.GetXml();
        }
        catch (Exception)
        { 
            return String.Empty;
        }
    }

    [WebMethod]
    public string GetLibrary()
    {
        return new DataSet().GetXml();
    }
}
