<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SME_SMECalendar" CodeFile="~/CMSWebParts/SME/SMECalendar.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>

<asp:Panel ID="smeCalendar" runat="server">
    <div class="">
        <cms:LocalizedDropDownList ID="ddlEventType" CssClass="category-dropdown" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlEventType_SelectedIndexChanged" />
        <asp:HiddenField ID="hdnWhere" runat="server" />
    </div>
    <div class="sme-calendar">
        <cms:UniGrid ID="gridCalendar" ZeroRowsText="Foo" runat="server" ShowActionsMenu="false" GridName="~/CMSWebParts/SME/CalendarList.xml" IsLiveSite="false" PageSize="0"></cms:UniGrid>
    </div>
</asp:Panel>