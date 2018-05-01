using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Castle.MicroKernel.ModelBuilder.Descriptors;

using CMS.PortalControls;
using CMS.Helpers;
using System.Data.SqlClient;
using System.Web.Configuration;
public partial class CMSWebParts_Custom_Personify_loadCommunitesList : CMSAbstractWebPart
{
    #region "Properties"

   

    private string _CommitteeFilter = "";
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
            litTitle.Text = Filter;
            whereCondition = "  where CustomerClassCode   in ('" + Filter + "')   ";
            }

             
            string childQueryRepeater = "  select * from dbo.Sme_CommiteesMaster  "+ whereCondition+ " order by CustomerClassCode  ";
            DataSet dsParent = new DataSet();
           
            DataTable dtChildMulti = new DataTable();
            dsParent = ExecQuery(childQueryRepeater);
            dtChildMulti = dsParent.Tables[0];

            rptSub.DataSource = dtChildMulti;
            rptSub.DataBind();

        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString() + "Executing Query<b> ");
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

