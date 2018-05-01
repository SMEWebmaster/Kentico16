<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Viewers_Documents_cmsdatagridcustom" CodeFile="~/CMSWebParts/Viewers/Documents/cmsdatagridcustom.ascx.cs" %>
<cms:LocalizedDropDownList ID="ddlEventType" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlEventType_SelectedIndexChanged" />
<asp:HiddenField ID="hdnWhere" runat="server" />
<cms:CMSEditModeButtonAdd ID="btnAdd" runat="server" />
<cms:CMSDataGrid ID="gridElem" EnableViewState="true" runat="server" />&nbsp;
