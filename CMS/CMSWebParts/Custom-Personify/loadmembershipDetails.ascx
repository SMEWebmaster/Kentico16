<%@ Control Language="C#" AutoEventWireup="true" CodeFile="loadmembershipDetails.ascx.cs" Inherits="CMSWebParts_Custom_Personify_loadmembershipDetailst" %>


<asp:Repeater ID="rptmain" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <h3><%# Eval("ShortName")%>
        </h3>
        <p>
            <%# Eval("WebShortDescription")%>
        </p>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>

<asp:Repeater ID="rptSub" runat="server">
    <HeaderTemplate>
      
    </HeaderTemplate>
    <ItemTemplate>
        <p><%# Eval("RateCodeDescription")%> - <%# Eval("FormattedPrice")%></p>
        <span class="learn-more"  > 
<a href="~/memberredirect/default.aspx?url=PersonifyEbusiness/Membership/JoinSME/MembershipJoinRegistration.aspx?productId=<%# Eval("ProductId")%>">
         &nbsp;&nbsp;&nbsp; JOIN NOW &nbsp;&nbsp;&nbsp; </a></span>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
<style type="text/css">
.learn-more a:before
{
content: ' ' !important; 
}
</style>
