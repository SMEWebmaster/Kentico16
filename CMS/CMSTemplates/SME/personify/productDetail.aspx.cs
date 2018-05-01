using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.UIControls;
public partial class CMSTemplates_SME_personify_productDetail : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctl = new Personify.WebControls.Store.UI.ProductDetailControl();
        objbase.InitPersonifyWebControl(ctl);
       // objbase.LoadDropdownList(REVIEW);
       // if (!string.IsNullOrEmpty(AllReviewsUrlIfOtherPage))
        { ctl.AllReviewsUrlIfOtherPage = "/personify/store/review"; }
       // objbase.LoadDropdownList(ddlBuyForGroupPageUrl);
      //  if (!string.IsNullOrEmpty(BuyForGroupPageUrl)) 
        { ctl.BuyForGroupPageUrl = "/personify/store/group"; }
       // objbase.LoadDropdownList(ddlCheckOutUrl);
       // if (!string.IsNullOrEmpty(CheckOutUrl)) 
        { ctl.CheckOutUrl = "/personify/store/checkout"; }
       // objbase.LoadDropdownList(ddlCreateReviewUrl);
     //   if (!string.IsNullOrEmpty(CreateReviewUrl))
        { ctl.CreateReviewUrl = "/personify/store/CreateReview"; ; }
      //  if (!string.IsNullOrEmpty(CustomCssClass)) { ctl.CustomCssClass = CustomCssClass; }
       // if (!string.IsNullOrEmpty(DonationOptions))
        { ctl.DonationOptions = "25,50,100,250,500"; }
       // objbase.LoadDropdownList(ddlErrorMessage);
       // if (!string.IsNullOrEmpty(ErrorMessage)) 
        { ctl.ErrorMessage = "Issue loading control"; }
       // objbase.LoadDropdownList(ddlGuestCheckoutLoginReturnUrlParameter);
       // if (!string.IsNullOrEmpty(GuestCheckoutLoginReturnUrlParameter))
        { ctl.GuestCheckoutLoginReturnUrlParameter = "guest"; }
       // objbase.LoadDropdownList(ddlGuestCheckoutLoginUrl);
     //   if (!string.IsNullOrEmpty(GuestCheckoutLoginUrl)) 
        { ctl.GuestCheckoutLoginUrl = "/personify/store/guestcheckout"; }
       // objbase.LoadDropdownList(ddlJoinPageUrl);
       // if (!string.IsNullOrEmpty(JoinPageUrl)) 
        { ctl.JoinPageUrl = "/join-renew"; }
       // objbase.LoadDropdownList(ddlLogInPageUrl);
       // if (!string.IsNullOrEmpty(LogInPageUrl)) 
        { ctl.LogInPageUrl = "login-join"; }
        //if (!string.IsNullOrEmpty(PayNowEnable)) 
        { ctl.PayNowEnable = false ; }
       // objbase.LoadDropdownList(ddlPayNowPageURL);
      //  if (!string.IsNullOrEmpty(PayNowPageURL))
        { ctl.PayNowPageURL = "/personify/store/paynow"; }
       // objbase.LoadDropdownList(ddlProductDetailUrl);
        //if (!string.IsNullOrEmpty(ProductDetailUrl))
        { ctl.ProductDetailUrl = "/personify/store/ProductDetail"; }
      //  if (!string.IsNullOrEmpty(ProductIdUrlParameter)) 
        { ctl.ProductIdUrlParameter = "ProductId"; }
       // objbase.LoadDropdownList(ddlReturnFromLoginPageParameter);

        /*
        if (!string.IsNullOrEmpty(ReturnFromLoginPageParameter)) 
        { ctl.ReturnFromLoginPageParameter = ReturnFromLoginPageParameter; }
        if (!string.IsNullOrEmpty(ShoppingCartLogInReasonMessage)) 
        { ctl.ShoppingCartLogInReasonMessage = ShoppingCartLogInReasonMessage; }
        if (!string.IsNullOrEmpty(ShoppingCartSavingsMemberHeaderMessage)) 
        { ctl.ShoppingCartSavingsMemberHeaderMessage = ShoppingCartSavingsMemberHeaderMessage; }
        if (!string.IsNullOrEmpty(ShoppingCartSavingsMemberMessage))
        { ctl.ShoppingCartSavingsMemberMessage = ShoppingCartSavingsMemberMessage; }
        if (!string.IsNullOrEmpty(ShoppingCartSavingsNotMemberHeaderMessage))
        { ctl.ShoppingCartSavingsNotMemberHeaderMessage = ShoppingCartSavingsNotMemberHeaderMessage; }
        if (!string.IsNullOrEmpty(ShoppingCartSavingsNotMemberMessage))
        { ctl.ShoppingCartSavingsNotMemberMessage = ShoppingCartSavingsNotMemberMessage; }
        //if (!string.IsNullOrEmpty(ShowDefaultPageTitle))
        //{ ctl.ShowDefaultPageTitle = System.Boolean.Parse(ShowDefaultPageTitle); }
         */
        //if (!string.IsNullOrEmpty(ShowFacebook)) 
        //{ ctl.ShowFacebook = System.Boolean.Parse(ShowFacebook); }
        //if (!string.IsNullOrEmpty(ShowLinkedIn)) 
        //{ ctl.ShowLinkedIn = System.Boolean.Parse(ShowLinkedIn); }
        //if (!string.IsNullOrEmpty(ShowMailToLink)) { ctl.ShowMailToLink = System.Boolean.Parse(ShowMailToLink); }
        //if (!string.IsNullOrEmpty(ShowPinterest)) { ctl.ShowPinterest = System.Boolean.Parse(ShowPinterest); }
        //if (!string.IsNullOrEmpty(ShowTwitter)) { ctl.ShowTwitter = System.Boolean.Parse(ShowTwitter); }
       // objbase.LoadDropdownList(ddlViewCartPageUrl);
      //  if (!string.IsNullOrEmpty(ViewCartPageUrl)) 
        { ctl.ViewCartPageUrl = "/personify/store/ViewCartPageUrl"; }
        phPersonifyControl.Controls.Add(ctl);
    }

     

}