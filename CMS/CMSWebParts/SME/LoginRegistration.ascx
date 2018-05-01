<%@ Control ClientIDMode="Static" Language="C#" AutoEventWireup="false" CodeFile="LoginRegistration.ascx.cs" Inherits="CMSWebParts_SME_LoginRegistration" %>

<asp:Panel id="divLoginRegisration" runat="server">
    <asp:Panel ID="pnlLogin" runat="server" title="Secure Sign In" CssClass="login-section" DefaultButton="btnLogin">
        <asp:UpdatePanel ID="upLogin" runat="server" ChildrenAsTriggers="true">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnLogin" />
            </Triggers>
            <ContentTemplate>
                <h3>Sign In:</h3>
                <p>
                    <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server">Username / Email Address:</asp:Label>
                    <asp:TextBox id="txtUsername" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvUsername" ControlToValidate="txtUserName" EnableClientScript="true" Display="Dynamic" runat="server" ErrorMessage="Username is required" ValidationGroup="Login" ForeColor="Red"/>
                </p>
                <p>
                    <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server">Password:</asp:Label>
                    <asp:TextBox id="txtPassword" TextMode="Password" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPassword" EnableClientScript="true" ErrorMessage="Password is required" ValidationGroup="Login" ForeColor="Red" />
                    <asp:CustomValidator ID="cvPassword" runat="server" Display="Dynamic" ControlToValidate="txtPassword" ValidationGroup="Login" ForeColor="Red" ErrorMessage="Invalid username or password" />
                </p>
                <p class="remember-me">
                    <input type="checkbox" checked="True" runat="server" id="ckRememberMe" /> <label style="display: inline" for="ckRememberMe">Remember Me?</label>
                </p>
                <div class="submit buttons">
                    <asp:LinkButton id="btnLogin" CssClass="btn btn-primary" runat="server" Text="Sign In" ValidationGroup="Login" />
                </div>
                <p>
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
    <asp:Panel ID="pnlRegister" runat="server" CssClass="register-section">
        <h3>Become a Member:</h3>
        <div>
            <div class="submit buttons">
                <a class="btn btn-primary" href="~/memberredirect/default.aspx?url/PersonifyEbusiness/Default.aspx?TabID=335" runat="server">Join Now!</a>
            </div>
            <p>
                Nec assum dignissim assueverit at. Ne duo dolorum sensibus. Has ut purto consequat, cum et putant percipitur
            </p>
        </div>
        <h3>Register:</h3>
        <div>
            <div class="submit buttons">
                <a class="btn btn-primary" href="http://smemi.personifycloud.com/SSO/Register.aspx" runat="server">Register</a>
            </div>
            <p>
                Idque mundi ut vim, eu omnes consequuntur eos, usu ex hinc gloriatur. Utinam vivendum sit et, nam at dicit ceteros imperdiet.
            </p>
        </div>
    </asp:Panel>
</asp:Panel>