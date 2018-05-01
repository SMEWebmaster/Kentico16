using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SSO;
/// <summary>
/// Summary description for SSOClient
/// </summary>
public class SSOClient : service
{ 
    string PersonifyVendorID = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
	string PersonifyVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
	string PersonifyVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
	string PersonifyVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
	string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];

    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private static string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private static string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
    static string ssoUri = ConfigurationManager.AppSettings["SSO.service"];
    public SSOClient() : base()
    {
        this.Url = ssoUri;
    }
}