<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ChangePassword.ascx.cs" Inherits="CMSWebParts_Personify_ChangePassword" %>

<div class="form-group col-sm-12">
    <div class="editing-form-label-cell col-sm-3">&nbsp;</div>
    <div class="editing-form-label-cell col-sm-9"><cms:MessagesPlaceholder ID="plcMess" runat="server" InfoTimeout="10000" IsLiveSite="false" /></div>
</div>
<asp:Panel ID="pnlReset" runat="server" DefaultButton="lbSubmit">
    <div class="form-group col-sm-12">
        <div class="editing-form-label-cell col-sm-3"><label for="txtNewPassword">New Password:</label></div>

        <div class="editing-form-value-cell col-sm-9">
            <asp:TextBox ID="txtNewPassword" ClientIDMode="Static" ValidationGroup="ChangePassword" TextMode="Password" runat="server"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="rfvNewPassword" ValidationGroup="ChangePassword" runat="server" ControlToValidate="txtNewPassword" ForeColor="Red" ErrorMessage="Required" Display="Dynamic" />
            <asp:CustomValidator ID="cVNewPassword" ValidationGroup="ChangePassword" runat="server" ControlToValidate="txtNewPassword" ForeColor="Red" ErrorMessage="Please use a stronger password." Display="Dynamic" />
        </div>
    </div>

    <div class="form-group col-sm-12">
        <div class="editing-form-label-cell col-sm-3"><label for="txtConfirmPassword">Confirm Password:</label></div>

        <div class="editing-form-value-cell col-sm-9">
            <asp:TextBox ID="txtConfirmPassword" ClientIDMode="Static" ValidationGroup="ChangePassword" TextMode="Password" runat="server"></asp:TextBox><br />
            <asp:RequiredFieldValidator ID="rfvConfirmPassword" ValidationGroup="ChangePassword" runat="server" ControlToValidate="txtConfirmPassword" ForeColor="Red" ErrorMessage="Required" Display="Dynamic" />
            <asp:CompareValidator ID="cPassword" runat="server" ControlToValidate="txtConfirmPassword" ValidationGroup="ChangePassword" Display="Dynamic" ControlToCompare="txtNewPassword" ForeColor="Red" ErrorMessage="Passwords must match." />
        </div>
    </div>

    <div class="form-group col-sm-12">
        <div class="editing-form-label-cell col-sm-3">&nbsp;</div>

        <div class="editing-form-value-cell col-sm-9">
            <div class="submit btn-realistic"><asp:LinkButton ID="lbSubmit" runat="server" ValidationGroup="ChangePassword"><span>Save New Password</span></asp:LinkButton></div>
            <div style="float:left">&nbsp;&nbsp;</div>
            <div class="submit btn-realistic blue"><asp:HyperLink ID="hlCancel" runat="server" NavigateUrl="~/"><span>Cancel</span></asp:HyperLink></div>
        </div>
    </div>
</asp:Panel>

