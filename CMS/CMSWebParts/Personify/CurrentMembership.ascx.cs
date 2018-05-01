using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;

using Personify.WebControls.Base.Business;

using Telerik.Web.UI;

public partial class CMSWebParts_Personify_CurrentMembership : CMSAbstractLayoutWebPart
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Page_InIt(object sender, EventArgs e)
    {

        PersonifyControlBase objbase = new PersonifyControlBase();
        // var ctrl = new Personify.WebControls.ShoppingCart.UI.CartPreviewControl();
        var ctrl = new Personify.WebControls.Profile.UI.PurchaseHistory();

        PersonifyControlBase obbase = new PersonifyControlBase();
        //Personify.WebControls.Base.Business.PersonifyIdentity xyz = objbase.GetPersonifyUser();
        SME.PersonifyContainer.ContainerHelper.EnsureRadAjaxManager(ctrl);
        objbase.InitPersonifyWebControl(ctrl);
        phPersonifyControl.Controls.Add(ctrl);
    }
}



