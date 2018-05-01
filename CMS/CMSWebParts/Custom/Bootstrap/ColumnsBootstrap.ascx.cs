using System;
using System.Collections;

using CMS.Helpers;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.Base;
using System.Text;

public partial class CMSWebParts_Layouts_ColumnsBootstrap : CMSAbstractLayoutWebPart
{
    #region "Variables"

    /// <summary>
    /// List of div IDs.
    /// </summary>
    private ArrayList divIds = new ArrayList();

    #endregion


    #region "Properties"

    /// <summary>
    /// Number of columns.
    /// </summary>
    public int Columns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Columns"), 1);
        }
        set
        {
            SetValue("Columns", value);
        }
    }

    public bool IncludeContainingRow
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("IncludeContainingRow"), true);
        }
        set
        {
            SetValue("IncludeContainingRow", value);
        }
    }

    public string ContainerCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ContainerCSS"), "");
        }
        set
        {
            SetValue("ContainerCSS", value);
        }
    }

    /// <summary>
    /// Maximum number of columns.
    /// </summary>
    public int MaxColumns
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaxColumns"), 12);
        }
        set
        {
            SetValue("MaxColumns", value);
        }
    }

    /// <summary>
    /// CSS column Prepend, ex col-sm-.
    /// </summary>
    public string ColumnCSSPrepend
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ColumnCSSPrepend"), "");
        }
        set
        {
            SetValue("ColumnCSSPrepend", value);
        }
    }


    /// <summary>
    /// First left column width.
    /// </summary>
    public string Column1Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column1Width"), "");
        }
        set
        {
            SetValue("Column1Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column2Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column2Width"), "");
        }
        set
        {
            SetValue("Column2Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column3Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column3Width"), "");
        }
        set
        {
            SetValue("Column3Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column4Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column4Width"), "");
        }
        set
        {
            SetValue("Column4Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column5Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column5Width"), "");
        }
        set
        {
            SetValue("Column5Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column6Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column6Width"), "");
        }
        set
        {
            SetValue("Column6Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column7Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column7Width"), "");
        }
        set
        {
            SetValue("Column7Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column8Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column8Width"), "");
        }
        set
        {
            SetValue("Column8Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column9Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column9Width"), "");
        }
        set
        {
            SetValue("Column9Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column10Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column10Width"), "");
        }
        set
        {
            SetValue("Column10Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column11Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column11Width"), "");
        }
        set
        {
            SetValue("Column11Width", value);
        }
    }

    /// <summary>
    /// Column width.
    /// </summary>
    public string Column12Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column12Width"), "");
        }
        set
        {
            SetValue("Column12Width", value);
        }
    }


    /// <summary>
    /// First left column AdditionalCSS.
    /// </summary>
    public string Column1AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column1AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column1AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column2AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column2AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column2AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column3AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column3AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column3AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column4AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column4AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column4AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column5AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column5AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column5AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column6AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column6AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column6AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column7AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column7AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column7AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column8AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column8AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column8AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column9AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column9AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column9AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column10AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column10AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column10AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column11AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column11AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column11AdditionalCSS", value);
        }
    }

    /// <summary>
    /// Column AdditionalCSS.
    /// </summary>
    public string Column12AdditionalCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Column12AdditionalCSS"), "");
        }
        set
        {
            SetValue("Column12AdditionalCSS", value);
        }
    }


    /// <summary>
    /// Row CSS
    /// </summary>
    public string RowCSS
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RowCSS"), "row");
        }
        set
        {
            SetValue("RowCSS", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Prepares the layout of the web part.
    /// </summary>
    protected override void PrepareLayout()
    {
        StartLayout();

        if (IsDesign)
        {
            // Start Complete Wrapper
            Append("<div class=\"" + (IncludeContainingRow ? RowCSS+" " : "") + ContainerCSS + " LayoutTable\" style=\"margin-left: 0; margin-right: 0; " + (ViewModeIsDesign() ? "" : "margin-top:30px;") + "\">", "<div class=\"" + ColumnCSSPrepend + MaxColumns.ToString() + "\">");
            if (ViewModeIsDesign())
            {
                //Append("<tr><td class=\"LayoutHeader\">");
                Append("<div class=\"" + (IncludeContainingRow ? RowCSS + " " : "") + ContainerCSS + "\">", "<div class=\"" + ColumnCSSPrepend + MaxColumns.ToString() + " LayoutHeader\" style=\"padding-left:0; padding-right:0\">");

                // Add header container
                AddHeaderContainer();

                //Append("</td></tr>");
                Append("</div></div>");


            }
            Append("<div class=\"" + (IncludeContainingRow ? RowCSS + " " : "") + ContainerCSS + "\" style=\"" + (ViewModeIsDesign() ? "" : "margin:5px 0;") + "\">");

        }

        // Prepare automatic width
        int cols = Columns;

        // Encapsulating div
        if (IsDesign && AllowDesignMode)
        {
            Append("<div id=\"", ShortClientID, "_all\">");
        }
        else
        {
            Append("<div>");
        }

        // Create columns
        CreateColumns(Columns);


        // End of encapsulating div
        Append("<div style=\"clear: both;\"></div></div>");

        if (IsDesign)
        {
            // Design containing row
            Append("</div>");

            // Footer with actions
            if (AllowDesignMode)
            {

                // Start Footer Row Wrapper
                Append("<div class=\"" + (IncludeContainingRow ? RowCSS+" " : "") + ContainerCSS + " LayoutFooter cms-bootstrap\"><div class=\" " + ColumnCSSPrepend + MaxColumns.ToString() + " LayoutFooterContent\" style=\"min-height:35px;\">");

                // Pane actions
                Append("<div class=\"LayoutLeftActions\">");
                if (Columns < MaxColumns)
                {
                    AppendAddAction("Add Column", "Columns");
                }
                if (Columns > 0)
                {
                    AppendRemoveAction("Remove Column", "Columns");
                }
                Append("</div>");

                // End Footer Row Wrapper
                Append("</div></div>");
            }
            // End complete wrapper
            Append("</div></div>");
        }

        FinishLayout();

    }

    /// <summary>
    /// Creates the columns in the layout.
    /// </summary>
    /// <param name="cols">Number of columns</param>
    protected void CreateColumns(int cols)
    {
        // Wrapper
        Append("<div class=\"" + (IncludeContainingRow ? RowCSS : "") + " " + ContainerCSS + " "+(IsDesign || PortalContext.ViewMode == ViewModeEnum.Edit ? "editModeOverlay" : "")+"\">");

        for (int i = 1; i <= cols; i++)
        {


            // Set the width property
            string widthPropertyName = "Column" + i + "Width";
            string additionalCSSPropertyName = "Column" + i + "AdditionalCSS";
            string colId = "col" + i;

            // Design mode container div
            if (IsDesign)
            {
                Append("<div ");
                // Design mode classes
                if (AllowDesignMode)
                {
                    Append(" class=\"LayoutLeftColumn\"");
                }

                Append(">");
            }

            // Actual Div
            Append("<div");

            // Column Width
            string thisColumnWidth = ValidationHelper.GetString(GetValue("Column" + i + "Width"), "");
            string thisColumnAdditionalCSS = ValidationHelper.GetString(GetValue("Column" + i + "AdditionalCSS"), "");

            if (!String.IsNullOrEmpty(thisColumnWidth))
            {
                Append(" class=\"", ColumnCSSPrepend + thisColumnWidth, (String.IsNullOrWhiteSpace(thisColumnAdditionalCSS) ? "" : " " + thisColumnAdditionalCSS), "\"");
            }
            else
            {
                // Need to have some column width, if not defined, will set to full width
                Append(" class=\"", ColumnCSSPrepend + MaxColumns.ToString(), (String.IsNullOrWhiteSpace(thisColumnAdditionalCSS) ? "" : " " + thisColumnAdditionalCSS), "\"");
            }

            if (IsDesign)
            {
                Append(" id=\"", ShortClientID, "_", colId, "\"");
            }

            Append(">");

            // Add the zone
            AddZone(ID + "_" + i, "[" + i + "/" + Columns.ToString() + "] Column");

            // Close Column Div
            Append("</div>");

            if (IsDesign)
            {
                Append("</div>");
            }

        }

        // End wrapper
        Append("</div>");
    }


    #endregion
}