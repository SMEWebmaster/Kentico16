using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.UIControls;
using CMS.CustomTables;
using System.Data;

public partial class CMSTemplates_SME_personify_MembershipJoinRegistration : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctl = new Personify.WebControls.Membership.UI.MembershipJoinRegistrationControl();
        objbase.InitPersonifyWebControl(ctl);
        //LoadDropdownList(ddlCancelUrl);
        //MTG_REG_CANCEL_URL
        //if (!string.IsNullOrEmpty(CancelUrl)) { ctl.CancelUrl = CancelUrl; }
        //if (!string.IsNullOrEmpty(CompanySearchEnable)) { ctl.CompanySearchEnable = System.Boolean.Parse(CompanySearchEnable); }
        //if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
        //LoadDropdownList(ddlErrorMessage);
        //if (!string.IsNullOrEmpty(ErrorMessage)) { ctl.ErrorMessage = ErrorMessage; }
        //LoadDropdownList(ddlImageDirectory);
        //if (!string.IsNullOrEmpty(ImageDirectory)) { ctl.ImageDirectory = ImageDirectory; }
        //if (!string.IsNullOrEmpty(LinkCompanyEnable)) { ctl.LinkCompanyEnable = System.Boolean.Parse(LinkCompanyEnable); }
        //if (!string.IsNullOrEmpty(ProductIdUrlParameter)) { ctl.ProductIdUrlParameter = ProductIdUrlParameter; }
        //if (!string.IsNullOrEmpty(QueryStringParametersToPreserve)) { ctl.QueryStringParametersToPreserve = QueryStringParametersToPreserve; }
        //if (!string.IsNullOrEmpty(RateCodeUrlParameter)) { ctl.RateCodeUrlParameter = RateCodeUrlParameter; }
        //LoadDropdownList(ddlStep2Url);
        //if (!string.IsNullOrEmpty(Step2Url)) { ctl.Step2Url = Step2Url; }
        //LoadDropdownList(ddlUserExistUrl);
        //if (!string.IsNullOrEmpty(UserExistUrl)) { ctl.UserExistUrl = UserExistUrl; }
        phPersonifyControl.Controls.Add(ctl);
    }
}