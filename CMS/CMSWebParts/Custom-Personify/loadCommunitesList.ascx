<%@ Control Language="C#" AutoEventWireup="true" CodeFile="loadCommunitesList.ascx.cs" Inherits="CMSWebParts_Custom_Personify_loadCommunitesList" %>
<asp:Repeater ID="rptMainCommitee" runat="server" OnItemDataBound="master_ItemDataBound">
    <HeaderTemplate>
        <div class="awards-content">
        <ul class="committeeList">
    </HeaderTemplate>
    <ItemTemplate>
        <li>
            <h1 class="awards-title"><%# Eval("Code").ToString().ToUpper().Replace("YOUNGLEADER","YOUNG LEADER")%>  COMMITTEES</h1>
                            <asp:Repeater ID="rptSub" runat="server">
                                <HeaderTemplate>
                                    <div class="slide">
                                    <ul class="committeeList" style="padding-top:1em; padding-bottom: 1em;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="view-more-new"><a href="/communitylisting?ID=<%# Eval("MasterCustomerId")%>"><%# Eval("Labelname")%></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </div>
                                </FooterTemplate>
                            </asp:Repeater>
        </li>
    </ItemTemplate>
    <FooterTemplate>
        </ul>
    </div>
    </FooterTemplate>
</asp:Repeater>
