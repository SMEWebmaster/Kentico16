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

using System.Configuration;
using CMS.CustomTables;
using CMS.DataEngine;

using System.Data.SqlClient;

using System.Web.Configuration;

using System.Xml;
using System.Xml.Serialization;
using CMS.CustomTables;
using personifyDataservice;

[assembly: RegisterCustomClass("Sme.Membershiplist", typeof(Sme.SMEMembership))]
namespace Sme
{
    /// <summary>
    /// Summary description for SMEPersonifyMeetingsTask
    /// </summary>
    public class SMEMembership : ITask
    {
        private string roleName = "niri_registered_event_";

        private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
        private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
        private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
        private static readonly string SUri = ConfigurationManager.AppSettings["svcUri_Base"];
        private static readonly string UserName = ConfigurationManager.AppSettings["svcLogin"];
        private static readonly string Password = ConfigurationManager.AppSettings["svcPassword"];
        private readonly SSO.service _wsSso = new SSO.service();

        public string Execute(TaskInfo ti)
        {
            this.runRoles();
            return null;
        }

        public void runRoles()
        {
            try
            {

                string del = "delete dbo.Sme_Membershiplist";
                execute(del);
                //string customerClassCode = CustomerData.CustomerClassCode.Value;
                Uri ServiceUri = new Uri(SUri);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.Credentials = new NetworkCredential(UserName, Password);
                string ProductId = string.Empty;
                string ProductCode = string.Empty;
                string OrganizationUnitId = string.Empty;
                string ShortName = string.Empty;
                string WebLongDescription = string.Empty;
                string WebShortDescription = string.Empty;

                var CommuniteeList = DataAccessLayer.WebMembershipJoinProducts.ToList();//DataAccessLayer.CustomerInfos.Where(p => p.RecordType == "T").Select(o => o).ToList();

                if (CommuniteeList != null)
                {
                    foreach (var community in CommuniteeList)
                    {
                        ProductId = community.ProductId.ToString();
                        ProductCode = community.ProductCode;
                        OrganizationUnitId = community.OrganizationUnitId;
                        ShortName = community.ShortName;
                        WebLongDescription = community.WebLongDescription.ToString();
                        WebShortDescription = community.WebShortDescription.ToString();
                        // Create new item for custom table with "Sme.CommiteesMaster" code name
                        var item = CustomTableItem.New("Sme.Membershiplist");
                        item.SetValue("ProductId", ProductId);
                        item.SetValue("ProductCode", ProductCode);
                        item.SetValue("OrganizationUnitId", OrganizationUnitId);
                        item.SetValue("ShortName", ShortName);
                        item.SetValue("WebLongDescription", WebLongDescription);
                        item.SetValue("WebShortDescription", WebShortDescription);
                        item.Insert();

                        StringBuilder documentsAddedStatus = new StringBuilder();
                        //string eventLocation = string.Empty;
                        string rootFolder = "error";
                        string folderName = "task";
                        string fileName = "DocumentsStatusLogged.txt";
                        string filePath = Path.Combine(
                            AppDomain.CurrentDomain.BaseDirectory,
                            rootFolder,
                            folderName,
                            fileName);
                        documentsAddedStatus.Append(
                                     "role Added" + ProductCode + ShortName.Trim()
                                    + Environment.NewLine);
                        using (StreamWriter writer = new StreamWriter(filePath, true))
                        {
                            writer.WriteLine(documentsAddedStatus);
                            writer.WriteLine(
                                Environment.NewLine
                                + "-----------------------------------------------------------------------------"
                                + Environment.NewLine);
                        }
                    }
                }
                 

            }
            catch (Exception ex)
            {
                string folderName = "error";
                string fileName = "ExceptionLogged.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, fileName);

                File.Delete(filePath);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(
                        "Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace + ""
                        + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(
                        Environment.NewLine
                        + "-----------------------------------------------------------------------------"
                        + Environment.NewLine);
                }

            }
        }

        private void execute(string query)
        {

            try
            {
                SqlConnection Connection = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                Connection.Open();
                SqlCommand command = new SqlCommand(query, Connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
                Connection.Close();

            }
            catch (Exception ex)
            {

            }
        }

    }

}
