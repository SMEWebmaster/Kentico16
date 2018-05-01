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
        <p><%# Eval("FormattedPrice")%></p>
<a href="~/memberredirect/default.aspx?url=/PersonifyEbusiness/Store/ProductDetails.aspx?productId=<%# Eval("ProductId")%>">
      <img  src="/getmedia/f91643bc-f92f-4901-94fd-2c0194ff0e3d/Join-Now-Main.png.aspx?width=180&amp;height=45&amp;ext=.png" /></a>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </FooterTemplate>
</asp:Repeater>
