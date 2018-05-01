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
using CMS.CustomTables;
using System.Data;

public partial class CMSTemplates_SME_personify_Join : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctrl = new Personify.WebControls.Membership.UI.MembershipJoinRegistrationControl();
        string customTableClassName = "Sme.personifyPages";
        string where = "Parametername ='MbrJoinStep2'";
        DataSet customTableRecord = CustomTableItemProvider.GetItems(customTableClassName, where);

      
        //if (!string.IsNullOrEmpty(CancelUrl)) { ctrl.CancelUrl = CancelUrl; }
        //if (!string.IsNullOrEmpty(CompanySearchEnable)) { ctrl.CompanySearchEnable = System.Boolean.Parse(CompanySearchEnable); }
        //if (!string.IsNullOrEmpty(CustomCssClass)) { ctrl.CustomCssClass = CustomCssClass; }
        //LoadDropdownList(ddlErrorMessage);
        //if (!string.IsNullOrEmpty(ErrorMessage)) { ctrl.ErrorMessage = ErrorMessage; }
        //LoadDropdownList(ddlImageDirectory);
        //if (!string.IsNullOrEmpty(ImageDirectory)) { ctrl.ImageDirectory = ImageDirectory; }
        //if (!string.IsNullOrEmpty(LinkCompanyEnable)) { ctrl.LinkCompanyEnable = System.Boolean.Parse(LinkCompanyEnable); }
        //if (!string.IsNullOrEmpty(ProductIdUrlParameter)) { ctrl.ProductIdUrlParameter = ProductIdUrlParameter; }
        //if (!string.IsNullOrEmpty(QueryStringParametersToPreserve)) { ctrl.QueryStringParametersToPreserve = QueryStringParametersToPreserve; }
        //if (!string.IsNullOrEmpty(RateCodeUrlParameter)) { ctrl.RateCodeUrlParameter = RateCodeUrlParameter; }
        //LoadDropdownList(ddlStep2Url);
        if (customTableRecord != null) { ctrl.Step2Url = customTableRecord.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        //LoadDropdownList(ddlUserExistUrl);
        //if (!string.IsNullOrEmpty(UserExistUrl)) { ctrl.UserExistUrl = UserExistUrl; }
        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);
    }
}