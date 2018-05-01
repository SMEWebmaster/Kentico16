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
    public string CommitteeFilter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CommitteeFilter"), String.Empty);
        }
        set
        {
            SetValue("CommitteeFilter", value);
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
           

            switch (CommitteeFilter)
            {
                case "1":
                    whereCondition = "";
                    break;
                case "2":
                     whereCondition = "  where CustomerClassCode not in ('CHAPTER','DIVISION','SECTION','WAAIME','WAAIMESECTION','Committee')   ";
                    break;
                     case "3":
                     whereCondition = "    where CustomerClassCode like '%WAAIME%'  ";
                    break;
                  default:
                     whereCondition = "  where CustomerClassCode='" + CommitteeFilter + "' ";
                    break;

            }

             string parentQueryRepeater = " select distinct  CustomerClassCode as Code     from dbo.Sme_CommiteesMaster " + whereCondition+ "  order by CustomerClassCode";
            string childQueryRepeater = "  select * from dbo.Sme_CommiteesMaster  "+ whereCondition+ " order by CustomerClassCode  ";
            DataSet dsParent = new DataSet();
            DataTable dtParentMulti = new DataTable();
            dsParent = ExecQuery(parentQueryRepeater);
            dtParentMulti = dsParent.Tables[0];


            dsParent = new DataSet();
            DataTable dtChildMulti = new DataTable();
            dsParent = ExecQuery(childQueryRepeater);
            dtChildMulti = dsParent.Tables[0];


            DataTable parentDt = dtParentMulti;
            parentDt.TableName = "ParentTable";
            DataTable ChildDt = dtChildMulti;
            ChildDt.TableName = "ChildTable";
            DataSet dsMulit = new DataSet();
            dsMulit.Tables.Add(parentDt.Copy());
            dsMulit.Tables.Add(ChildDt.Copy());

            dsMulit.Relations.Add(new DataRelation("relation", dsMulit.Tables[0].Columns["Code"], dsMulit.Tables[1].Columns["CustomerClassCode"]));

            rptMainCommitee.DataSource = dsMulit;
            rptMainCommitee.DataBind();

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
    public void master_ItemDataBound2(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item | e.Item.ItemType == ListItemType.AlternatingItem)
        {
            DataRowView drv = (DataRowView)e.Item.DataItem;
            Repeater childRep = e.Item.FindControl("rptSub2") as Repeater;
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

