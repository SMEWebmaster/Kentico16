using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.UIControls;
public partial class CMSTemplates_SME_personify_myaccount : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {


        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctl = new Personify.WebControls.Profile.UI.MyContactInformation();
        objbase.InitPersonifyWebControl(ctl);

        ctl.DefaultPhoto = "";
        ctl.ErrorMessage = "Error Loading the apge ";

        if (!string.IsNullOrEmpty(Title)) { ctl.Title = Title; }
        phPersonifyControl.Controls.Add(ctl);



        var ctl2 = new Personify.WebControls.Profile.UI.MyMeetingsControl();
        objbase.InitPersonifyWebControl(ctl2);
        ///add these properties 
        //if (!string.IsNullOrEmpty(AddnlQueryParam)) { ctl.AddnlQueryParam = AddnlQueryParam; }
        //if (!string.IsNullOrEmpty(ControlTitle)) { ctl.ControlTitle = ControlTitle; }
        //if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
        //LoadDropdownList(ddlErrorMessage);
        //if (!string.IsNullOrEmpty(ErrorMessage)) { ctl.ErrorMessage = ErrorMessage; }
        //LoadDropdownList(ddlMtgRegUrl);
        //if (!string.IsNullOrEmpty(MtgRegUrl)) { ctl.MtgRegUrl = MtgRegUrl; }
        //if (!string.IsNullOrEmpty(OrderNoParam)) { ctl.OrderNoParam = OrderNoParam; }
        //LoadDropdownList(ddlOrderSummaryUrl);
        //if (!string.IsNullOrEmpty(OrderSummaryUrl)) { ctl.OrderSummaryUrl = OrderSummaryUrl; }
        //LoadDropdownList(ddlProductDetailsUrl);
        //if (!string.IsNullOrEmpty(ProductDetailsUrl)) { ctl.ProductDetailsUrl = ProductDetailsUrl; }
        //if (!string.IsNullOrEmpty(ProductIdParam)) { ctl.ProductIdParam = ProductIdParam; }
        phPersonifyControl2.Controls.Add(ctl2);




        var ctl3 = new Personify.WebControls.Profile.UI.CommitteePositions();
        objbase.InitPersonifyWebControl(ctl3);
        //if (!string.IsNullOrEmpty(CommMbrIdParam)) { ctl.CommMbrIdParam = CommMbrIdParam; }
        //if (!string.IsNullOrEmpty(CommTermDetailsModeParam)) { ctl.CommTermDetailsModeParam = CommTermDetailsModeParam; }
        //if (!string.IsNullOrEmpty(CommTermDetailsReturnParam)) { ctl.CommTermDetailsReturnParam = CommTermDetailsReturnParam; }
        //LoadDropdownList(ddlCommTermDetailsUrl);
        //if (!string.IsNullOrEmpty(CommTermDetailsUrl)) { ctl.CommTermDetailsUrl = CommTermDetailsUrl; }
        //if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
        //LoadDropdownList(ddlErrorMessage);
        //if (!string.IsNullOrEmpty(ErrorMessage)) { ctl.ErrorMessage = ErrorMessage; }
        //if (!string.IsNullOrEmpty(TimeFrame)) { ctl.TimeFrame = TimeFrame; }
        //if (!string.IsNullOrEmpty(Title)) { ctl.Title = Title; }
        phPersonifyControl3.Controls.Add(ctl3);

    }
}