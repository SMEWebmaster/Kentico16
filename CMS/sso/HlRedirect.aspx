<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HlRedirect.aspx.cs" Inherits="sso_HLlogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
 <iframe id="MyIframe" runat="server" 
src="http://smemi.personifycloud.com/SSO/Login.aspx?vi=9&vt=141a7874a9fecd5d8fb7309383f9ceb25ed5e8d412395e74cc1e3d1354156fc425d729a4f7f0727c0007c147f92f845c099bccc2b45fdae59e6561a3caedf3e60bd7414c76271e87fc3382e86a80f9aa26bfce44de23556b97757d9185de9619"
 scrolling="no" frameborder="0"   
style="border:none; overflow:hidden; width:100px; height:21px;" allowTransparency="true"></iframe>
    <a href="#" id="ahref" runat="server"  > Link </a>
    </div>
    </form>
</body>
</html>
