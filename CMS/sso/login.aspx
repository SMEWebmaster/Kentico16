<%@ Page Language="C#" AutoEventWireup="true" CodeFile="login.aspx.cs" Inherits="login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Literal ID="lit1" runat="server"/>
            <asp:HyperLink ID="lnkLogin" CssClass="logInOut" runat="server">Login</asp:HyperLink>
            <asp:HyperLink ID="lnkLogout" Visible="false" runat="server">Logout</asp:HyperLink>
            <asp:Panel ID="LoggedinSystems" runat="server" Visible="false">
                <div>
                    <h1>Personify pages</h1>
                    <a  target="_blank" href="http://smemitst.personifycloud.com/PersonifyEbusiness/default.aspx">Personify pages</a>
                </div>
                
                 <div>
                    <h1>ONE MINE</h1>
                    <a target="_blank" href="/sso/redirect.aspx?code=OM">Redirect To One Mine Pages</a>
                </div>
                <div>
                    <h1>Higher logic </h1>
                    <!--/sso/redirect.aspx?code=HL-->
                    <a href="#" target="_blank" >Redirect To Higher logic Pages</a>
                    <div id="previewWidget"></div>
                    <script type="text/javascript" src="https://api.connectedcommunity.org/widgetscripts/widgets/hlwidgetcommon.js">
<script type="text/javascript" src="https://api.connectedcommunity.org/widgetscripts/widgets/hlwidgetcommon.js"></script>
                    <script type="text/javascript" src="http://api.connectedcommunity.org/widgetscripts/widgets/latestDiscussion.js"></script>
                    <script type="text/javascript">
                        document.addEventListener('DOMContentLoaded', function () {
                            hl.latestDiscussion('previewWidget', {
                                discussionKey: '',
                                maxToRetrieve: '3',
                                subjectLength: '50',
                                contentLength: '160',
                                moreUrl: 'http://i.sme.org',
                                showLoginStatus: '1',
                                loginUrl: 'http://i.sme.org/HigherLogic/Security/LoginBounce.aspx?dialog=1',
                                domainUrl: 'http://i.sme.org',
                                cbUseBioBubble: '0',
                                includeStaff: '1',
                                HLIAMKey: '194e4c0b-1dae-45c5-8d6e-749f12b21e8f'
                            });
                        });
                    </script>
                </div>

            </asp:Panel>


        </div>
    </form>
</body>
</html>
