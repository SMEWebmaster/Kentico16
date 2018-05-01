using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using CMS.SettingsProvider;


using CMS.Helpers;
using System.Configuration;
using System.Net;
using System.IO;
using Telerik.Web.UI;


public partial class CMSTemplates_SME_testpersonify : System.Web.UI.UserControl
{
    protected void Page_InIt(object sender, EventArgs e)
    {
       
        var vendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"].ToString();
        var vendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"].ToString();
        var vendorId = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];

        var encryptedVendorToken = RijndaelAlgorithm.GetVendorToken(Request.Url.AbsoluteUri, vendorPassword,
          vendorBlock, "Username", "Password", true);
        PersonifyControlBase objbase = new PersonifyControlBase();
        // var ctrl = new Personify.WebControls.ShoppingCart.UI.CartPreviewControl();
        var ctrl = new Personify.WebControls.Profile.UI.PurchaseHistory();

        PersonifyControlBase obbase = new PersonifyControlBase();

        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);
    }
}