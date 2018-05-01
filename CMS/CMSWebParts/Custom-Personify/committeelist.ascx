<%@ Control Language="C#" AutoEventWireup="true" CodeFile="committeelist.ascx.cs" Inherits="CMSWebParts_Custom_Personify_committeelist" %>
  <h2><asp:Literal ID="committeeName" runat="server"></asp:Literal></h2>
<asp:Repeater ID="rptparent" runat="server">
                 <ItemTemplate>
               <h4> <%# Eval("CommitteeLabelName")%></h4>
                     </ItemTemplate>
                </asp:Repeater>

            <asp:Repeater ID="rptSub" runat="server">
                <HeaderTemplate>
                    <!-- Table goes in the document BODY -->
                    <table class="alt-rows" id="alt-color">
                        <tr>
                            <th>Name</th>
                            <th>Position</th>
                            <th>Start</th>
                            <th>End</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>   <%# Eval("CommitteeMemberLastFirstName")%></td>
                        <td>    <%#Eval("PositionSubcodeString") != null && Eval("PositionSubcodeString") != "" ? Eval("PositionSubcodeString") + "&nbsp;" : "" %> <%# Eval("PositionCodeDescriptionDisplay")%></td>
                        <td> 
                                <%# Eval("Begindate", "{0:MM/dd/yyyy}") %>
                        </td>
                        <td> 
                                   <%# Eval("Enddate", "{0:MM/dd/yyyy}") %>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>