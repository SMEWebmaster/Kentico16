﻿using System;

using CMS.Helpers;
using CMS.OnlineMarketing;

public partial class CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetupPrologue : CMSSalesForceDialogPage
{
    /// <summary>
    /// Short link to help topic page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "J4JsAw";


    public string AuthorizationSetupUrl
    {
        get
        {
            string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetup.aspx");
            UriBuilder builder = new UriBuilder(URLHelper.GetAbsoluteUrl(baseUrl))
            {
                Port = -1,
                Scheme = "https"
            };
            baseUrl = builder.Uri.AbsoluteUri;
            string url = URLHelper.AddParameterToUrl(baseUrl, "pid", Request.Params["pid"]);

            return URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
        }
    }


    public string AuthorizationSetupHandlerUrl
    {
        get
        {
            string baseUrl = URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/SalesForce/AuthorizationSetupHandler.ashx");
            UriBuilder builder = new UriBuilder(URLHelper.GetAbsoluteUrl(baseUrl))
            {
                Port = -1,
                Scheme = "https"
            };

            return String.Format("{0}?callback=?", builder.Uri.AbsoluteUri);
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterJQuery(Page);
        PageTitle.TitleText = GetString("sf.authorization.title");
        PageTitle.IsDialog = false;
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;

        ConfirmationElement.Text =  GetString("sf.authorizationprologue.success");
        ErrorElement.Text = GetString("sf.authorizationprologue.error");
    }
}