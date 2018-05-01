<%@ Control Language="C#" AutoEventWireup="true" CodeFile="loadbyCommunities.ascx.cs" Inherits="CMSWebParts_Custom_Personify_loadCommunitesList" %>
 
        <h3><asp:Literal id="litTitle" runat="server"/>  COMMITTEES</h3>
                            <asp:Repeater ID="rptSub" runat="server">
                                <HeaderTemplate>
                                    <ul class="committeeList">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="view-more-new"><a href="/communitylisting?ID=<%# Eval("MasterCustomerId")%>"><%# Eval("Labelname")%></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
   