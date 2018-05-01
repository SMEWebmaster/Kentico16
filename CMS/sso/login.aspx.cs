using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data;
using System.Web.Security;

using IMS;

using SSO;
using System.Collections.Specialized;

using personifyDataservice;
using System.Net;
public partial class login : System.Web.UI.Page
{

    #region personify constants
    public const string ProductImageDesc = "Product Image Directory, default key is 'PersonifyImageDirectory'.  If key is blank and 'PersonifyImageDirectory' does not return a result, the images folder will be used.";
    public const string ProductImageTitle = "Image Directory";
    public const string DefaultProductDirectoryKey = "PersonifyImageDirectory";
    public const string DefaultProductDirectoryValue = "images";
    public const string DefaultProductUrlParameter = "productId";
    public const string ProductIdUrlParameterTitle = "ProductId URL Parameter";
    public const string ProductIdUrlParameterDesc = "The QueryString parameter name that will be used to read the product id. Same parameter will be used to set the product id when the page is redirected.";
    public const string DefaultDailyRateCodeDescription = "Daily Rate";
    public const string DailyRateCodeDescriptionParameterTitle = "Descriptive Text for Daily Rates in Rate Dropdown";
    public const string DailyRateCodeDescriptionParameterDesc = "The text that will be shown to the user as Text for selection of Daily Rate Products.";
    public const string NotFoundImageTitle = "Default Image Name";
    public const string NotFoundImageDesc = "Image Name of image to display if the Product does not have one.";
    public const string NotFoundImageDefault = "NotFoundImage.jpg";
    public const string DefaultRateCodeUrlParameter = "rateCode";
    public const string RateCodeUrlParameterTitle = "RateCode URL Parameter";
    public const string RateCodeUrlParameterDesc = "The QueryString parameter name that will be used to read the rate code. Same parameter will be used to set the ratecode when the page is redirected.";
    public const string CancelUrl = "Cancel Return URL";
    public const string CancelUrlDesc = "The URL to take the user when clicking cancel.";
    public const string CheckOutURL = "Checkout URL";
    public const string CheckOutURLDesc = "The URL for Checkout of the current cart.";
    public const string ViewCartURL = "View Cart URL";
    public const string ViewCartURLDesc = "The URL for viewing the current cart.";
    public const string QueryStringParameterToPreserve = "QueryString Parameters To Preserve";
    public const string QueryStringParameterToPreserveDesc = "Comma seperated list of QueryString parameters to pass on to next page. Read from the current QueryString.";
    public const string ReturnUrlRateParameter = "Query String Name for Return URL";
    public const string ReturnUrlRateParameterDefault = "returnurl";
    public const string ReturnUrlRateParameterDesc = "Query String Name for the return URL. Return URL is where the user will be redirected after page is completed.";
    public const string LoginUrl = "Login URL";
    public const string LoginUrlDesc = "The URL for the user to Login.";
    public const string GuestCheckoutLoginUrl = "Guest Checkout Login URL";
    public const string GuestCheckoutLoginUrlDesc = "The URL to redirect when the guest user is starting checkout process.";
    public const string GuestCheckoutLoginReturnUrlParameter = "Guest Checkout Login Return Url Parameter";
    public const string GuestCheckoutLoginReturnUrlParameterDesc = "The paramter to set up the return url for the guest checkout login";
    public const string ResetPasswordUrl = "Reset Password URL";
    public const string ResetPasswordUrlDesc = "The URL for the user to reset their password. Defaults to SSO Password Reset URL.";
    public const string RegistrationUrl = "Registration URL";
    public const string RegistrationUrlDesc = "The URL for the user to register.";
    public const string ViewRegistrationUrl = "View Registration URL";
    public const string ViewRegistrationUrlDesc = "The URL for the user to view existing registration.";

    //meetings calendar
    public const string ShowFiltersDescription = "Shows/hides all filters for meeting search.";
    public const string ShowFiltersTitle = "Show filters";

