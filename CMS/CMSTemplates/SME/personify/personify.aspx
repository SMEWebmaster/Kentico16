<%@ Page Title="" Language="C#" MasterPageFile="~/CMSTemplates/SME/Masterpages/main.master" AutoEventWireup="true" CodeFile="personify.aspx.cs" Inherits="CMSTemplates_SME_personify_personify" %>

<%@ Register Src="~/CMSTemplates/SME/testpersonify.ascx" TagPrefix="uc1" TagName="testpersonify" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:testpersonify runat="server" ID="testpersonify" />
</asp:Content>


