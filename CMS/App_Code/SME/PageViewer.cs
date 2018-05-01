using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

/// <summary>
/// Summary description for PageViewer
/// </summary>
public class PageViewer: Page 
{
	  
         private Personify.WebControls.Base.Business.PersonifyIdentity _personifyUser = null;

    public Personify.WebControls.Base.Business.PersonifyIdentity PersonifyUser
    {
        get
        {
            return _personifyUser;
        }
        set
        {
            _personifyUser = value;
        }
    }

    protected void Page_PreInit(object sender, EventArgs e)
    {
        ScriptManager scriptManager = null;
        RadAjaxManager ajaxManager = null;

        foreach (Control control in Page.Form.Controls)
        {
            if (control is ScriptManager) scriptManager = (ScriptManager)control;
            if (control is RadAjaxManager) ajaxManager = (RadAjaxManager)control;
        }

        if (scriptManager == null)
        {
            scriptManager = new RadScriptManager { ID = "RadScriptManager1" };
            Page.Form.Controls.AddAt(0, scriptManager);
        }
         
        if (ajaxManager == null)
        {
            ajaxManager = new RadAjaxManager { ID = "RadAjaxManager1" };
            Page.Form.Controls.AddAt(1, ajaxManager);
        }
        
    }

}