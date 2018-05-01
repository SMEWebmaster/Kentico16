using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.MacroEngine;
using CMS.Helpers;
using System.Configuration;
using System.Net;
using personifyDataservice;
using CMS.EventLog;

[assembly: CMS.RegisterExtension(typeof(SMEMacroMethods), typeof(SystemNamespace))]
public class SMEMacroMethods : MacroMethodContainer
{
    private static readonly string SUri = ConfigurationManager.AppSettings["svcUri_Base"];
    private static readonly string UserName = ConfigurationManager.AppSettings["svcLogin"];
    private static readonly string Password = ConfigurationManager.AppSettings["svcPassword"];


    [MacroMethod(typeof(string), "Returns renewal status of a given user.", 0)]
    [MacroMethodParam(0, "UserName", typeof(string), "UserName of member.")]
    public static object GetRenewalStatus(EvaluationContext context, params object[] parameters)
    {

        // make sure current user is logged in
        if (!AuthenticationHelper.IsAuthenticated())
            return "join";

        // by default use current logged in user
        UserInfo user = MembershipContext.AuthenticatedUser;

        // if username is supplied, try to look up user with username
        if (parameters.Length == 1)
        {
            user = UserInfoProvider.GetUserInfo(parameters[0].ToString());
        }

        // If user is null, then exit
        if (user == null)
            return "join";

        // some users had a | at the end of their customer ID, remove that and everything after
        string customerID = user.GetStringValue("PersonifyIdentifier", "");

        if (String.IsNullOrEmpty(customerID))
            return "join";
        else if (customerID.Contains("|"))
        {
            customerID = customerID.Substring(0, (customerID.IndexOf('|')));
        }

        String status = CacheHelper.Cache(cs => LookupRenewalStatus(cs, customerID), new CacheSettings(10, "renewalstatus|" + customerID));

        return status;

    }

    [MacroMethod(typeof(string), "Returns list of most recent search terms.", 0)]
    public static object GetRecentSearchTerms(EvaluationContext context, params object[] parameters)
    {
        List<ShoojuSearchTerm> searchTerms = CacheHelper.Cache(cs => ShoojuServiceHelper.GetMostPopularSearches(cs), new CacheSettings(10, "shooju|mostrecentsearches"));
        return searchTerms;
    }

    private static String LookupRenewalStatus(CacheSettings cs, String CustomerID)
    {

        // Build service call
        Uri ServiceUri = new Uri(SUri);
        PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
        DataAccessLayer.Credentials = new NetworkCredential(UserName, Password);

        List<OrderDetailInfo> orderList = new List<OrderDetailInfo>();
        try
        {

            DataAccessLayer.RenewalNationalMemberships.Where(rm => rm.ShipMasterCustomerId == CustomerID);


            // Get all unpaid or canceled membership orders
            var orderInfo = DataAccessLayer.OrderDetailInfos.Where(o => o.ShipMasterCustomerId == CustomerID && o.ProductCode == "PROF1YR");
            // 

            if (orderInfo == null)
                return "current";

            orderList = orderInfo.ToList();
            orderList = orderList.OrderByDescending(o => o.OrderNumber).Take(2).ToList(); // get 2 most recent orders
        }
        catch (Exception excp)
        {
            EventLogProvider.LogException("RenewalStatus", "Lookup", excp, SiteContext.CurrentSiteID, "CustomerID : " + CustomerID);
            return "join";
        }

        if (cs.Cached)
        {
            // Sets a cache dependency for the data
            // The data is removed from the cache if the objects represented by the dummy key are modified (all user objects in this case)
            cs.CacheDependency = CacheHelper.GetCacheDependency("cms.user|all");
        }

        /*
        // No orders, then re
        if (orderList == null)
            return "join";
        else if (orderList.Count == 0)
            return "current";
        // 1 open order, then renew or join if last order was canceled
        else if (orderList.Count == 1)
        {
            if (orderList[0].LineStatusCode == "C")  // last order was unpaid, they're no longer an active member
                return "join";
            else // Line Status Code = P 
                return "renew";
        }
            
        // more than 1 open order, then they need to join
        else
            return "join";
            */

        if (orderList.Count == 0)
        {
            return "join";
        }
        else if (orderList.Count == 1)
        {
            if (orderList[0].LineStatusCode == "A") // 1 order that's been approved
                return "current";
            else
                return "join";
        }
        else
        {
            //EventLogProvider.LogInformation("Renewal", "Lookup", "Order #1: " + orderList[0].LineStatusCode + " " + orderList[0].OrderDate + " " + orderList[0].FulfillStatusCodeString + "<br />Order #2" + orderList[1].LineStatusCode + " " + orderList[1].OrderDate + " " + orderList[1].FulfillStatusCodeString);

            if (orderList[0].LineStatusCode == "A") // Latest membership was paid
                return "current";

            if (orderList[0].LineStatusCode == "C" && orderList[1].LineStatusCode == "C")  // all orders canceled
                return "join";

            // previous order was processed - Active            
            if (orderList[1].LineStatusCode == "A")
            {
                if (orderList[0].LineStatusCode == "P") // open renewal - Pending
                    return "renew";
                else if (orderList[0].LineStatusCode == "C") // Most recent order canceled
                    return "join";
            }

            return "current";
        }

    }

}