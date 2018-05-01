using CMS.Membership;
using CMS.SiteProvider;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using CMS.WorkflowEngine;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using CMS.WebDAV;
using CMS.CustomTables;
using System.Configuration;
using CMS.DataEngine;
using CMS.Globalization;
using System.Reflection;
using System.Net;
using System.IO;
namespace CMSAppAppCode.SME
{
    public class Utility
    {
       
        /// <summary>
        /// Check if the user is Admin
        /// </summary>
        /// <returns>status</returns>
        public static bool IsUserInAdminRole()
        {
            // check current user is Admin
            return (MembershipContext.AuthenticatedUser.IsGlobalAdministrator
                    || MembershipContext.AuthenticatedUser.IsInRole(SMEConstant.ROLE_ADMIN, SiteContext.CurrentSiteName));
        }
		
		/// <summary>
        /// Check if the user is Authenticated
        /// </summary>
        /// <returns>status</returns>
        public static bool IsUserAuthenticated()
        {
            // check current user is Authenticated
            return (RequestContext.IsUserAuthenticated);
        }

       
        //After getting dataset of all document filtering data as per required conditions
        public static DataSet UpdateDocumentsDataset(DataSet ds)
        {
            // Display and initialize grid if datasource is not empty
            for (int i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
            {
                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[i]["DocumentWorkflowStepID"].ToString()))
                {
                    int stepId = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentWorkflowStepID"]);
                    var workFlow = string.Empty;
                    if (stepId > 0)
                    {
                        // Get workflow step display name
                        WorkflowStepInfo wsi = WorkflowStepInfoProvider.GetWorkflowStepInfo(stepId);
                        if (wsi != null)
                        {
                            workFlow = ResHelper.LocalizeString(wsi.StepDisplayName);

                        }

                        //if its in draft state then display documents created by current user only
                        if (workFlow == SMEConstant.WORKFLOW_DRAFT)
                        {
                            int userID = MembershipContext.AuthenticatedUser.UserID;
                            int DocumentCreatedByUserID = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentCreatedByUserID"]);
                            int DocumentModifiedByUserID = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentModifiedByUserID"]);
                            string NodeAliasPath = ds.Tables[0].Rows[i]["NodeAliasPath"].ToString();
                            int DocumentID = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentID"]);
                            int DocumentCheckedOutVersionHistoryID = Convert.ToInt32(ds.Tables[0].Rows[i]["DocumentCheckedOutVersionHistoryID"]);
                            //checking that current document is already rejected or not
                            DataSet dsRejected = WorkflowHistoryInfoProvider.GetWorkflowHistories("WasRejected = 1 AND VersionHistoryID = " + DocumentCheckedOutVersionHistoryID + " ", null, 100, null);
                            //checking wether that document is already is published or not. if its already published then anyone can see and edit it
                            //if that document is rejected then it will display to all and it will not go inside if condition
                            bool isPublished = isPublishedStatusBynodeAliasPath(NodeAliasPath);
                            if (!isPublished && DataHelper.DataSourceIsEmpty(dsRejected))
                            {
                                //if a document is modified by me then it should display to me although its not created by me
                                //if that document is not created or not modified then no need to display on my list
                                if (DocumentCreatedByUserID != userID && DocumentModifiedByUserID != userID)
                                {
                                    ds.Tables[0].Rows[i].Delete();
                                }
                            }

                        }

                    }
                }

            }
            ds.Tables[0].AcceptChanges();
            return ds;
        }

