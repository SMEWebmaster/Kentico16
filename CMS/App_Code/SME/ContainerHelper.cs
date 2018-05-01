using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Personify.WebControls.Base.Attributes;
 
using Personify.WebControls.Base.Providers;
using System.Reflection;
using Personify.WebControls.Base.PersonifyDataServicesBase;
using Telerik.Web.UI;
using System.Web.UI;
 
using System.Web.UI.WebControls;
using System.Web;

namespace SME.PersonifyContainer
{
    using System.Web.UI.HtmlControls;

    public enum ContainerEditorViewModes
	{
		ControlPicker,
		ControlSetting,
		ParameterEditor
	}

	public class ContainerHelper
	{

		/*#region "Constants"
		public string  ControlNameKey = "CONTROL_NAME";
		public string  CustomTitleKey = "CUSTOM_TITLE";
		public string  PersonifyIdentityKey = "PERSONIFY_IDENTITY";
			#endregion
        public string ParameterIdKey = "ParameterId";

		#region "Script,Ajax,Style"
		public static ScriptManager EnsureScriptManager(Control control)
		{
			if ((control == null)) {
				throw new ArgumentException("Control cannot be null", "control");
			}

			ScriptManager scriptManager = RadScriptManager.GetCurrent(control.Page);
			if ((scriptManager == null)) {
				scriptManager = new RadScriptManager();
				scriptManager.ID = typeof(RadScriptManager).Name;
				control.Page.Form.Controls.AddAt(0, scriptManager);
			}

            //if ((ContainerSettings.DefaultSetting.RequireJQuery)) {
            //    ScriptReference jqueryScript = scriptManager.Scripts.FirstOrDefault(s => s.Path.ToLower() == ContainerSettings.DefaultSetting.JQueryScriptPath.ToLower());
            //    if ((jqueryScript == null)) {
            //        jqueryScript = new ScriptReference();
            //        jqueryScript.Path = ContainerSettings.DefaultSetting.JQueryScriptPath;
            //        scriptManager.Scripts.Add(jqueryScript);
            //    }
            //}

			return scriptManager;
		}

		public static RadAjaxManager EnsureRadAjaxManager(Control control)
		{
			if ((control == null)) {
				throw new ArgumentException("Control cannot be null", "control");
			}

			RadAjaxManager ajaxManager = RadAjaxManager.GetCurrent(control.Page);
			if ((ajaxManager == null)) {
				EnsureScriptManager(control);
				ajaxManager = new RadAjaxManager();
				ajaxManager.ID = typeof(RadAjaxManager).Name;

                //Form form = control.Page.Form;
                //if ((form != null)) {
                //    form.Controls.Add(ajaxManager);
                //}
                HtmlForm f = control.Page.Form;
                if ((f != null))
                {
                    f.Controls.Add(ajaxManager);
                }
			}

			return ajaxManager;
		}

		public static RadStyleSheetManager EnsureRadStyleSheetManager(Control control)
		{
			if ((control == null)) {
				throw new ArgumentException("Control cannot be null", "control");
			}

			RadStyleSheetManager styleManager = RadStyleSheetManager.GetCurrent(control.Page);
			if ((styleManager == null)) {
				EnsureScriptManager(control);
				styleManager = new RadStyleSheetManager();
				styleManager.ID = typeof(RadStyleSheetManager).Name;
				control.Page.Controls.Add(styleManager);
			}

			return styleManager;
		}
		#endregion
        */
	}
}
