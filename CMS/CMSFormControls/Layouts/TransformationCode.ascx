<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TransformationCode.ascx.cs" Inherits="CMSFormControls_Layouts_TransformationCode" %>

<%@ Register Src="~/CMSAdminControls/UI/Macros/MacroEditor.ascx" TagName="MacroEditor" TagPrefix="cms" %>

<script type="text/javascript" language="javascript">
    function AddCssHandlerScript() {
        document.getElementById('editCss').style.display = 'block';
        document.getElementById('cssLink').style.display = 'none';
        return false;
    }
</script>
<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel runat="server" ID="lblType" ResourceString="DocumentType_Edit_Transformation_Edit.TransformType" AssociatedControlID="drpType" CssClass="control-label" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSDropDownList runat="server" ID="drpType" AutoPostBack="true" OnSelectedIndexChanged="drpTransformationType_SelectedIndexChanged"
                EnableViewState="true" />
            <span>
                <cms:LocalizedHyperlink ID="lnkHelp" runat="server" EnableViewState="false" CssClass="form-control-text" />
            </span>
        </div>
    </div>
    <div class="form-group">
        <asp:Panel runat="server" ID="pnlDirectives" CssClass="NORTL CodeDirectives">
            <asp:Label runat="server" ID="lblDirectives" EnableViewState="false" />
        </asp:Panel>
        <cms:CMSHtmlEditor runat="server" ID="tbWysiwyg" Width="98%" Height="300" Visible="false" />
        <cms:MacroEditor runat="server" ID="txtCode" ShortID="e" Visible="false" />
    </div>
    <div class="form-group">
        <asp:PlaceHolder runat="server" ID="plcCssLink">
            <div id="cssLink">
                <cms:LocalizedButton runat="server" ID="btnStyles" EnableViewState="false" ResourceString="general.addcss"
                    OnClientClick="return AddCssHandlerScript();" ButtonStyle="Default" />
            </div>
        </asp:PlaceHolder>
        <div id="editCss" style="<%=(plcCssLink.Visible ? "display: none": "")%>">
            <cms:LocalizedHeading runat="server" ID="lblCSS" ResourceString="Container_Edit.ContainerCSS" EnableViewState="false" CssClass="editing-form-category-caption" Level="4" IsAnchor="True" />
            <cms:ExtendedTextArea ID="txtCSS" runat="server" EditorMode="Advanced" Language="CSS" Width="99%" Height="200px" />
        </div>
    </div>
</div>
