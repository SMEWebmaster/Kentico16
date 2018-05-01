using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class sso_redirect : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Request.QueryString["code"] != "")
        {
            string value = Request.QueryString["code"].ToString();

            switch (value)
            {
                case "OM":
                    Response.Redirect("/sso/ssoonemine.aspx");
                    break;
                case "HL":
                    Response.Redirect("/sso/HlRedirect.aspx");
                    break;
                    case "OT":
                    Response.Redirect("/sso/ssoonetunnel.aspx");
                    break;
                 case "ME":
                    Response.Redirect("/sso/ssome.aspx");
                    break;
            }
        }
    }
}