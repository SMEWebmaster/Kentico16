using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.Membership;
using System.Data;
using System.Configuration;
using System.Net;

public partial class memberredirect_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string PersonifyDNNDomain = "http://" + ConfigurationManager.AppSettings["PersonifyBaseURN"];

        string loginURL = PersonifyDNNDomain + "/PersonifyEbusiness/Default.aspx?TabId=71&SSOForce=Y";

        string KenticologinURL = "/login.aspx?rurl=";

        bool flagpersonifyMember = false;

        bool loggedin = CMS.Helpers.RequestContext.IsUserAuthenticated;
        string fullUrl = Request.RawUrl;
        string site = "";
        if (Request.QueryString["site"] != null)
        {
            site = "&site=" + Request.QueryString["site"];

        }

        if (loggedin)
        {
            UserInfo userdata =
                               CMS.Membership.UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

            DataTable dt = UserInfoProvider.GetUserRoles(userdata);

            if (dt.Rows.Count > 0 && dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["RoleName"].ToString().ToLower().Contains("peronifyuser"))
                    {
                        flagpersonifyMember = true;
                    }

                }
            }
        }

        if (Request.QueryString["returnUrl"] != null)
        {
            if (loggedin)
            {
                Session["returnurl"] = "";

                string Rurl = fullUrl.Replace("?site=sme&", "?");
                Rurl = Rurl.Replace("/memberredirect/default.aspx?returnurl=", "");
                Rurl = Rurl.Replace("&site=sme", "");
                Rurl = Rurl.Replace("PersonifyEbusiness/PersonifyEbusiness/", "PersonifyEbusiness/");
                Rurl = Rurl.Replace("PersonifyEbusiness", "/PersonifyEbusiness");
                Rurl = Rurl.Replace("//", "/");
                Rurl = Rurl.Replace("/memberredirect/default.aspx?returnUrl=", "");
                Response.Redirect(loginURL + "&returnurl=" + Server.UrlEncode(Rurl + site));
            }
            //if (loggedin && flagpersonifyMember)
            //{
            //    Session["returnurl"] = "";

            //    string Rurl = fullUrl.Replace("?site=sme&", "?");
            //    Rurl = Rurl.Replace("/memberredirect/default.aspx?returnurl=", "");
            //    Rurl = Rurl.Replace("&site=sme", "");
            //    Rurl = Rurl.Replace("PersonifyEbusiness/PersonifyEbusiness/", "PersonifyEbusiness/");
            //    Rurl = Rurl.Replace("PersonifyEbusiness", "/PersonifyEbusiness");
            //    Rurl = Rurl.Replace("//", "/");
            //    Rurl = Rurl.Replace("/memberredirect/default.aspx?returnUrl=", "");
            //    Response.Redirect(loginURL + "&returnurl=" + Server.UrlEncode(Rurl + site));
            //}
            //else if (loggedin && (!flagpersonifyMember))
            //{
            //    Response.Redirect("/home?CMSUSER");
            //}
            else
            {
                string Rurl = fullUrl.Replace("?site=sme&", "?");
                Rurl = Rurl.Replace("/memberredirect/default.aspx?returnurl=", "");
                Rurl = Rurl.Replace("&site=sme", "");
                Rurl = Rurl.Replace("PersonifyEbusiness/PersonifyEbusiness/", "PersonifyEbusiness/");
                Rurl = Rurl.Replace("PersonifyEbusiness", "/PersonifyEbusiness");
                Rurl = Rurl.Replace("//", "/");

                Response.Redirect(KenticologinURL + Server.UrlEncode(Rurl + site));//+"<br/>" + Session["redirectUrl"]);
            }
        }
        if (Request.QueryString["rurl"] != null)
        {

            if (flagpersonifyMember)
            {
                string Rurl = fullUrl.Replace("?site=sme&", "?");

                Rurl = Rurl.Replace("/memberredirect/default.aspx?rurl=", "");
                Rurl = Rurl.Replace("&site=sme", "");
                Rurl = Rurl.Replace("PersonifyEbusiness/PersonifyEbusiness/", "PersonifyEbusiness/");
                Rurl = Rurl.Replace("PersonifyEbusiness", "/PersonifyEbusiness");
                Rurl = Rurl.Replace("//", "/");
                Rurl = Rurl.Replace("?rurl=", "");
                Rurl = Rurl.Replace("/login.aspx", "");

                Rurl = Rurl.ToLower();
                if (Rurl.Contains("http:"))
                {
                    Rurl = Rurl.Replace("http://smemi.personifycloud.com/", "");
                    Rurl = Rurl.Replace("http:/smemi.personifycloud.com/", "");
                    Rurl = Rurl.Replace("personifyebusiness", "/personifyebusiness");
                }

                Response.Redirect(loginURL + "&returnUrl=" + Server.UrlEncode(Rurl + site));
            }
            else
            {
                Response.Redirect("/home?CMSUSER");
            }

        }

        
        if (Request.QueryString["url"] != null) //url parameter for non login pages 
        {
            string refereURl = "";

            try
            {
                refereURl = Request.UrlReferrer.ToString();
            }
            catch (Exception ex)
            {

            }



            string Rurl = fullUrl.Replace("?site=sme&", "?");

            string Rurl2 = Rurl;

            if (Rurl2.ToLower().Contains("guest"))
            {
                Session["GuestCheckout"] = "Y";
            }
            else
            {
                Session["GuestCheckout"] = "N";
            }

            Rurl = Rurl.Replace("/memberredirect/default.aspx?url=", "");
            Rurl = Rurl.Replace("&site=sme", "");
            Rurl = Rurl.Replace("PersonifyEbusiness/PersonifyEbusiness/", "PersonifyEbusiness/");

            Rurl = Rurl.Replace("PersonifyEbusiness", "/PersonifyEbusiness");
            Rurl = Rurl.Replace("//", "/");


            if (loggedin && flagpersonifyMember)
            {
                if (Rurl.Contains("?"))
                {
                    Rurl = Rurl + "&" + site;
                }
                else
                {
                    Rurl = Rurl + "?" + site;
                }
                Rurl = Rurl.Replace("&&", "&");

                if (refereURl != "")
                {
                    if (refereURl.Contains("uca"))
                    {
                        refereURl = "site=uca";

                    }
                    else
                    {
                        refereURl = "site=sme";
                    }
                }


                string UrlRet = loginURL + "&returnUrl=" + Rurl;
                if (refereURl == "")
                {
                    refereURl = "site=sme";
                }

                if (UrlRet.Contains("?"))
                {
                    UrlRet = UrlRet + "&" + refereURl;
                }
                else
                {
                    UrlRet = UrlRet + "?" + refereURl;
                }

                if (!(UrlRet.Contains("SSOForce=Y")))
                {
                    if (UrlRet.Contains("?"))
                    {
                        UrlRet = UrlRet + "&SSOForce=Y";
                    }
                    else
                    {
                        UrlRet = UrlRet + "?SSOForce=Y";
                    }
                }


                Response.Redirect(UrlRet);
            }
            else
            {
                if (refereURl != "")
                {
                    if (refereURl.Contains("uca"))
                    {
                        refereURl = "site=uca";

                    }
                    else
                    {
                        refereURl = "site=sme";
                    }
                }


                string UrlRet = PersonifyDNNDomain + (Rurl);
                if (UrlRet.Contains("?"))
                {
                    UrlRet = UrlRet + "&" + refereURl;
                }
                else
                {
                    UrlRet = UrlRet + "?" + refereURl;
                }

                if (!(UrlRet.Contains("SSOForce=Y")))
                {
                    if (UrlRet.Contains("?"))
                    {
                        UrlRet = UrlRet + "&SSOForce=Y";
                    }
                    else
                    {
                        UrlRet = UrlRet + "?SSOForce=Y";
                    }
                }

                Response.Redirect(UrlRet);
            }
        }
    }
}