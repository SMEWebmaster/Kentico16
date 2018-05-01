using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PersonifyHacks
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("TEST", "asdfsdfsfdsdfdsf"));

            //Uri ServiceUri = new Uri("http://smemitst.personifycloud.com/PersonifyDataServices/PersonifyData.svc");
            //var reference = new ServiceReference1.PersonifyEntitiesBase(ServiceUri);

            //reference.Credentials = new NetworkCredential("admin", "admin");

            //var membershipDetails = reference.CustomerInfos.Where(p => p.CustomerClassCode == "CHAPTER" && p.RecordType == "T").Select(o => o).ToList().OrderBy(m => m.LabelName);

            //membershipDetails.Equals(membershipDetails);
        }
    }
}