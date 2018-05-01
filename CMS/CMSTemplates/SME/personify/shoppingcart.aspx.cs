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
public partial class CMSTemplates_SME_personify_shoppingcart : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctl = new Personify.WebControls.ShoppingCart.UI.ShoppingCartControl();
        objbase.InitPersonifyWebControl(ctl);
        string customTableClassName = "Sme.personifyPages";
        ctl.CartItemPreviewMode = false;
        string where2 = "Parametername ='CHECKOUTURL'";
        DataSet CHECKOUTURL = CustomTableItemProvider.GetItems(customTableClassName, where2);

        if (CHECKOUTURL != null) { ctl.CheckOutUrl = CHECKOUTURL.Tables[0].Rows[0]["ParameterValue"].ToString(); }


        // LoadDropdownList(ddlContinueShoppingPageUrl);
        string where3 = "Parametername ='Continue'";
        DataSet ddlContinueShoppingPageUrl = CustomTableItemProvider.GetItems(customTableClassName, where3);

        if (ddlContinueShoppingPageUrl != null) { ctl.ContinueShoppingPageUrl = ddlContinueShoppingPageUrl.Tables[0].Rows[0]["ParameterValue"].ToString(); }

        //   if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
        //  LoadDropdownList(ddlErrorMessage);
        // if (!string.IsNullOrEmpty(ErrorMessage)) { ctl.ErrorMessage = ErrorMessage; }
        // LoadDropdownList(ddlGuestLogInPageUrl);
        string where4 = "Parametername ='Guest Login Page'";
        DataSet ddlGuestLogInPageUrl = CustomTableItemProvider.GetItems(customTableClassName, where4);

        if (ddlGuestLogInPageUrl != null) { ctl.GuestLogInPageUrl = ddlGuestLogInPageUrl.Tables[0].Rows[0]["ParameterValue"].ToString(); }

        ctl.ProductIdUrlParameter = "productId";
        ctl.MeetingRegistrationProductIdParameter = "productId";
        /*  LoadDropdownList(ddlMeetingRegistrationAffiliatePageUrl);
          if (!string.IsNullOrEmpty(MeetingRegistrationAffiliatePageUrl)) { ctl.MeetingRegistrationAffiliatePageUrl = MeetingRegistrationAffiliatePageUrl; }
          if (!string.IsNullOrEmpty(MeetingRegistrationAffiliateProductIdParameter)) { ctl.MeetingRegistrationAffiliateProductIdParameter = MeetingRegistrationAffiliateProductIdParameter; }
          LoadDropdownList(ddlMeetingRegistrationPageUrl);
          if (!string.IsNullOrEmpty(MeetingRegistrationPageUrl)) { ctl.MeetingRegistrationPageUrl = MeetingRegistrationPageUrl; }
          if (!string.IsNullOrEmpty(MeetingRegistrationProductIdParameter)) { ctl.MeetingRegistrationProductIdParameter = MeetingRegistrationProductIdParameter; }
          if (!string.IsNullOrEmpty(ProductIdUrlParameter)) { ctl.ProductIdUrlParameter = "productId"; }
          LoadDropdownList(ddlProductMeetingUrl);
          if (!string.IsNullOrEmpty(ProductMeetingUrl)) { ctl.ProductMeetingUrl = ProductMeetingUrl; }*/
        //LoadDropdownList(ddlProductMemberJoinUrl);
        //if (!string.IsNullOrEmpty(ProductMemberJoinUrl)) { ctl.ProductMemberJoinUrl = ProductMemberJoinUrl; }
        //LoadDropdownList(ddlProductPageUrl);
        //if (!string.IsNullOrEmpty(ProductPageUrl)) { ctl.ProductPageUrl = ProductPageUrl; }
        //LoadDropdownList(ddlReturnFromGuestLoginPageParameter);
        //if (!string.IsNullOrEmpty(ReturnFromGuestLoginPageParameter)) { ctl.ReturnFromGuestLoginPageParameter = ReturnFromGuestLoginPageParameter; }
        //if (!string.IsNullOrEmpty(ShoppingCartEmptyMessage)) { ctl.ShoppingCartEmptyMessage = ShoppingCartEmptyMessage; }
        //LoadDropdownList(ddlViewCartUrl);
        // { ctl. = ViewCartUrl; }
        phPersonifyControl.Controls.Add(ctl);
         
        var ctl2 = new Personify.WebControls.ShoppingCart.UI.ShoppingCartSummaryControl();
        objbase.InitPersonifyWebControl(ctl2);

        string customTableClassName2 = "Sme.personifyPages";
        ctl.CartItemPreviewMode = false;
        string w2 = "Parametername ='Login'";
        DataSet Logins = CustomTableItemProvider.GetItems(customTableClassName2, w2);

        if (CHECKOUTURL != null) { ctl2.ButtonProcessRedirectPageUrl = CHECKOUTURL.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        if (Logins != null) { ctl2.LogInPageUrl = Logins.Tables[0].Rows[0]["ParameterValue"].ToString(); }
        ctl2.CouponWasNotAppliedMessage = "Coupon was not applied";
        ctl2.ButtonProcessCaption = "Checkout";
        ctl2.ShoppingCartSummaryHeaderMessage = "Summary";
        ctl2.ShippingNotesMessage = "Items may be shipped from different warehouses.";
        phPersonifyControl2.Controls.Add(ctl2);

    }
}