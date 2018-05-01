using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[Title("optioncategory_edit.itemlistlink")]
[UIElement(ModuleName.ECOMMERCE, "Products.OptionCategories")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Options : CMSProductsPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "xoBsAw";

    #endregion


    #region "Variables"

    private SKUInfo sku;

    #endregion


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ucOptions.OnCheckPermissions += ucOptions_OnCheckPermissions;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup help
        object options = new
        {
            helpName = "lnkProductEditHelp",
            helpUrl = UIContextHelper.GetDocumentationTopicUrl(HELP_TOPIC_LINK)
        };
        ScriptHelper.RegisterModule(this, "CMS/DialogContextHelpChange", options);

        if (ProductID > 0)
        {
            sku = SKUInfoProvider.GetSKUInfo(ProductID);

            EditedObject = sku;

            if (sku != null)
            {
                // Check site ID
                CheckEditedObjectSiteID(sku.SKUSiteID);

                ucOptions.ProductID = ProductID;

                // Add new category button in HeaderAction
                CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("com.productoptions.select"),
                    OnClientClick = ucOptions.GetAddCategoryJavaScript(),
                    Enabled = ECommerceContext.IsUserAuthorizedToModifySKU(sku.IsGlobal)
                });

                // New button is active in editing of global product only if global option categories are allowed and user has GlobalModifyPermission permission
                bool enabledButton = (sku.IsGlobal) ? ECommerceSettings.AllowGlobalProductOptions(CurrentSiteName) && ECommerceContext.IsUserAuthorizedToModifyOptionCategory(true)
                    : ECommerceContext.IsUserAuthorizedToModifyOptionCategory(false);

                // Create new enabled/disabled category button in HeaderAction
                var dialogUrl = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/ProductOptions/OptionCategory_New.aspx?ProductID=" + sku.SKUID + "&dialog=1");
                dialogUrl = UIContextHelper.AppendDialogHash(dialogUrl);

                CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("com.productoptions.new"),
                    Enabled = enabledButton,
                    ButtonStyle = ButtonStyle.Default,
                    OnClientClick = "modalDialog('" + dialogUrl + "','NewOptionCategoryDialog', 1000, 800);"
                });
            }
        }
    }


    private void ucOptions_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (permissionType == CMSAdminControl.PERMISSION_MODIFY)
        {
            CheckProductModifyAndRedirect(EditedObject as SKUInfo);
        }
    }

    #endregion
}