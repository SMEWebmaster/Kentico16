using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Shooju;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.CustomTables;

/// <summary>
/// Helper class to communicate with Shooju search service
/// </summary>
public static class ShoojuServiceHelper
{
    public static List<ShoojuSearchTerm> GetMostPopularSearches(CacheSettings cs)
    {

        List<ShoojuSearchTerm> searchTerms = new List<ShoojuSearchTerm>();

        if (cs.Cached)
        {
            // Sets a cache dependency for the data
            // The data is removed from the cache if the objects represented by the dummy key are modified (all user objects in this case)
            cs.CacheDependency = CacheHelper.GetCacheDependency("cms.user|all");
        }

        // Lookup code provided by Serge at Shooju

        var conn = new Connection("https://sme.shooju.com", "logreader", "QV2m33BTOCjtjEV3heacLdUrZOPuyBjFKuPe2EiWN8RZl6VPDN");

        var query_params = new Dictionary<string, object>
            {
                { "query", "@fields.user:ms.sme @fields.url_rule:/series"},
                { "per_page", 10},
                { "facets", "@fields.params.query" },
            };
        var res = conn.RawGet("/apilogs", query_params);
        var results = res.GetValue("facets").ToObject<Dictionary<string, dynamic>>();
        var terms = ((JArray)results["@fields.params.query"]["terms"]).ToList();

        int count = 0;
        string strTerm = "";
        terms.ForEach(delegate (JToken term)
        {
            if (count < 10)
            {
                var full_query = term["term"].ToString();
                if (full_query.Contains(") AND ("))
                {
                    var query = full_query.Split(new string[] { ") AND" }, StringSplitOptions.None)[0];
                    query = query.Substring(1, query.Length - 1);

                    strTerm = ValidationHelper.GetString(query, "");
                    if (!IsTermInRestrictedList(strTerm))
                    {
                        searchTerms.Add(new ShoojuSearchTerm(strTerm));
                        count++;
                    }
                }
            }
        });

        return searchTerms;
    }


    private static bool IsTermInRestrictedList(String term)
    {
        string customTableClassName = "smenet.ignoresearchterms";

        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);
        if (customTable != null)
        {
            CustomTableItem item = CustomTableItemProvider.GetItems(customTableClassName).WhereEquals("LOWER(SearchTerm)", term.ToLower()).FirstObject;

            if (item != null)
                return true;                                    
        }
        
        return false;
    }

}