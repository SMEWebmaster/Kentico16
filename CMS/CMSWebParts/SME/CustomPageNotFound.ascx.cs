using CMS.PortalControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CMSWebParts_SME_CustomPageNotFound : CMSAbstractWebPart
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string[] url = Request.RawUrl.Split('/');
        if (url[1].ToLower().Contains("uca") || url[1].ToLower().Contains("uca-of-sme"))
        {
            Response.Redirect("/uca-of-sme/specialpages/page-not-found");
        }
    }
}