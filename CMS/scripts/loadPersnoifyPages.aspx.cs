using CMS.CustomTables;
using CMS.DataEngine;
using CMS.Helpers;
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

public partial class scripts_loadPersnoifyPages : System.Web.UI.Page
{
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private static readonly string SUri = ConfigurationManager.AppSettings["svcUri_Base"];
    private static readonly string UserName = ConfigurationManager.AppSettings["svcLogin"];
    private static readonly string Password = ConfigurationManager.AppSettings["svcPassword"];
    private readonly SSO.service _wsSso = new SSO.service();
    List<WebControlParameter> _webControlParameters;
    private const string ShoppingCartGuidCookieName = "PersonifyShoppingCartGUID";

    protected void Page_Load(object sender, EventArgs e)
    {

        Uri ServiceUri = new Uri(SUri);
        PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
        DataAccessLayer.Credentials = new NetworkCredential(UserName, Password);
        _webControlParameters = DataAccessLayer.WebControlParameters.ToList();

         
        if (_webControlParameters.Count > 0)
        {
            foreach (var x  in _webControlParameters)
            {
                //  

                string customTableClassName = "Sme.personifyPages";
                string where = "ParameterName='" + x.ParameterName + "'";

                // Check if Custom table 'Sme.CommiteesMembers' exists
                DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);


                CustomTableItem item = CustomTableItem.New("Sme.personifyPages");
                item.SetValue("ParameterValue", x.ParameterValue);
                item.SetValue("ParameterName", x.ParameterName);
                item.Insert();
                //DataSet customTableRecord = CustomTableItemProvider.GetItems(customTableClassName, where);

                //int ItemID = 0;
                // if (!DataHelper.DataSourceIsEmpty(customTableRecord))
                //    {
                //        // Get the custom table item ID
                //        ItemID = ValidationHelper.GetInteger(customTableRecord.Tables[0].Rows[0][0], 0);
                //    }

                //if (customTable != null)
                //{
                //    if (ItemID == 0)
                //    {
                //        CustomTableItem item = CustomTableItem.New("Sme.CommiteesMembers");
                //        item.SetValue("ParameterValue", x.ParameterValue);
                //        item.SetValue(" ParameterName", x.ParameterName);
                //        item.Insert();
                //    }

                //    else
                //    {

                //        if (!DataHelper.DataSourceIsEmpty(customTableRecord))
                //        {
                //            CustomTableItem updateItem = CustomTableItemProvider.GetItem(ItemID, customTableClassName);
                //            if (updateItem != null)
                //            {
                //                updateItem.SetValue("ParameterValue", x.ParameterValue);
                //                updateItem.SetValue(" ParameterName", x.ParameterName);

                //                updateItem.Update();
                //            }
                //        }
                //    }
                //}

                ///
                Response.Write(x.ParameterValue + "---" + x.ParameterName + "<br/>");
            }
        }
    }


}