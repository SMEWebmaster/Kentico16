<%@ Control Language="C#" AutoEventWireup="true" CodeFile="login-control.ascx.cs" Inherits="LoginControl" %>
 
    <asp:Literal id="litHead" runat="Server"/>
    
    <asp:Panel runat="server" ID="pnlSiteLogin" DefaultButton="LoginButton">
        <div class="heading"><a href="#">Login / My Profile</a></div>
        <div class="body">
            <%--<input type="text" name="Username" placeholder="Username" />--%>
            <cms:cmstextbox id="txtUsername" runat="server" maxlength="100" placeholder="Email Address" class="" />
            <%--<input type="text" name="Password" placeholder="Password" class=""/>--%>
            <asp:TextBox ID="txtPassword" runat="server" MaxLength="110" placeholder="Password" class="" TextMode="Password" />
            <div class="forgot"><a href="/ForgotPassword">Forgot?</a></div>
            <div class="buttons">
                <div class="submit">

                    <asp:LinkButton ID="LoginButton" runat="server" Text="Login" CommandName="Login" OnClick="btnLogin_Click" />
                </div>
                <div class="remember-me">
                    <input type="checkbox" class="check" id="chkRememberMe" runat="server" />
                    Remember Me
                </div>
                <div class="remember-me"><a href="/membership/join-or-renew">Not a Member? Join Now.</a></div>


                <asp:PlaceHolder runat="server" ID="phErrorMessage" Visible="false">
                    <li class="login-notice" style="color: red;">
                        <i class="fa fa-bell"></i>
                        <asp:Literal runat="server" ID="litError" />
                    </li>
                </asp:PlaceHolder>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlLoggedInMember" runat="server" Visible="False">

        <div class="heading"><a href="/membership/log-in-my-profile">Member Area</a></div>
        <div class="body">
            <asp:Label Text="" ID="lblLoggedin" runat="server" />
            <ul>
                <li><i class="fa fa-unlock-alt"></i>&nbsp;&nbsp;<a href="~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx" target="_self">My Account</a></li>

            <li><i class="fa fa-group"></i>&nbsp;&nbsp;<a href="~/memberredirect/default.aspx?url=/personifyebusiness/Membership/JoinSME.aspx" target="_self">Join SME</a></li>
            </ul>
            <div class="buttons">
                <div class="submit" id="divSubmit" runat="server">

                    <asp:LinkButton ID="btnLogout" runat="server" CssClass="readmore btn btn-primary" Text="Logout" OnClick="btnLogout_Click"></asp:LinkButton>
                </div>


            </div>
        </div>

<iframe id="MyIframe" runat="server" 
src="~/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx"
 scrolling="no" frameborder="0"  Visible="false"
style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe>
    </asp:Panel>

      <asp:Literal id="litFoot" runat="Server"/>