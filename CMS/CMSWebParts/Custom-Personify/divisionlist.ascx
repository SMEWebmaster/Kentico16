<%@ Control Language="C#" AutoEventWireup="true" CodeFile="divisionlist.ascx.cs" Inherits="CMSWebParts_Custom_Personify_committeelist" %>
 
<h3><asp:Literal ID="litTitle" runat="server"/></h3>
      
                            <asp:Repeater ID="rptSub" runat="server">
                                <HeaderTemplate>
                                    <ul class="committeeList no-dots">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="view-more-new"><a href="/communitylisting?ID=<%# Eval("RelatedMasterCustomerId")%>"><%# Eval("RelatedName")%></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
       