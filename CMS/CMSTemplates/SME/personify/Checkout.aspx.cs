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

public partial class CMSTemplates_SME_personify_Checkout : TemplatePage
{
    protected void Page_InIt(object sender, EventArgs e)
    {
        PersonifyControlBase objbase = new PersonifyControlBase();
        var ctrl = new Personify.WebControls.ShoppingCart.UI.CheckoutControl();

        PersonifyControlBase obbase = new PersonifyControlBase();


        ctrl.BackToCartUrl = "/personify/store/ViewCartPageUrl";
        ctrl.CancelButtonRedirectPageUrl = "";
        ctrl.CheckoutCompletedRedirectPageUrl = "";
        ctrl.CouponWasNotAppliedMessage = "Coupon was not applied";
        ctrl.ErrorMessage = "Order is empty. Please add items to it before checkout.";
        ctrl.ProductIdUrlParameter = "productid";
       
        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);
    }
}