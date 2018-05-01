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


using System.Reflection;
using CMS.Helpers;
using CMS.Membership;
 

public partial class CMSWebParts_Custom_Personify_committeelist : CMSAbstractWebPart
{

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
            string id = "";
            if (Request.QueryString["ID"] != null)
            {
                id = Request.QueryString["ID"];

                /////cache it 



                string parentQueryRepeater =
                    " select distinct CommitteeLabelName from Sme_CommiteesMembers where  committeeMasterCustomer ='"
                    + id + "'  ";
                DataSet dsParent = new DataSet();

                string dataServiceUri = ConfigurationManager.AppSettings["svcUri_Base"];
                string dataServiceUserName = ConfigurationManager.AppSettings["svcLogin"];
                string dataServicePassword = ConfigurationManager.AppSettings["svcPassword"];


                /* Uri ServiceUri = new Uri("http://smemi.personifycloud.com/PersonifyDataServices/PersonifyDatasme.svc");
                 Personify.WebControls.Base.PersonifyDataServicesBase.PersonifyEntitiesBase DataAccessLayer =
                     new Personify.WebControls.Base.PersonifyDataServicesBase.PersonifyEntitiesBase(ServiceUri);*/


                Uri ServiceUri = new Uri(dataServiceUri);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.IgnoreMissingProperties = true;
                // DataAccessLayer.Credentials = new NetworkCredential("admin", "admin");
                //  DataAccessLayer.Credentials = new NetworkCredential("admin", "admin");

                DataAccessLayer.Credentials = new NetworkCredential(dataServiceUserName, dataServicePassword);
                var CommitteeList =
                    DataAccessLayer.CustomerInfos.Where(p => p.MasterCustomerId == id).Select(o => o).ToList();
                var CommitteeLabelName = CommitteeList.Select(p => p.LabelName).Distinct().ToList();
                try
                {
                    if (CommitteeLabelName != null)
                    {
                        if (CommitteeLabelName.Count > 0)
                        {
                            committeeName.Text = CommitteeLabelName[0].ToString();
                        }
                    }
                }
                catch (Exception ex2)
                {

                }

                /* cached Section */
                DataTable myTable = new DataTable();
                using (
                    var cs = new CachedSection<DataTable>(
                        ref myTable,
                        60000,
                        true,
                        null,
                        "PersonifyUsersfor|" + Request.QueryString["ID"]))
                {
                    if (cs.LoadData)
                    {
                        var CommuniteeList =
                            DataAccessLayer.CommitteeMembers.Where(p => p.CommitteeMasterCustomer == id)
                                .Select(o => o)
                                .ToList();
                        var current = CommuniteeList.Where(p => p.EndDate >= DateTime.Now).Select(o => o).ToList();
                        current = current.Where(p => p.BeginDate <= DateTime.Now).Select(o => o).ToList();
                        //.GroupBy(x => x.MemberAddressId, (key, group) => group.First());
                        var distinct =
                            current.GroupBy(x => x.CommitteeMemberLastFirstName, (key, group) => group.First()).ToList();
                        //current.Select(o => o.MemberAddressId).Distinct().Select(o => o).ToList();
                        if (distinct != null)
                        {

                            myTable =
                                ToDataTable<personifyDataservice.CommitteeMember>(
                                    current);
                            myTable.Columns.Add("SortOrder", typeof(System.Int32));

                            foreach (DataRow dr in myTable.Rows)
                            {
                                //need to set value to MyRow column
                                dr["SortOrder"] =
                                    Convert.ToInt16(
                                        returnDepartmentNumber(
                                            dr["PositionCodeDescriptionDisplay"].ToString().ToLower()));
                                // or set it to some other value
                            }

                        }
                        cs.Data = myTable;
                    }
                }

                /* cached Section */
                DataView dv = myTable.DefaultView;
                dv.Sort = "SortOrder asc";
                DataTable sortedDT = dv.ToTable();
                rptSub.DataSource = sortedDT;
                rptSub.DataBind();
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.ToString() + "Executing Query<b>  ");
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

    /// <summary>
    /// Convert a List{T} to a DataTable.
    /// </summary>
    private DataTable ToDataTable<T>(List<T> items)
    {
        var tb = new DataTable(typeof(T).Name);

        PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo prop in props)
        {
            Type t = GetCoreType(prop.PropertyType);
            tb.Columns.Add(prop.Name, t);
        }


        foreach (T item in items)
        {
            var values = new object[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                values[i] = props[i].GetValue(item, null);
            }

            tb.Rows.Add(values);
        }

        return tb;
    }

    /// <summary>
    /// Determine of specified type is nullable
    /// </summary>
    public static bool IsNullable(Type t)
    {
        return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// Return underlying type if type is Nullable otherwise return the type
    /// </summary>
    public static Type GetCoreType(Type t)
    {
        if (t != null && IsNullable(t))
        {
            if (!t.IsValueType)
            {
                return t;
            }
            else
            {
                return Nullable.GetUnderlyingType(t);
            }
        }
        else
        {
            return t;
        }
    }

    public int returnDepartmentNumber(string depthName)
    {
        //Chairman Elect  Vice Chairman	Program Chair  Co-Chairman
        switch (depthName.ToLower().Replace("'", ""))
        {
            case "president":
                return 2;
            case "president elect":
                return 3;
            case "past president":
                return 4;
            case "director":
                return 5;
            case "executive director, ex officio":
                return 6;
            case "chairman":
                return 7;
            case "vice chairman":
                return 8;
            case "secretary":
                return 9;
            case "chairman elect":
                return 10;
            case "program chair":
                return 11;
            case "co-chairman":
                return 12;
            case "Member":
                return 13;

            case "sme staff":
                return 14;





            default:
                return 100;
        }
    }


    #endregion
}