        //On_ExternalDatabound Displaying Workflow Status and disabling edit and delete button based on condition
        public static string WorkFlowStatusDisplay(int stepId, string currOrgID, object sender)
        {
            var workFlow = string.Empty;
            if (stepId > 0)
            {
                // Get workflow step display name
                WorkflowStepInfo wsi = WorkflowStepInfoProvider.GetWorkflowStepInfo(stepId);
                if (wsi != null)
                {
                    workFlow = ResHelper.LocalizeString(wsi.StepDisplayName);

                }
                // if workflow is Archive then we have to disable delete button
                if (workFlow == SMEConstant.WORKFLOW_ARCHIVED)
                {
                    System.Web.UI.WebControls.TableCell orgActions = ((System.Web.UI.WebControls.TableCell)sender);
                    System.Web.UI.WebControls.TableRow Actions = ((System.Web.UI.WebControls.TableRow)orgActions.Parent);
                    ((CMSButton)(Actions.Cells[0].FindControl("adelete"))).Enabled = false;
                    //((CMSButton)(Actions.Cells[0].FindControl("aedit"))).Enabled = true;
                }
                // if workflow is "Approval await" then we have to disable edit and delete button for author
                else if (workFlow == SMEConstant.WORKFLOW_APPROVAL)
                {
                    if (IsUserInAdminRole())
                    {
                        System.Web.UI.WebControls.TableCell orgActions = ((System.Web.UI.WebControls.TableCell)sender);
                        System.Web.UI.WebControls.TableRow Actions = ((System.Web.UI.WebControls.TableRow)orgActions.Parent);
                        ((CMSButton)(Actions.Cells[0].FindControl("adelete"))).Enabled = false;
                        ((CMSButton)(Actions.Cells[0].FindControl("aedit"))).Enabled = false;
                    }
                    else
                    {
                        System.Web.UI.WebControls.TableCell orgActions = ((System.Web.UI.WebControls.TableCell)sender);
                        System.Web.UI.WebControls.TableRow Actions = ((System.Web.UI.WebControls.TableRow)orgActions.Parent);
                        ((CMSButton)(Actions.Cells[0].FindControl("adelete"))).Enabled = true;
                        ((CMSButton)(Actions.Cells[0].FindControl("aedit"))).Enabled = true;
                    }
                }
            }
            return workFlow;
        }

        // name of document is a link button if document is published
        public static System.Web.UI.WebControls.HyperLink namelink(object sender)
        {
            var eventlink = new System.Web.UI.WebControls.HyperLink();
            System.Web.UI.WebControls.TableCell orgLinkText = ((System.Web.UI.WebControls.TableCell)sender);
            var eventName = Convert.ToString(orgLinkText.Text);
            System.Web.UI.WebControls.TableRow nodepath = ((System.Web.UI.WebControls.TableRow)orgLinkText.Parent);
            //getting noadAlias path
            string nodeAliasPath = Convert.ToString(nodepath.Cells[1].Text);
            //getting workflow name
            var WorkFlow = WorkFlowStatusBynodeAliasPath(nodeAliasPath);
            //if workflow is published then only link will come on event name
            if (WorkFlow == SMEConstant.WORKFLOW_PUBLISHED)
            {
                eventlink = new System.Web.UI.WebControls.HyperLink()
                {
                    NavigateUrl = nodeAliasPath,
                    Text = eventName.Length >= 80 ? eventName.Substring(0, 80) : eventName,
                    ToolTip = eventName
                };
            }
            return eventlink;
        }
        //Delete the document if that is not published
        public static void DeleteDocument(TreeProvider tree, TreeNode node)
        {
            WorkflowManager workflowManager = WorkflowManager.GetInstance(tree);
            WorkflowInfo workflow = workflowManager.GetNodeWorkflow(node);
            var isPublishedDocument = node.PublishedVersionExists;
            // Check if the document uses workflow
            if (workflow != null)
            {
                if (isPublishedDocument)
                {
                    // Archive the document
                    workflowManager.ArchiveDocument(node, null);
                }
                else
                {
                    //if document is not published then permanetly delete it
                    node.Destroy();
                }

            }
        }
        /// <summary>
        /// Getting WorkFlowStatus by noadAliaspath
        /// </summary>
        /// <param name="data">noadAliaspath</param>
        public static string WorkFlowStatusBynodeAliasPath(string nodeAliasPath)
        {
            // Create an instance of the Tree provider first
            //TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            TreeProvider tree = new TreeProvider();

            // Get the document
            TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, nodeAliasPath, "en-us", false, null, false, false, false);
            var workFlow = string.Empty;
            if (node != null)
            {
                //Get the document per NodeID and culture
                TreeNode tn = tree.SelectSingleNode(node.NodeID, "en-us");

                //Return its workflow step ID
                int iStepID = tn.DocumentWorkflowStepID;

                if (iStepID > 0)
                {
                    // Get workflow step display name
                    WorkflowStepInfo wsi = WorkflowStepInfoProvider.GetWorkflowStepInfo(iStepID);
                    if (wsi != null)
                    {
                        workFlow = ResHelper.LocalizeString(wsi.StepDisplayName);

                    }
                }
            }

