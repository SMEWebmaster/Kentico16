using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Management;

/// <summary>
/// Summary description for SMENETSettings
/// </summary>
public static class SMENETSettings
{
    public static string PersonifyBaseURN
    {
        get
        {
            return ConfigurationManager.AppSettings["PersonifyBaseURN"];
        }
    }
}