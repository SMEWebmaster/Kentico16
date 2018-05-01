<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ReferralSourceSelector.ascx.cs"
    Inherits="CMSFormControls_Personify_ReferralSourceSelector" %>
<%@ Register TagName="Selector" TagPrefix="personify" Src="~/CMSFormControls/Personify/PersonifyAppSubCodesSelector.ascx" %>

<personify:Selector ID="selector" AppCode="REFERRAL_SOURCE" Subsystem="CUS" Type="Demographic" runat="server" />