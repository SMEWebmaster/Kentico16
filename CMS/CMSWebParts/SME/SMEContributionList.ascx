<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_SME_SMEContributionList" CodeFile="~/CMSWebParts/SME/SMEContributionList.ascx.cs" %>
<%@ Register Src="~/CMSWebParts/SME/ContributionList.ascx" TagName="ContributionList" TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/SME/EditForm.ascx" TagName="ContributionEdit" TagPrefix="cms" %>
<cms:ContributionList ID="list" runat="server" />


<link href="/CMSPages/GetResource.ashx?stylesheetname=CalendarStyles" type="text/css" rel="stylesheet"/>
<link href="/CMSPages/GetResource.ashx?stylesheetname=CalendarGrid" type="text/css" rel="stylesheet"/>
<link href="/CMSPages/GetResource.ashx?stylesheetname=CalendarFootable" type="text/css" rel="stylesheet"/>
<link href="/CMSPages/GetResource.ashx?stylesheetname=Footable.paginate" type="text/css" rel="stylesheet"/>

<!--<script src="/CMSScripts/Custom/SME/footable.js" type="text/javascript"></script>
<script src="/CMSScripts/Custom/SME/footable.sortable.js" type="text/javascript"></script>
<script src="/CMSScripts/Custom/SME/footable.paginate.calendar.js" type="text/javascript"></script>-->

<script src="/CMSScripts/Custom/SME/footable_1.js?v=2-0-1" type="text/javascript"></script>
    <script src="/CMSScripts/Custom/SME/footable.sort.js?v=2-0-1" type="text/javascript"></script>
    <script src="/CMSScripts/Custom/SME/footable.paginate.js?v=2-0-1" type="text/javascript"></script>

 
