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

public partial class CMSTemplates_SME_personify_Renew : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctrl = new Personify.WebControls.Profile.UI.MembershipRenew();

        if (!string.IsNullOrEmpty(ContinueMessage)) { ctrl.ContinueMessage = ContinueMessage; }
        if (!string.IsNullOrEmpty(CustomCssClass)) { ctrl.CustomCssClass = CustomCssClass; }
        LoadDropdownList(ddlErrorMessage);
        if (!string.IsNullOrEmpty(ErrorMessage)) { ctrl.ErrorMessage = ErrorMessage; }
        LoadDropdownList(ddlJoinUrl);
        if (!string.IsNullOrEmpty(JoinUrl)) { ctrl.JoinUrl = JoinUrl; }
        LoadDropdownList(ddlRenewUrl);
        if (!string.IsNullOrEmpty(RenewUrl)) { ctrl.RenewUrl = RenewUrl; }
        if (!string.IsNullOrEmpty(RenewUrlParameter)) { ctrl.RenewUrlParameter = RenewUrlParameter; }
        if (!string.IsNullOrEmpty(Title)) { ctrl.Title = Title; }
        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);
    }
}