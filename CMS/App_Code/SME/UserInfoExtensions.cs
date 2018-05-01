using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS;
using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Membership;

/// <summary>
/// Summary description for CustomUserInfo
/// </summary>
public static class UserInfoExtensions
{
    public static string GetPersonifyIdentifier(this UserInfo userInfo)
    {
        return userInfo.GetStringValue("PersonifyIdentifier", String.Empty);
    }

    public static void SetPersonifyIdentifier(this UserInfo userInfo, string personifyIdentifier)
    {
        userInfo.SetValue("PersonifyIdentifier", personifyIdentifier);
    }

    public static string GetLastCustomerToken(this UserInfo userInfo)
    {
        return userInfo.GetStringValue("LastCustomerToken", String.Empty);
    }

    public static void SetLastCustomerToken(this UserInfo userInfo, string lastCustomerToken)
    {
        userInfo.SetValue("LastCustomerToken", lastCustomerToken);
    }

}