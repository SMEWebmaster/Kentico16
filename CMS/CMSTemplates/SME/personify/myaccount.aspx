<%@ Page Title="" Language="C#" MasterPageFile="~/CMSTemplates/SME/Masterpages/main.master" AutoEventWireup="true" CodeFile="myaccount.aspx.cs" Inherits="CMSTemplates_SME_personify_myaccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
  <div class="personify-my-account">
    <div class="container">
        <asp:PlaceHolder ID="phPersonifyControl" runat="server"></asp:PlaceHolder>
    </div>
    <div class="container">
        <asp:PlaceHolder ID="phPersonifyControl2" runat="server"></asp:PlaceHolder>
    </div>
    <div class="container">
        <asp:PlaceHolder ID="phPersonifyControl3" runat="server"></asp:PlaceHolder>
    </div>
  </div>
</asp:Content>

