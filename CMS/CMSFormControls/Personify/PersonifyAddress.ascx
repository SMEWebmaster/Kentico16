<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSFormControls_PersonifyAddress" CodeFile="PersonifyAddress.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector" TagPrefix="cms" %>
<div id="divCountry" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell col-sm-3">
        <asp:Label ID="lblCountry" runat="server" CssClass="EditingFormLabel" AssociatedControlID="drpCountry" Text="Country"></asp:Label>
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:DropDownList ID="drpCountry" runat="server" AutoPostBack="true" />
        </div>
        <br />
    </div>
</div>
<div id="divState" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell col-sm-3">
        <asp:Label ID="lblState" runat="server" CssClass="EditingFormLabel" AssociatedControlID="drpState" Text="State / Province"></asp:Label>
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:DropDownList ID="drpState" runat="server" />
        </div>
        <br />
    </div>
</div>
<div id="divAddress1" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell  col-sm-3">
        <asp:Label ID="lblAddress1" AssociatedControlID="txtAddress1" CssClass="EditingFormLabel" runat="server" Text="Address Line 1" />
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:TextBox ID="txtAddress1" runat="server" />
        </div>
        <br />
    </div>
</div>
<div id="divAddress2" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell col-sm-3">
        <asp:Label ID="lblAddress2" AssociatedControlID="txtAddress2" CssClass="EditingFormLabel" runat="server" Text="Address Line 2" />
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:TextBox ID="txtAddress2" runat="server" />
        </div>
        <br />
    </div>
</div>
<div id="divCity" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell col-sm-3">
        <asp:Label ID="lblCity" AssociatedControlID="txtCity" CssClass="EditingFormLabel" runat="server" Text="City" />
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:TextBox ID="txtCity" runat="server" />
        </div>
        <br />
    </div>
</div>
<div id="divPostalCode" class="form-group col-sm-12" runat="server">
    <div class="editing-form-label-cell col-sm-3">
        <asp:Label ID="lblPostalCode" AssociatedControlID="txtPostalCode" CssClass="EditingFormLabel" runat="server" Text="Postal Code" />
    </div>
    <div class="editing-form-value-cell col-sm-9">
        <div class="EditingFormControlNestedControl editing-form-control-nested-control">
            <asp:TextBox ID="txtPostalCode" runat="server" />
        </div>
        <br />
    </div>
</div>

