using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for KenticoSkin
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class KenticoSkin : System.Web.Services.WebService
{


    [WebMethod]
    public string loadTopNavigation(string url)
    {

        try
        {
            string ret = "";
            var uri = new Uri(url);
            var doc = new HtmlWeb().Load(url);
            var navigation = doc.DocumentNode.SelectNodes("//ul[@class='uti-nav']");
            if (navigation != null)
            {
                var sbNavigation = new StringBuilder(navigation[0].InnerHtml);
                string remoteBase = string.Format("https://{0}", uri.Host);
                sbNavigation.Replace("href=\"/", String.Format("href=\"{0}/", remoteBase));
                sbNavigation.Replace("src=\"/", String.Format("src=\"{0}/", remoteBase));
                sbNavigation.Replace("resultsurl=\"/", String.Format("resultsurl=\"{0}/", remoteBase));
                if (url.Contains("uca"))
                {
                    ret = sbNavigation.ToString();
                    ret = ret.Replace("<li><a href='/uca/login'>Login </a></li>", "");
                }
                else
                {
                    ret = sbNavigation.ToString();
                    ret = ret.Replace("<li><a href='/personifyebusiness/login-join'>Login </a></li>", "");

                }
                return ret;
            }
            else
            {
                LoginUsertokentico.WriteError("Doesnt Load for this-----loadTopNavigation>>>>", "For skinng the Navigation for  : " + url);
                return "";
            }
        }
        catch (Exception ex)
        {
            LoginUsertokentico.WriteError(ex.ToString(), "For skinng the Navigation for loadNavigation : " + url);
            return null;
        }
    }

    [WebMethod]
    public string loadNavigation(string url)
    {

        try
        {
            string ret = "";
            var uri = new Uri(url);
            var doc = new HtmlWeb().Load(url);

           if (url.Contains("uca"))
            {
                var navigation = doc.DocumentNode.SelectNodes("//nav");
                if (navigation != null)
                {
                    var sbNavigation = new StringBuilder(navigation[0].InnerHtml);
                    string remoteBase = string.Format("https://{0}", uri.Host);
                    sbNavigation.Replace("href=\"/", String.Format("href=\"{0}/", remoteBase));
                    sbNavigation.Replace("src=\"/", String.Format("src=\"{0}/", remoteBase));
                    sbNavigation.Replace("resultsurl=\"/", String.Format("resultsurl=\"{0}/", remoteBase));
                   
                    string x = "<div class=\"container\">" + 
                            " <button class=\" btn-responsive-nav btn-inverse\" data-toggle=\"collapse\" data-target=\".nav-main-collapse\">" +
                            " <i class=\"fa fa-bars\">MENU</i></button>" +
                            " </div><div class=\"navbar-collapse nav-main-collapse collapse\">  <nav class=\"nav-main mega-menu\">";

                    ret = x + sbNavigation.ToString() + "</nav> </div>";
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/jquery.easing.js\"></script>";
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/bootstrap.js\"></script>";
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/commom.js\"></script>";
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/theme.js\"></script>";
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/theme.init.js\"></script>"; 
                    ret = ret  + "<script type=\"text/javascript\" src=\"https://www.smenet.org/CMSScripts/Custom/SME/bootstrap.js\"></script>"; 

                  
                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=UCA_Theme\" type=\"text/css\" rel=\"stylesheet\" /> ";
                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=SME-bootstrap\" type=\"text/css\" rel=\"stylesheet\" /> ";
                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=UCA-Styles\" type=\"text/css\" rel=\"stylesheet\" /> ";
                    ret = ret  +  "<link href=\"https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css\" type=\"text/css\" rel=\"stylesheet\" /> ";

                    ret = ret + "<script type=\"text/javascript\">  $(\"#header .container .btn-responsive-nav\").click(function(e) { e.preventDefault();}); </script>"; 
                }
            } 
           else
            { var navigation = doc.DocumentNode.SelectNodes("//nav");
                if (navigation != null)
                {
                    var sbNavigation = new StringBuilder(navigation[0].InnerHtml);
                    string remoteBase = string.Format("https://{0}", uri.Host);
                    sbNavigation.Replace("href=\"/", String.Format("href=\"{0}/", remoteBase));
                    sbNavigation.Replace("src=\"/", String.Format("src=\"{0}/", remoteBase));
                    sbNavigation.Replace("resultsurl=\"/", String.Format("resultsurl=\"{0}/", remoteBase));
                   
                    string x = "<div class=\"container\">" + 
                            " <button class=\" btn-responsive-nav btn-inverse\" data-toggle=\"collapse\" data-target=\".nav-main-collapse\">" +
                            " <i class=\"fa fa-bars\">MENU</i></button>" +
                            " </div><div class=\"navbar-collapse nav-main-collapse collapse\">  <nav class=\"nav-main mega-menu\">";

                    ret = x + sbNavigation.ToString() + "</nav> </div>";
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/jquery.easing.js\"></script>";
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/bootstrap.js\"></script>";
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/commom.js\"></script>";
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/theme.js\"></script>";
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org//CMSScripts/Custom/mob-nav/theme.init.js\"></script>"; 
                    ret = ret  +  "<script type=\"text/javascript\" src=\"https://www.smenet.org/CMSScripts/Custom/SME/bootstrap.js\"></script>"; 

                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=SME-bootstrap\" type=\"text/css\" rel=\"stylesheet\" /> ";
                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=SMETheme\" type=\"text/css\" rel=\"stylesheet\" /> ";
                  
                    ret = ret  +  "<link href=\"https://www.smenet.org/CMSPages/GetResource.ashx?stylesheetname=SME-Styles\" type=\"text/css\" rel=\"stylesheet\" /> ";
                    ret = ret  +  "<link href=\"https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css\" type=\"text/css\" rel=\"stylesheet\" /> ";

                    ret = ret + " <script type=\"text/javascript\">  $(\"#header .container .btn-responsive-nav\").click(function(e) { e.preventDefault();}); </script>"; 
                }

            }
            return ret;
            

        }
        catch (Exception ex)
        {
            LoginUsertokentico.WriteError(ex.ToString(), "For skinng the Navigation for loadNavigation : " + url);
            return null;

        }

    }
    [WebMethod]
    public string loadfooter(string url)
    {

        try
        {
            var uri = new Uri(url);
            var doc = new HtmlWeb().Load(url);
            var footer = doc.DocumentNode.SelectNodes("//footer");
            if (footer != null)
            {
                var sbNavigation = new StringBuilder(footer[0].InnerHtml);
                string remoteBase = string.Format("https://{0}", uri.Host);
                sbNavigation.Replace("href=\"/", String.Format("href=\"{0}/", remoteBase));
                sbNavigation.Replace("src=\"/", String.Format("src=\"{0}/", remoteBase));
                sbNavigation.Replace("resultsurl=\"/", String.Format("resultsurl=\"{0}/", remoteBase));
                return sbNavigation.ToString();
            }
            else
            {
                LoginUsertokentico.WriteError("Doesnt Load for this-----loadfooter>>>>", "For skinng the Navigation for  : " + url);
                return "";
            }
        }
        catch (Exception ex)
        {
            LoginUsertokentico.WriteError(ex.ToString(), "For skinng the Navigation for loadfooter----> : " + url);
            return null;

        }


    }

}
