<%@ Page Language="C#" AutoEventWireup="true" CodeFile="HLlogin.aspx.cs" Inherits="sso_HLlogin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
<div id="previewWidget"></div>
<script type="text/javascript" src="https://api.connectedcommunity.org/widgetscripts/widgets/hlwidgetcommon.js">
<script type="text/javascript" src="https://api.connectedcommunity.org/widgetscripts/widgets/hlwidgetcommon.js"></script>
<script type="text/javascript" src="http://api.connectedcommunity.org/widgetscripts/widgets/latestDiscussion.js"></script>
<script type="text/javascript">
document.addEventListener('DOMContentLoaded', function() {
hl.latestDiscussion('previewWidget', {
discussionKey:'',
maxToRetrieve:'3',
subjectLength:'50',
contentLength:'160',
moreUrl:'http://i.sme.org',
showLoginStatus:'1',
loginUrl:'http://i.sme.org/HigherLogic/Security/LoginBounce.aspx?dialog=1',
domainUrl:'http://i.sme.org',
cbUseBioBubble:'0',
includeStaff:'1',
HLIAMKey:'194e4c0b-1dae-45c5-8d6e-749f12b21e8f'
});
});
</script>

    </div>
    </form>
</body>
</html>
