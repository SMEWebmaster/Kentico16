using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMS.Base;
using CMS.UIControls;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Localization;

[CustomUniGridTransformations]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Attribute class that ensures the loading of custom UniGrid transformations.
    /// </summary>
    private class CustomUniGridTransformationsAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// The system executes the Init method of the CMSModuleLoader attributes when the application starts.
        /// </summary>
        public override void Init()
        {
            // Registers the #html UniGrid transformation
            UniGridTransformations.Global.RegisterTransformation("#html", CustomHtmlText);

            // Registers the #customDateFormat UniGrid transformation
            UniGridTransformations.Global.RegisterTransformation("#customDateFormat", CustomDateFormat);

            UniGridTransformations.Global.RegisterTransformation("#customURL", CustomURL);

            UniGridTransformations.Global.RegisterTransformation("#customEvent", CustomEvent);

        }

        private static object CustomURL(object parameter)
        {
            //return "<a href='" + parameter.ToString().ToLowerCSafe() + "'>" + parameter.ToString().ToLowerCSafe() + "</a>";
            return "<a href='/events/" + ValidationHelper.GetString(parameter, "").ToLower().Replace(' ','-') + "'>" + ValidationHelper.GetString(parameter, "") + "</a>";
        }

        private static object CustomEvent(object parameter)
        {
            return ValidationHelper.GetString(parameter, "").Replace('_', ' ');
        }

        // Method that defines the #html UniGrid transformation
        // Wraps the UniGrid column text value into a <p> element
        private static object CustomHtmlText(object parameter)
        {
            return "<p>" + ValidationHelper.GetString(parameter, "") + "</p>";
        }


        // Method that defines the #customDateFormat UniGrid transformation
        // Converts the date time to MMM-dd-yyyy format
        private static object CustomDateFormat(object parameter)
        {
            //return DateTime.Parse(parameter.ToString()).ToShortDateString();
            return DateTime.Parse(parameter.ToString().Trim()).ToString("dd-MMMM-yyyy");
            
        }
    }
}

///// <summary>
///// Summary description for UniGridTrans
///// </summary>
//public class UniGridTrans
//{
//    public UniGridTrans()
//    {
//        //
//        // TODO: Add constructor logic here
//        //
//    }
//}