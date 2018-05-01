using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Base;
using CMS.DocumentEngine;
using CMS.SiteProvider;
using CMS.WorkflowEngine;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.UIControls;
using CMS.WorkflowEngine.Definitions;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Content_Controls_EditMenu : EditMenu, IExtensibleEditMenu
{
    #region "Variables"

    // Actions
    protected SaveAction save = null;
    protected SaveAction saveAnother = null;
    protected SaveAction saveAndClose = null;
    protected HeaderAction newVersion = null;
    protected DocumentApproveAction approve = null;
    protected DocumentPublishAction publish = null;
    protected DocumentRejectAction reject = null;
    protected DocumentCheckInAction checkin = null;
    protected DocumentCheckOutAction checkout = null;
    protected DocumentUndoCheckOutAction undoCheckout = null;
    protected DocumentArchiveAction archive = null;
    protected HeaderAction delete = null;
    protected HeaderAction applyWorkflow = null;
    protected HeaderAction prop = null;
    protected HeaderAction spellcheck = null;
    protected HeaderAction convert = null;

    private FormModeEnum originalMode = FormModeEnum.Update;
    private WorkflowStepInfo mStep;

    private bool? mAllowSave;
    private bool mSendNotification = true;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Menu control
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            return menu;
        }
    }


    /// <summary>
    /// Indicates if Save action is allowed.
    /// </summary>
    public override bool AllowSave
    {
        get
        {
            if (mAllowSave == null)
            {
                return DocumentManager.AllowSave;
            }

            return mAllowSave.Value;
        }
        set
        {
            mAllowSave = value;
        }
    }


    /// <summary>
    /// Indicates if the menu is enabled
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return menu.Enabled;
        }
        set
        {
            menu.Enabled = value;
        }
    }


    /// <summary>
    /// Indicates if small icons should be used for actions
    /// </summary>
    public override bool UseSmallIcons
    {
        get
        {
            return menu.UseSmallIcons;
        }
        set
        {
            menu.UseSmallIcons = value;
        }
    }


    /// <summary>
    /// Document node
    /// </summary>
    public override TreeNode Node
    {
        get
        {
            return DocumentManager.Node;
        }
    }


    /// <summary>
    /// Node ID
    /// </summary>
    public override int NodeID
    {
        get
        {
            return DocumentManager.NodeID;
        }
        set
        {
            DocumentManager.NodeID = value;
        }
    }


    /// <summary>
    /// Workflow manager
    /// </summary>
    public WorkflowManager WorkflowManager
    {
        get
        {
            return DocumentManager.WorkflowManager;
        }
    }


    /// <summary>
    /// If true, the access permissions to the items are checked.
    /// </summary>
    public override bool CheckPermissions
    {
        get
        {
            return DocumentManager.CheckPermissions;
        }
        set
        {
            DocumentManager.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Indicates if workflow actions should be displayed and handled
    /// </summary>
    public override bool HandleWorkflow
    {
        get
        {
            return DocumentManager.HandleWorkflow;
        }
        set
        {
            DocumentManager.HandleWorkflow = value;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return DocumentManager.StopProcessing;
        }
        set
        {
            DocumentManager.StopProcessing = value;
        }
    }


    /// <summary>
    /// Current step
    /// </summary>
    protected WorkflowStepInfo Step
    {
        get
        {
            return mStep ?? (mStep = DocumentManager.Step);
        }
    }


    #endregion


    #region "Constructors"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Content_Controls_EditMenu()
    {
        ShowSave = true;
        ShowCheckOut = true;
        ShowCheckIn = true;
        ShowUndoCheckOut = true;
        ShowApprove = true;
        ShowArchive = true;
        ShowSubmitToApproval = true;
        ShowReject = true;
        ShowProperties = false;
        ShowSpellCheck = false;
        ShowDelete = false;
        ShowApplyWorkflow = true;
        ShowCreateAnother = true;
        ShowSaveAndClose = false;
        RefreshInterval = 500;
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Set CSS class
        pnlContainer.CssClass = IsLiveSite ? "" : "cms-edit-menu";
        menu.UseBasicStyles = IsLiveSite;

        // Register events
        DocumentManager.OnAfterAction += OnAfterAction;
        DocumentManager.OnBeforeAction += OnBeforeAction;
        DocumentManager.OnCheckConsistency += OnCheckConsistency;

        // Ensure document information panel
        if (!IsLiveSite)
        {
            DocumentManager.LocalDocumentPanel = pnlDoc;
        }

        ComponentEvents.RequestEvents.RegisterForEvent("RemoveWireframe", RemoveWireframe);
    }


    protected override void OnLoad(EventArgs e)
    {
        // Handle callback in OnLoad event because of ShortIDs
        var parameters = Request.Form["params"];
        if (parameters != null)
        {
            if (parameters.StartsWithCSafe(CALLBACK_ID + ClientID))
            {
                string[] args = parameters.Split(new[] { CALLBACK_SEP }, StringSplitOptions.None);
                DocumentManager.Mode = FormModeEnum.Update;
                DocumentManager.ClearNode();
                DocumentManager.NodeID = ValidationHelper.GetInteger(args[1], 0);
                DocumentManager.CultureCode = ValidationHelper.GetString(args[2], null);
                Response.Clear();
                bool refreshNode = (Node.NodeSiteID != SiteContext.CurrentSiteID);
                if (Step != null)
                {
                    Response.Write(CALLBACK_ID + DocumentManager.GetDocumentInfo(HandleWorkflow) + CALLBACK_SEP + DocumentManager.RefreshActionContent.ToString().ToLowerCSafe() + CALLBACK_SEP + Step.StepID + CALLBACK_SEP + refreshNode.ToString().ToLowerCSafe());
                }
                else
                {
                    Response.Write(CALLBACK_ID + DocumentManager.GetDocumentInfo(false) + CALLBACK_SEP + "false" + CALLBACK_SEP + "0" + CALLBACK_SEP + refreshNode.ToString().ToLowerCSafe());
                }
                Response.End();
                return;
            }
        }

        base.OnLoad(e);

        // Keep original mode
        originalMode = DocumentManager.Mode;

        // Perform full post-back if not in update panel
        menu.PerformFullPostBack = !ControlsHelper.IsInUpdatePanel(this);

        // Initialize e-mail check box
        if ((DocumentManager.Mode != FormModeEnum.Insert) && (DocumentManager.Mode != FormModeEnum.InsertNewCultureVersion))
        {
            WorkflowInfo workflow = DocumentManager.Workflow;
            if (workflow != null)
            {
                // Keep the value
                if (RequestHelper.IsPostBack())
                {
                    mSendNotification = chkEmails.Checked;
                }
                chkEmails.Checked = workflow.SendEmails(DocumentManager.SiteName, WorkflowEmailTypeEnum.Unknown);
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadMenu();

        // Hide menu if no actions or document is displayed through preview link or on a live site
        bool menuVisible = !VirtualContext.IsPreviewLinkInitialized && (menu.HasAnyVisibleAction() || plcControls.Visible || plcAdditionalControls.Visible);
        if (menuVisible)
        {
            pnlContainer.Visible = true;

            // Display document information
            DocumentManager.ShowDocumentInfo(HandleWorkflow);
        }

        // Ensure visibility
        plcMenu.Visible = menuVisible || !string.IsNullOrEmpty(pnlDoc.Label.Text) || lblInfo.Visible;

        if (plcMenu.Visible)
        {
            RegisterActionScripts();
        }

        // Always register script to provide functionality even if the menu is hidden
        ScriptHelper.RegisterEditScript(Page, false);
    }


    protected override void Render(HtmlTextWriter writer)
    {
        base.Render(writer);

        PostBackOptions opt = new PostBackOptions(this, null)
        {
            PerformValidation = false
        };

        Page.ClientScript.RegisterForEventValidation(opt);
    }


    /// <summary>
    /// Registers action scripts
    /// </summary>
    private void RegisterActionScripts()
    {
        StringBuilder sb = new StringBuilder();

        if (spellcheck != null)
        {
            // Register spell checker script
            ScriptHelper.RegisterSpellChecker(Page);

            sb.Append("var spellURL = '", (IsLiveSite ? AuthenticationHelper.ResolveDialogUrl("~/CMSFormControls/LiveSelectors/SpellCheck.aspx") : AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Content/CMSDesk/Edit/SpellCheck.aspx")), "'; \n");
            sb.Append("function SpellCheck_", ClientID, "(spellURL) { checkSpelling(spellURL); } var offsetValue = 7;");
        }

        bool addComment = false;
        sb.Append("function CheckConsistency_", ClientID, "() { ", DocumentManager.GetJSFunction("CONS", null, null), "; } \n");
        if ((save != null) && save.Enabled) { sb.Append("function SaveDocument_", ClientID, "(createAnother) { ", save.OnClientClick, DocumentManager.GetJSSaveFunction("createAnother"), "; } \n"); }
        if (approve != null) { addComment = true; sb.Append("function Approve_", ClientID, "(stepId, comment) { ", approve.OnClientClick, DocumentManager.GetJSFunction(approve.EventName, "stepId", "comment"), "; } \n"); }
        if (reject != null) { addComment = true; sb.Append("function Reject_", ClientID, "(historyId, comment) { ", reject.OnClientClick, DocumentManager.GetJSFunction(reject.EventName, "historyId", "comment"), "; } \n"); }
        if (publish != null) { addComment = true; sb.Append("function Publish_", ClientID, "(comment) { ", publish.OnClientClick, DocumentManager.GetJSFunction(publish.EventName, null, "comment"), "; } \n"); }
        if (archive != null) { addComment = true; sb.Append("function Archive_", ClientID, "(stepId, comment) { ", archive.OnClientClick, DocumentManager.GetJSFunction(archive.EventName, "stepId", "comment"), "; } \n"); }
        if (checkout != null) { sb.Append("function CheckOut_", ClientID, "() { ", checkout.OnClientClick, DocumentManager.GetJSFunction(checkout.EventName, null, null), "; } \n"); }
        if (checkin != null) { addComment = true; sb.Append("function CheckIn_", ClientID, "(comment) { ", checkin.OnClientClick, DocumentManager.GetJSFunction(checkin.EventName, null, "comment"), "; } \n"); }
        if (undoCheckout != null) { sb.Append("function UndoCheckOut_", ClientID, "() { ", undoCheckout.OnClientClick, DocumentManager.GetJSFunction(undoCheckout.EventName, null, null), "; } \n"); }
        if (addComment) { sb.Append("function AddComment_", ClientID, "(name, documentId, menuId) { ", DocumentManager.GetJSFunction(ComponentEvents.COMMENT, "name|documentId|menuId", null), "; } \n"); }

        // Register the script
        if (sb.Length > 0)
        {
            ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "EditMenuActions" + ClientID, ScriptHelper.GetScript(sb.ToString()));
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds additional control to pnlAdditionalControls panel that shows this control in right part of panel.
    /// </summary>
    /// <param name="control">Control to be added</param>
    /// <exception cref="ArgumentNullException"><paramref name="control"/> is null</exception>
    public void AddAdditionalControl(Control control)
    {
        if (control == null)
        {
            throw new ArgumentNullException("control");
        }

        plcAdditionalControls.Visible = true;
        plcAdditionalControls.Controls.Add(control);
    }


    private void ClearProperties()
    {
        // Clear actions
        save = null;
        saveAnother = null;
        saveAndClose = null;
        approve = null;
        reject = null;
        checkin = null;
        checkout = null;
        undoCheckout = null;
        archive = null;
        delete = null;
        prop = null;
        spellcheck = null;
        publish = null;
        newVersion = null;
        applyWorkflow = null;

        mStep = null;

        // Clear security result
        DocumentManager.ClearProperties();
    }


    private void ReloadMenu()
    {
        if (StopProcessing)
        {
            return;
        }

        // Raise event handler
        RaiseOnBeforeReloadMenu();

        // Handle several reloads
        ClearProperties();

        var showWorkflowButtons = HandleWorkflow;
        if (!HideStandardButtons)
        {
            if (Step == null)
            {
                // Do not handle workflow
                HandleWorkflow = false;
            }

            bool hideSave = false;

            // If content should be refreshed
            if (DocumentManager.RefreshActionContent)
            {
                // Display action message
                if (Step != null)
                {
                    WorkflowActionInfo action = WorkflowActionInfoProvider.GetWorkflowActionInfo(Step.StepActionID);
                    string name = (action != null) ? action.ActionDisplayName : Step.StepDisplayName;
                    string str = (action != null) ? "workflow.actioninprogress" : "workflow.stepinprogress";
                    string text = string.Format(ResHelper.GetString(str, ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(name, ResourceCulture)));
                    text = ScriptHelper.GetLoaderInlineHtml(Page, text);

                    InformationText = text;
                    EnsureRefreshScript();
                    hideSave = true;
                }
            }

            // Handle save action
            if (ShowSave && !hideSave)
            {
                save = new SaveAction(Page)
                {
                    OnClientClick = RaiseGetClientValidationScript(ComponentEvents.SAVE, DocumentManager.ConfirmChanges ? DocumentManager.GetAllowSubmitScript() : null),
                    Tooltip = ResHelper.GetString("EditMenu.Save", ResourceCulture)
                };

                // If not allowed to save, disable the save item
                if (!AllowSave)
                {
                    save.OnClientClick = null;
                    save.Enabled = false;
                }

                // New version
                if (DocumentManager.IsActionAllowed(DocumentComponentEvents.CREATE_VERSION))
                {
                    newVersion = new HeaderAction
                                     {
                                         Text = ResHelper.GetString("EditMenu.NewVersionIcon", ResourceCulture),
                                         Tooltip = ResHelper.GetString("EditMenu.NewVersion", ResourceCulture),
                                         EventName = DocumentComponentEvents.CREATE_VERSION
                                     };
                }
            }

            // Document update
            if (DocumentManager.Mode == FormModeEnum.Update)
            {
                if (Node != null)
                {
                    string submitScript = DocumentManager.GetSubmitScript();

                    #region "Workflow actions"

                    if (HandleWorkflow)
                    {
                        // Check-out action
                        if (ShowCheckOut && DocumentManager.IsActionAllowed(DocumentComponentEvents.CHECKOUT))
                        {
                            checkout = new DocumentCheckOutAction(Page)
                                {
                                    Tooltip = ResHelper.GetString("EditMenu.CheckOut", ResourceCulture),
                                    ButtonStyle = ButtonStyle.Default
                                };
                        }

                        // Undo check-out action
                        if (ShowUndoCheckOut && DocumentManager.IsActionAllowed(DocumentComponentEvents.UNDO_CHECKOUT))
                        {
                            undoCheckout = new DocumentUndoCheckOutAction(Page)
                                {
                                    Tooltip = ResHelper.GetString("EditMenu.UndoCheckout", ResourceCulture),
                                    OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.UNDO_CHECKOUT, "if(!confirm(" + ScriptHelper.GetString(ResHelper.GetString("EditMenu.UndoCheckOutConfirmation", ResourceCulture)) + ")) { return false; }"),
                                    ButtonStyle = ButtonStyle.Default
                                };
                        }

                        // Check-in action
                        if (ShowCheckIn && DocumentManager.IsActionAllowed(DocumentComponentEvents.CHECKIN))
                        {
                            checkin = new DocumentCheckInAction(Page)
                                {
                                    Tooltip = ResHelper.GetString("EditMenu.CheckIn", ResourceCulture),
                                    OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.CHECKIN, submitScript),
                                    ButtonStyle = ButtonStyle.Default
                                };

                            // Add check-in comment
                            AddCommentAction(DocumentComponentEvents.CHECKIN, checkin);
                        }

                        // Approve action
                        if (DocumentManager.IsActionAllowed(DocumentComponentEvents.APPROVE))
                        {
                            if ((Step == null) || Step.StepIsEdit)
                            {
                                if (ShowSubmitToApproval)
                                {
                                    approve = new DocumentApproveAction(Page)
                                        {
                                            Text = ResHelper.GetString("EditMenu.IconSubmitToApproval", ResourceCulture),
                                            Tooltip = ResHelper.GetString("EditMenu.SubmitToApproval", ResourceCulture),
                                            OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.APPROVE, submitScript),
                                            ButtonStyle = ButtonStyle.Default
                                        };
                                }
                            }
                            else
                            {
                                if (ShowApprove)
                                {
                                    approve = new DocumentApproveAction(Page)
                                        {
                                            Tooltip = ResHelper.GetString("EditMenu.Approve", ResourceCulture),
                                            OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.APPROVE, submitScript),
                                            ButtonStyle = ButtonStyle.Default
                                        };
                                }
                            }
                        }

                        // Reject action
                        if (ShowReject && DocumentManager.IsActionAllowed(DocumentComponentEvents.REJECT))
                        {
                            var prevSteps = WorkflowManager.GetPreviousSteps(Node);
                            int prevStepsCount = prevSteps.Count;

                            if (prevStepsCount > 0)
                            {
                                reject = new DocumentRejectAction(Page)
                                    {
                                        Tooltip = ResHelper.GetString("EditMenu.Reject", ResourceCulture),
                                        OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.REJECT, submitScript),
                                        ButtonStyle = ButtonStyle.Default
                                    };

                                // For workflow managers allow reject to specified step
                                if (WorkflowManager.CanUserManageWorkflow(CurrentUser, Node.NodeSiteName))
                                {
                                    if (prevStepsCount > 1)
                                    {
                                        foreach (var s in prevSteps)
                                        {
                                            reject.AlternativeActions.Add(new DocumentRejectAction(Page)
                                                {
                                                    Text = string.Format(ResHelper.GetString("EditMenu.RejectTo", ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName, ResourceCulture))),
                                                    Tooltip = ResHelper.GetString("EditMenu.Reject", ResourceCulture),
                                                    OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.REJECT, submitScript),
                                                    CommandArgument = s.RelatedHistoryID.ToString()
                                                });
                                        }
                                    }
                                }

                                // Add reject comment
                                AddCommentAction(DocumentComponentEvents.REJECT, reject);
                            }
                        }

                        // Get next step info
                        List<WorkflowStepInfo> steps = DocumentManager.NextSteps;
                        int stepsCount = steps.Count;
                        WorkflowInfo workflow = DocumentManager.Workflow;

                        // Handle multiple next steps
                        if (approve != null)
                        {
                            string actionName = DocumentComponentEvents.APPROVE;
                            bool publishStepVisible = false;

                            // Get next approval step info
                            var approveSteps = steps.FindAll(s => !s.StepIsArchived);
                            int aprroveStepsCount = approveSteps.Count;
                            if (aprroveStepsCount > 0)
                            {
                                var nextS = approveSteps[0];

                                // Only one next step
                                if (aprroveStepsCount == 1)
                                {
                                    if (nextS.StepIsPublished)
                                    {
                                        publishStepVisible = true;
                                        actionName = DocumentComponentEvents.PUBLISH;
                                        approve.Text = ResHelper.GetString("EditMenu.IconPublish", ResourceCulture);
                                        approve.Tooltip = ResHelper.GetString("EditMenu.Publish", ResourceCulture);
                                    }

                                    // There are also archived steps
                                    if (stepsCount > 1)
                                    {
                                        // Set command argument
                                        approve.CommandArgument = nextS.StepID.ToString();
                                    }

                                    // Process action appearance
                                    ProcessAction(approve, Step, nextS);
                                }
                                // Multiple next steps
                                else
                                {
                                    // Check if not all steps publish steps
                                    if (approveSteps.Exists(s => !s.StepIsPublished))
                                    {
                                        approve.Tooltip = ResHelper.GetString("EditMenu.ApproveMultiple", ResourceCulture);
                                    }
                                    else
                                    {
                                        actionName = DocumentComponentEvents.PUBLISH;
                                        approve.Text = ResHelper.GetString("EditMenu.IconPublish", ResourceCulture);
                                        approve.Tooltip = ResHelper.GetString("EditMenu.ApproveMultiple", ResourceCulture);
                                    }

                                    // Make action inactive
                                    approve.OnClientClick = null;
                                    approve.Inactive = true;

                                    // Process action appearance
                                    ProcessAction(approve, Step, null);

                                    string itemText = (Step == null) || Step.StepIsEdit ? "EditMenu.SubmitTo" : "EditMenu.ApproveTo";
                                    string itemDesc = (Step == null) || Step.StepIsEdit ? "EditMenu.SubmitToApproval" : "EditMenu.Approve";

                                    foreach (var s in approveSteps)
                                    {
                                        DocumentApproveAction app = new DocumentApproveAction(Page)
                                            {
                                                Text = string.Format(ResHelper.GetString(itemText, ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName, ResourceCulture))),
                                                Tooltip = ResHelper.GetString(itemDesc, ResourceCulture),
                                                OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.APPROVE, submitScript),
                                                CommandArgument = s.StepID.ToString()
                                            };

                                        if (s.StepIsPublished)
                                        {
                                            publishStepVisible = true;
                                            app.Text = string.Format(ResHelper.GetString("EditMenu.PublishTo", ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName, ResourceCulture)));
                                            app.Tooltip = ResHelper.GetString("EditMenu.Publish", ResourceCulture);
                                        }

                                        // Process action appearance
                                        ProcessAction(app, Step, s);

                                        // Add step
                                        approve.AlternativeActions.Add(app);
                                    }
                                }

                                // Display direct publish button
                                if (WorkflowManager.CanUserManageWorkflow(CurrentUser, Node.NodeSiteName) && !publishStepVisible && Step.StepAllowPublish)
                                {
                                    // Add approve action as a alternative action
                                    if ((approve.AlternativeActions.Count == 0) && !nextS.StepIsPublished)
                                    {
                                        // Make action inactive
                                        approve.Tooltip = ResHelper.GetString("EditMenu.ApproveMultiple", ResourceCulture);
                                        approve.OnClientClick = null;
                                        approve.Inactive = true;

                                        // Add approve action
                                        string itemText = Step.StepIsEdit ? "EditMenu.SubmitTo" : "EditMenu.ApproveTo";
                                        string itemDesc = Step.StepIsEdit ? "EditMenu.SubmitToApproval" : "EditMenu.Approve";
                                        DocumentApproveAction app = new DocumentApproveAction(Page)
                                            {
                                                Text = string.Format(ResHelper.GetString(itemText, ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nextS.StepDisplayName, ResourceCulture))),
                                                Tooltip = ResHelper.GetString(itemDesc, ResourceCulture),
                                                OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.APPROVE, submitScript),
                                                CommandArgument = nextS.StepID.ToString()
                                            };

                                        // Process action appearance
                                        ProcessAction(app, Step, nextS);

                                        approve.AlternativeActions.Add(app);
                                    }

                                    // Add direct publish action
                                    publish = new DocumentPublishAction(Page)
                                        {
                                            Tooltip = ResHelper.GetString("EditMenu.ApprovePublish", ResourceCulture),
                                            OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.PUBLISH, submitScript),
                                            ButtonStyle = ButtonStyle.Default
                                        };

                                    // Process action appearance
                                    ProcessAction(approve, Step, nextS);

                                    approve.AlternativeActions.Add(publish);
                                }

                                // Add approve comment
                                AddCommentAction(actionName, approve);
                            }
                            else
                            {
                                bool displayAction = false;
                                if (!workflow.IsBasic && (Step != null) && !Step.StepAllowBranch)
                                {
                                    // Transition exists, but condition doesn't match
                                    var transitions = WorkflowManager.GetStepTransitions(Step, WorkflowTransitionTypeEnum.Manual);
                                    if (transitions.Count > 0)
                                    {
                                        WorkflowStepInfo s = WorkflowStepInfoProvider.GetWorkflowStepInfo(transitions[0].TransitionEndStepID);
                                        if (!s.StepIsArchived)
                                        {
                                            // Publish text
                                            if (s.StepIsPublished)
                                            {
                                                approve.Text = ResHelper.GetString("EditMenu.IconPublish", ResourceCulture);
                                                approve.Tooltip = ResHelper.GetString("EditMenu.Publish", ResourceCulture);
                                            }

                                            // Inform user
                                            displayAction = true;
                                            approve.Enabled = false;

                                            // Process action appearance
                                            ProcessAction(approve, Step, null);
                                        }
                                    }
                                }

                                if (!displayAction)
                                {
                                    // There is not next step
                                    approve = null;
                                }
                            }
                        }

                        // Archive action
                        if ((ShowArchive || ForceArchive) && DocumentManager.IsActionAllowed(DocumentComponentEvents.ARCHIVE))
                        {
                            // Get next archive step info
                            var archiveSteps = steps.FindAll(s => s.StepIsArchived);
                            int archiveStepsCount = archiveSteps.Count;

                            archive = new DocumentArchiveAction(Page)
                                {
                                    Tooltip = ResHelper.GetString("EditMenu.Archive", ResourceCulture),
                                    OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.ARCHIVE, "if(!confirm(" + ScriptHelper.GetString(ResHelper.GetString("EditMenu.ArchiveConfirmation", ResourceCulture)) + ")) { return false; }" + submitScript),
                                    ButtonStyle = ButtonStyle.Default
                                };

                            // Multiple archive steps
                            if (archiveStepsCount > 1)
                            {
                                // Make action inactive
                                archive.Tooltip = ResHelper.GetString("EditMenu.ArchiveMultiple", ResourceCulture);
                                archive.OnClientClick = null;
                                archive.Inactive = true;

                                const string itemText = "EditMenu.ArchiveTo";
                                const string itemDesc = "EditMenu.Archive";

                                foreach (var s in archiveSteps)
                                {
                                    DocumentArchiveAction arch = new DocumentArchiveAction(Page)
                                        {
                                            Text = string.Format(ResHelper.GetString(itemText, ResourceCulture), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(s.StepDisplayName, ResourceCulture))),
                                            Tooltip = ResHelper.GetString(itemDesc, ResourceCulture),
                                            OnClientClick = RaiseGetClientValidationScript(DocumentComponentEvents.ARCHIVE, "if(!confirm(" + ScriptHelper.GetString(ResHelper.GetString("EditMenu.ArchiveConfirmation", ResourceCulture)) + ")) { return false; }" + submitScript),
                                            CommandArgument = s.StepID.ToString()
                                        };

                                    // Process action appearance
                                    ProcessAction(arch, Step, s);

                                    // Add step
                                    archive.AlternativeActions.Add(arch);
                                }

                                // Add archive comment
                                AddCommentAction(DocumentComponentEvents.ARCHIVE, archive);
                            }
                            else if (archiveStepsCount == 1)
                            {
                                var nextS = archiveSteps[0];

                                // There are also approve steps
                                if (stepsCount > 1)
                                {
                                    // Set command argument
                                    archive.CommandArgument = nextS.StepID.ToString();
                                }

                                // Process action appearance
                                ProcessAction(archive, Step, nextS);

                                // Add archive comment
                                AddCommentAction(DocumentComponentEvents.ARCHIVE, archive);
                            }
                            else
                            {
                                bool displayAction = ForceArchive;
                                if (!workflow.IsBasic && !Step.StepAllowBranch)
                                {
                                    // Transition exists, but condition doesn't match
                                    var transitions = WorkflowManager.GetStepTransitions(Step, WorkflowTransitionTypeEnum.Manual);
                                    if (transitions.Count > 0)
                                    {
                                        WorkflowStepInfo s = WorkflowStepInfoProvider.GetWorkflowStepInfo(transitions[0].TransitionEndStepID);
                                        if (s.StepIsArchived)
                                        {
                                            // Inform user
                                            displayAction = true;
                                            archive.Enabled = false;

                                            // Process action appearance
                                            ProcessAction(archive, Step, null);
                                        }
                                    }
                                }

                                if (!displayAction)
                                {
                                    // There is not next step
                                    archive = null;
                                }
                                else
                                {
                                    // Add archive comment
                                    AddCommentAction(DocumentComponentEvents.ARCHIVE, archive);
                                }
                            }
                        }
                    }

                    // Display Apply workflow button if user has permission to manage workflow and the button should be displayed and document is not linked
                    if (DisplayApplyWorkflowButton(showWorkflowButtons))
                    {
                        ScriptHelper.RegisterDialogScript(Page);

                        applyWorkflow = new HeaderAction
                        {
                            Text = ResHelper.GetString("WorkflowProperties.Apply", ResourceCulture),
                            Tooltip = ResHelper.GetString("EditMenu.WorkflowApply", ResourceCulture),
                            OnClientClick = string.Format("modalDialog('{0}','ApplyWorkflow', 770, 200, null, null, true); return false;", URLHelper.ResolveUrl("~/CMSModules/Workflows/Pages/ApplyWorkflow.aspx?documentid=" + Node.DocumentID)),
                            ButtonStyle = ButtonStyle.Default
                        };
                    }

                    #endregion

                    // Delete action
                    if (AllowSave && ShowDelete)
                    {
                        delete = new DeleteAction(Page)
                        {
                            OnClientClick = RaiseGetClientValidationScript(ComponentEvents.DELETE, "Delete_" + ClientID + "(" + NodeID + "); return false;"),
                            Tooltip = ResHelper.GetString("EditMenu.Delete", ResourceCulture),
                            ButtonStyle = ButtonStyle.Default
                        };
                    }

                    // Properties action
                    if (ShowProperties)
                    {
                        prop = new HeaderAction
                        {
                            Text = ResHelper.GetString("EditMenu.IconProperties", ResourceCulture),
                            Tooltip = ResHelper.GetString("EditMenu.Properties", ResourceCulture),
                            OnClientClick = "Properties(" + NodeID + "); return false;",
                            ButtonStyle = ButtonStyle.Default
                        };
                    }

                    // Convert action
                    if (Node.IsWireframe() && (PortalContext.ViewMode == ViewModeEnum.EditForm))
                    {
                        convert = new HeaderAction
                        {
                            Text = ResHelper.GetString("EditMenu.IconConvert", ResourceCulture),
                            Tooltip = ResHelper.GetString("EditMenu.Convert|EditMenu.IconConvert", ResourceCulture),
                            OnClientClick = "ConvertDocument(" + Node.NodeParentID + ", " + Node.DocumentID + "); return false;",
                            ButtonStyle = ButtonStyle.Default
                        };
                    }
                }
            }
            // Ensure create another action
            else if (DocumentManager.Mode == FormModeEnum.Insert)
            {
                if (AllowSave && ShowCreateAnother)
                {
                    string saveAnotherScript = DocumentManager.GetSaveAnotherScript();
                    saveAnother = new SaveAction(Page)
                    {
                        RegisterShortcutScript = false,
                        Text = ResHelper.GetString("editmenu.iconsaveandanother", ResourceCulture),
                        Tooltip = ResHelper.GetString("EditMenu.SaveAndAnother", ResourceCulture),
                        OnClientClick = RaiseGetClientValidationScript(ComponentEvents.SAVE, (DocumentManager.ConfirmChanges ? DocumentManager.GetAllowSubmitScript() : "") + saveAnotherScript),
                        CommandArgument = "another"
                    };
                }
            }

            // Ensure spell check action
            if (AllowSave && ShowSave && ShowSpellCheck)
            {
                spellcheck = new HeaderAction
                {
                    Text = ResHelper.GetString("EditMenu.IconSpellCheck", ResourceCulture),
                    Tooltip = ResHelper.GetString("EditMenu.SpellCheck", ResourceCulture),
                    OnClientClick = "SpellCheck_" + ClientID + "(spellURL); return false;",
                    RedirectUrl = "#",
                    ButtonStyle = ButtonStyle.Default
                };
            }

            if (AllowSave && ShowSaveAndClose)
            {
                string saveAndCloseScript = DocumentManager.GetSaveAndCloseScript();
                saveAndClose = new SaveAction(Page)
                {
                    RegisterShortcutScript = false,
                    Text = ResHelper.GetString("editmenu.iconsaveandclose", ResourceCulture),
                    Tooltip = ResHelper.GetString("EditMenu.SaveAndClose", ResourceCulture),
                    OnClientClick = RaiseGetClientValidationScript(ComponentEvents.SAVE, (DocumentManager.ConfirmChanges ? DocumentManager.GetAllowSubmitScript() : "") + saveAndCloseScript),
                    CommandArgument = "saveandclose"
                };
            }
        }

        // Add actions in correct order
        menu.ActionsList.Clear();

        AddAction(saveAndClose);
        AddAction(save);
        AddAction(saveAnother);
        AddAction(newVersion);
        if (HandleWorkflow)
        {
            AddAction(checkout);
            AddAction(undoCheckout);
            AddAction(checkin);
            AddAction(reject);
            AddAction(approve);
            AddAction(archive);
        }
        AddAction(spellcheck);
        AddAction(delete);
        AddAction(prop);
        AddAction(convert);
        AddAction(applyWorkflow);

        // Show temporary to set correct visibility of checkbox for sending e-mails
        plcControls.Visible = true;

        // Set e-mails checkbox
        chkEmails.Visible = WorkflowManager.CanUserManageWorkflow(CurrentUser, DocumentManager.SiteName) && ((approve != null) || (reject != null) || (archive != null));
        if (chkEmails.Visible)
        {
            chkEmails.ResourceString = ResHelper.GetString("WorfklowProperties.SendMail", ResourceCulture);
            if (!DocumentManager.Workflow.SendEmails(DocumentManager.SiteName, WorkflowEmailTypeEnum.Unknown))
            {
                chkEmails.Enabled = false;
                chkEmails.ToolTip = ResHelper.GetString("wf.emails.disabled", ResourceCulture);
            }
        }

        // Hide placeholder if there is no visible functional control
        plcControls.Visible = pnlRight.Controls.Cast<Control>().Any(c => (c.Visible && !(c is LiteralControl)));

        // Add remove wireframe action
        if (ShowRemoveWireframe)
        {
            AddAction(new HeaderAction
            {
                Text = ResHelper.GetString("Wireframe.Remove", ResourceCulture),
                Tooltip = ResHelper.GetString("Wireframe.Remove", ResourceCulture),
                CommandName = "RemoveWireframe",
                OnClientClick = "return confirm(" + ScriptHelper.GetLocalizedString("Wireframe.ConfirmRemove") + ")",
                ButtonStyle = ButtonStyle.Default
            });
        }

        // Add create wireframe action
        if (ShowCreateWireframe)
        {
            TreeNode node = DocumentManager.Node;
            if ((node != null) && (node.NodeWireframeTemplateID <= 0))
            {
                // Add create wireframe action
                AddAction(new HeaderAction
                              {
                                  Text = ResHelper.GetString("Wireframe.Create", ResourceCulture),
                                  Tooltip = ResHelper.GetString("Wireframe.Create", ResourceCulture),
                                  RedirectUrl = String.Format("~/CMSModules/Content/CMSDesk/Properties/CreateWireframe.aspx?nodeid={0}&culture={1}", node.NodeID, node.DocumentCulture),
                                  ButtonStyle = ButtonStyle.Default
                              });
            }
        }

        // Add extra actions
        if (mExtraActions != null)
        {
            foreach (HeaderAction action in mExtraActions)
            {
                AddAction(action);
            }
        }

        // Set the information text
        if (!String.IsNullOrEmpty(InformationText))
        {
            lblInfo.Text = InformationText;
            lblInfo.CssClass = "LeftAlign EditMenuInfo";
            lblInfo.Visible = true;
        }
    }


    private bool DisplayApplyWorkflowButton(bool showWorkflowButtons)
    {
        var allowed = !Node.IsLink && !IsLiveSite && (PortalContext.ViewMode != ViewModeEnum.EditLive) && ShowApplyWorkflow && (Step == null) && showWorkflowButtons && DocumentManager.IsActionAllowed(DocumentComponentEvents.APPLY_WORKFLOW) && Service<ILicenseService>.Entry().CheckLicense(WorkflowInfo.TYPEINFO.Feature, null, false);

        // Check workflow count
        if (allowed)
        {
            var workflows = WorkflowInfoProvider.GetWorkflows(string.Format("WorkflowEnabled <> 0 AND (WorkflowType <> {0} OR WorkflowType IS NULL)", (int)WorkflowTypeEnum.Automation), null, 1, "WorkflowID");
            allowed = !DataHelper.DataSourceIsEmpty(workflows);
        }

        return allowed;
    }


    /// <summary>
    /// Remove wireframe handler
    /// </summary>
    protected void RemoveWireframe(object sender, EventArgs e)
    {
        TreeNode node = DocumentManager.Node;

        DocumentManager.RemoveWireframe();

        PortalContext.ViewMode = ViewModeEnum.Design;

        ScriptHelper.RegisterStartupScript(this, typeof(string), "Refresh", ScriptHelper.GetScript(String.Format(
            "RefreshTree({0}, {0});SelectNode({0});",
            node.NodeID
        )));
    }


    private void EnsureRefreshScript()
    {
        PostBackOptions options = new PostBackOptions(this)
                        {
                            PerformValidation = false
                        };

        string postback = ControlsHelper.GetPostBackEventReference(menu, options, false, true);
        string externalRefreshScript = null;
        if (!string.IsNullOrEmpty(OnClientStepChanged))
        {
            externalRefreshScript = string.Format("clearInterval(refTimerId_{0}); {1};", ClientID, OnClientStepChanged);
        }

        const string commonScript = @"
String.prototype.startsWith = function (str) { return (this.match('^' + str) == str); };

function VerifyData(incomingData) {
    if (incomingData.startsWith('" + CALLBACK_ID + @"')) {
        return incomingData.replace('" + CALLBACK_ID + @"', '');
    }
    else {
        data = null;
    }

    return data;
}
";
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "refCommon", commonScript, true);

        StringBuilder sb = new StringBuilder();
        sb.Append(@"
var refTimerId_", ClientID, @" = 0;

function RfMenu_DoPostBack_", ClientID, @"() {", postback, @"}

function RfMenu_Succ_", ClientID, @"(data, textStatus, jqXHR) {
    if(data != null) {
        var hdn = document.getElementById('", hdnParam.ClientID, @"');
        var args = data.split('", CALLBACK_SEP, @"');
        var stop = (args[1] == 'false');
        var stepId = args[2];
        var slectnode = (args[3] == 'true');

        if(stop) {
            clearInterval(refTimerId_", ClientID, @");
            if(slectnode) {
                setTimeout('RefreshTree(", Node.NodeParentID, ", ", NodeID, @");SelectNode(", Node.NodeID, ")', ", RefreshInterval, @");
            }
            else {
                setTimeout('RefreshTree(", Node.NodeParentID, ", ", NodeID, @");RfMenu_DoPostBack_", ClientID, "()', ", RefreshInterval, @");
            }
        }
        else {
            // Step changed
            if(hdn.value != stepId) {
                var lbl = document.getElementById('", DocumentManager.DocumentInfoLabel.ClientID, @"');
                if(lbl != null) {
                    lbl.innerHTML = args[0];
                }",
                externalRefreshScript, @"
            }
        }
        hdn.value = stepId;
    }
    else {
        clearInterval(refTimerId_", ClientID, @");
        setTimeout('RefreshTree(", Node.NodeParentID, ", ", NodeID, @");SelectNode(", Node.NodeID, ")', ", RefreshInterval, @");
    }
}

function RfMenu_Err_", ClientID, @"(jqXHR, textStatus, errorThrown) {
    var err = '';
    if ((errorThrown != undefined) && (errorThrown != null) && (errorThrown != '')) {
        err = ' (' + errorThrown + ')';
        clearInterval(refTimerId_", ClientID, @");
        alert(err);
    }
}

function RfMenu_", ClientID, @"() {
    $cmsj.ajax({
        cache: false,
        type: 'POST',
        data: 'params=", CALLBACK_ID + ClientID, CALLBACK_SEP, DocumentManager.NodeID, CALLBACK_SEP, DocumentManager.CultureCode, @"',
        context: document.body,
        success: RfMenu_Succ_" + ClientID, @",
        error: RfMenu_Err_", ClientID, @", 
        dataType: 'text',
        dataFilter: VerifyData
    });
}

refTimerId_", ClientID, @" = setInterval('RfMenu_", ClientID, "()', 200);");

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ref_" + ClientID, sb.ToString(), true);
    }


    private void ProcessAction(HeaderAction action, WorkflowStepInfo currentStep, WorkflowStepInfo nextStep)
    {
        if (action == null)
        {
            return;
        }

        string nextStepName = null;
        SourcePoint def = null;
        if (nextStep != null)
        {
            WorkflowTransitionInfo transition = nextStep.RelatedTransition;
            if (transition != null)
            {
                def = currentStep.GetSourcePoint(transition.TransitionSourcePointGUID);
                nextStepName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nextStep.StepDisplayName, ResourceCulture));
            }
        }
        else
        {
            def = currentStep.StepDefinition.DefinitionPoint;
        }

        if (def != null)
        {
            if (!string.IsNullOrEmpty(def.Text))
            {
                action.Text = string.Format(HTMLHelper.HTMLEncode(ResHelper.LocalizeString(def.Text, ResourceCulture)), nextStepName);
            }

            if (!string.IsNullOrEmpty(def.Tooltip))
            {
                action.Tooltip = string.Format(ResHelper.LocalizeString(def.Tooltip, ResourceCulture), action.Text);
            }
        }
    }


    /// <summary>
    /// Adds menu action.
    /// </summary>
    /// <param name="action">Action</param>
    protected void AddAction(HeaderAction action)
    {
        if (action != null)
        {
            if (string.IsNullOrEmpty(action.ValidationGroup))
            {
                action.ValidationGroup = ActionsValidationGroup;
            }

            // Save action
            menu.ActionsList.Add(action);
        }
    }


    /// <summary>
    /// Adds comment action.
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="action">Current action</param>
    private void AddCommentAction(string name, HeaderAction action)
    {
        DocumentManager.RenderScript = true;
        string resName = name;
        if ((name == DocumentComponentEvents.APPROVE) && (Step != null) && (Step.StepIsEdit))
        {
            resName += "Submit";
        }

        CommentAction comment = new CommentAction(Page, resName)
        {
            Enabled = action.Enabled,
            Tooltip = ResHelper.GetString("EditMenu.Comment" + resName, ResourceCulture),
            OnClientClick = string.Format("AddComment_{0}('{1}',{2},'{0}');", ClientID, name, Node.DocumentID),
            ValidationGroup = ActionsValidationGroup,
        };
        action.AlternativeActions.Add(comment);
    }

    #endregion


    #region "Button handling"

    // Original values
    string originalDocumentName;
    DateTime originalPublishFrom = DateTime.MinValue;
    DateTime originalPublishTo = DateTime.MinValue;
    bool wasArchived;
    bool wasInPublishedStep;


    void OnCheckConsistency(object sender, SimpleDocumentManagerEventArgs e)
    {
        if (!e.IsValid)
        {
            ScriptHelper.RefreshTree(Page, NodeID, Node.NodeParentID);
        }
    }


    void OnBeforeAction(object sender, DocumentManagerEventArgs e)
    {
        TreeNode node = e.Node;

        // Backup original document's values - before saving
        originalDocumentName = node.DocumentName;
        originalPublishFrom = node.DocumentPublishFrom;
        originalPublishTo = node.DocumentPublishTo;
        wasArchived = node.IsArchived;
        wasInPublishedStep = node.IsInPublishStep;

        // Set send e-mails settings
        WorkflowManager.SendEmails = mSendNotification;
    }


    protected void OnAfterAction(object sender, DocumentManagerEventArgs args)
    {
        // Render UI scripts (Refresh tree/Split mode etc.)
        if (!IsLiveSite)
        {
            // Try get dialog flag from page instance
            bool isDialog = false;
            CMSContentPage page = Page as CMSContentPage;
            if ((page != null) && (page.RequiresDialog))
            {
                isDialog = true;
            }

            // Set refresh tree value with dependence on dialog mode (for dialog mode is by default true)
            bool refreshTree = isDialog;

            switch (args.ActionName)
            {

                case DocumentComponentEvents.APPROVE:
                    {
                        WorkflowStepInfo originalStep = args.OriginalStep;
                        WorkflowStepInfo nextStep = args.CurrentStep;

                        // Approve from published or archived step
                        bool allowRefresh = DocumentManager.AutoCheck && ((originalStep == null) || originalStep.StepIsArchived || originalStep.StepIsPublished);

                        // Next step is published or edit step
                        allowRefresh |= (nextStep != null) ? (nextStep.StepIsPublished || nextStep.StepIsEdit) : true;

                        // Refresh content tree when step is 'Published' or scope has been removed and icons should be displayed
                        refreshTree |= allowRefresh && DocumentHelper.IconsUsed(IconType.Published | IconType.VersionNotPublished | IconType.NotPublished);
                    }
                    break;


                case DocumentComponentEvents.CREATE_VERSION:
                case DocumentComponentEvents.PUBLISH:
                    refreshTree = true;
                    break;


                case DocumentComponentEvents.ARCHIVE:
                    refreshTree |= DocumentHelper.IconsUsed(IconType.Archived | IconType.Published | IconType.NotPublished);
                    break;


                case DocumentComponentEvents.REJECT:
                    refreshTree |= DocumentHelper.IconsUsed(IconType.CheckedOut | IconType.NotPublished);
                    break;


                case DocumentComponentEvents.CHECKIN:
                    refreshTree |= DocumentHelper.IconsUsed(IconType.CheckedOut);
                    break;


                case DocumentComponentEvents.CHECKOUT:
                case DocumentComponentEvents.UNDO_CHECKOUT:
                    refreshTree |= DocumentHelper.IconsUsed(IconType.CheckedOut);
                    EnsureSplitModeScript();
                    break;


                case ComponentEvents.SAVE:
                    if (string.IsNullOrEmpty(args.SaveActionContext))
                    {
                        refreshTree = ProcessSaveAction(originalMode, args.OriginalStep, isDialog);
                    }
                    break;
            }

            if (refreshTree)
            {
                ScriptHelper.RefreshTree(Page, NodeID, Node.NodeParentID);
            }
        }
    }


    /// <summary>
    /// Process save action with special script handling
    /// </summary>
    /// <param name="origMode">Original workflow mode</param>
    /// <param name="originalStep">Original workflow step</param>
    /// <param name="isDialog">Indicates whether is in dialog</param>
    private bool ProcessSaveAction(FormModeEnum origMode, WorkflowStepInfo originalStep, bool isDialog)
    {
        bool refreshTree = false;

        switch (origMode)
        {
            case FormModeEnum.Update:
                if (DocumentManager.AutoCheck)
                {
                    if ((originalStep == null) || originalStep.StepIsArchived || originalStep.StepIsPublished)
                    {
                        refreshTree = true;
                    }
                }

                // Document properties changed
                refreshTree |= ((originalDocumentName != Node.DocumentName) || (wasInPublishedStep != Node.IsInPublishStep) || (wasArchived != Node.IsArchived)
                                         || (((originalPublishFrom != Node.DocumentPublishFrom) || (originalPublishTo != Node.DocumentPublishTo))
                                             && (DocumentManager.AutoCheck || (WorkflowManager.GetNodeWorkflow(Node) == null))));

                break;

            case FormModeEnum.Insert:
            case FormModeEnum.InsertNewCultureVersion:
            case FormModeEnum.Convert:

                TreeNode node = Node;

                // Handle new culture version for linked documents from different site
                int sourceNodeId = QueryHelper.GetInteger("sourcenodeid", 0);
                if (sourceNodeId > 0)
                {
                    node = node.TreeProvider.SelectSingleNode(TreeProvider.ALL_SITES, null, node.DocumentCulture, false, node.NodeClassName, "NodeID = " + sourceNodeId, null, -1, false);
                }

                // Create another document
                if (DocumentManager.CreateAnother)
                {
                    string refreshScript = string.Empty;

                    // Call RefreshTree() only when not in a modal dialog, otherwise the dialog would be closed
                    if (!isDialog)
                    {
                        refreshScript = "RefreshTree(" + node.NodeParentID + "," + node.NodeParentID + ");";
                    }

                    refreshScript += " CreateAnother();";
                    AddScript(refreshScript);
                }
                else
                {
                    // Refresh frame in split mode
                    if ((origMode == FormModeEnum.InsertNewCultureVersion) && UIContext.DisplaySplitMode && (CultureHelper.GetOriginalPreferredCulture() != node.DocumentCulture))
                    {
                        AddScript("SplitModeRefreshFrame();");
                    }
                    else
                    {
                        string refreshScript = null;
                        string tab = "Page";

                        // If not menu item type nor EditLive view mode, switch to form mode to keep editing the form
                        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (PortalContext.ViewMode != ViewModeEnum.EditLive))
                        {
                            refreshScript = String.Format("SetMode('{0}', true);", ViewModeEnum.EditForm);

                            PortalContext.ViewMode = ViewModeEnum.EditForm;
                            tab = "EditForm";
                        }

                        refreshScript += String.Format("RefreshTree({0}, {0}); SelectNode({0}, null, '{1}');", node.NodeID, tab);

                        // Document tree is refreshed and new document is displayed
                        AddScript(refreshScript);
                    }
                }
                break;
        }

        return refreshTree;
    }

    #endregion
}

