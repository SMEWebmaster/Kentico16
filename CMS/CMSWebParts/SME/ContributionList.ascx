<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SME_ContributionList" CodeFile="ContributionList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/SME/EditForm.ascx" TagName="EditForm" TagPrefix="cms" %>



<script type="text/javascript">
    jQuery(function() {
        jQuery('table').find('> tbody > tr:not(.footable-row-detail):nth-child(even)').addClass('zebra');
        jQuery('.ContributionsEdit .DropDownField option').eq(0).text("Select One")
		jQuery('table').footable();
    });
</script>

<div class="container-calendar clearfix">
    <div class="calendar-wrapper">
        <asp:Panel ID="pnlList" CssClass="" runat="server">

            <cms:LocalizedLinkButton ID="btnNewDoc" runat="server" EnableViewState="false" OnClick="btnNewDoc_Click" CssClass="btn btn-default" />
            <cms:LocalizedLinkButton ID="btnUsersToBeApproved" runat="server" EnableViewState="false" OnClick="btnUsersToBeApproved_Click" CssClass="btn btn-default pull-right" Visible="false" Text="View Approval Pending Events list" />
            <cms:LocalizedLinkButton ID="btnCalendar" runat="server" EnableViewState="false" OnClick="btnCalendar_Click" CssClass="btn btn-default pull-right" Text="View Calendar" Visible="false" />

            <div class="sort" id="categoryDropdown" runat="server">
                <h5>Sort by:</h5>
                <div class="sort-by">
                    <cms:LocalizedDropDownList ID="ddlEventType" CssClass="category-dropdown" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ddlEventType_SelectedIndexChanged" />
                    <asp:HiddenField ID="hdnWhere" runat="server" />
                </div>
            </div>

            <asp:Repeater ID="rptEvents" runat="server">
                <HeaderTemplate>
                    <table data-filter="#filter" class="footable">
                        <thead>
                            <tr>
                                <th class="cal-header" data-class="expand" data-sort-initial="true" data-type="numeric">Dates</th>
                                <th class="cal-header" data-type="alphabetical">Event</th>
                                <th data-hide="phone,tablet" class="cal-header" data-type="alphabetical">Event Type</th>
                                <th data-hide="phone" class="cal-header" data-type="alphabetical">Location</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="cal-date" data-value="<%#Eval("DateTicks")%>">
                            <%# string.Format("{0:MMM dd, yyyy} ", Eval("StartDate")) %>
                        </td>
                        <td>
                            <h5>
                                <%# "<a href='"+Eval("NodeAliasPath")+"' style='font-size:13px;'>"+Eval("EventName")+"</a>" %>
                            </h5>
                            <p>
                                <%# Eval("EventDetails").ToString().Length>200?Eval("EventDetails").ToString().Substring(0,200) + "...<a href='"+Eval("NodeAliasPath")+"' style='font-size:12px;font-weight:400;'>Read More</a>":Eval("EventDetails")%>
                            </p>
                        </td>
                        <td><%# Eval("CategoryDisplayName").ToString()%></td>
                        <td><%# Eval("Location")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
					<tfoot class="hide-if-no-paging">
                        <tr>
                            <td colspan="5">
                                <div class="pagination pagination-centered"><span>Pages:</span></div>
                            </td>
                        </tr>
                    </tfoot>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <asp:Repeater ID="rptApprovalEvents" runat="server" Visible="false">
                <HeaderTemplate>
                    <table data-filter="#filter" class="footable">
                        <thead>
                            <tr>
                                <th class="cal-header" data-class="expand" data-sort-initial="true" data-type="numeric">Dates</th>
                                <th class="cal-header" data-type="alphabetical">Event</th>
                                <th data-hide="phone,tablet" class="cal-header" data-type="alphabetical">Event Type</th>
                                <th data-hide="phone,tablet" class="cal-header" data-type="alphabetical">Location</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="cal-date" data-value="<%#Eval("DateTicks")%>">
                            <%# string.Format("{0:MMM dd, yyyy}", Eval("StartDate")) %>
                        </td>
                        <td>
                            <h5>
                                <asp:LinkButton ID="view" runat="server" Visible="true" Style="font-size: 13px;" Text='<%#Eval("EventName")%>' OnClick="view_Click" CommandArgument='<%# Eval("NodeID") %>'></asp:LinkButton>

                            </h5>
                            <p>
                                <%# Eval("EventDetails").ToString().Length>200?Eval("EventDetails").ToString().Substring(0,200) + "...<a href='"+Eval("NodeAliasPath")+"' style='font-size:12px;font-weight:400;'>Read More</a>":Eval("EventDetails")%>
                            </p>
                        </td>
                        <td><%# Eval("CategoryDisplayName").ToString()%></td>
                        <td><%# Eval("Location")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
					<tfoot class="hide-if-no-paging">
                        <tr>
                            <td colspan="5">
                                <div class="pagination pagination-centered"><span>Pages:</span></div>
                            </td>
                        </tr>
                    </tfoot>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />


            <asp:Label ID="info" runat="server" Visible="false"></asp:Label>

        </asp:Panel>

    </div>

    <asp:Panel ID="pnlEdit" CssClass="ContributionsEdit" runat="server">
        <h2>
            <asp:Label ID="headingText" CssClass="HeadingInfo" runat="server" Text="Add Calendar Event"></asp:Label></h2>
        <cms:EditForm runat="server" ID="editDoc" />
        <div class="form-group">
            <div class="form-group">
                <div class="col-sm-offset-5 col-sm-5">
                    <cms:LocalizedLinkButton Visible="false" ID="btnList" runat="server" EnableViewState="false" OnClick="btnList_Click" CssClass="btn btn-danger" />
                </div>
            </div>
        </div>
    </asp:Panel>
	