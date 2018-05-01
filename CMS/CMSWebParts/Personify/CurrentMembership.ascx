<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Personify_CurrentMembership" CodeFile="~/CMSWebParts/Personify/CurrentMembership.ascx.cs" %>
<div class="container">
    <asp:PlaceHolder ID="phPersonifyControl" runat="server"></asp:PlaceHolder>
    
    Testing 
    <personify:CurrentMembership ID="cntrlId" runat="server" />
     <div class="personifyeditcontrols" style="display:none;">                
                <div><label>Additional CSS Class For Control:</label><asp:TextBox ID="txtCustomCssClass" runat="server" text="custom"></asp:TextBox>
                    <span>Set this class if using multiple controls of the same type and need finer CSS specification for a single control.</span></div>
<div><label>Error Message:</label><asp:DropDownList ID="ddlErrorMessage" runat="server"></asp:DropDownList><span>Error Message, default key is 'PersonifyErrorMessage'.  If key is blank and 'PersonifyErrorMessage' does not return a result, the default error message 'An error occurred while [ACTION]. If the problem persists please contact the site administrator.' will be used. [ACTION] will be substituted with whatever action the control was performing at the time of the error.</span></div>
<div><label>Title:</label><asp:TextBox ID="txtTitle" runat="server">Current Membership</asp:TextBox><span>The header text of the control.</span></div>
               
            </div>
</div>