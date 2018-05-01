using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.OnlineMarketing;
using CMS.WorkflowEngine;

// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "ma.process.list", "~/CMSModules/ContactManagement/Pages/Tools/Automation/List.aspx", null)]
[Breadcrumb(1, "ma.process.new")]

[Help("ma_process_new")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Process_New : CMSAutomationPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!CurrentUser.IsGlobalAdministrator)
        {
            PageTitle title = PageTitle;
            title.TitleText = GetString("ma.process.new");
        }

        var url = UIContextHelper.GetElementUrl(ModuleName.ONLINEMARKETING, "EditProcess");
        url = URLHelper.AddParameterToUrl(url, "displayTitle", "false");
        url = URLHelper.AddParameterToUrl(url, "saved", "1");
        url = URLHelper.AddParameterToUrl(url, "objectId", "{%EditedObject.ID%}");
        editElem.Form.RedirectUrlAfterCreate = AddSiteQuery(url, null);

        // Check permissions
        editElem.Form.SecurityCheck.Resource = ModuleName.ONLINEMARKETING;
        editElem.Form.SecurityCheck.Permission = "ManageProcesses";
        editElem.Form.OnBeforeSave += Form_OnBeforeSave;
    }


    void Form_OnBeforeSave(object sender, EventArgs e)
    {
        editElem.CurrentWorkflow.WorkflowRecurrenceType = ProcessRecurrenceTypeEnum.Recurring;
        editElem.CurrentWorkflow.WorkflowAllowedObjects = ";" + ContactInfo.OBJECT_TYPE + ";";
        editElem.CurrentWorkflow.WorkflowType = WorkflowTypeEnum.Automation;
    }
}
