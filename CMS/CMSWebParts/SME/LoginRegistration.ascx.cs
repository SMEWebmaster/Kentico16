using CMS.Helpers;
using CMS.PortalControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CMSWebParts_SME_LoginRegistration : CMSAbstractWebPart
{
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        btnLogin.Click += BtnLogin_Click;
        cvPassword.ServerValidate += CvPassword_ServerValidate;

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            if (!RequestHelper.IsPostBack())
            {
                txtUsername.Focus();
            }
        }
    }

    private void CvPassword_ServerValidate(object source, ServerValidateEventArgs args)
    {
        throw new NotImplementedException();
    }

    private void BtnLogin_Click(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }
}