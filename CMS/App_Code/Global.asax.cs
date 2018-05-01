using System;
using System.Web;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

/// <summary>
/// Application methods.
/// </summary>
public class Global : CMSHttpApplication
{
    #region "Methods"

    public Global()
    {
#if DEBUG
        // Set debug mode
        SystemContext.IsWebProjectDebug = true;
#endif

        ApplicationEvents.PreInitialized.Execute += EnsureDynamicModules;
    }


    /// <summary>
    /// Ensures that modules from the App code assembly are registered.
    /// </summary>
    private static void EnsureDynamicModules(object sender, EventArgs e)
    {
        ModuleEntryManager.EnsureModule<CMSModuleLoader>();

        var discovery = new ModuleDiscovery();
        var assembly = typeof(CMSModuleLoader).Assembly;
        foreach (var module in discovery.GetModules(assembly))
        {
            ModuleEntryManager.EnsureModule(module);
        }
    }

    private const string DoLogoutQSKey = "logout";
    private const string PersonifySessionKey = "PersonifyToken";
    private const string SSOTokenCookie = "SSOToken";
    private const string UserNameSessionKey = "UserName";
    private const string PasswordSessionKey = "Password";
    private const string RetryCountSessionKey = "RetryCount";
    private const string RememberMeSessionKey = "RememberMe";
    private const string ReturnURLSessionKey = "ReturnURL";

    new void Session_Start(object sender, EventArgs e)
    {
        if (CMS.Helpers.RequestContext.IsUserAuthenticated)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                if (HttpContext.Current.Request.Url.AbsoluteUri.Contains("ssohandler"))
                {
                    return;
                }

                if (HttpContext.Current.Handler != null && HttpContext.Current.Handler.GetType().Name.Contains("portaltemplate"))
                {
                    if (HttpContext.Current.Session["SessionStarted"] == null)
                    {
                        HttpContext.Current.Session["SessionStarted"] = new object();

                        var ssoToken = Request.Cookies[SSOTokenCookie];

                        if (ssoToken != null)
                        {
                            HttpContext.Current.Session[ReturnURLSessionKey] = HttpContext.Current.Request.Url.AbsoluteUri;
                            Response.Redirect("~/sso/ssohandler.aspx");
                        }
                    }
                }
            }
        }
    }

    #endregion
}