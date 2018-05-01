using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class sso_TestSSOHandler : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["UserName"] = "eddie_cewood5555@yahoo.com";
        Session["Password"] = "Password1";
        Session["RetryCount"] = null;
        Session["PersonifyToken"] = null;

        Response.Redirect("~/sso/ssohandler.aspx");
    }
}