using CMS.CustomTables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.UIControls;

public partial class CMSTemplates_SME_personify_CreateMembership : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctl = new Personify.WebControls.Common.UI.RegistrationControl();
        objbase.InitPersonifyWebControl(ctl);
        string customTableClassName = "Sme.personifyPages";
        string where = "Parametername ='MTG_REG_CANCEL_URL'";
        DataSet MTG_REG_CANCEL_URL = CustomTableItemProvider.GetItems(customTableClassName, where);

        string where1 = "Parametername ='RosterPage'";
        DataSet RosterPage = CustomTableItemProvider.GetItems(customTableClassName, where1);


        string where2 = "Parametername ='CHECKOUTURL'";
        DataSet CHECKOUTURL = CustomTableItemProvider.GetItems(customTableClassName, where2);


        string where3 = "Parametername ='UserAlreadyExists'";
        DataSet UserAlreadyExists = CustomTableItemProvider.GetItems(customTableClassName, where3);


        if (MTG_REG_CANCEL_URL != null) { ctl.CancelUrl = MTG_REG_CANCEL_URL.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        if (RosterPage != null) { ctl.RosterPageURL = RosterPage.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        if (CHECKOUTURL != null) { ctl.CancelUrl = MTG_REG_CANCEL_URL.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        if (UserAlreadyExists != null) { ctl.UserExistUrl = UserAlreadyExists.Tables[0].Rows[0]["ParameterValue"].ToString(); }


        //if (!string.IsNullOrEmpty(CommitteePositionDetailsPageURL)) { ctl.CommitteePositionDetailsPageURL = CommitteePositionDetailsPageURL; }
        ctl.CommMbrIdParam = "CommMbrId";
        ctl.CommMbrMcidParam = "CommMbrMcid";
        ctl.CommMbrNameParam = "CommMbrName";
        ctl.CommMbrScidParam = "CommMbrScid";
        ctl.CommMcidParam = "CommMcid";
        ctl.CommScidParam = "CommScid";
        ctl.CommTermDetailsModeParam = "Mode";
        ctl.CommTermDetailsReturnParam = "return";
        ctl.CompanySearchEnable = true;
        // if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
        //LoadDropdownList(ddlErrorMessage);
        ctl.ErrorMessage = "ErrorMessage";
        ctl.LinkCompanyEnable = true;
        //  if (!string.IsNullOrEmpty(QueryStringParametersToPreserve)) { ctl.QueryStringParametersToPreserve = QueryStringParametersToPreserve; }

        ////RosterPage
        //if (!string.IsNullOrEmpty(RosterPageURL)) { ctl.RosterPageURL = RosterPageURL; }
        //LoadDropdownList(ddlStep2Url);
        //if (!string.IsNullOrEmpty(Step2Url))
        //{
        //    ctl.Step2Url = referbackURL; //Step2Url;
        //}
        //Response.Write(referbackURL);

        phPersonifyControl.Controls.Add(ctl);


        ////////////////////
        /// login request control 
        /// 
        var ctrl = new Personify.WebControls.Common.UI.LoginRequestControl();
        objbase.InitPersonifyWebControl(ctrl);
         { ctrl.QueryStringForReturnURL = "returnurl"; }
         phPersonifyControl2.Controls.Add(ctl);
    }
}