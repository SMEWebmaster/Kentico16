<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>
<%@ Register Src="~/CMSFormControls/PersonifyAddress.ascx" TagName="PersonifyAddress" TagPrefix="sme" %>
<%@ Register Src="~/CMSFormControls/Demographics/CertificationsSelector.ascx" TagName="CertificationsSelector" TagPrefix="sme" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <sme:PersonifyAddress ID="PersonifyAddress1" runat="server" />
        <sme:CertificationsSelector ID="CertificationsSelector1" runat="server" />
    </div>
    </form>
</body>
</html>
