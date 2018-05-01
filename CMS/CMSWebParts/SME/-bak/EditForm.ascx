<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SME_EditForm"
    CodeFile="EditForm.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/SME/DependentControls/editmenu.ascx" TagName="editmenu"
    TagPrefix="cms" %>
<asp:Panel runat="server" ID="pnlForm" CssClass="EditForm">
    <asp:Panel runat="server" ID="pnlTitle" CssClass="PageHeader" Visible="false">
        <cms:pagetitle id="titleElem" runat="server" setwindowtitle="false" />
    </asp:Panel>
    <cms:cmsdocumentmanager id="docMan" runat="server" />
    <asp:Panel runat="server" ID="pnlSelectClass" CssClass="PageContent">
        <strong>
            <asp:Label ID="lblInfo" runat="server" CssClass="ContentLabel" EnableViewState="false" />
        </strong>
        <br />
        <asp:Label ID="lblError" runat="server" CssClass="ContentError" ForeColor="Red" EnableViewState="false" />
        <br />
        <cms:unigrid id="gridClass" runat="server" gridname="~/CMSModules/Content/Controls/UserContributions/EditForm.xml"
            showactionsmenu="false" showobjectmenu="false" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlNewCulture" CssClass="PageContent" Visible="false">
        <strong>
            <asp:Label ID="lblNewCultureInfo" runat="server" CssClass="ContentLabel" /></strong><br />
        <br />
        <table>
            <tr>
                <td>
                    <cms:cmsradiobutton id="radEmpty" runat="server" groupname="NewVersion" checked="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:cmsradiobutton id="radCopy" runat="server" groupname="NewVersion" />
                </td>
            </tr>
            <tr id="divCultures" style="<%=(radCopy.Checked ? "display: block;": "display: none;")%>">
                <td>
                    <asp:Panel runat="server" ID="pnlCultures" CssClass="SoftSelectionBorder">
                        <cms:cmslistbox runat="server" id="lstCultures" datatextfield="DocumentCulture" datavaluefield="DocumentID"
                            cssclass="ContentListBoxLow" rows="7" />
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>&nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <cms:cmsbutton id="btnOk" runat="server" onclick="btnOK_Click" buttonstyle="Primary" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlEdit">
        <div class="clear"></div>
        <asp:Panel runat="server" ID="pnlWorkFlowInfo" CssClass="PageWorkFlowInfo" Visible="false">
            <cms:cmsdocumentpanel id="pnlDoc" runat="server" />
        </asp:Panel>
        <%--<div id="CKToolbarUC" style="clear: both;">
        </div>--%>
        <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
		
            <cms:cmsform runat="server" id="formElem" cssclass="UserContributionForm"
                htmlareatoolbarlocation="Out:CKToolbarUC" showokbutton="false" islivesite="true" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlMenu" CssClass="ContentEditMenu cms-bootstrap-js">
            <cms:editmenu id="menuElem" runat="server" showproperties="false" renderscript="false"
                showdelete="false" showspellcheck="false" showcreateanother="false" />
            <asp:Button ID="cancelBtn" runat="server" Text="Back" OnClick="cancelBtn_Click" CssClass="btn btn-danger"></asp:Button>
        </asp:Panel>
        <cms:cmsbutton id="btnDelete" runat="server" visible="false" enableviewstate="true" buttonstyle="Default" onclick="btnDelete_Click" />
        <cms:cmsbutton id="btnRefresh" runat="server" enableviewstate="true" buttonstyle="Default" usesubmitbehavior="false" onclick="btnRefresh_Click" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDelete" CssClass="PageContent" Visible="false">
        <strong>
            <asp:Label ID="lblQuestion" runat="server" CssClass="ContentLabel" EnableViewState="false" /></strong><br />
        <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="false" />
        <br />
        <asp:PlaceHolder ID="plcCheck" runat="server">
            <cms:cmscheckbox id="chkDestroy" runat="server" cssclass="ContentCheckbox" />
            <br />
            <cms:cmscheckbox id="chkAllCultures" runat="server" cssclass="ContentCheckbox" />
            <br />
            <br />
        </asp:PlaceHolder>
        <cms:cmsbutton id="btnYes" runat="server" buttonstyle="Default" onclick="btnYes_Click" />
        <cms:cmsbutton id="btnNo" runat="server" buttonstyle="Default" onclick="btnNo_Click" />
    </asp:Panel>
    <asp:Panel ID="pnlInfo" runat="server" CssClass="PageContent">
        <asp:Label ID="lblFormInfo" runat="server" EnableViewState="false" CssClass="ContentLabel" />
    </asp:Panel>
</asp:Panel>