            return workFlow;
        }

        /// <summary>
        /// Getting PublishedStatus by noadAliaspath
        /// Checking wether that document is published before or not
        /// </summary>
        /// <param name="data">noadAliaspath</param>
        public static bool isPublishedStatusBynodeAliasPath(string nodeAliasPath)
        {
            var isPublished = false;
            // Create an instance of the Tree provider first
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Get the document
            TreeNode node = tree.SelectSingleNode(SiteContext.CurrentSiteName, nodeAliasPath, "en-us", false, null, false, false, false);

            if (node != null)
            {
                //Get the document per NodeID and culture
                TreeNode tn = tree.SelectSingleNode(node.NodeID, "en-us");

                isPublished = node.PublishedVersionExists;

            }
            return isPublished;

        }
        
        /// <summary>
        /// Getting UserName who modified document last by UserID
        /// </summary>
        /// <param name="data">UserID</param>
        public static string GetModifiedUserName(int UserID)
        {
            UserInfo modifiedUser = UserInfoProvider.GetUserInfo(UserID);
            var modifiedUserName = modifiedUser.FullName;
            return modifiedUserName;
        }


        //Reverting to published version if i am editing a published document and then go for edit it.
        //But i have not changed any thing and click on cancel button.
        //Document should shown as published again
        //It was showing as draft because its creating a new version.So added code here for showing that document as published.
        public static bool RollbackToPreviousVersion(int NodeID)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);

            // Prepare parameters
            string siteName = SiteContext.CurrentSiteName;
            string culture = "en-us";
            string where = null;
            string orderBy = null;
            string columns = null;

            // Get the document
            TreeNode node = DocumentHelper.GetDocument(NodeID, culture, tree);

            if (node != null)
            {
                // Prepare the WHERE condition for the oldest document version
                where = "DocumentID = " + node.DocumentID;
                orderBy = "ModifiedWhen DESC";
                int topN = 2;

                // Get the version ID
                DataSet versionHistory = VersionHistoryInfoProvider.GetVersionHistories(where, orderBy, topN, columns);

                if (!DataHelper.DataSourceIsEmpty(versionHistory) && node.IsPublished == true)
                {
                    // Create the Version history info object
                    //get the details of previous version of this document
                    VersionHistoryInfo version = new VersionHistoryInfo(versionHistory.Tables[0].Rows[1]);
                    VersionManager versionManager = VersionManager.GetInstance(tree);
                    //get the workflow stepid of previous version of this document
                    int WorkFlowStepID = Convert.ToInt32(version.VersionWorkflowStepID);
                    //move that document to previous version workflow state
                    MoveToWorkflowStep(WorkFlowStepID, tree, node);

                }
                else if (!DataHelper.DataSourceIsEmpty(versionHistory) && versionHistory.Tables[0].Rows.Count > 1)
                {
                    // Create the Version history info object
                    //get the details of previous version of this document
                    VersionHistoryInfo version = new VersionHistoryInfo(versionHistory.Tables[0].Rows[1]);
                    VersionManager versionManager = VersionManager.GetInstance(tree);
                    //get the workflow stepid of previous version of this document
                    int WorkFlowStepID = Convert.ToInt32(version.VersionWorkflowStepID);
                    //move that document to previous version workflow state
                    MoveToWorkflowStep(WorkFlowStepID, tree, node);

                }
            }
            return true;
        }
        //move that document to previous version workflow state
        //it will move to the state or previous version that we edited
        protected static bool MoveToWorkflowStep(int VersionWorkFlowStepID, TreeProvider tree, TreeNode node)
        {
            bool versionChangedStatus = false;

            WorkflowManager workflowManager = WorkflowManager.GetInstance(tree);
            WorkflowInfo workflow = workflowManager.GetNodeWorkflow(node);

            // Check if the document uses workflow
            if (workflow != null)
            {
                // Check if the workflow doesn't use automatic publishing, otherwise, documents can't change workflow steps.
                if (!workflow.WorkflowAutoPublishChanges)
                {
                    // Check if the current user can move the document to the next step
                    if (workflowManager.CheckStepPermissions(node, WorkflowActionEnum.Reject))
                    {
                        // Get workflowStepInfo object based on workflow step ID
                        WorkflowStepInfo workflowStep = WorkflowStepInfoProvider.GetWorkflowStepInfo(VersionWorkFlowStepID);
                        if (workflowStep.StepDisplayName.ToLower() == SMEConstant.WORKFLOW_PUBLISHED.ToLower())
                        {
                            // Move the document to the specified step
                            workflowManager.MoveToSpecificStep(node, workflowStep);
                            versionChangedStatus = true;
                        }
                        else if (workflowStep.StepDisplayName.ToLower() == SMEConstant.WORKFLOW_ARCHIVED.ToLower())
                        {
                            // Archive the document
                            workflowManager.ArchiveDocument(node, null);
                            // Move the document to the specified step
                            //workflowManager.MoveToSpecificStep(node, workflowStep);
                            versionChangedStatus = true;
                        }
                    }
                }
            }
            return versionChangedStatus;

        }

        
    }
}