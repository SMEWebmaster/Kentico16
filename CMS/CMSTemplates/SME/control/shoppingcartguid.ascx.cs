using System;
using System.Linq;
using System.Web;
using System.Configuration;
using Personify.WebControls.Base.PersonifyDataServicesBase;
using Personify.WebControls.Base.Utilities;
using CMS.Membership;
using System.Data;

public partial class CMSTemplates_SME_control_shoppingcartguid : System.Web.UI.UserControl
{
    private const int CookieDurationDays = 90;
    LoginUsertokentico objKenticoService = new LoginUsertokentico();
    private const string PersonifySessionKey = "PersonifyToken";

    private readonly string _personifyImsUrl = ConfigurationManager.AppSettings["IMSWebReferenceURL"];
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];

    protected void Page_Init(object sender, EventArgs e)
    {
        if (IsPostBack || Request.QueryString["ektronPageBuilderEdit"] != null || Request.QueryString["cmsMode"] == "Preview") return;
        CheckShoppingCartCookie();
    }

    private void CheckShoppingCartCookie()
    {
        bool flagadmin = false;
        bool userApi = objKenticoService.CheckLoginUser("");
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
        {
            return;
        }

        HttpCookie ck = Request.Cookies[PersonifyControlBase.PersonifyShoppingCartGuidCookieName];

        if (userApi)
        {
            if (ck != null)
            {
                if (!string.IsNullOrEmpty(ck.Value))
                {

                    if (HttpContext.Current.Session["userClass"] != null)
                    {
                        userinfo ui = (userinfo)HttpContext.Current.Session["userClass"];


                        //if (ud.CustomProperties[remoteId] != null)

                        string masterCustomerId = ui.ID.ToString();
                        if (!string.IsNullOrEmpty(masterCustomerId))
                        {
                            TransferShoppingCartItems(ck.Value, masterCustomerId);
                            ClearShoppingCartCookie();
                        }
                    }
                }
            }



        }
        else
        {
            if (ck == null || string.IsNullOrEmpty(ck.Value))
            {
                CreateShoppingCartCookie();
            }
        }
    }

    private void ClearShoppingCartCookie()
    {
        var ck = HttpContext.Current.Request.Cookies[PersonifyControlBase.PersonifyShoppingCartGuidCookieName];
        if (ck == null || ck.Value == null) return;
        var ssoCookie = Response.Cookies[PersonifyControlBase.PersonifyShoppingCartGuidCookieName];
        if (ssoCookie == null) return;

        ssoCookie.Expires = DateTime.Now.AddDays(-1);
        ssoCookie.Value = "";
        Response.Cookies.Add(ssoCookie);
    }

    private static void CreateShoppingCartCookie()
    {
        var cookieValue = HttpContext.Current.Session.SessionID;
        var ck = HttpContext.Current.Request.Cookies[PersonifyControlBase.PersonifyShoppingCartGuidCookieName];
        if (ck != null)
        {
            HttpContext.Current.Response.Cookies.Remove(PersonifyControlBase.PersonifyShoppingCartGuidCookieName);
        }

        var dtExpire = DateTime.Now.AddDays(CookieDurationDays);

        var myCookie = new HttpCookie(PersonifyControlBase.PersonifyShoppingCartGuidCookieName)
        {
            Value = cookieValue,
            Expires = dtExpire
        };

        HttpContext.Current.Response.Cookies.Add(myCookie);
    }

    //private static void TransferCart(string anonymousUserId, string masterCustomerId, int subId)
    //{
    //    var postContent = "<TransferCartInput><GUID>" + anonymousUserId + "</GUID><MasterCustomerId>" + masterCustomerId + "</MasterCustomerId><SubCustomerId>" + subId + "</SubCustomerId></TransferCartInput>";

    //    var output = PersonifyUtils.DoPost("TransferCart", postContent);

    //    //if (output != null && output.Length > 0)
    //    //{
    //    //}
    //}

    private void TransferShoppingCartItems(string anonymousUserId, string masterCustomerId)
    {
        var items = SvcClient.Client.Context.WebShoppingCartItems
            .Where(x => x.AnonymousUserId == anonymousUserId)
            .ToList();

        if (items.Count == 0) { return; }

        foreach (var item in items)
        {
            item.AnonymousUserId = null;
            item.MasterCustomerId = masterCustomerId;
            SvcClient.Client.Save<WebShoppingCartItem>(item);
        }

        //CurrentIdentity.ShoppingGUID = null;
        //SessionCache.Remove(DiagnosticsTypes.DiagnosticsShoppingGUIDKey);
    }
}