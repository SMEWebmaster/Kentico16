<%@ Control Language="C#" AutoEventWireup="true" CodeFile="IndustrySelector.ascx.cs"
    Inherits="CMSFormControls_Personify_IndustrySelector" %>
<%@ Register TagName="Selector" TagPrefix="personify" Src="~/CMSFormControls/Personify/PersonifyAppSubCodesSelector.ascx" %>

<personify:Selector ID="selector" AppCode="INDUSTRY" Subsystem="CUS" Type="Demographic" runat="server" />