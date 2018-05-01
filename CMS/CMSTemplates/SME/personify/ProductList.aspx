<%@ Page Language="C#" MasterPageFile="~/CMSTemplates/SME/Masterpages/main.master" AutoEventWireup="true" CodeFile="ProductList.aspx.cs" Inherits="CMSTemplates_SME_personify_ProductList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="container">
        <asp:PlaceHolder ID="phPersonifyControl" runat="server"></asp:PlaceHolder>

        <div><label>Product Detail Url:</label><asp:DropDownList ID="ddlProductDetailUrl" runat="server"></asp:DropDownList><span></span></div>
    </div>
</asp:Content>
