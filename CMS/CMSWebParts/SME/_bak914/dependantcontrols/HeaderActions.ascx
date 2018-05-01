<%@ Control Language="C#" AutoEventWireup="true" CodeFile="HeaderActions.ascx.cs" Inherits="CMSWebParts_SME_DependentControls_HeaderActions" %>

<asp:PlaceHolder ID="plcMenu" runat="server">
	<cms:CMSUpdatePanel ID="pnlUp" runat="server" UpdateMode="Conditional">
		<ContentTemplate>
			<asp:Panel ID="pnlActions" runat="server" Visible="true" EnableViewState="false"
				CssClass="btn-actions">
			</asp:Panel>
			<asp:Panel ID="pnlAdditionalControls" runat="server" Visible="true" EnableViewState="false"
				CssClass="dont-check-changes">
				<asp:PlaceHolder ID="plcAdditionalControls" runat="server" />
			</asp:Panel>
			<asp:Panel runat="server" Visible="false" ID="pnlClear" CssClass="clearfix">
			</asp:Panel>
		</ContentTemplate>
	</cms:CMSUpdatePanel>
</asp:PlaceHolder>
<script>
    function formValid() {
        var isValid = true;

        if ($(".eventname").val() == "") {
            isValid = false;
            alert("You must enter a MEETING / EVENT NAME - this is a BRIEF title for your event.");
        }

        if ($(".eventdescription").val() == "") {
            isValid = false;
            alert("You must enter a brief description of event ");
        }

        if ($(".eventstartdate").val() == "") {
            isValid = false;
            alert("You must enter a valid START date for the event (for example 01/15/2009).");
        }

        if (!ValidateEmail($(".eventemail").val())) {
            isValid = false;
            alert("You must provide a VALID E-MAIL ADDRESS.");
        }

        if (isValid == false)
            return isValid;
        else
            alert('Thank you for submitting Event');
    }
    </script>