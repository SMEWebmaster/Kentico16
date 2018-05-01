<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSFormControls/Basic/SMEDirectUploadControl.ascx.cs"
    Inherits="CMSFormControls_Basic_SMEDirectUploadControl" %>
<%@ Register Src="~/CMSModules/Content/Controls/Attachments/DocumentAttachments/DirectUploader.ascx"
    TagName="DirectUploader" TagPrefix="cms" %>
<cms:DirectUploader ID="directUpload" runat="server" />
