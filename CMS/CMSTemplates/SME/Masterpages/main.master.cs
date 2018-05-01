using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Net;
using System.IO;
using System.Text;
using System.Data;

using CMS.UIControls;
using CMS.Forums;
using CMS.Helpers;
using CMS.Base;
using CMS.Controls;
using CMS.PortalEngine;
using CMS.OnlineForms;
using CMS.DataEngine;
using CMS.Localization;
using Personify.WebControls.ShoppingCart.Business;
using Personify.WebControls.ShoppingCart.Providers;
using CMS.Membership;
using SSO;
using System.Configuration;
using personifyDataservice;

public partial class CMSTemplates_SME_Masterpages_main : TemplateMasterPage
{//  private readonly Ektron.Cms.API.User.User _ektronUserApi = new Ektron.Cms.API.User.User();
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];

    private readonly SSO.service _wsSso = new SSO.service();
    private readonly List<WebControlParameter> _webControlParameters;
    private const string ShoppingCartGuidCookieName = "PersonifyShoppingCartGUID";
    public const int CookieDurationDays = 90;
    protected override void CreateChildControls()
    {
        base.CreateChildControls();
        PageManager = CMSPortalManager1;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        this.ltlHeaderTags.Text = this.HeaderTags;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       // manScript.RegisterAsyncPostBackControl(btnSubmitNewsletter);

        this.littile.Text = CMS.DocumentEngine.DocumentContext.CurrentDocument.DocumentName;//this.Title.ToString();
       // ShoppingCart();
    }

    /// <summary>
    /// Update Shopping cart Icon 
    /// </summary>
    public void ShoppingCart()
    {
        var a = UpdateShoppingCartInfo();
        itemcount.Text = a.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

  
    //Get ShoppingCart Item 
    protected long UpdateShoppingCartInfo()
    {
        var personifyIdentity = GetPersonifyUser();

        var webShoppingCartItemsSegment = new WebShoppingCartItemsSegment();
        var selectedSegment = webShoppingCartItemsSegment.SegmentId;

        var shoppingCartSegments = new ShoppingCartProvider().GetShoppingCartSegments(personifyIdentity);

        var webShoppingCartItemsSegment2 =
            shoppingCartSegments.FirstOrDefault(x => x.SegmentId == selectedSegment) ??
            ((shoppingCartSegments != null && shoppingCartSegments.Count > 0) ? shoppingCartSegments[0] : null);
        var shoppingCartItems = new ShoppingCartProvider().GetShoppingCartItems(personifyIdentity,
            webShoppingCartItemsSegment2);

        return shoppingCartItems.Count;
    }

    protected Personify.WebControls.Base.Business.PersonifyIdentity GetPersonifyUser()
    {
        //if (this.Page is PageBuilderViewer)
        //{
        //    Personify.WebControls.Base.Business.PersonifyIdentity pageUser = (this.Page as PageBuilderViewer).PersonifyUser;
        //    if (pageUser != null)
        //    {
        //        return pageUser;
        //    }
        //}

        var user = new Personify.WebControls.Base.Business.PersonifyIdentity
        {
            ContainerName = "Kentico",
            CurrencyCode = "USD",
            SubCustomerId = 0
        };



        // var userApi = new Ektron.Cms.UserAPI();

        // if (userApi.IsLoggedIn && !userApi.IsAdmin())
        //  if (!string.IsNullOrEmpty(MembershipContext.AuthenticatedUser.FirstName))
        if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
        {
            if (HttpContext.Current.Session["PersonifyToken"] != null)
            {
                HttpContext.Current.Session["PersonifyToken"] = ValidateCustomerToken(HttpContext.Current.Session["PersonifyToken"].ToString());

                if (!string.IsNullOrEmpty(HttpContext.Current.Session["PersonifyToken"] as string))
                {
                    //Ektron.Cms.UserData ud = _ektronUserApi.GetUser(userApi.UserId);
                    UserInfo ud = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserID);

                    // string cRemoteid = ConfigurationManager.AppSettings["EktronCustomProperty_MasterCustomerId"];
                    if (HttpContext.Current.Session["userClass"] != null)
                    {
                        userinfo ui = (userinfo)HttpContext.Current.Session["userClass"];
                        if (ui != null)
                        {
                            user.CustomerName = MembershipContext.AuthenticatedUser.FirstName + " " + MembershipContext.AuthenticatedUser.LastName;
                            user.MasterCustomerId = ui.ID;
                            //"RemoteID";//ud.CustomProperties[cRemoteid].Value.ToString(); ;

                            user.IsLoggedIn = true;
                            user.ShoppingGUID = ui.ID;
                            user.MasterCustomerId = ui.ID;
                        }

                    }
                    //  if (ud.CustomProperties[cRemoteid] != null)
                    {

                        //user.IsMember = false;
                    }
                }
            }
        }
        ///pass dummy credentials if not logged in 
        if (string.IsNullOrEmpty(user.MasterCustomerId))
        {
            //return dummy user
            user.CustomerName = "Steven Karl";
            user.IsMember = false;
            user.IsLoggedIn = false;
            user.MasterCustomerId = "01786850";

        }
        if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
        {

            bool flagadmin = false;
            UserInfo userdata = CMS.Membership.UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

            DataTable dt = UserInfoProvider.GetUserRoles(userdata);

            if (dt.Rows.Count > 0 && dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["RoleName"].ToString().ToLower().Contains("administrator"))
                    {
                        flagadmin = true;


                    }

                }
            }

            if (flagadmin)
            { user.IsAdministrator = true; }
            else
            {

                var ck = HttpContext.Current.Request.Cookies[PersonifyControlBase.PersonifyShoppingCartGuidCookieName];
                if (ck != null && ck.Value != null)
                {
                    user.ShoppingGUID = ck.Value;
                }
                else
                {
                    var cookieValue = HttpContext.Current.Session.SessionID;

                    DateTime dtExpire = DateTime.Now.AddDays(CookieDurationDays);

                    var myCookie = new HttpCookie(PersonifyControlBase.PersonifyShoppingCartGuidCookieName)
                    {
                        Value = cookieValue,
                        Expires = dtExpire
                    };

                    HttpContext.Current.Response.Cookies.Add(myCookie);

                    user.ShoppingGUID = cookieValue;
                }

            }

            PageViewer p = new PageViewer();
            p.PersonifyUser = user;


        }



        return user;
    }

    private string ValidateCustomerToken(string customerToken)
    {
        SSOCustomerTokenIsValidResult res = _wsSso.SSOCustomerTokenIsValid(_personifySsoVendorName, _personifySsoVendorPassword, customerToken);

        if (res.Valid && !string.IsNullOrEmpty(res.NewCustomerToken))
        {
            return res.NewCustomerToken;
        }
        else
        {
            return null;
        }
    }
}
