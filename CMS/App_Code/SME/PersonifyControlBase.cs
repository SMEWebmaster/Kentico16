using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Configuration;
using Personify.WebControls.Base;
using Personify.WebControls.Base.PersonifyDataServicesBase;
using SSO;

using CMS.UIControls;
using CMS.CMSHelper;
using CMS.Membership;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.DocumentEngine;
using System.Data;
using System.Collections.Generic;

/// <summary>
/// Summary description for PersonifyControlBase
/// </summary>
public class PersonifyControlBase 
{
  //  private readonly Ektron.Cms.API.User.User _ektronUserApi = new Ektron.Cms.API.User.User();
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword =  ConfigurationManager.AppSettings["PersonifySSO_Password"];

    private readonly SSO.service _wsSso = new SSO.service();
    private readonly List<WebControlParameter> _webControlParameters;
    private const string ShoppingCartGuidCookieName = "PersonifyShoppingCartGUID";
    public const int CookieDurationDays = 90;

    public PersonifyControlBase()
    {
        _wsSso.Url = _personifySsoUrl;
        _webControlParameters = PersonifyUtils.WebControlParameters;
    }

    public static string PersonifyShoppingCartGuidCookieName
    {
        get
        {
            return ShoppingCartGuidCookieName;
        }
    }

    public  void InitPersonifyWebControl(object oControl)
    {
        if (oControl is IPersonifyBaseUserControl)
        {
            if (oControl is PersonifyBaseUserControl)
            {
                var pbc = oControl as PersonifyBaseUserControl;
                pbc.CurrentIdentity = GetPersonifyUser();

            }
            else
            {
                if (oControl is PersonifyBaseUserControlNoSkin)
                {
                    var pbc = oControl as PersonifyBaseUserControlNoSkin;
                    pbc.CurrentIdentity = GetPersonifyUser();
                }
            }
        }
    }

    public void LoadDropdownList(DropDownList ddl)
    {
        ddl.DataTextField = "ParameterName";
        ddl.DataValueField = "ParameterName";
        ddl.DataSource = _webControlParameters;
        ddl.DataBind();

        ddl.Items.Insert(0, new ListItem("", ""));
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

                if( flagadmin)
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