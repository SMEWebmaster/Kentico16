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

public partial class CMSFormControls_Personify_EducationLevelSelector : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return selector.Enabled;
        }
        set
        {
            selector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            return selector.Value;
        }
        set
        {
            selector.Value = value;
        }
    }


    /// <summary>
    /// Returns display name of the value.
    /// </summary>
    public override string ValueDisplayName
    {
        get
        {
            return selector.ValueDisplayName;
        }
    }


    /// <summary>
    /// Gets or sets selected value.
    /// </summary>
    public string SelectedValue
    {
        get
        {
            return selector.SelectedValue;
        }
        set
        {
            selector.SelectedValue = value;
        }
    }


    /// <summary>
    /// Gets or sets selected index. Returns -1 if no element is selected.
    /// </summary>
    public int SelectedIndex
    {
        get
        {
            return selector.SelectedIndex;
        }
        set
        {
            selector.SelectedIndex = value;
        }
    }


    /// <summary>
    /// Gets dropdown list control.
    /// </summary>
    public DropDownList DropDownList
    {
        get
        {
            return selector.DropDownList;
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
}