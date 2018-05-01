using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using personifyDataservice;
using System.Configuration;

public partial class CMSFormControls_PersonifyAppSubCodesSelector : FormEngineUserControl
{

    string PersonifyVendorID = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
    string PersonifyVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    string PersonifyVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    string PersonifyVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private static string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private static string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
    static string sUri = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];

    #region "Variables"

    private string mSelectedValue;
    private bool? mEditText;
    private string mOnChangeScript;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return dropDownList.Enabled;
        }
        set
        {
            dropDownList.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return dropDownList.SelectedValue;
        }
        set
        {
            LoadAndSelectList();

            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    value = ConvertInputValue(value);
                }

                mSelectedValue = ValidationHelper.GetString(value, String.Empty);

                EnsureActualValueAsItem();

                dropDownList.ClearSelection();
                ListItem item = dropDownList.Items.FindByValue(mSelectedValue);
                if (item != null)
                {
                    item.Selected = true;
                }
            }
        }
    }


    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return dropDownList.SelectedItem.Text;
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            return dropDownList.SelectedValue;
        }
        set
        {
            dropDownList.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets selected index. Returns -1 if no element is selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            return dropDownList.SelectedIndex;
        }
        set
        {
            dropDownList.SelectedIndex = value;
        }
    }


    /// <summary>
    /// Gets dropdown list control.
    /// </summary>
    public DropDownList DropDownList
    {
        get
        {
            return dropDownList;
        }
    }

    /// <summary>
    /// Indicates whether or not to use first value as default if default value is empty.
    /// </summary>
    [Obsolete("This property is not used anymore.")]
    public bool FirstAsDefault
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets Javascript code that is executed when selected item is changed.
    /// </summary>
    public string OnChangeClientScript
    {
        get
        {
            return mOnChangeScript ?? ValidationHelper.GetString(GetValue("OnChangeClientScript"), String.Empty);
        }
        set
        {
            mOnChangeScript = value;
        }
    }

    /// <summary>
    /// Macro source.
    /// </summary>
    public string AppCode
    {
        get
        {
            return GetValue("AppCode", String.Empty);
        }
        set
        {
            SetValue("AppCode", value);
        }
    }

    public string Subsystem
    {
        get
        {
            return GetValue("Subsystem", String.Empty);
        }
        set
        {
            SetValue("Subsystem", value);
        }
    }

    public string Type
    {
        get
        {
            return GetValue("Type", String.Empty);
        }
        set
        {
            SetValue("Type", value);
        }
    }


    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // When DependsOnAnotherField is turned on we need force reload because of loading macro values from another fields which may be loaded from ViewState
        LoadAndSelectList(DependsOnAnotherField);

        CheckRegularExpression = true;
        CheckFieldEmptiness = true;

        ApplyCssClassAndStyles(dropDownList);
    }


    /// <summary>
    /// Loads and selects control.
    /// </summary>
    /// <param name="forceReload">Indicates if items should be reloaded even if control contains some values</param>
    private void LoadAndSelectList(bool forceReload = false)
    {
        if (forceReload)
        {
            // Keep selected value
            mSelectedValue = dropDownList.SelectedValue;

            // Clears values if forced reload is requested
            dropDownList.Items.Clear();
        }

        if (dropDownList.Items.Count == 0)
        {

            try
            {
                if (string.IsNullOrEmpty(AppCode))
                {
                    throw new InvalidOperationException("AppCode must be set.");
                }
                else if (string.IsNullOrEmpty(Subsystem))
                {
                    throw new InvalidOperationException("Subsystem must be set.");
                }
                else if (string.IsNullOrEmpty(Type))
                {
                    throw new InvalidOperationException("Type must be set.");
                }
                else
                {
                    // Init selector for countries
                    var service = new PersonifyEntitiesBase(new Uri(svcUri_Base));
                    service.IgnoreMissingProperties = true;
                    service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
                    var appCode = this.AppCode;
                    var subsystem = this.Subsystem;
                    var type = this.Type;
                    var appsubcodes = service.ApplicationSubcodes.Where(x => x.ActiveFlag == true && x.AvailableToWebFlag == true && x.Code == appCode && x.Subsystem == subsystem && x.Type == type).ToList().OrderBy(x => x.DisplayOrder).ToList();

                    dropDownList.DataSource = appsubcodes;
                    dropDownList.DataTextField = "Description";
                    dropDownList.DataValueField = "Subcode";
                    dropDownList.DataBind();
                    dropDownList.Items.Insert(0, new ListItem("-- Please Select --", String.Empty));
                    dropDownList.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            FormHelper.SelectSingleValue(mSelectedValue, dropDownList, true);
        }
    }


    private void ApplyCssClassAndStyles(WebControl control)
    {
        if (!String.IsNullOrEmpty(CssClass))
        {
            control.AddCssClass(CssClass);
            CssClass = null;
        }

        if (!String.IsNullOrEmpty(ControlStyle))
        {
            control.Attributes.Add("style", ControlStyle);
            ControlStyle = null;
        }
    }


    /// <summary>
    /// Displays exception control with current error.
    /// </summary>
    /// <param name="ex">Thrown exception</param>
    private void DisplayException(Exception ex)
    {
        FormControlError ctrlError = new FormControlError();
        ctrlError.FormControlName = FormFieldControlTypeCode.DROPDOWNLIST;
        ctrlError.InnerException = ex;
        Controls.Add(ctrlError);
        dropDownList.Visible = false;
    }


    /// <summary>
    /// Ensures that a value which is not among DDL items but is present in the database is added to DDL items collection.
    /// </summary>
    private void EnsureActualValueAsItem()
    {
        var item = dropDownList.Items.FindByValue(mSelectedValue);
        if (item == null)
        {
            dropDownList.Items.Add(new ListItem(mSelectedValue));
        }
    }

    #endregion
}