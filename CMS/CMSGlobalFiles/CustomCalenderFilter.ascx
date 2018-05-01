<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CustomCalenderFilter.ascx.cs" Inherits="CMSGlobalFiles_CustomCalenderFilter" %>
<table>
    <tr>
        <td style="padding: 2px">
            <cms:LocalizedLabel ID="lblDepartment" runat="server" Text="Product department" DisplayColon="true">
            </cms:LocalizedLabel>
        </td>
        <td style="padding: 2px">
            <cms:LocalizedDropDownList ID="drpDepartment" runat="server" Width="180">
            </cms:LocalizedDropDownList>
        </td>
    </tr>

    <tr>
        <td colspan="2" style="padding: 2px">
            <cms:LocalizedButton ID="btnFilter" runat="server" Text="Apply Filter" />
        </td>
    </tr>
</table>
