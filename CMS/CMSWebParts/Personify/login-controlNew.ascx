<%@ Control Language="C#" AutoEventWireup="true" CodeFile="login-controlNew.ascx.cs" Inherits="LoginControl" %>

<div class='personify-login-bucket'>

    <asp:Panel runat="server" ID="pnlSiteLogin" DefaultButton="LoginButton">
        <style type="text/css">
            .registration_form {
                padding: 0 0 50px 0;
            }

                .registration_form input[type="text"], .registration_form input[type="name"], .registration_form input[type="email"], .registration_form input[type="password"], .registration_form textarea {
                    width: 96%;
                    padding: 0 2%;
                }

                .registration_form select {
                    background-position: 98% center;
                }

            .content-block {
                position: absolute;
                right: -141px;
                top: 9px;
            }

                .content-block > img {
                    display: inline-block;
                    float: left;
                }

                .content-block p {
                    border: 1px solid #000;
                    border-radius: 4px;
                    display: inline-block;
                    float: right;
                    font-size: 11px;
                    margin: -19px 0 0 6px;
                    padding: 0;
                    text-align: center;
                    width: 99px;
                }

                .content-block p {
                    visibility: hidden;
                }

            .registration_form .editing-form-value-cell {
                position: relative;
            }

            @media only screen and (max-width:767px) {
                .article-page-content {
                    padding: 50px 5% 20px;
                }

                .registration_form .container {
                    padding: 0;
                }

                .registration_form .row {
                    margin: 0;
                }
            }
        </style>
        <h3>Sign In</h3>
        <div class="body">
            <div class="col-sm-8">
                <div class="form-horizontal">
                    <div class="form-group col-sm-12">
                        <div class="editing-form-label-cell">
                            <asp:Label ID="lblUserName" runat="server" AssociatedControlID="txtUsername" Text="Email Address" />
                        </div>
                        <br />
                        <div class="editing-form-value-cell">
                            <cms:CMSTextBox ID="txtUsername" runat="server" MaxLength="100" ClientIDMode="Static" ValidationGroup="Login" />
                            <asp:RequiredFieldValidator ID="rfUsername" runat="server" ControlToValidate="txtUsername" Display="Dynamic" ValidationGroup="Login" ForeColor="Red" Text="Required" />
                            <div class="content-block">
                                <img src="~/App_Themes/SME-Styles/images/question.png"><p>The Email address you have on SME File</p>
                            </div>

                        </div>
                        <br />
                    </div>

                    <div class="form-group col-sm-12">
                        <div class="editing-form-label-cell">
                            <asp:Label ID="lblPassword" AssociatedControlID="txtPassword" runat="server" Text="Password" />
                        </div>
                        <br />
                        <div class="editing-form-value-cell">
                            <asp:TextBox ID="txtPassword" runat="server" MaxLength="110" ValidationGroup="Login" TextMode="Password" ClientIDMode="Static" />
                            <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" Display="Dynamic" ValidationGroup="Login" ForeColor="Red" Text="Required" />
                            <asp:CustomValidator ID="cvUsernamePassword" runat="server" ControlToValidate="txtPassword" ForeColor="Red" Display="Dynamic" ValidationGroup="Login" Text="Invalid username/password combination." />
                            <div class="content-block">
                                <img src="~/App_Themes/SME-Styles/images/question.png"><p>Don't remember? Click forgot login.</p>
                            </div>

                        </div>
                        <br />
                    </div>

                </div>
                <br />
                <div class="remember-me">
                    <input type="checkbox" class="check" id="chkRememberMe" runat="server" />
                    Remember Me
                </div>
                <br />
                <div class="submit btn-realistic"><asp:LinkButton  ID="LoginButton" runat="server" CommandName="Login" ValidationGroup="Login" ClientIDMode="Static">Sign In</asp:LinkButton></div>
                <div style="float:left">&nbsp;&nbsp;</div>
                <div class="submit btn-realistic blue"><asp:HyperLink ID="ForgotButton" runat="server" NavigateUrl="/ForgotPassword" ClientIDMode="Static">Forgot Login</asp:HyperLink></div>
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="phErrorMessage" Visible="false">
            <ul>
                <li class="login-notice" style="color: red;">
                    <i class="fa fa-bell"></i>
                    <asp:Literal runat="server" ID="litError" />
                </li>
            </ul>
        </asp:PlaceHolder>
    </asp:Panel>


    <asp:Panel ID="pnlLoggedInMember" runat="server" Visible="False">

        <script type="text/javascript">
            function redirectToMyAccount() {
                try {
                    var gaAllTags = ga.getAll();
                    console.log('GA loaded')
                    window.location.href = "/memberredirect/default.aspx?returnurl=/personifyebusiness/MyAccount.aspx";
                } catch (e) {
                    console.log('Analytics not loaded yet: ' + e.toString());
                    var myVar = setTimeout(redirectToMyAccount, 100);
                }
            }

            setTimeout(redirectToMyAccount, 100);
        </script>

    </asp:Panel>
</div>

<script type="text/javascript">
    $(document).ready(function () {
        $('.content-block img').mouseenter(function () {

            $(this).siblings('p').css('visibility', 'visible')
        })
        $('.content-block img').mouseleave(function () {

            $(this).siblings('p').css('visibility', 'hidden')
        })
        $('#LoginButton').click(function () {
            if ($('#txtUsername').val() == "" || $('#txtPassword').val() == "") {
                $('.req').css('display', 'block')
                return false;
            }
            else {
                $('.req').css('display', 'none');
                var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
                if (!pattern.test($('#txtUsername').val())) {
                    $('.invldemail').css('display', 'block')
                    return false;
                }
                return true;
            }
        });

    });
</script>
