using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;

using CMS.FormControls;
using CMS.Helpers;
using personifyDataservice;

public partial class CMSFormControls_Inputs_TextBox : FormEngineUserControl
{
    private readonly string PersonifyVendorBlock = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string PersonifyVendorPassword = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string PersonifyVendorName = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string PersonifyVendorID = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorID"];

    private readonly string svcLogin = System.Configuration.ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = System.Configuration.ConfigurationManager.AppSettings["svcPassword"];
    private readonly string svcUri_Base = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];


    #region "Public properties"

    /// <summary>
    /// Gets or sets encoded textbox value.
    /// </summary>
    public override object Value
    {
        get
        {
            return String.Empty;
        }
        set
        {
            //
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        drpCountry.SelectedIndexChanged += DrpCountry_SelectedIndexChanged;
        if (!this.Page.IsPostBack)
        {
            Uri serviceUri = new Uri(svcUri_Base);

            var service = new PersonifyEntitiesBase(serviceUri);
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);


            var countries = service.Countries.Where(x => x.ActiveFlag == true).ToList().OrderBy(x => x.CountryCode == "USA" ? 0 : 1).ThenBy(x => x.CountryDescription).ToList();

            drpCountry.DataSource = countries;
            drpCountry.DataTextField = "CountryDescription";
            drpCountry.DataValueField = "CountryCode";
            drpCountry.SelectedIndex = 0;
            drpCountry.DataBind();

            BindStateDropdown(service);
        }
    }

    private void BindStateDropdown(PersonifyEntitiesBase service)
    {
        var selectedCountry = drpCountry.SelectedValue;

        var state = service.States.Where(x => x.ActiveFlag == true && x.CountryCodeString == selectedCountry).ToList();

        drpState.DataTextField = "StateDescription";
        drpState.DataValueField = "StateCode";
        drpState.SelectedIndex = 0;
        drpState.DataBind();
    }

    private void DrpCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        Uri serviceUri = new Uri(svcUri_Base);

        var service = new PersonifyEntitiesBase(serviceUri);
        service.IgnoreMissingProperties = true;
        service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);

        BindStateDropdown(service);
    }

    #endregion
}