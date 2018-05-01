using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;

using personifyDataservice;
 using System.Net;


using CMS.Helpers;
using CMS.PortalControls;
using System.Configuration;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
 
 
public partial class CMSWebParts_Custom_Personify_committeelist : CMSAbstractWebPart
{

    #region "Properties"
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];

    /// <summary>
    /// Load pages individually.
    /// </summary>
    public string DivisionFilter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DivisionFilter"), String.Empty);
        }
        set
        {
            SetValue("DivisionFilter", value);
            //  srcMedia.OrderBy = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Response.Write(CommitteeFilter);
            bindCommunities();
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }


    public void bindCommunities()
    {
        try
        {

            string whereCondition = "";
            if (DivisionFilter != null)
            {
                litTitle.Text = "Division Committees"; 
                whereCondition = "  where CustomerClassCode='" + DivisionFilter + "' ";
                string dataServiceUri =  ConfigurationManager.AppSettings["svcUri_Base"];
                string dataServiceUserName = ConfigurationManager.AppSettings["svcLogin"];
                string dataServicePassword = ConfigurationManager.AppSettings["svcPassword"];
                
                
                Uri ServiceUri = new Uri(dataServiceUri);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.IgnoreMissingProperties = true;
                DataAccessLayer.Credentials = new NetworkCredential(dataServiceUserName, dataServicePassword);
                var cusDemographics = DataAccessLayer.CusRelationships.Where(p => p.MasterCustomerId ==  DivisionFilter ).Select(o => o).ToList();
                cusDemographics.Where(p => p.RelationshipType == "Committee").Select(o => o);
                cusDemographics.Where(p => p.RelationshipType == "Committee").Select(o => o);
                if (cusDemographics != null)
                {
                    rptSub.DataSource = cusDemographics;
                    rptSub.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            //Response.Write(ex.ToString() + "Executing Query<b> ");
        }
    }

    public void master_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView drv = (DataRowView)e.Item.DataItem;
            Repeater childRep = e.Item.FindControl("rptSub") as Repeater;
            childRep.DataSource = drv.CreateChildView("relation");
            childRep.DataBind();
        }
    }
 

    public DataSet ExecQuery(string pQuery)
    {
        SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
        try
        {
            conn.Open();
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(pQuery, conn);
            da.Fill(ds);
            return ds;
        }
        catch (Exception ex)
        {

            return null;
        }
        finally
        {
            conn.Close();
        }
    }

    #endregion
}

