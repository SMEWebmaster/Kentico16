<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SME_FormControls_CategorySelector"
    CodeFile="CategorySelector.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniSelector ID="selectCategory" runat="server" ReturnColumnName="CategoryName"
            ObjectType="cms.categorylist" ResourcePrefix="categoryselector" OrderBy="CategoryNamePath"
            AdditionalColumns="CategoryNamePath,CategoryEnabled" SelectionMode="SingleDropDownList"
            AllowEmpty="true" IsLiveSite="false" AllowEditTextBox="true" DisabledItems="personal" WhereCondition="CategoryNamePath LIKE '/Events/%'" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