    public const string ShowKeywordFilterDescription = "Shows 'Keyword' filter for meeting search.";
    public const string ShowKeywordFilterTitle = "Show 'Keyword' filter";
    public const string KeywordFilterTitleDescription = "Section title to be displayed for 'Keyword' filter.";
    public const string KeywordFilterTitle = "'Keyword' filter title";
    public const string KeywordFilterTitleDefault = "Keyword";

    public const string ShowDateFilterDescription = "Shows 'Date' filter for meeting search.";
    public const string ShowDateFilterTitle = "Show 'Date' filter";
    public const string DateFilterTitleDescription = "Section title to be displayed for 'Date' filter.";
    public const string DateFilterTitle = "'Date' filter title";
    public const string DateFilterTitleDefault = "Date";

    public const string ShowSponsorFilterDescription = "Shows 'Organizer/Sponsor' filter for meeting search.";
    public const string ShowSponsorFilterTitle = "Show 'Organizer/Sponsor' filter";
    public const string SponsorFilterTitleDescription = "Section title to be displayed for 'Organizer/Sponsor' filter.";
    public const string SponsorFilterTitle = "'Organizer/Sponsor' filter title";
    public const string SponsorFilterTitleDefault = "Organizer/Sponsor";
    public const string PreFilterSponsorDescription = "Organizer/Sponsor pre filter values (separated with commas)";
    public const string PreFilterSponsorTitle = "Organizer/Sponsor - Pre filter";
    public const string PreFilterSponsorDefault = "";

    public const string PreFilterProdClassCodeDescription = "Product Class Code pre filter values (separated with commas)";
    public const string PreFilterProdClassCodeTitle = "Product Class Code - Pre filter";
    public const string PreFilterProdClassCodeDefault = "";

    public const string UniquePageKeyDescription = "This is used to distinguish the meeting calendar configuration on multiple pages (unique session for each calendar)";
    public const string UniquePageKeyTitle = "Unique Page Key";
    public const string UniquePageKeyDefault = "";

    public const string ShowEventFormatFilterDescription = "Shows 'Event Format' filter for meeting search.";
    public const string ShowEventFormatFilterTitle = "Show 'Event Format' filter";
    public const string EventFormatFilterTitleDescription = "Section title to be displayed for 'Event Format' filter.";
    public const string EventFormatFilterTitle = "'Event Format' filter title";
    public const string EventFormatFilterTitleDefault = "Event Format";
    public const string PreFilterEventFormatDescription = "Event Format pre filter values (separated with commas)";
    public const string PreFilterEventFormatTitle = "Event Format - Pre filter";
    public const string PreFilterEventFormatDefault = "";

    public const string ShowTopicFilterDescription = "Shows 'Topic' filter for meeting search.";
    public const string ShowTopicFilterTitle = "Show 'Topic' filter";
    public const string TopicFilterTitleDescription = "Section title to be displayed for 'Topic' filter.";
    public const string TopicFilterTitle = "'Topic' filter title";
    public const string TopicFilterTitleDefault = "Topic";

    public const string ShowCreditsFilterDescription = "Shows 'Credits' filter for meeting search.";
    public const string ShowCreditsFilterTitle = "Show 'Credits' filter";
    public const string CreditsFilterTitleDescription = "Section title to be displayed for 'Credits' filter.";
    public const string CreditsFilterTitle = "'Credits' filter title";
    public const string CreditsFilterTitleDefault = "Credits";

    public const string ShowLocStateFilterDescription = "Shows 'Loc. by State' filter for meeting search.";
    public const string ShowLocStateFilterTitle = "Show 'Loc. by State' filter";
    public const string LocStateFilterTitleDescription = "Section title to be displayed for 'Loc. by State' filter.";
    public const string LocStateFilterTitle = "'Loc by State' filter title";
    public const string LocStateFilterTitleDefault = "Location (State)";

