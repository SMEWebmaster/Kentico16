using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.DocumentEngine;
using CMS.Membership;
using CMS.Scheduler;
using CMS.SiteProvider;
using System.Data;
using CMS;
using CMS.Helpers;
using System.Net;
using System.Text;
using System.IO;
using personifyDataservice;
using System.Configuration;
using CMS.EventLog;

[assembly: RegisterCustomClass("PersonifyMeetings.SMEPersonifyMeetingsTask", typeof(PersonifyMeetings.SMEPersonifyMeetingsTask))]
namespace PersonifyMeetings
{
    /// <summary>
    /// Summary description for SMEPersonifyMeetingsTask
    /// </summary>
    public class SMEPersonifyMeetingsTask : ITask
    {
        public string Execute(TaskInfo ti)
        {
            try
            {
                string eventName = string.Empty;
                string eventStartdate = string.Empty;
                string eventEnddate = string.Empty;
                string eventDescription = string.Empty;
                string eventCategory = string.Empty;
                long eventProductId;
                StringBuilder documentsAddedStatus = new StringBuilder();//string eventLocation = string.Empty;
                string eventWebsite = string.Empty;
                string eventContact = string.Empty;
                string eventFax = string.Empty;
                string eventFacilityID = string.Empty;

                //Communicates with personify web service and gets the list of meetings

                string dataServiceUri = ConfigurationManager.AppSettings["svcUri_Base"];
                string dataServiceUserName = ConfigurationManager.AppSettings["svcLogin"];
                string dataServicePassword = ConfigurationManager.AppSettings["svcPassword"];

                Uri ServiceUri = new Uri(dataServiceUri);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.Credentials = new NetworkCredential(dataServiceUserName, dataServicePassword);
                var eventsListPersonify = DataAccessLayer.WebProductViews.Where(p => p.Subsystem == "MTG" && p.MeetingEndDate >= DateTime.Now.AddDays(-2)).ToList();

                DataSet eventsList = DocumentHelper.GetDocuments("SME.EVENT")
                                                          .OnSite(SiteContext.CurrentSiteName)
                                                          .Culture("en-us")
                                                          .CombineWithDefaultCulture(false)
                                                          .All();

                string un = "smeadmin";
                string pwd = "533@dm1n";

                //authenticates user for publishing documents/pages/events
                AuthenticateUser(un, pwd);

                UserInfo ui = UserInfoProvider.GetUserInfo(un);


                // Create new instance of the Tree provider
                TreeProvider tree = new TreeProvider(ui);

                // Get parent node
                TreeNode parentNode = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/events-professional-development/events", "en-us");

                foreach (var events in eventsListPersonify)
                {
                    eventName = events.ShortName;
                    eventProductId = events.ProductId;
                    eventStartdate = events.MeetingStartDate.ToString();
                    eventEnddate = events.MeetingEndDate.ToString();
                    eventDescription = events.WebShortDescription;
                    eventCategory = events.ProductClassCodeString;
                    eventFacilityID = events.FacilityMasterCustomerId;

                    string[] addressDelimiter = new string[] { "\r\n" };
                    string[] address;
                    string eventAddress = string.Empty;
                    string eventCity = string.Empty;
                    string eventCountry = string.Empty;
                    string eventStateProvince = string.Empty;
                    string eventZipPostalCode = string.Empty;
                    string eventLocation = string.Empty;
                    string eventEmail = string.Empty;
                    string eventPhone = string.Empty;
                    bool isAllowSocialEvents = events.ProductClassCodeString.ToLower() != "social_event";

                    if (eventFacilityID != "" && eventFacilityID != null)
                    {
                        var CommuniteeList = DataAccessLayer.AddressInfos.Where(p => p.MasterCustomerId == eventFacilityID).ToList();

                        foreach (var item in CommuniteeList)
                        {
                            address = item.AddressLabel.Split(addressDelimiter, StringSplitOptions.None);
                            eventAddress = address[0] + " " + address[1];
                            eventCity = item.City;
                            eventCountry = item.CountryCode;
                            eventStateProvince = item.State;
                            eventZipPostalCode = item.PostalCode;
                            eventLocation = item.City + ", " + item.CountryCode;
                            eventEmail = item.MailStop;
                            eventPhone = item.PersonalLine;
                        }
                    }

                    if (parentNode != null)
                    {
                        if (!DocumentExists(events.ProductId, eventsList) && isAllowSocialEvents)
                        {
                            // Create documents

                            var newNode = CMS.DocumentEngine.TreeNode.New("SME.EVENT", tree);
                            newNode.DocumentName = events.LongName;
                            newNode.NodeAlias = eventName;
                            newNode.DocumentCulture = "en-us";
                            newNode.SetValue("EventName", eventName);
                            newNode.SetValue("ProductId", eventProductId);
                            newNode.SetValue("StartDate", eventStartdate);
                            newNode.SetValue("EndDate", eventEnddate);
                            newNode.SetValue("EventDetails", eventDescription);
                            newNode.SetValue("EventCategory", eventCategory);
                            newNode.SetValue("Location", eventLocation);
                            //newNode.SetValue("TargetUrl", eventWebsite);
                            //newNode.SetValue("Contact", eventContact);
                            newNode.SetValue("email", eventEmail);
                            newNode.SetValue("Address", eventAddress);
                            newNode.SetValue("City", eventCity);
                            newNode.SetValue("State_Province", eventStateProvince);
                            newNode.SetValue("Zip_PostalCode", eventZipPostalCode);
                            newNode.SetValue("Country", eventCountry);
                            newNode.SetValue("Phone", eventPhone);
                            newNode.SetValue("AllowPersonifyUpdate", true);
                            //newNode.SetValue("Fax", eventFax);
                            newNode.DocumentPageTemplateID = 24357;
                            newNode.Insert(parentNode);
                            //newNode.Publish();
                            documentsAddedStatus.Append("Meeting " + eventName + " added into kentico at " + DateTime.Now + Environment.NewLine);
                        }
                        else
                        {
                            // Update the Document
                            var updateNode = eventsList.Tables[0].AsEnumerable().Where(row => events.ProductId == row.Field<long?>("ProductId")).Select(row => new
                            {
                                ID = row.Field<int>("NodeId"),
                            }).ToList();
                            if (updateNode.Count == 1 && isAllowSocialEvents)
                            {
                                TreeNode node = tree.SelectSingleNode(updateNode[0].ID, "en-us", "SME.Event");

                                if (node.GetBooleanValue("AllowPersonifyUpdate", false))
                                {
                                    //node.DocumentName = events.LongName;
                                    //node.NodeAlias = eventName;
                                    node.DocumentCulture = "en-us";
                                    //node.SetValue("EventName", eventName);
                                    node.SetValue("ProductId", eventProductId);
                                    node.SetValue("StartDate", eventStartdate);
                                    node.SetValue("EndDate", eventEnddate);
                                    //this is the optional field which editors can update from CMS Desk.
                                    //node.SetValue("EventDetails", eventDescription);
                                    node.SetValue("EventCategory", eventCategory);
                                    node.SetValue("Location", eventLocation);
                                    //node.SetValue("TargetUrl", eventWebsite);
                                    //node.SetValue("Contact", eventContact);
                                    node.SetValue("email", eventEmail);
                                    node.SetValue("Address", eventAddress);
                                    node.SetValue("City", eventCity);
                                    node.SetValue("State_Province", eventStateProvince);
                                    node.SetValue("Zip_PostalCode", eventZipPostalCode);
                                    node.SetValue("Country", eventCountry);
                                    node.SetValue("Phone", eventPhone);
                                    //node.SetValue("Fax", eventFax);

                                    node.SetValue("LastUpdate", DateTime.Now);

                                    node.DocumentPageTemplateID = 24357;
                                    node.Update();
                                    DocumentHelper.UpdateDocument(node);
                                    //node.Publish();
                                    documentsAddedStatus.Append("Meeting " + eventName + " updated in kentico at " + DateTime.Now + Environment.NewLine);
                                }
                                else
                                {
                                    documentsAddedStatus.Append("Meeting " + eventName + " skipped due to document setting in document at " + DateTime.Now + Environment.NewLine);
                                }
                            }
                        }
                    }
                }



                EventLogProvider.LogInformation("PersonifyMeeting", "Import", documentsAddedStatus.ToString());

            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("PersonifyMeeting", "Import", ex);
            }
            return null;
        }

        /// <summary>
        /// Check if Document is already present in Content Tree.
        /// </summary>
        /// <param name="id"> ID </param>
        /// <returns> True / False </returns>

        public static bool DocumentExists(long id, DataSet eventDocuments)
        {
            var contains = false;
            if (!DataHelper.DataSourceIsEmpty(eventDocuments))
            {
                contains = eventDocuments.Tables[0].AsEnumerable().Any(row => id == row.Field<long?>("ProductId"));
            }

            return contains;
        }

        /// <summary>
        /// Authenticates user
        /// </summary>
        /// <returns></returns>
        private void AuthenticateUser(string un, string pwd)
        {
            // Get the user
            UserInfo user = UserInfoProvider.GetUserInfo(un);
            if (user != null)
            {
                AuthenticationHelper.AuthenticateUser(un, pwd, SiteContext.CurrentSiteName);
            }
        }

    }

}