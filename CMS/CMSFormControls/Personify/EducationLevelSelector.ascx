<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EducationLevelSelector.ascx.cs"
    Inherits="CMSFormControls_Personify_EducationLevelSelector" %>
<%@ Register TagName="Selector" TagPrefix="personify" Src="~/CMSFormControls/Personify/PersonifyAppSubCodesSelector.ascx" %>

<personify:Selector ID="selector" AppCode="EDUC_LEVEL" Subsystem="CUS" Type="Demographic" runat="server" />