    public const string ShowLocDistanceFilterDescription = "Shows 'Loc. by Distance' filter for meeting search.";
    public const string ShowLocDistanceFilterTitle = "Show 'Loc. by Distance' filter";
    public const string LocDistanceFilterTitleDescription = "Section title to be displayed for 'Loc. by Distance' filter.";
    public const string LocDistanceFilterTitle = "'Loc by Distance' filter title";
    public const string LocDistanceFilterTitleDefault = "Location (Distance)";

    public const string ShowMasterProductSessionsDescription = "Shows sub products which are also master products.";
    public const string ShowMasterProductSessionsTitle = "Show Master Product sessions";

    public const string WorkflowName_MemberJoin = "MemberJoin";
    public const string WorkflowName_MeetingRegistration = "MeetingRegistration";

    public const string ShowCreditsColumnInEventsDescription = "Shows 'Credits' column in list of events after meeting search.";
    public const string ShowCreditsColumnInEventsTitle = "Show 'Credits' column in list of events";
    public const string CreditsColumnTitleDescription = "To customize 'Credits' column title in the list of events.";
    public const string CreditsColumnTitle = "Credits column title";
    public const string CreditsColumnTitleDefault = "Credits";

    public const string ShowSponsorsColumnInEventsDescription = "Shows 'Sponsors' column in list of events after meeting search.";
    public const string ShowSponsorsColumnInEventsTitle = "Show 'Sponsors' column in list of events";
    public const string SponsorColumnTitleDescription = "To customize 'Sponsors' column title in the list of events.";
    public const string SponsorColumnTitle = "Sponsors column title";
    public const string SponsorColumnTitleDefault = "Organizer/Sponsor";

    public const string ShowCreditTypeInEventsDescription = "Shows 'Credit Type' in Credits column of list of events.";
    public const string ShowCreditTypeInEventsTitle = "Show 'Credit Type' in Credits Column";
    public const string ThresholdLevelDescription = "Sets threshold level for number of seats available";
    public const string ThresholdLevelTitle = "Threshold level";
    public const string EventPopupTextDescription = "Sets additional text for Event Popup";
    public const string EventPopupTextTitle = "Additional text configuration for Event Popup";
    public const string EventPopupTextDefault = "Additional discounts may apply!<br>Login to see if you qualify for a lower rate.";

    //meetings wizard
    public const string CancelWizardUrlDesc = "URL to go when 'Cancel' is hit on any step";
    public const string CancelWizardUrl = "Cancel URL";

    public const string CSMManagersURLDefault = "CSM_MANAGERS_PROFILE_URL";
    public const string CSMManagersURLTitle = "Managers Profile URL (Use Key = CSM_MANAGERS_PROFILE_URL)";
    public const string CSMManagersURLDescription = "URL of the Managers Profile Page.";

    public const string CSMRosterMemberURL = "CSM_ROSTERPROFILE_URL";
    public const string CSMRosterMemberURLTitle = "Roster Member's Profile URL (Use Key = CSM_ROSTERPROFILE_URL)";
    public const string CSMRosterMemberURLDescription = "URL of Roster Member Profile Page.";

    public const string CSMRosterURL = "CSM_ROSTER_URL";
    public const string CSMRosterURLTitle = "Roster Listing URL (Use Key = CSM_ROSTER_URL)";
    public const string CSMRosterURLDescription = "URL of Roster Listing Page.";

    public const string CSMCompanyProfileURL = "CSM_COMPANY_PROFILE_URL";
    public const string CSMCompanyProfileURLTitle = "Company's Profile URL (Use Key = CSM_COMPANY_PROFILE_URL)";
    public const string CSMCompanyProfileURLDescription = "URL of Company Profile Page.";

    //Committee
    public const string CommMbrIdParamDefault = "CommMbrId";
    public const string CommTermDetailsReturnParamDefault = "return";
    public const string CommTermDetailsModeParamDefault = "Mode";
    public const string CommMbrMcidParamDefault = "CommMbrMcid";
    public const string CommMbrScidParamDefault = "CommMbrScid";
    public const string CommMbrNameParamDefault = "CommMbrName";
    public const string CommMcidParamDefault = "CommMcid";
    public const string CommScidParamDefault = "CommScid";

