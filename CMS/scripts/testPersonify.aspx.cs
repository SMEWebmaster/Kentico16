using personifyDataservice;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CustomTables;
using System.Data.SqlClient;
using personifyDataservicesme;

public partial class scripts_addCommittees : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
      string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
     string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
     string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
         Uri ServiceUri = new Uri(svcUri_Base);
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.Credentials = new NetworkCredential(svcLogin, svcPassword);

                ///bind data membership details
                var membershipDetails = DataAccessLayer.CustomerInfos.Where(o => o.CustomerClassCode == "CHAPTER");
                foreach(var member in membershipDetails)
                {
                    Response.Write(member.CustomerClassCode);
                    
               }
       
    }


   
    
}