<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Product_Edit_Variants.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Variants"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" HideTitle="true" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="header-panel">
                        <cms:LocalizedButton runat="server" ID="btnCancel" ButtonStyle="Primary" EnableViewState="false"
                            ResourceString="general.cancel" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" MaxLogLines="1000" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content1" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
                <cms:UniGrid runat="server" ID="ugVariants" ShortID="uv" ObjectType="ecommerce.skuvariant"
                    IsLiveSite="false" Columns="SKUID, SKUName, SKUNumber, SKUPrice, SKUAvailableItems, SKUEnabled, SKUSiteID, SKUReorderAt" OrderBy="SKUName" FilterLimit="1" ShowObjectMenu="false">
                    <GridActions>
                        <ug:Action Name="edit" ExternalSourceName="SKUID" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
                        <ug:Action Name="delete" ExternalSourceName="SKUID" Caption="$General.Delete$" FontIconClass="icon-bin"
                            FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
                    </GridActions>
                    <GridColumns>
                        <ug:Column Source="SKUID" Wrap="false" Visible="false" />
                        <ug:Column Source="##ALL##" Name="SKUName" ExternalSourceName="skuname" Caption="$general.name$" Wrap="false" AllowSorting="true" Sort="SKUName" />
                        <ug:Column Source="##ALL##" Name="SKUNumber" ExternalSourceName="skunumber" Caption="$com.sku.skunumber$" Wrap="false" Sort="SKUNumber" AllowSorting="true" />
                        <ug:Column Source="##ALL##" Name="SKUPrice" ExternalSourceName="skuprice" Caption="$product_list.productprice$" Wrap="false" CssClass="TextRight" AllowSorting="true" Sort="SKUPrice" />
                        <ug:Column Source="##ALL##" Name="SKUAvailableItems" ExternalSourceName="skuavailableitems" Caption="$product_list.productavailableitems$" Wrap="false" CssClass="TextRight" />
                        <ug:Column Source="SKUEnabled" ExternalSourceName="#yesno" Caption="$com.sku.enabled$" Wrap="false" />
                        <ug:Column CssClass="filling-column" />
                    </GridColumns>
                    <GridOptions DisplayFilter="true" FilterPath="~/CMSModules/ECommerce/Controls/Filters/ProductVariantFilter.ascx" ShowSelection="true" SelectionColumn="SKUID" />
                </cms:UniGrid>
                <asp:Panel ID="pnlFooter" runat="server" Visible="false" CssClass="form-horizontal mass-action  dont-check-changes">
                    <div class="form-group">
                        <div class="mass-action-value-cell">
                            <cms:CMSDropDownList ID="drpWhat" runat="server" AutoPostBack="true" />
                            <cms:CMSDropDownList ID="drpAction" runat="server" OnSelectedIndexChanged="drpAction_SelectedIndexChanged" AutoPostBack="true" />
                            <asp:Panel runat="server" ID="pnlNewPrice" Visible="false" CssClass="control-group-inline">
                                <cms:LocalizedLabel ID="lblNewPrice" runat="server" ResourceString="com.variant.newprice"
                                    AssociatedControlID="txtNewPrice" CssClass="form-control-text input-label" />
                                <cms:CMSTextBox ID="txtNewPrice" runat="server" CssClass="input-price" />
                            </asp:Panel>
                            <cms:CMSButton ID="btnOk" runat="server" OnClick="btnOk_Clicked" ButtonStyle="Primary" />
                        </div>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>