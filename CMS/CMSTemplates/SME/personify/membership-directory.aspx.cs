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
using CMS.UIControls;
using System.Data;
using CMS.CustomTables;
public partial class CMSTemplates_SME_personify_membership_direcotry : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {

        //ErrorMessage  CSSClass ProductIdUrlParameter 
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctrl = new Personify.WebControls.Membership.UI.MembershipListingControl();
        string customTableClassName = "Sme.personifyPages";
        string where = "Parametername ='ProductDetails'";
        DataSet customTableRecord = CustomTableItemProvider.GetItems(customTableClassName, where);
        if (customTableRecord != null) { ctrl.Step1RegistrationUrl = customTableRecord.Tables[0].Rows[0]["ParameterValue"].ToString(); }
 ctrl.ProductIdUrlParameter = "productid"; 
 
 
         
        //LoadDropdownList(ddlErrorMessage);
        //if (!string.IsNullOrEmpty(ErrorMessage)) { ctrl.ErrorMessage = ErrorMessage; }
        //if (!string.IsNullOrEmpty(ProductIdUrlParameter)) { ctrl.ProductIdUrlParameter = "productId"; }
        //if (!string.IsNullOrEmpty(RateCodeUrlParameter)) { ctrl.RateCodeUrlParameter = RateCodeUrlParameter; }
        //LoadDropdownList(ddlStep1RegistrationUrl);
        //if (!string.IsNullOrEmpty(Step1RegistrationUrl)) { ctrl.Step1RegistrationUrl = Step1RegistrationUrl; }

        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);

    }
}