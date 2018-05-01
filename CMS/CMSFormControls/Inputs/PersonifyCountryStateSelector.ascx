<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSFormControls_Inputs_TextBox" CodeFile="PersonifyCountryStateSelector.ascx.cs" %>
<cms:CMSTextBox ID="txtAddress1" runat="server" />
<cms:CMSTextBox ID="txtAddress2" runat="server" Visible="false" />
<cms:CMSDropDownList ID="drpCountry" runat="server" AutoPostBack="true" />
<cms:CMSDropDownList ID="drpState" runat="server" />