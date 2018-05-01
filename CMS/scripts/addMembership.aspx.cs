using personifyDataservice;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CustomTables;
using System.Data.SqlClient;
public partial class scripts_addMembership : System.Web.UI.Page
{

    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private static readonly string SUri = ConfigurationManager.AppSettings["svcUri_Base"];
    private static readonly string UserName = ConfigurationManager.AppSettings["svcLogin"];
    private static readonly string Password = ConfigurationManager.AppSettings["svcPassword"];
    private readonly SSO.service _wsSso = new SSO.service();


    protected void Page_Load(object sender, EventArgs e)
    {


        LoadCommunitiesfromDataservice();

    }

    public void LoadCommunitiesfromDataservice()
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

                    Response.Write(" Committee : " + ProductCode + " in the commitee group : " + ShortName + " Product Id : " + ProductId +"<br/>");
                }
            }
        }
        catch (Exception Ex)
        {
            Response.Write(Ex.ToString());

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