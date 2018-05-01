using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
public partial class sso_HLlogin : System.Web.UI.Page
{
    public Boolean loggegedin = false;
    public int _maxresults = 4;
    public bool _aslist = false;
    string HLIAMKey = "2EB17C85-84D1-4888-B1B6-F5EDF1A4396D";

    protected void Page_Load(object sender, EventArgs e)
    {
         if (Session["cuctomerReturnToken"] != null)
            {
                ahref.HRef= "http://community.smenet.org/?ct="+ Session["cuctomerReturnToken"].ToString(); 
               // Response.Redirect(ahref.HRef);
            }
    }

    
}

 