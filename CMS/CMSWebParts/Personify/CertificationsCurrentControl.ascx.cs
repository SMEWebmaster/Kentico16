using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.PortalControls;
using CMS.Helpers;
using Personify.WebControls.Profile.UI;

public partial class CMSWebParts_Personify_CertificationsCurrentControl : CMSAbstractWebPart
{
    #region "Properties"

    

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            PersonifyControlBase objbase = new PersonifyControlBase();
            var ctrl = new Personify.WebControls.Profile.UI.CertificationsCurrentControl();
            objbase.InitPersonifyWebControl(ctrl);
            phPersonifyControl.Controls.Add(ctrl);
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        SetupControl();
    }

    #endregion
}



