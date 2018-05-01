using CMS.Helpers;
namespace CMSAppAppCode.SME
{
    /// <summary>
    /// ant for Right Mobile.
    /// </summary>
    public class SMEConstant
    {
        #region "Role Properties"
        /* Roles Strings start */
        /// <summary>
        /// Get set the admin role class
        /// </summary>
        public static string ROLE_ADMIN
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.AdminRoleClassName");
            }
        }
        #endregion

        #region "Workflow Status"
        /* Workflow strings start */
        /// <summary>
        /// Get set workflow status archived.
        /// </summary>
        public static string WORKFLOW_ARCHIVED
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStatusArchived");
            }
        }
        /// <summary>
        /// Get set workflow status approval.
        /// </summary>
        public static string WORKFLOW_APPROVAL
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStatusApproval");
            }
        }
        /// <summary>
        /// Get set workflow status published.
        /// </summary>
        public static string WORKFLOW_PUBLISHED
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStatusPublished");
            }
        }
        /// <summary>
        /// Get set workflow status draft.
        /// </summary>
        public static string WORKFLOW_DRAFT
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStatusDraft");
            }
        }
        /// <summary>
        /// Get set workflow status rejected.
        /// </summary>
        public static string WORKFLOW_REJECTED
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStatusRejected");
            }
        }
        /// <summary>
        /// Get set workflow statename edit.
        /// </summary>
        public static string WORKFLOW_STEPNAME_EDIT
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.WorkflowStepNameEdit");
            }
        }
        /* Workflow strings end */
        #endregion

        #region "Document action event name"
        /*Contributionlist cloned header actions strings start*/
        /// <summary>
        /// Get set action event name.
        /// </summary>
        public static string ACTIONEVENTNAME_DOCUMENTCREATEVERSION
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.ActionEventNameDocumentCreateVersion");
            }
        }
        /// <summary>
        /// Get set action event name.
        /// </summary>
        public static string ACTIONEVENTNAME_DOCUMENTARCHIVE
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.ActionEventNameDocumentArchive");
            }
        }
        /// <summary>
        /// Get set action event name.
        /// </summary>
        public static string ACTIONEVENTNAME_DOCUMENTAPPROVE
        {
            get
            {
                return "DocumentApprove";
                //Get the value from localization string.
                return ResHelper.GetString("SME.ActionEventNameDocumentApprove");
            }
        }
        /// <summary>
        /// Get set action event name.
        /// </summary>
        public static string ACTIONEVENTNAME_DOCUMENTSAVE
        {
            get
            {
                //Get the value from localization string.
                return ResHelper.GetString("SME.ActionEventNameSave");
            }
        }
        /*Contributionlist cloned header actions strings start*/
        #endregion

        public const string CUSTOM_APPROVAL_AWAIT = "Awaiting Approval";
    }
}