    public const string CommCommitteeDetailsURL = "CMT_DETAIL_URL";
    public const string CommCommitteeDetailsTitle = "Committee Details URL(Use key = CMT_DETAIL_URL)";
    public const string CommCommitteeDetailsDescription = "URL of the Committee Detail Page";

    public const string CommCommitteeMemberDetailsURL = "CMT_MBR_DETAIL_URL";
    public const string CommCommitteeMemberDetailsTitle = "Committee Member Details URL(Use key = CMT_MBR_DETAIL_URL)";
    public const string CommCommitteeMemberDetailsDescription = "URL of the Committee Member Detail Page";
     #endregion

    private string PersonifySSOVendorName = "TIMSS"; //ConfigurationManager.AppSettings["PersonifySSO_VendorName"].ToString();

    private string PersonifySSOVendorPassword = "DAC851F82447E20CA8FEE0BF0E5EB64B";//ConfigurationManager.AppSettings["PersonifySSO_Password"].ToString();

    private string PersonifySSOVendorBlock = "E670617D84FD6DF0F8F32866939E97C5";// ConfigurationManager.AppSettings["PersonifySSO_Block"].ToString();

    private string PersonifyLoginURL = "http://smemitst.personifycloud.com/SSO/login.aspx";// ConfigurationManager.AppSettings["Personify_LoginURL"].ToString();

    private string PersonifySSOVendorID = "7";//ConfigurationManager.AppSettings["PersonifySSO_VendorID"].ToString();

    private string PersonifySSOUrl = "http://smemitst.personifycloud.com/SSO/login.aspx";//ConfigurationManager.AppSettings["personify.SSO.service"].ToString();

    private SSO.service wsSSO = new service();
    private IMS.IMService _wsIms = new IMS.IMService();
    LoginUsertokentico objKenticoService = new LoginUsertokentico();
    //  private sso2.serviceSoap ws2 = null;321`

    //?ct=bf0d24aec07dcbb9bcd2e30c8a8ece5f5e8a7be87e2be5b0777a80ed8677c913168df116d11f9a8605b6494176601b386f695dd1b5cea2faa1344de54fd29528

