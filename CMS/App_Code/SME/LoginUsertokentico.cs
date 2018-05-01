#region"namespaces"
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

using CMS.Helpers;

#region "Kentico Namespaces"
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.CMSHelper;
using CMS.Community;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.TreeEngine;
using CMS.WorkflowEngine;
using CMS.SettingsProvider;
using TreeNode = CMS.DocumentEngine.TreeNode;

using CMS.Membership;
using CMS.EventLog;

#endregion
#endregion

/// <summary>
/// Summary description for LoginUsertokentico
/// </summary>
//[WebService(Namespace = "http://tempuri.org/")]
//[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class LoginUsertokentico //: System.Web.Services.WebService 
{

    DataSet ds = new DataSet();
    private string codeName = "";
    private int mSiteId = 0;

    public Boolean CheckLoginUser(string username)
    {
        Boolean flag = false;
        try
        {
            if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
            // if (CMS.CMSHelper.CMSContext.CurrentUser.UserName.ToString() == username)
            //  if (CMS.Membership.MFAuthenticationHelper..CurrentUser.UserName.ToString() == username)
            {

                //  HttpContext.Current.Session.Remove(AuthenticatedUser.UserName.ToString());
                flag = true;
            }
            else
            {
                flag = false;

            }
           
           // flag = false;

        }
        catch (Exception ex)
        {
            flag = false;
            
             EventLogProvider.LogException("CheckLoginUser", "Get", ex);
        }

        return flag;
    }


    public string Logout(string UserName)
    {

        string retTxt = "0 | error";
        try
        {
            if (CMS.Membership.AuthenticationHelper.IsAuthenticated())
            {
                // if (CMS.CMSHelper.CMSContext.CurrentUser.UserName.ToString() == UserName)
                {

                    HttpContext.Current.Session.Clear();
                    //  CMSContext.CurrentUser = null;
                    HttpContext.Current.Response.Cache.SetNoStore();
                    HttpContext.Current.Response.Cookies[".ASPXFORMSAUTH"].Expires = DateTime.Now.AddYears(-1);
                    retTxt = "0 | " + UserName + "";
                }

            }
            else { HttpContext.Current.Response.Cookies[".ASPXFORMSAUTH"].Expires = DateTime.Now.AddYears(-1); }

        }
        catch (Exception ex)
        {
            WriteError(ex.ToString(), "logoutUser");
        }

        finally
        {
            // setCookieValue("ASPXFORMSAUTH", "");
        }
        return retTxt;
    }


    public string CreateUpdateLoginUserinKentico(string username, string firstname, string lastname, string email, string groupNames, bool memberflag, bool orgflag, string customerIdentifier, string customerToken)
    {
        string UserCreated = "";
        try
        {
            UserInfo userUpdate = CMS.Membership.UserInfoProvider.GetUserInfo(username);

            if (!(userUpdate != null))
            {
                UserInfo user = new UserInfo();
                user.UserName = username;
                user.FirstName = firstname;
                user.LastName = lastname;
                user.FullName = firstname + " " + lastname;
                user.Email = email;
                user.SetPersonifyIdentifier(customerIdentifier);
                user.SetLastCustomerToken(customerToken);
                // user.IsEditor = false;

                user.PreferredCultureCode = "en-us";
                user.PasswordFormat = "SHA1";
                user.Enabled = true;
                //user.IsExternal = true;
                CMS.Membership.UserInfoProvider.SetUserInfo(user);
                CMS.Membership.UserInfoProvider.SetPassword(username, username);
                UserInfoProvider.AddUserToSite(username, CMS.SiteProvider.SiteContext.CurrentSiteName);
                UserCreated = "UserName: " + username + "<br/> " + "Password: " + username + ": ";
            }
            else
            {
                //UpdateUserinKentico(userUpdate); 
                //string shaPass = CalculateSHA1("", Encoding.ASCII);
                //string kenticoPass = userUpdate.GetValue("UserPassword").ToString();
                // if (kenticoPass != shaPass)
                // {
                IDataClass userObj = DataClassFactory.NewDataClass("cms.user", userUpdate.UserID);
                string userName = (string)userObj.GetValue("username");
                userObj.SetValue("UserName", userUpdate.UserName.ToString());
                // userObj.SetValue("UserPassword", GenerateSHA1(username.ToString()));
                userObj.SetValue("FirstName", firstname);
                userObj.SetValue("LastName", lastname);
                userObj.SetValue("FullName", firstname + " " + lastname);
                userObj.SetValue("Email", email);
                userObj.SetValue("PersonifyIdentifier", customerIdentifier);
                userObj.SetValue("LastCustomerToken", customerToken);
                userObj.Update();
                UserCreated = "UserName: " + username + "<br/> " + "Password: " + username + ": ";
                //}


            }
            UserInfo userNew = CMS.Membership.UserInfoProvider.GetUserInfo(username);
            // groups(userNew, groupNames);

            ///add user to roles for protection purposes 
            AddUserToRole(userNew, groupNames, memberflag, orgflag);

            //login User too 
            UserCreated = LoginUserinKentico(userNew.UserName);
            return UserCreated;

        }

        catch (Exception ex)
        {
            //delete user if created 
              EventLogProvider.LogException("CreateUpdateUserinKentico", "Get", ex);
            return null;
        }

    }
    #region  "Roles API"



    public void AddUserToRole(UserInfo user, string roleName, bool flag, bool orgflag)
    {
        try
        {
            DeleteUserRole(user);
            // Get role and user objects
            roleName = roleName.TrimEnd() + ",";
            string[] grps = roleName.Split(',');

            if (grps.Length > 0)
            {
                for (int i = 0; i <= grps.Length - 1; i++)
                {
                    if (grps[i].ToString() != "" && grps[i].ToString() != null)
                    {
                        string appendedRoleName = "sso_" + grps[i].ToString().ToLower();
                        RoleInfo role = RoleInfoProvider.GetRoleInfo(appendedRoleName, CMS.SiteProvider.SiteContext.CurrentSiteID);
                        // UserInfo user = UserInfoProvider.GetUserInfo("test");

                        if ((role != null) && (user != null))
                        {
                            // Create new user role object
                            UserRoleInfo userRole = new UserRoleInfo();

                            // Set the properties
                            userRole.UserID = user.UserID;
                            userRole.RoleID = role.RoleID;

                            // Save the user role
                            UserRoleInfoProvider.SetUserRoleInfo(userRole);
                            CMS.Membership.UserInfoProvider.AddUserToRole(user.UserName, appendedRoleName.ToLower(), CMS.SiteProvider.SiteContext.CurrentSiteName);
                            //return true;
                        }
                        else
                        {
                            // create role and add user to it 
                            CreateUpdateRole(appendedRoleName);
                            CMS.Membership.UserInfoProvider.AddUserToRole(user.UserName, appendedRoleName, CMS.SiteProvider.SiteContext.CurrentSiteName);

                        }
                    }

                }
            }
          /*  if (flag)//member-role
            {
                MemberNonMemberRoles("sso_member", user);
            }
            else //non-Memberrole
            { MemberNonMemberRoles("sso_nonmember", user); }

            if (orgflag)//Organisation-role
            {
                MemberNonMemberRoles("sso_organisation", user);
            }
            else //non Organisation-Memberrole
            { MemberNonMemberRoles("sso_individual", user); }*/

        }
        catch (Exception ex)
        {
            
             EventLogProvider.LogException("Issue while AddUserToRole Roles", "Get", ex);
        }
    }

    private void MemberNonMemberRoles(string roletype, UserInfo user)
    {
        RoleInfo role = RoleInfoProvider.GetRoleInfo(roletype, CMS.SiteProvider.SiteContext.CurrentSiteID);
        // UserInfo user = UserInfoProvider.GetUserInfo("test");

        if ((role != null))
        {
            // Create new user role object
            UserRoleInfo userRole = new UserRoleInfo();

            // Set the properties
            userRole.UserID = user.UserID;
            userRole.RoleID = role.RoleID;

            // Save the user role
            UserRoleInfoProvider.SetUserRoleInfo(userRole);
            CMS.Membership.UserInfoProvider.AddUserToRole(user.UserName, roletype, CMS.SiteProvider.SiteContext.CurrentSiteName);
            //return true;
        }
        else
        {
            // create role and add user to it 
            CreateUpdateRole(roletype);
            CMS.Membership.UserInfoProvider.AddUserToRole(user.UserName, roletype, CMS.SiteProvider.SiteContext.CurrentSiteName);

        }
    }

    public void DeleteUserRole(UserInfo user)
    {
        try
        {
            // Get role and user objects
            // RoleInfo role = RoleInfoProvider.GetRoleInfo(roleName.ToString(), CMS.SiteProvider.SiteContext.CurrentSiteID);
            //  UserInfo user = UserInfoProvider.GetUserInfo("MyNewUser");
            DataTable dt = UserInfoProvider.GetUserRoles(user);

            if (dt.Rows.Count > 0 && dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["RoleName"].ToString().ToLower().Contains("sso_"))
                    {
                        UserRoleInfo deleteRole = UserRoleInfoProvider.GetUserRoleInfo(user.UserID, Convert.ToInt32(dr["RoleID"]));
                        // Delete the user role
                        UserRoleInfoProvider.DeleteUserRoleInfo(deleteRole);

                    }

                }
            }
        }
        catch (Exception ex)
        {
            
            EventLogProvider.LogException("Issue while DeleteUserRole", "Get", ex);
        }

    }


    public void CreateUpdateRole(string roleName)
    {
        try
        {
            // Create new role object
            RoleInfo newRole = new RoleInfo();
            RoleInfo role = RoleInfoProvider.GetRoleInfo(roleName, CMS.SiteProvider.SiteContext.CurrentSiteID);
            if (role == null)
            {
                //create role 
                newRole.DisplayName = roleName.ToString().ToLower();
               roleName= roleName.ToString().ToLower();
               /*roleName = roleName.Replace("_", "");
               roleName = roleName.Replace(".", "");
               roleName = roleName.Replace(",", "");
               roleName = roleName.Replace(">", "");*/
                WriteError(roleName, "Role for User loggedin ");
                newRole.RoleName = roleName;
                newRole.SiteID = CMS.SiteProvider.SiteContext.CurrentSiteID;
                // Save the role
                RoleInfoProvider.SetRoleInfo(newRole);

            }
            else
            {
                // Set the properties
                newRole.DisplayName = roleName.ToString().ToLower();
                newRole.SiteID = CMS.SiteProvider.SiteContext.CurrentSiteID;
                // Save the role
                RoleInfoProvider.SetRoleInfo(newRole);
            }
        }
        catch (Exception ex)
        {
           
            EventLogProvider.LogException("Issue while Update/Creating Roles", "Get", ex);
        }
    }

    public bool DeleteRole()
    {
        //// Get the role
        //RoleInfo deleteRole = RoleInfoProvider.GetRoleInfo("MyNewRole", CMS.SiteProvider.SiteContext.CurrentSiteID);

        //// Delete the role
        //RoleInfoProvider.DeleteRoleInfo(deleteRole);

        //return (deleteRole != null);
        return false;
    }
    #endregion

    public string LoginUserinKentico(string username)
    {
        string flag = "false";
        try
        {
            UserInfo userAuth = CMS.Membership.UserInfoProvider.GetUserInfo(username);
            if (userAuth != null)
            {
                if (Membership.ValidateUser(username, username))
                {
                    FormsAuthentication.SetAuthCookie(userAuth.UserName, true);

                    CMS.Helpers.RequestStockHelper.Remove(userAuth.UserName);
                    CMS.Helpers.RequestStockHelper.Add(userAuth.UserName, new CurrentUserInfo(userAuth, false));
                    CMS.Membership.AuthenticationHelper.SetCurrentUser(new CurrentUserInfo(userAuth, false));
                    CMS.Membership.AuthenticationHelper.AuthenticateUser(userAuth.UserName, true);
                    // Set view mode to live site after login to prevent bar with "Close preview mode"
                    // CMSContext.ViewMode = CMS.PortalEngine.ViewModeEnum.LiveSite;

                    // Ensure response cookie
                    CookieHelper.EnsureResponseCookie(FormsAuthentication.FormsCookieName);
                    CookieHelper.AllowCookies = true;
                    //add cookie to the domain
                    var Cookie = FormsAuthentication.GetAuthCookie(userAuth.UserName, false);
                    //Cookie.Domain = "kentico.networkats.com";//CMSContext.CurrentSiteName;
                    Cookie.Expires = DateTime.Now.AddDays(1);
                    Cookie.HttpOnly = true;
                    HttpContext.Current.Response.Cookies.Add(Cookie);

                    flag = Cookie.Value.ToString();

                }

            }
        }
        catch (Exception ex)
        {
            
             EventLogProvider.LogException("LoginUserinKentico", "Get", ex);

        }
        return flag;
    }

    public Boolean loginUser(string UserName)
    {
        Boolean logedin = false;
        string Uname = UserName;
        UserInfo ui = UserInfoProvider.GetUserInfoForSitePrefix(Uname, CMS.SiteProvider.SiteContext.CurrentSite);
        if (RequestHelper.IsMixedAuthentication() && UserInfoProvider.UseSafeUserName)
        {
            if (ui == null)
            {
                Uname = ValidationHelper.GetSafeUserName(UserName, CMS.SiteProvider.SiteContext.CurrentSiteName);

                ui = UserInfoProvider.GetUserInfoForSitePrefix(Uname, CMS.SiteProvider.SiteContext.CurrentSite);
                if (ui != null)
                {
                    // Authenticate user by site or global safe username
                    CMS.Membership.AuthenticationHelper.AuthenticateUser(ui.UserName, false);
                }
            }
        }
        if (ui != null)
        {
            // If user name is site prefixed, authenticate user manually 
            if (UserInfoProvider.IsSitePrefixedUser(ui.UserName))
            {
                CMS.Membership.AuthenticationHelper.AuthenticateUser(ui.UserName, false);
                logedin = true;
            }

            // Log activy
            string siteName = CMS.SiteProvider.SiteContext.CurrentSiteName;
            // if ((CMSContext.ViewMode == CMS.PortalEngine.ViewModeEnum.LiveSite) && CMS.WebAnalytics.ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName) && CMS.WebAnalytics.ActivitySettingsHelper.UserLoginEnabled(siteName))
            /*         if ( CMS.WebAnalytics.ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName) && CMS.WebAnalytics.ActivitySettingsHelper.UserLoginEnabled(siteName))
                 {
                     int contactId = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
             
                     CMS.WebAnalytics.ActivityLogHelper.UpdateContactLastLogon(contactId);
                     if (CMS.WebAnalytics.ActivitySettingsHelper.ActivitiesEnabledForThisUser(ui))
                     {
                         TreeNode currentDoc = CMS.DocumentEngine.DocumentContext.CurrentDocument;

                    
                                 
                         CMS.WebAnalytics.ActivityLogProvider.LogLoginActivity(contactId, ui, CMS.Helpers.RequestContext.CurrentRelativePath,
                             (currentDoc != null ? currentDoc.NodeID : 0), CMS.SiteProvider.SiteContext.CurrentSiteName, CMS.WebAnalytics.AnalyticsHelper.Campaign, (currentDoc != null ? currentDoc.DocumentCulture : null));
                     }
                 }*/
        }

        return logedin;
    }

    # region "calculate/get shea password encryption Kentico"
    /// <summary>
    /// Calculates SHA1 hash
    /// </summary>
    /// <param name="text">input string</param>
    /// <param name="enc">Character encoding</param>
    /// <returns>SHA1 hash</returns>
    public static string CalculateSHA1(string text, Encoding enc)
    {
        byte[] buffer = enc.GetBytes(text);
        SHA1CryptoServiceProvider cryptoTransformSHA1 =
        new SHA1CryptoServiceProvider();
        string hash = BitConverter.ToString(
            cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

        return hash;
    }

    public static string GenerateSHA1(string text)
    {

        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(text, "SHA1");

        return hash;
    }
    #endregion

    #region "logerrors"
    public static void WriteError(string errorMessage, string methodName)
    {
        try
        {
            string path = "~/Error/" + DateTime.Today.ToString("dd-mm-yy") + ".txt";
            if ((!File.Exists(System.Web.HttpContext.Current.Server.MapPath(path))))
            {
                File.Create(System.Web.HttpContext.Current.Server.MapPath(path)).Close();
            }
            using (StreamWriter w = File.AppendText(System.Web.HttpContext.Current.Server.MapPath(path)))
            {
                w.WriteLine("***********************************************************************");
                // w.WriteLine(Constants.vbCrLf + "Log Entry : ");
                w.WriteLine("{0}", DateTime.Now.ToString());
                string err = "Error in: " + System.Web.HttpContext.Current.Request.Url.ToString() + ". Error Message:" + errorMessage;
                w.WriteLine(err);
                w.WriteLine("__________________________");
                w.WriteLine(methodName);
                w.WriteLine("__________________________");
                w.WriteLine("***********************************************************************");

                w.Flush();
                w.Close();
            }
        }
        catch (Exception ex)
        {
           // WriteError(ex.Message, "When updating ErrorMessage");
             EventLogProvider.LogException("When updating ErrorMessage", "Get", ex);
        }

    }

    #endregion

}
