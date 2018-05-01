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


public partial class CMSWebParts_Custom_Personify_loadmembershipDetailst : CMSAbstractWebPart
{

    #region "Properties"
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];

    /// <summary>
    /// Load pages individually.
    /// </summary>
    public string Filter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("filter"), String.Empty);
        }
        set
        {
            SetValue("filter", value);
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
            if (Filter != null)
            {
 
                Uri ServiceUri = new Uri(svcUri_Base);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.IgnoreMissingProperties = true;
                DataAccessLayer.Credentials = new NetworkCredential(svcLogin, svcPassword);

                ///bind data membership details

                var membershipDetails = DataAccessLayer.WebMembershipJoinProducts.Where(p => p.ProductId == Convert.ToInt64(Filter)).Select(o => o).ToList();

                if (membershipDetails != null)
                {
                    rptmain.DataSource = membershipDetails;
                    rptmain.DataBind();
                }

                ////bind price 
               var cusDemographics = DataAccessLayer.ProductPricingInfos.Where(p => p.ProductId == Convert.ToInt64(Filter)).Select(o => o).ToList().OrderBy(m => m.SortOrder);

                if (cusDemographics != null)
                {
                    rptSub.DataSource = cusDemographics;
                    rptSub.DataBind();
                }
            }

        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString() + "Executing Query<b> ");
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

