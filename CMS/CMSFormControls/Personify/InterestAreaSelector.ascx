<%@ Control Language="C#" AutoEventWireup="true" CodeFile="InterestAreaSelector.ascx.cs"
    Inherits="CMSFormControls_Personify_InterestAreaSelector" %>
<%@ Register TagName="Selector" TagPrefix="personify" Src="~/CMSFormControls/Personify/PersonifyAppSubCodesSelector.ascx" %>

<personify:Selector ID="selector" AppCode="INT_AREA" Subsystem="CUS" Type="Demographic" runat="server" />