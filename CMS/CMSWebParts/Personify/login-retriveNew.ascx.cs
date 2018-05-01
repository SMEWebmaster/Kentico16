using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Xml;
using System.Xml.Linq;

using CMS.DataCom;
using CMS.UIControls;
using CMS.TreeEngine;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.CMSHelper;
using CMS.FileManager;
using CMS.WorkflowEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using System.Data;
using System.Data.SqlClient;

using CMS.PortalControls;
using CMS.PortalEngine;
using System.Security.Cryptography;
using System.Text;
using CMS.SettingsProvider;
using CMS.Helpers;
using System.Configuration;
using System.Net;
using System.IO;
using SSO;
using Personify.WebControls.Base.Providers;
using IMS;
using personifyDataservice;


public partial class UserInfoDataforKentico : CMSAbstractWebPart
{
    #region "Global Variables"

    DataSet ds = new DataSet();
    DataSet groupDs = new DataSet();
    string ID, FirstName, LastName, Email, web_login, IsDisabled, MemberType = "", lastpage;

    string DOMAIN = ".networkats.com";
    string RolePrefix = "ats_";
    string tokenid = "x";


    LoginUsertokentico objKenticoService = new LoginUsertokentico();
    private const string PersonifySessionKey = "PersonifyToken";

    private readonly string _personifyImsUrl = ConfigurationManager.AppSettings["IMSWebReferenceURL"];
    private readonly string _personifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
    private readonly string _personifySsoVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
    private readonly string _personifySsoVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
    private readonly string _personifySsoVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
    private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
    private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
    private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];
	private readonly string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
	private readonly string PersonifyVendorID = ConfigurationManager.AppSettings["PersonifySSO_VendorID"];

    #endregion

    private service _wsSso = new service();



    protected void Page_Load(object sender, EventArgs e)
    {
		if (!Page.IsPostBack && Convert.ToInt32(Session["retryAuth"]) != Convert.ToInt32(ConfigurationManager.AppSettings["PersonifyRetryCount"]))
        {
            _wsSso = new service { Url = _personifySsoUrl };

            if (Request.QueryString["action"] == "logout")
            {
                string returnUrl;

                if (Request.QueryString["returnUrl"] != null && !string.IsNullOrEmpty(Request.QueryString["returnUrl"]))
                {
                    returnUrl = Request.QueryString["returnUrl"];
                }
                else
                {
                    returnUrl = Request.ServerVariables["PATH_INFO"];
                }

                if (_wsSso != null)
                {
                    _wsSso.Dispose();
                }


                if (returnUrl.ToLower().Trim().Contains("?logging_out") == false) returnUrl = returnUrl + "?logging_out=true";

                Response.Redirect(returnUrl, true);
            }
            else
            {
                if (!(CMS.Membership.AuthenticationHelper.IsAuthenticated()))
				{
					try
					{
						CheckSsoToken();
					}
					catch (Exception execp)
					{
						EventLogProvider.LogException(execp.Source, execp.StackTrace, execp);
					}
                }
                else
                {
                    bool flagpersonifyMember = false;

                    if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
                    {
                        UserInfo userdata =
                            CMS.Membership.UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

                        DataTable dt = UserInfoProvider.GetUserRoles(userdata);

                        if (dt.Rows.Count > 0 && dt != null)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["RoleName"].ToString().ToLower().Contains("peronifyuser"))
                                {
                                    flagpersonifyMember = true;
                                }

                            }
                        }
                    }

                    if (Session[PersonifySessionKey] == null && flagpersonifyMember)
                    {
                        objKenticoService.Logout(MembershipContext.AuthenticatedUser.UserName);
                    }
                }
            }
        }
    }

    //This method uses the msterCustomerID (remoteID) and looks up the coresponding UserID value in the database for this user.
    //This is done because looking up user by UserName will not work when the username is updated in AMS (Personify).  
    //This method returns the UserId in Ektron based on RemoteID/CustomerID from Personify.

    private string AuthenticateCustomer(string customerIdentifier, string email, string userName)
    {
        string sMasterCustomerId = "";
        string pfirstname = "";
        string plastname = "";
        int subCustomerId = 0;
        string groupslist = "";
        try
        {
            if(customerIdentifier.Contains('|'))
			{
		    var aIdentifiers = customerIdentifier.Split('|');
            sMasterCustomerId = aIdentifiers[0];
            subCustomerId = int.Parse(aIdentifiers[1]);
			}
			else
			{
			sMasterCustomerId=customerIdentifier;
			}

			//*******Start Custom Dataservice code to get Firstname, Lastname***********//
            Uri ServiceUri = new Uri(svcUri_Base);
            LoginUsertokentico.WriteError("AuthenticateCustomer ID ==>", sMasterCustomerId.ToString());
            PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
			DataAccessLayer.IgnoreMissingProperties=true;
            DataAccessLayer.Credentials = new NetworkCredential(svcLogin, svcPassword);
            
            var userdetails =
                DataAccessLayer.CusNameDemographics.Where(p => p.MasterCustomerId == sMasterCustomerId)
                    .Select(o => o)
                    .ToList()
                    .FirstOrDefault();
           
            pfirstname = null;
            plastname = null;
            if (userdetails == null)
            {
                pfirstname = @"&nbsp;";
                plastname = @"&nbsp;";
            }
            if (userdetails != null && string.IsNullOrWhiteSpace(userdetails.FirstName))
            {
                pfirstname = @"&nbsp;";
            }
            else
            {
                if (userdetails != null) pfirstname = userdetails.FirstName;
            }
            if (userdetails != null && string.IsNullOrWhiteSpace(userdetails.LastName))
            {
                plastname = @"&nbsp;";

            }
            else
            {
                if (userdetails != null) plastname = userdetails.LastName;
            }
            //*******End Custom Dataservice code to get Firstname, Lastname***********//

            string[] memberGroups = GetImsroles(sMasterCustomerId, subCustomerId);

            if (memberGroups.Length > 0)
            {
                foreach (string s in memberGroups)
                {
                    if (s.Length > 0)
                    {
                        groupslist += s + ",";
                    }
                }
            }

            groupslist += "peronifyUser" + ",";



            string login = objKenticoService.CreateUpdateLoginUserinKentico(
                userName,
                pfirstname,
                plastname,
                email,
                groupslist,
                true,
                false);
            userinfo uInfo = new userinfo
            {
                ID = sMasterCustomerId,
                Token = Session["PersonifyToken"].ToString(),
                email = email,
                firstname = pfirstname,
                lastname = plastname,
                username = userName,
                groupNames = groupslist
            };


            Session["userClass"] = uInfo;
            return login;
        }
        catch (Exception exception)
        {
            EventLogProvider.LogException("AuthenticateCustomer", "Get", exception);
            LoginUsertokentico.WriteError("AuthenticateCustomer", exception.ToString());
            return null;
        }


    }


    private void CheckSsoToken()
    {
        bool loggedin = objKenticoService.CheckLoginUser("");
        if (!loggedin)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ct"]))
            {
                var customerToken = Request.QueryString["ct"];
                Session["cuctomerReturnToken"] = customerToken;
                var decryptedToken = DecryptCustomerToken(customerToken);
                Session["OpenLoginBox"] = true;

                var finalToken = "";
                if (decryptedToken != "")
                {
                    finalToken = ValidateCustomerToken(decryptedToken);
                }
				// in case of corrupted decrypted token, until success, retry authentication x times as specified in web config to get new customer token
				if (finalToken == null && Convert.ToInt32(Session["retryAuth"]) < Convert.ToInt32(ConfigurationManager.AppSettings["PersonifyRetryCount"]))
				{
					EventLogProvider.LogInformation("Personify corrupt decryption string", "Personify", "customer token: " + customerToken + "decrypted token: " + decryptedToken);
					RetryAuth();
				}
				Session.Remove("uname");
				Session.Remove("pwd");
                var customerIdentifier = "";
                string emailaddress = null;
                string userName = null;

                if (finalToken != "")
                {
                    customerIdentifier = ValidateUser(finalToken, ref emailaddress, ref userName);
                    Session["PersonifyToken"] = finalToken;
                    Session["redirecttomember"] = true;
					Session["retryAuth"] = Convert.ToInt32(ConfigurationManager.AppSettings["PersonifyRetryCount"]);
                    LoginUsertokentico.WriteError("token for " + userName, finalToken);
                }


                var eUserData = AuthenticateCustomer(customerIdentifier, emailaddress, userName);

                if (eUserData == null) return;

                //use master customer id as the default password
                if (customerIdentifier != null)
                {
                    var defaultPassword = customerIdentifier.Split('|')[0];
                    if (Session["PersonifyToken"] == null && !string.IsNullOrEmpty(finalToken))
                    {
                        Session["PersonifyToken"] = finalToken;
                    }
                }

                if (_wsSso != null)
                {
                    _wsSso.Dispose();
                }

                var hasToken = Request.Url.AbsoluteUri.IndexOf("ct=", StringComparison.Ordinal) > 0;
                Session["redirecttomember"] = true;

                Response.Redirect(
                    hasToken
                        ? Request.Url.AbsoluteUri.Substring(0,
                            Request.Url.AbsoluteUri.IndexOf("ct=", StringComparison.Ordinal) - 1)
                        : Request.Url.AbsoluteUri, true);
            }
            else
            {
                if (Session["LoginAttempted"] != null)
                {
                    bool loginAttempted;
                    bool.TryParse(Session["LoginAttempted"].ToString(), out loginAttempted);
                    if (loginAttempted)
                    {
                        Session["OpenLoginBox"] = true;
                        Session.Remove("LoginAttempted");
                    }
                }

            }
        }
        else
        {
            bool flagpersonifyMember = false;
            //if logged in as admin user  keep the userlogged in 
            /// If personify token is expired logout user 
            /// 
            if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
            {
                UserInfo userdata =
                  CMS.Membership.UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserName);

                DataTable dt = UserInfoProvider.GetUserRoles(userdata);

                if (dt.Rows.Count > 0 && dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["RoleName"].ToString().ToLower().Contains("peronifyuser"))
                        {
                            flagpersonifyMember = true;
                        }

                    }
                }

                ///if personify key is null or empty 
                /// 
                if (flagpersonifyMember)
                {
                    string tokenReturn = null;
                    if (Session[PersonifySessionKey] != null)
                    {
                        tokenReturn = this.ValidateCustomerToken(Session[PersonifySessionKey].ToString());
                    }

                    if ((Session[PersonifySessionKey] == null)|| tokenReturn == null)
                    {
                       /* Response.Redirect(
                        Request.ServerVariables["PATH_INFO"] + "?action=logout&returnurl=" +
                        Server.UrlEncode(Request.Url.AbsoluteUri), true);*/

                        //logout user 
                        if (Session["userClass"] != null && Session["userClass"] != "")
                        {
                             userinfo ui = (userinfo)Session["userClass"];
                             objKenticoService.Logout(ui.username);
                             System.Web.Security.FormsAuthentication.SignOut();
                              HttpContext.Current.Response.Cookies["ASPXFORMSAUTH"].Expires = DateTime.Now.AddYears(-1);

                             
                             Response.Redirect("/logout.aspx");
                        }
                    }
                }
            }
        }
    }

    private string DecryptCustomerToken(string customerToken)
    {
        var res = _wsSso.CustomerTokenDecrypt(_personifySsoVendorName, _personifySsoVendorPassword,
            _personifySsoVendorBlock, customerToken);
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
        var res = _wsSso.SSOCustomerTokenIsValid(_personifySsoVendorName, _personifySsoVendorPassword, customerToken);
        if (res.Valid && !string.IsNullOrEmpty(res.NewCustomerToken))
        {
            return res.NewCustomerToken;
        }
        return null;
    }

    private string ValidateUser(string ssoToken, ref string email, ref string userName)
    {
        var bCustomerExists = false;
        var timssIdentifier = "";

        var res = _wsSso.TIMSSCustomerIdentifierGet(_personifySsoVendorName, _personifySsoVendorPassword, ssoToken);

        if (!string.IsNullOrEmpty(res.CustomerIdentifier))
        {
            timssIdentifier = res.CustomerIdentifier;

            var customerResult = _wsSso.SSOCustomerGet(_personifySsoVendorName, _personifySsoVendorPassword,
                timssIdentifier);

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


    private string[] GetImsroles(string masterCustomerId, int subCustomerId)
    {
        var timssId = masterCustomerId + "|" + subCustomerId;
        var roleList = new List<string>();

        using (var myim = new IMService { Url = _personifyImsUrl })
        {
            IMSCustomerRoleGetResult imsCustomerRoleGetResult = myim.IMSCustomerRoleGetByTimssCustomerId(_personifySsoVendorName, _personifySsoVendorPassword, timssId);

            if (imsCustomerRoleGetResult.CustomerRoles == null)
                return new[] { string.Empty };

            foreach (var roledetail in imsCustomerRoleGetResult.CustomerRoles)
            {

                roleList.Add(roledetail.Value);

            }

            return roleList.ToArray();
        }

    }

	/// <summary>
	/// Resend the authentication call to the SSO service
	/// </summary>
	private void RetryAuth()
	{
		var uName = Convert.ToString(Session["uname"]);
		var pwd = Convert.ToString(Session["pwd"]);
		var vendorToken = RijndaelAlgorithm.GetVendorToken(Request.Url.AbsoluteUri.Split('&')[0], _personifySsoVendorPassword, _personifySsoVendorBlock, uName, pwd, true);
		var url = string.Format("{0}?vi={1}&vt={2}", PersonifyAutoLoginUrl, PersonifyVendorID, vendorToken);
		Response.Redirect(url);
	}
}
