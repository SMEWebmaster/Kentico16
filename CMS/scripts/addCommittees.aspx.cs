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
using personifyDataservicesme;

public partial class scripts_addCommittees : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /// load add Communitties 
        
        
        LoadCommunitiesfromDataservice();
       
    }

    public void LoadCommunitiesfromDataservice()
    {
        try
        {
          string del = "delete  Sme_CommiteesMaster";
           execute(del);
            //string customerClassCode = CustomerData.CustomerClassCode.Value;
            Uri ServiceUri = new Uri("http://smemi.personifycloud.com/PersonifyDataServices/PersonifyDatasme.svc");
                personifyDataservicesme.PersonifyEntitiesSME DataAccessLayer =
                    new personifyDataservicesme.PersonifyEntitiesSME(ServiceUri);
                DataAccessLayer.Credentials = new NetworkCredential("admin", "admin");
           
                var CommuniteeList = DataAccessLayer.CustomerInfos.Where(p => p.RecordType == "T" ).Select(o => o).ToList();
                     CommuniteeList = CommuniteeList.Where(p => p.CustomerStatusCode == "ACTIVE").Select(o => o).ToList();

                 
            string MasterCustomerId = string.Empty;
            string LabelName = string.Empty;
            string CustomerClassCode = string.Empty;
            string CurrencyCode = string.Empty;
            string SubCustomerId = string.Empty;
//
            //var CommuniteeList = DataAccessLayer.CustomerInfos.Where(p => p.RecordType == "T").Select(o => o).ToList();
                //CommuniteeList = CommuniteeList.Where(p => p.CustomerStatusCode  == "ACTIVE").Select(o => o).ToList();

            if (CommuniteeList != null)
            {
                foreach (var community in CommuniteeList)
                {
                    MasterCustomerId = community.MasterCustomerId;
                    LabelName = community.LabelName;
                    CustomerClassCode = community.CustomerClassCode;
                    CurrencyCode = community.CurrencyCode;
                    SubCustomerId = community.SubCustomerId.ToString();

                    // Create new item for custom table with "Sme.CommiteesMaster" code name
                     var item = CustomTableItem.New("Sme.CommiteesMaster");
                    item.SetValue("MasterCustomerId", MasterCustomerId);
                    item.SetValue("LabelName", LabelName);
                    item.SetValue("CustomerClassCode", CustomerClassCode);
                    item.SetValue("CurrencyCode", CurrencyCode);
                    item.SetValue("SubCustomerId", SubCustomerId);
                    item.Insert();
                   
                    Response.Write("Added Committee : " + LabelName + " in the commitee group : " + CustomerClassCode + "<br/>");
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