using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

 
using AjaxControlToolkit;

using CMS.Controls;
using CMS.Helpers;
using CMS.PortalControls;
using System.Configuration;


using Personify.WebControls.Base.Business;


public partial class CMSWebParts_NetworkATS_UsernamePasswordControl : CMSAbstractLayoutWebPart
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        unpw.CurrentIdentity = new PersonifyIdentity()
                {
                    MasterCustomerId = "000000001475",
                    SubCustomerId = 0,
                    IsLoggedIn = true
                };

    }
    
}