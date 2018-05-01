using System;

using AjaxControlToolkit;
using System.Collections.Generic;
using CMS.Controls;
using CMS.Helpers;
using CMS.PortalControls;
using System.Linq;

public partial class CMSWebParts_Layouts_Accordion_custom : CMSAbstractLayoutWebPart
{
    #region "Public properties"

    /// <summary>
    /// Number of panes.
    /// </summary>
    public int Panes
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Panes"), 2);
        }
        set
        {
            SetValue("Panes", value);
        }
    }


    /// <summary>
    /// Pane headers.
    /// </summary>
    public string PaneHeaders
    {
        get
        {
            return ValidationHelper.GetString(GetValue("PaneHeaders"), "");
        }
        set
        {
            SetValue("PaneHeaders", value);
        }
    }


    /// <summary>
    /// Active pane index.
    /// </summary>
    public int ActivePaneIndex
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("ActivePaneIndex"), -1);
        }
        set
        {
            SetValue("ActivePaneIndex", value);
        }
    }


    /// <summary>
    /// Require opened pane.
    /// </summary>
    public bool RequireOpenedPane
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("RequireOpenedPane"), false);
        }
        set
        {
            SetValue("RequireOpenedPane", value);
        }
    }


    /// <summary>
    /// Width.
    /// </summary>
    public string Width
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Width"), "");
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Header CSS class.
    /// </summary>
    public string HeaderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("HeaderCSSClass"), "");
        }
        set
        {
            SetValue("HeaderCSSClass", value);
        }
    }


    /// <summary>
    /// Selected header CSS class.
    /// </summary>
    public string SelectedHeaderCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SelectedHeaderCSSClass"), "");
        }
        set
        {
            SetValue("SelectedHeaderCSSClass", value);
        }
    }


    /// <summary>
    /// Content CSS class.
    /// </summary>
    public string ContentCSSClass
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ContentCSSClass"), "");
        }
        set
        {
            SetValue("ContentCSSClass", value);
        }
    }


    /// <summary>
    /// Fade transitions.
    /// </summary>
    public bool FadeTransitions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("FadeTransitions"), false);
        }
        set
        {
            SetValue("FadeTransitions", value);
        }
    }


    /// <summary>
    /// Transition duration (ms).
    /// </summary>
    public int TransitionDuration
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("TransitionDuration"), 500);
        }
        set
        {
            SetValue("TransitionDuration", value);
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

        Append("<div");

        // Width
        string width = Width;
        if (!string.IsNullOrEmpty(width))
        {
            Append(" style=\"width: ", width, "\"");
        }

        if (IsDesign)
        {
            Append(" id=\"", ShortClientID, "_env\">");

            Append("<table class=\"LayoutTable\" cellspacing=\"0\" style=\"width: 100%;\">");

            if (ViewModeIsDesign())
            {
                Append("<tr><td class=\"LayoutHeader\" colspan=\"2\">");

                // Add header container
                AddHeaderContainer();

                Append("</td></tr>");
            }

            Append("<tr><td id=\"", ShortClientID, "_info\" style=\"width: 100%;\">");
        }
        else
        {
            Append(">");
        }

        // Add the tabs
        Accordion acc = new Accordion();
        acc.ID = "acc";
        // AddControl(acc);

        if (IsDesign)
        {
            Append("</td>");

            if (AllowDesignMode)
            {
                // Width resizer
                Append("<td class=\"HorizontalResizer\" onmousedown=\"" + GetHorizontalResizerScript("env", "Width", false, "info") + " return false;\">&nbsp;</td>");
            }

            Append("</tr>");
        }

        // Pane headers
        string[] headers = TextHelper.EnsureLineEndings(PaneHeaders, "\n").Split('\n');

        Array.Sort<string>(headers);

        foreach (string s in headers)
        {
            //  Response.Write(s + "<br/>" );
        }
        #region remove old pane
        //for (int i = 1; i <= Panes; i++)
        //// for (int i = Panes; i <= 1; i--)
        //{

        //    // Create new pane
        //    AccordionPane pane = new AccordionPane();
        //    pane.ID = "pane" + i;

        //    // Prepare the header
        //    string header = null;
        //    if (String.IsNullOrEmpty(header))
        //    {
        //        header = "Pane " + i;
        //        Response.Write(Panes + "<br/>");
        //    }
        //    if (headers.Length >= i)
        //    {
        //        header = ResHelper.LocalizeString(headers[i - 1]);
        //        //  Response.Write(header + "<br/>");
        //    }


        //    pane.Header = new TextTransformationTemplate(header);
        //    acc.ContentCssClass = "customPane" + i;
        //    acc.HeaderCssClass = "customPane" + i;
        //    acc.Panes.Add(pane);

        //    AddZone(ID + "_" + i, header, pane.ContentContainer);
        //}
        #endregion
        #region new sorting
        SortedDictionary<string, int> d = new SortedDictionary<string, int>();
        SortedDictionary<string, int> outofthebox = new SortedDictionary<string, int>();

        for (int i = 1; i <= Panes; i++)
        {

            //Create new pane 
            AccordionPane pane = new AccordionPane();
            pane.ID = "pane" + i;

            ///pane header 
            string header = null;
            if (String.IsNullOrEmpty(header))
            {
                header = "Pane " + i;
                // Response.Write(header + "<br/>");
                d.Add(header, i);
            }

            if (headers.Length >= i)
            {
                header = ResHelper.LocalizeString(headers[i - 1]);
                //  Response.Write(header + "<br/>");
                outofthebox.Add(header, i);
            }

            #region remove

            /*  pane.Header = new TextTransformationTemplate(header);
            acc.ContentCssClass = "customPane" + i;
            acc.HeaderCssClass = "customPane" + i;
            acc.Panes.Add(pane);

            AddZone(ID + "_" + i, header, pane.ContentContainer);*/
            #endregion
        }
        // Response.Write(d.Count);
        ///remove the number items which are there in  headers 

        if (d.Count > 0)
        {
            foreach (KeyValuePair<string, int> p in d.Reverse())
            {
                // Response.Write(p.Key+ " --- "+ p.Value + "<br/>");
                int count = headers.Length;
                //dont add the data with headers which is already defined
                if (p.Value > count)
                {
                    AccordionPane pane2 = new AccordionPane();
                    pane2.ID = "pane" + p.Value;

                    pane2.Header = new TextTransformationTemplate(p.Key);
                    acc.ContentCssClass = "customPane" + p.Value;
                    acc.HeaderCssClass = "customPane" + p.Value;
                    acc.Panes.Add(pane2);

                    AddZone(ID + "_" + p.Value, p.Key, pane2.ContentContainer);
                }
            }
        }
        if (outofthebox.Count > 0)
        {
            foreach (var p in outofthebox.Reverse())
            {
                AccordionPane pane2 = new AccordionPane();
                pane2.ID = "pane" + p.Value;

                pane2.Header = new TextTransformationTemplate(p.Key);
                acc.ContentCssClass = "customPane" + p.Value;
                acc.HeaderCssClass = "customPane" + p.Value;
                acc.Panes.Add(pane2);
                AddZone(ID + "_" + p.Value, p.Key, pane2.ContentContainer);
            }
        }

        #endregion
        AddControl(acc);
        // Setup the accordion
        if ((ActivePaneIndex >= 1) && (ActivePaneIndex <= acc.Panes.Count))
        {
            acc.SelectedIndex = ActivePaneIndex - 1;
        }

        acc.ContentCssClass = ContentCSSClass;
        acc.HeaderCssClass = HeaderCSSClass;
        acc.HeaderSelectedCssClass = SelectedHeaderCSSClass;

        acc.FadeTransitions = FadeTransitions;
        acc.TransitionDuration = TransitionDuration;
        acc.RequireOpenedPane = RequireOpenedPane;

        // If no active pane is selected and doesn't require opened one, do not preselect any
        if (!acc.RequireOpenedPane && (ActivePaneIndex < 0))
        {
            acc.SelectedIndex = -1;
        }

        if (IsDesign)
        {
            if (AllowDesignMode)
            {
                Append("<tr><td class=\"LayoutFooter cms-bootstrap\" colspan=\"2\"><div class=\"LayoutFooterContent\">");

                // Pane actions
                Append("<div class=\"LayoutLeftActions\">");

                AppendAddAction(ResHelper.GetString("Layout.AddPane"), "Panes");
                if (Panes > 1)
                {
                    AppendRemoveAction(ResHelper.GetString("Layout.RemoveLastPane"), "Panes");
                }

                Append("</div></div></td></tr>");
            }

            Append("</table>");
        }

        Append("</div>");

        FinishLayout();
    }

    #endregion
}
