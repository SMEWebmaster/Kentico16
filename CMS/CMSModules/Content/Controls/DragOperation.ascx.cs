using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.ExtendedControls;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_DragOperation : ContentActionsControl
{
    #region "Private variables"

    protected int nodeId = 0;
    protected int targetNodeId = 0;
    private string canceledString = null;

    protected string action = null;

    protected static Hashtable mInfos = new Hashtable();

    protected TreeNode node = null;
    protected TreeNode targetNode = null;
    protected bool childNodes = false;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    public string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mInfos["CopyMoveError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["CopyMoveError_" + ctlAsync.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Current Info.
    /// </summary>
    public string CurrentInfo
    {
        get
        {
            return ValidationHelper.GetString(mInfos["CopyMoveInfo_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mInfos["CopyMoveInfo_" + ctlAsync.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Gets the document node which is moved / copied / linked
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return node;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterEditScript(Page, false);

        // Initialize events
        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        // Get the data
        nodeId = QueryHelper.GetInteger("nodeid", 0);
        targetNodeId = QueryHelper.GetInteger("targetnodeid", 0);
        action = QueryHelper.GetString("action", "");

        if (!RequestHelper.IsCallback())
        {
            // Register the main CMS script
            ScriptHelper.RegisterCMS(Page);

            // Get the node
            node = TreeProvider.SelectSingleNode(nodeId);
            targetNode = TreeProvider.SelectSingleNode(targetNodeId, TreeProvider.ALL_CULTURES);

            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;

            if ((node != null) && (targetNode != null))
            {
                string targetName = targetNode.DocumentNamePath;
                bool isRoot = targetNode.NodeClassName.EqualsCSafe("cms.root", true);

                // Get the real target node
                if (!isRoot && (action.IndexOfCSafe("position", true) >= 0))
                {
                    // Get the target order and real parent ID
                    int newTargetId = targetNode.NodeParentID;
                    TreeNode realTargetNode = TreeProvider.SelectSingleNode(newTargetId);
                    if (realTargetNode != null)
                    {
                        targetName = realTargetNode.DocumentNamePath;
                    }
                }

                // Initialize resource strings, images
                btnCancel.OnClientClick = ctlAsync.GetCancelScript(true) + "return false;";
                btnNo.OnClientClick = "SelectNode(" + node.NodeID + "); return false;";

                lblTarget.Text = GetString("ContentOperation.TargetDocument") + " <strong>" + HTMLHelper.HTMLEncode(targetName) + "</strong>";

                switch (action.ToLowerCSafe())
                {
                    case "movenode":
                    case "movenodeposition":
                    case "movenodefirst":
                        // Setup page title text and image
                        titleElemAsync.TitleText = GetString("ContentRequest.StartMove");
                        canceledString = "ContentRequest.MoveCanceled";
                        headQuestion.Text = GetString("ContentMove.Question");
                        chkCopyPerm.Text = GetString("contentrequest.preservepermissions");
                        break;

                    case "copynode":
                    case "copynodeposition":
                    case "copynodefirst":
                        // Setup page title text and image
                        titleElemAsync.TitleText = GetString("ContentRequest.StartCopy");
                        canceledString = "ContentRequest.CopyingCanceled";
                        childNodes = chkChild.Checked;
                        plcCopyCheck.Visible = node.NodeHasChildren;
                        chkChild.ResourceString = "contentrequest.copyunderlying";

                        headQuestion.Text = GetString("ContentCopy.Question");
                        chkCopyPerm.Text = GetString("contentrequest.copypermissions");
                        break;

                    case "linknode":
                    case "linknodeposition":
                    case "linknodefirst":
                        // Setup page title text and image
                        titleElemAsync.TitleText = GetString("ContentRequest.StartLink");
                        canceledString = "ContentRequest.LinkCanceled";
                        childNodes = chkChild.Checked;
                        plcCopyCheck.Visible = node.NodeHasChildren;
                        chkChild.ResourceString = "contentrequest.linkunderlying";

                        headQuestion.Text = GetString("ContentLink.Question");
                        chkCopyPerm.Text = GetString("contentrequest.copypermissions");
                        break;

                    default:
                        ShowError(GetString("error.notsupported"));
                        pnlAction.Visible = false;
                        break;
                }
            }
            else
            {
                // Hide everything
                pnlContent.Visible = false;
            }
        }
    }

    #endregion


    #region "Button actions"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;
        pnlContent.Visible = false;

        EnsureLog();
        CurrentError = string.Empty;
        CurrentInfo = string.Empty;

        // Perform the action
        ctlAsync.RunAsync(DoAction, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Action methods"

    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void DoAction(object parameter)
    {
        // Get the target node
        if (targetNode == null)
        {
            return;
        }

        if (node == null)
        {
            return;
        }

        try
        {
            switch (action.ToLowerCSafe())
            {
                case "movenode":
                case "movenodeposition":
                case "movenodefirst":
                    {
                        AddLog(GetString("ContentRequest.StartMove"));
                    }
                    break;

                case "copynode":
                case "copynodeposition":
                case "copynodefirst":
                    {
                        AddLog(GetString("ContentRequest.StartCopy"));
                    }
                    break;

                case "linknode":
                case "linknodeposition":
                case "linknodefirst":
                    {
                        AddLog(GetString("ContentRequest.StartLink"));
                    }
                    break;
            }

            // Process the action
            TreeNode newNode = ProcessAction(node, targetNode, action, childNodes, true, chkCopyPerm.Checked);
            if (newNode != null)
            {
                int refreshId = newNode.NodeID;

                // Refresh tree
                ctlAsync.Parameter = "RefreshTree(" + refreshId + ", " + refreshId + "); \n" + "SelectNode(" + refreshId + ");";
            }
        }
        catch (ThreadAbortException)
        {
        }
    }


    /// <summary>
    /// Adds the alert error message to the response.
    /// </summary>
    /// <param name="message">Message</param>
    protected override void AddError(string message)
    {
        CurrentError = message;
    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public void AddScript(string script)
    {
        if (script != null)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
        }
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        CurrentInfo = GetString(canceledString);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();");
        if (!String.IsNullOrEmpty(CurrentInfo))
        {
            ShowConfirmation(CurrentInfo);
        }
        CurrentLog.Close();
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        CurrentLog.Close();
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        CurrentLog.Close();

        if (ctlAsync.Parameter != null)
        {
            AddScript(ctlAsync.Parameter.ToString());
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);

        return log;
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected override void AddLog(string newLog)
    {
        EnsureLog();

        LogContext.AppendLine(newLog);
    }

    #endregion
}