    //private long PageId
    //{IMSWebReferenceURL
    //    get
    //    {
    //        var pageId = Request.QueryString["Pageid"];
    //        return Isa.Utilities.Common.CastObjectToLong(pageId);
    //    }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        # region Check loggedin Kentico
        bool loggedin = false;
        loggedin = objKenticoService.CheckLoginUser("");
        try
        {
            Response.Write(Session["PersonifyToken"]);

            lit1.Text = Session["PersonifyToken"].ToString();
        }
        catch (Exception)
        {

        }
        #endregion
        if (loggedin)
        {
            loggedintosystem();
           
           
        }
        else
        {

            if (Request.QueryString["ct"] != null)
            {
                string customerToken = Request.QueryString["ct"];

                string decryptedToken = DecryptCustomerToken(customerToken);
                string finalToken = "";
                if (decryptedToken != "")
                {
                    finalToken = ValidateCustomerToken(decryptedToken);
                }

                string customerIdentifier = "";
                string emailaddress = null;
                string userName = null;

                if (finalToken != "")
                {
                    customerIdentifier = ValidateUser(finalToken, ref emailaddress, ref userName);
                    Session["PersonifyToken"] = finalToken;
                    lit1.Text = finalToken;

                }

                AuthenticateCustomer(customerIdentifier, emailaddress, userName);

                //////////////login user to kentico and redirect to page where the User is coming from 
                // Response.Write(emailaddress + "---" + userName);
                var redirectUrl = "~/login.aspx";//.GetEktronFriendlyUrl(this.PageId, out urlPath);
                string urlPath = "";
                lnkLogout.NavigateUrl = urlPath + "?action=logout&returnurl=" + Server.UrlEncode(redirectUrl);
                lnkLogout.Text = string.Format("Logout {0}", userName);
                //////////////////////

                /// 
                lnkLogout.Visible = true;
                lnkLogin.Visible = false;
                loggedintosystem();
            }

            else
            {
                if (!IsPostBack)
                {
                    //if (Request.QueryString["ektronPageBuilderEdit"] == null)
                    {

                        ////   wsSSO = new sso2.service();
                        //  wsSSO.Url = PersonifySSOUrl;
                        string VendorToken = GetSSOVendorToken(Request.Url.AbsoluteUri);

                        if (!string.IsNullOrEmpty(VendorToken))
                        {

                            ///http://smemitst.personifycloud.com/personifyebusiness/Home/Login/tabid/71/Default.aspx?returnurl=%2fPersonifyEbusiness%2fdefault.aspx
                            string RedirectURL = string.Format(PersonifyLoginURL, PersonifySSOVendorID, VendorToken);

                            // RedirectURL = GetLoginUrlWithReturnUrl();
                            lnkLogin.NavigateUrl = RedirectURL + "?vi=" + PersonifySSOVendorID + "&vt=" + VendorToken;


                            CheckForSSOToken();
                        }
                        else
                        {
                            //  this.Visible = false;
                        }
                    }
                }
            }
        }
       /* if (Request.QueryString["action"] != null)
        {
            if (Request.QueryString["action"].ToString().ToLower() == "logout")
            {
                Response.Redirect("/logout.aspx");

            }
        } */
    }
    public void loggedintosystem()
    {
        lnkLogin.Visible = false;
        var redirectUrl = "~/login.aspx";//.GetEktronFriendlyUrl(this.PageId, out urlPath);
        string urlPath = "";
        string userName = CMS.Membership.MembershipContext.AuthenticatedUser.UserName;

        lnkLogout.NavigateUrl = urlPath + "?action=logout&returnurl=" + Server.UrlEncode(redirectUrl);
        lnkLogout.Text = string.Format("Logout {0}", userName);
        lnkLogout.Visible = true;
        LoggedinSystems.Visible = true;
          
    }
    #region login detials
    private string AuthenticateCustomer(string customerIdentifier, string email, string userName)
    {
        string[] aIdentifiers = customerIdentifier.Split('|');
        string sMasterCustomerId = aIdentifiers[0];
        //string SubCustomerID = aIdentifiers[1];
        //need to account for no customer ID
        return KenticoAuthentication(sMasterCustomerId, userName, email);
    }


    private string KenticoAuthentication(string masterCustomerId, string userName, string email)
    {

        var changed = false;

        string UserName = userName;
        string remoteId = masterCustomerId;
        string FirstName = userName;
        string LastName = userName;
        string Email = email;
        string password = masterCustomerId;
        //string customerClassCode = CustomerData.CustomerClassCode.Value;
        Uri ServiceUri = new Uri("http://smemitst.personifycloud.com/PersonifyDataServices/PersonifyData.svc");
        PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
        DataAccessLayer.Credentials = new NetworkCredential("admin", "admin123");

        var cusDemographics = DataAccessLayer.CusNameDemographics.Where(p => p.MasterCustomerId == masterCustomerId).Select(o => o).ToList().FirstOrDefault();

        if (cusDemographics != null)
        {
            FirstName = cusDemographics.FirstName;
            LastName = cusDemographics.LastName;    

        }

        var groups = _wsIms.IMSCustomerRoleGet(PersonifySSOVendorName, PersonifySSOVendorPassword, Session["PersonifyToken"].ToString());

        string groupslist = "";
        if (groups != null)
        {
           // foreach (var  p in groups.CustomerRoles)
            {
             //  Response.Write(p.Value  ); 
            }
            //{
            //    groupslist = groupslist + p + ",";
            //}
        }
        //////
        /// login to Kentico with the information 
        string login = ""; //objKenticoService.CreateUpdateLoginUserinKentico(userName, FirstName, LastName, Email, groupslist, true, false);
        ///////add to Session 
                        
        userinfo uInfo = new userinfo
        {
            ID = masterCustomerId,
            Token = Session["PersonifyToken"].ToString(),
            email = email,
            firstname = FirstName,
            lastname = LastName,
            username = userName,
            groupNames = groupslist
        };

        Session["userClass"] = uInfo;


        return "";

    }
    #endregion
    #region token login
    private string DecryptCustomerToken(string customerToken)
    {
        CustomerTokenDecryptResult res = wsSSO.CustomerTokenDecrypt(PersonifySSOVendorName, PersonifySSOVendorPassword, PersonifySSOVendorBlock, customerToken);

        if (!string.IsNullOrEmpty(res.CustomerToken))
        {
            return res.CustomerToken;
        }
        else
        {
            return null;
        }
    }

    private string ValidateCustomerToken(string customerToken)
    {
        SSOCustomerTokenIsValidResult res = wsSSO.SSOCustomerTokenIsValid(PersonifySSOVendorName, PersonifySSOVendorPassword, customerToken);

        if (res.Valid && !string.IsNullOrEmpty(res.NewCustomerToken))
        {
            return res.NewCustomerToken;
        }
        return null;
    }

    private string ValidateUser(string ssoToken, ref string email, ref string userName)
    {
        bool bCustomerExists = false;
        string timssIdentifier = "";

        TIMSSCustomerIdentifierGetResult res = wsSSO.TIMSSCustomerIdentifierGet(PersonifySSOVendorName, PersonifySSOVendorPassword, ssoToken);

        if (!string.IsNullOrEmpty(res.CustomerIdentifier))
        {
            timssIdentifier = res.CustomerIdentifier;

            SSOCustomerGetResult customerResult = wsSSO.SSOCustomerGet(PersonifySSOVendorName, PersonifySSOVendorPassword, timssIdentifier);

            if (customerResult.Errors == null)
            {
                bCustomerExists = customerResult.UserExists;
                email = customerResult.Email;
                userName = customerResult.UserName;


            }
        }

        if (bCustomerExists)
        {
            return timssIdentifier;
        }
        return null;
    }


    #endregion
    private
        void CheckForSSOToken()
    {

        var urlPath = string.Empty;
        //var redirectUrl = ContentController.GetEktronFriendlyUrl(this.PageId, out urlPath);

        //lnkLogout.NavigateUrl = urlPath + "?action=logout&returnurl=" + Server.UrlEncode(redirectUrl);
        //lnkLogout.Text = string.Format("Logout {0}", ud.Username);

        //lnkLogin.Visible = false;
        //lnkLogout.Visible = true;

    }

    public string GetSSOVendorToken(string URLPath)
    {
        //  c.VendorTokenEncrypt()
        //try
        //{


        //}
        //catch (Exception ex)
        //{
        //    Response.Write(ex.ToString());
        //    return null;
        //}
        VendorTokenEncryptResult res = wsSSO.VendorTokenEncrypt(PersonifySSOVendorName, PersonifySSOVendorPassword, PersonifySSOVendorBlock, Request.Url.AbsoluteUri);

        if (!string.IsNullOrEmpty(res.VendorToken))
        {
            return res.VendorToken;
        }
        else
        {
            return null;
        }

        // return "05c8df954bc55c87bbfb86691083c8ba";


    }
    private string GetLoginUrlWithReturnUrl()
    {
        string returnUrl = ConstructURLWithExistingQueryString(PersonifyLoginURL, Request.QueryString, QueryStringParameterToPreserve);
        string loginUrlWithReturnUrl = AddReturnUrlToUrl(PersonifyLoginURL, ReturnUrlRateParameter, returnUrl);
        return loginUrlWithReturnUrl;
    }

    //public static string ConstructURLWithExistingQueryString(string targetUrl, NameValueCollection queryString)
    //{
    //    return ConstructURLWithExistingQueryString(targetUrl, queryString, queryString.AllKeys);
    //}

    public static string ConstructURLWithExistingQueryString(string targetUrl, NameValueCollection queryString, string queryStringParametersToPreserve)
    {
        string[] keys = new string[0];
        if (queryStringParametersToPreserve != null)
        {
            keys = queryStringParametersToPreserve.Split(',');
        }
        return string.Format(targetUrl, queryString, keys);
    }
    public static string ConstructURLWithExistingQueryString(string targetUrl, NameValueCollection queryString)
    {
        return string.Format(targetUrl, queryString, queryString.AllKeys);
    }
    public static string AddReturnUrlToUrl(string targetUrl, string returnUrlParater, string returnUrl)
    {
        return AddParameterToUrlIfNotThere(targetUrl, returnUrlParater, HttpUtility.UrlEncode(returnUrl));
    }

    public static string AddFullPathIfNeeded(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }
        if (url.ToLower().StartsWith("http"))
        {
            return url;
        }
        if (url.StartsWith("\\") || url.StartsWith("/"))
        {
            return FullyQualifiedApplicationPath + url;
        }
        return FullyQualifiedApplicationPath + "/" + url;
    }

    public static string AddParameterToUrlIfNotThere(string url, string queryStringParameterName, string queryStringParameterValue)
    {
        if (string.IsNullOrEmpty(url))
        {
            return url;
        }

        bool alreadyHasParameter = url.Contains("?" + queryStringParameterName + "=") || url.Contains("&" + queryStringParameterName + "=");
        if (alreadyHasParameter)
        {
            return url;
        }
        if (!url.Contains("?"))
        {
            return url + "?" + queryStringParameterName + "=" + queryStringParameterValue;
        }
        else if (url.EndsWith("?"))
        {
            return url + queryStringParameterName + "=" + queryStringParameterValue;
        }
        return url + "&" + queryStringParameterName + "=" + queryStringParameterValue;
    }

    public static string FullyQualifiedApplicationPath
    {
        get
        {
            //Return variable declaration
            var appPath = string.Empty;

            //Getting the current context of HTTP request
            var context = HttpContext.Current;

            //Checking the current context content
            if (context != null)
            {
                //Formatting the fully qualified website url/name
                appPath = string.Format("{0}://{1}{2}{3}",
                                        context.Request.Url.Scheme,
                                        context.Request.Url.Host,
                                        context.Request.Url.Port == 80
                                            ? string.Empty
                                            : ":" + context.Request.Url.Port,
                                        context.Request.ApplicationPath);
            }

            if (!appPath.EndsWith("/"))
                appPath += "/";

            return appPath;
        }
    }


    // private string _registrationUrl;

    //// [PersonifyWebInterpreted(Title = BasePersonifyWebAttributeConstants.RegistrationUrl, Description = BasePersonifyWebAttributeConstants.RegistrationUrlDesc)]
    // public string RegistrationUrl
    // {
    //     get { return GetInterpretedPropertyValue(_registrationUrl); }
    //     set { _registrationUrl = value; }
    // }

    //private string _loginUrl;
    ////[PersonifyWebInterpreted(Title = BasePersonifyWebAttributeConstants.LoginUrl, Description = BasePersonifyWebAttributeConstants.LoginUrlDesc)]
    //public string LoginUrl
    //{
    //    get { return GetInterpretedPropertyValue(_loginUrl); }
    //    set { _loginUrl = value; }
    //}

    //protected virtual string GetInterpretedPropertyValue(string key)
    //{
    //    return Manager.GetInterpretedPropertyValue(key);
    //}
    //protected ManagerController Manager
    //{
    //    get
    //    {
    //        PutCurrentControlInformationForLogs();

    //        return _managerController;
    //    }
    //}

    //private void PutCurrentControlInformationForLogs()
    //{
    //    // needed for logs
    //    WebRequestCache.Set("PBUControl", new WeakReference(this, false));
    //    WebRequestCache.Set("PBUControl.Name", this.GetType().Name);
    //    WebRequestCache.Set("PBUControl.ClientId", this.ClientID);
    //}

}