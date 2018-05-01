<%@ Page Title="" Language="C#" MasterPageFile="~/CMSTemplates/Naifa/main.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="service_Default" %>

<%@ Register Src="~/CMSModules/Content/Controls/SearchDialog.ascx" TagName="SearchDialog"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/SmartSearch/Controls/SearchResults.ascx" TagName="SearchResults"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<div id="content-main" class="row-fluid">
<div class="container"><!-- content main container -->

sss  <div class="MenuBox">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:SearchDialog ID="searchDialog" runat="server" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <br />
    </div>
  <asp:Panel runat="server" ID="pnlBody">
        <asp:Panel ID="pnlResultsSQL" runat="server">
            <cms:CMSSearchResults ID="repSearchSQL" runat="server" Path="/%" CheckPermissions="true" CssClass="SearchResults" />
            <cms:SearchResults ID="repSmartSearch" runat="server" Path="/%" CheckPermissions="true" CssClass="SearchResults" />
        </asp:Panel>
    </asp:Panel>  
     </div>
     
     </div> 
</asp:Content>

