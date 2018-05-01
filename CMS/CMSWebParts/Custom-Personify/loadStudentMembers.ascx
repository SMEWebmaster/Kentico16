<%@ Control Language="C#" AutoEventWireup="true" CodeFile="loadStudentMembers.ascx.cs" Inherits="CMSWebParts_Custom_Personify_loadmembershipDetailst" %>

 <div class="awards-content">
        <ul class="committeeList">
  
        <li>
            <h3 class="awards-title"><asp:Literal id="litTitle" runat="server"/>  COMMITTEES</h3>
                            <asp:Repeater ID="rptSub" runat="server">
                                <HeaderTemplate>
                                    <div class="slide">
                                    <ul class="committeeList" style="padding-top:1em; padding-bottom: 1em;">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li class="view-more-new"><a href="/listing?ID=<%# Eval("MasterCustomerId")%>"><%# Eval("Labelname")%></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </div>
                                </FooterTemplate>
                            </asp:Repeater>
        </li>
    
  
        </ul>
    </div>

<style type="text/css">
.learn-more a:before
{
content: ' ' !important; 
}
</style>
