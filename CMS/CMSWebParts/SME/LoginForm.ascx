<%@ Control ClientIDMode="Static" Language="C#" AutoEventWireup="false" CodeFile="LoginForm.ascx.cs" Inherits="CMSWebParts_SME_LoginForm" %>

<asp:Panel ID="pnlLogin" runat="server" title="Secure Sign In" style="max-width: 435px" DefaultButton="btnLogin">
    <asp:UpdatePanel ID="upLogin" style="max-width: 435px" runat="server" ChildrenAsTriggers="true">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnLogin" />
        </Triggers>
        <ContentTemplate>
            <h3 style="max-width: 435px">Sign In:</h3>
            <p style="max-width: 435px">
                <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server">Username / Email Address:</asp:Label>
                <asp:TextBox id="txtUsername" runat="server" />
                <asp:RequiredFieldValidator ID="rfvUsername" ControlToValidate="txtUserName" EnableClientScript="true" Display="Dynamic" runat="server" ErrorMessage="Username is required" ValidationGroup="Login" ForeColor="Red"/>
            </p>
            <p style="max-width: 435px">
                <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server">Password:</asp:Label>
                <asp:TextBox id="txtPassword" TextMode="Password" runat="server" />
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPassword" EnableClientScript="true" ErrorMessage="Password is required" ValidationGroup="Login" ForeColor="Red" />
                <asp:CustomValidator ID="cvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPassword" ValidationGroup="Login" ForeColor="Red" ErrorMessage="Invalid username or password" />
            </p>
            <p class="remember-me" style="max-width: 435px">
                <input type="checkbox" checked="True" runat="server" id="ckRememberMe" /> <label style="display: inline" for="ckRememberMe">Remember Me?</label>
            </p>
            <div class="submit buttons">
                <asp:LinkButton id="btnLogin" CssClass="btn btn-primary" runat="server" Text="Sign In" ValidationGroup="Login" />
            </div>
            <p style="max-width: 435px">
                <a id="aForgotPassword" href="#">Forgot Password?</a>
            </p>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdateProgress ID="upLoginProgress" runat="server" AssociatedUpdatePanelID="upLogin">
        <ProgressTemplate>
            <div class="update-progress-panel">
                <img src="~/App_Themes/SME-Styles/images/Loading.gif" title="loading..." runat="server"/>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Panel>