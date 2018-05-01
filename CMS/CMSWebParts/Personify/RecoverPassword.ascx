<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RecoverPassword.ascx.cs" Inherits="CMSWebParts_Personify_RecoverPassword" %>

<div class="form-group col-sm-12">
    <div class="editing-form-label-cell col-sm-3">&nbsp;</div>
    <div class="editing-form-label-cell col-sm-9"><cms:MessagesPlaceholder ID="plcMess" runat="server" InfoTimeout="10000" IsLiveSite="false" /></div>
</div>
<asp:Panel ID="pnlReset" runat="server" DefaultButton="lbSubmit">
        <div class="form-group col-sm-12">
            <div class="editing-form-label-cell col-sm-3"><label for="txtEmailAddress">Email:</label></div>

            <div class="editing-form-value-cell col-sm-9">
                <asp:TextBox ID="txtEmailAddress" ClientIDMode="Static" ValidationGroup="RecoverPassword" runat="server"></asp:TextBox><br />
                <asp:RequiredFieldValidator ID="rfvEmailAddress" ValidationGroup="RecoverPassword" runat="server" ControlToValidate="txtEmailAddress" ForeColor="Red" ErrorMessage="Required" Display="Dynamic" />
            </div>
        </div>

        <div class="form-group col-sm-12">
            <div class="editing-form-label-cell col-sm-3">&nbsp;</div>

            <div class="editing-form-value-cell col-sm-9">
                <div class="submit btn-realistic">
                    <asp:LinkButton ID="lbSubmit" Text="Send Password Reset" runat="server" ValidationGroup="RecoverPassword"><span>Send Password Reset</span></asp:LinkButton>
                </div>
                <div style="float:left">&nbsp;&nbsp;</div>
                <div class="submit btn-realistic blue">
                    <asp:HyperLink ID="hlCancel" runat="server" NavigateUrl="~/" ><span>Cancel</span></asp:HyperLink>
                </div>
            </div>
        </div>
</asp:Panel>

