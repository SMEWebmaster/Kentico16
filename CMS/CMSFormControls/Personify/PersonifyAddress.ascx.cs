using System;
using System.Web.UI.WebControls;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Globalization;
using CMS.UIControls;
using personifyDataservice;

public partial class CMSFormControls_PersonifyAddress : FormEngineUserControl
{
    private readonly string PersonifyVendorBlock = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string PersonifyVendorPassword = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string PersonifyVendorName = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string PersonifyVendorID = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorID"];

    private readonly string svcLogin = System.Configuration.ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = System.Configuration.ConfigurationManager.AppSettings["svcPassword"];
    private readonly string svcUri_Base = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];

    public override object Value
    {
        get
        {
            var formattedAddress = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", txtAddress1.Text, txtAddress2.Text, txtCity.Text, drpState.Items.Count > 0 ? drpState.SelectedValue : String.Empty, drpCountry.Items.Count > 0 ? drpCountry.SelectedValue : String.Empty, txtPostalCode.Text);

            return formattedAddress;
        }
        set
        {
            if (value != null)
            {
                var stringValue = value.ToString();
                if (String.IsNullOrEmpty(stringValue))
                {
                    var pipedDelimiatedAddressArray = value.ToString().Split(new char[] { '|' });

                    for (int i = 0; i < pipedDelimiatedAddressArray.Length; i++)
                    {
                        try
                        {
                            var item = pipedDelimiatedAddressArray[i];

                            if (!String.IsNullOrEmpty(item))
                            {
                                switch (i)
                                {
                                    case 0:
                                        txtAddress1.Text = item;
                                        break;
                                    case 1:
                                        txtAddress2.Text = item;
                                        break;
                                    case 2:
                                        txtCity.Text = item;
                                        break;
                                    case 3:
                                        drpState.SelectedValue = item;
                                        break;
                                    case 4:
                                        drpCountry.SelectedValue = item;
                                        break;
                                    case 5:
                                        txtPostalCode.Text = item;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }

    #region "Methods"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        drpCountry.SelectedIndexChanged += DrpCountry_SelectedIndexChanged;
        if (StopProcessing)
        {
            return;
        }
        else if (!this.IsPostBack)
        {
            // Init selector for countries
            var service = new PersonifyEntitiesBase(new Uri(svcUri_Base));
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
            var countries = service.Countries.Where(x => x.ActiveFlag == true).ToList().OrderBy(x => x.CountryCode == "USA" ? 0 : 1).ThenBy(x => x.CountryDescription).ToList();

            drpCountry.DataSource = countries;
            drpCountry.DataTextField = "CountryDescription";
            drpCountry.DataValueField = "CountryCode";
            drpCountry.DataBind();
            drpCountry.Items.Insert(0, new ListItem("-- Please Select --", String.Empty));
            drpCountry.SelectedValue = "USA";

            PopulateState(service);
        }
    }

    private void DrpCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        var service = new PersonifyEntitiesBase(new Uri(svcUri_Base));
        service.IgnoreMissingProperties = true;
        service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
        PopulateState(service);
    }

    private void PopulateState(PersonifyEntitiesBase service)
    {
        var selectedCountryValue = drpCountry.SelectedValue;

        if (String.IsNullOrEmpty(selectedCountryValue))
        {
            divState.Visible = false;
            if (drpState.Items.Count > 0)
            {
                drpState.Items.Clear();
            }
            txtAddress1.Text = String.Empty;
            divAddress1.Visible = false;
            txtAddress2.Text = String.Empty;
            divAddress2.Visible = false;
            txtCity.Text = String.Empty;
            divCity.Visible = false;
            txtPostalCode.Text = String.Empty;
            divPostalCode.Visible = false;
        }
        else
        {
            divState.Visible = true;
            divAddress1.Visible = true;
            divAddress2.Visible = true;
            divCity.Visible = true;
            divPostalCode.Visible = true;

            var state = service.States.Where(x => x.ActiveFlag == true && x.CountryCodeString == selectedCountryValue).ToList().OrderBy(x => x.CountryCodeString);

            drpState.DataSource = state;
            drpState.DataTextField = "StateDescription";
            drpState.DataValueField = "StateCode";
            drpState.DataBind();
            if (drpState.Items.Count > 0)
            {
                divState.Visible = true;
                drpState.Items.Insert(0, new ListItem("-- Please Select --", String.Empty));
                drpState.SelectedIndex = 0;
            }
            else
            {
                divState.Visible = false;
            }
        }
    }
    #endregion
}