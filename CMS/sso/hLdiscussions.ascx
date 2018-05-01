<%@ Control Language="C#" AutoEventWireup="true" CodeFile="hLdiscussions.ascx.cs" Inherits="pages_controls_hLdiscussions" %>
<asp:Repeater ID="dataRepeater" runat="server">

    <ItemTemplate>
        <div class="discussions">
		<img src="<%# DataBinder.Eval(Container.DataItem, "Author.PictureUrl")%>">
            <h4>
                
                <a href="<%# GetURL((string)Eval("LinkToDiscussion")) %>" target="_blank">
                    <%# DataBinder.Eval(Container.DataItem, "DiscussionName")%>  </a></h4>
			<div class="disc-content">
            <p>
                By:
         <a href="<%# GetURL((string)Eval("Author.LinkToProfile")) %>" target="_blank">
             <%# DataBinder.Eval(Container.DataItem, "Author.DisplayName")%></a>
            </p>
            <p>
                <%# string.Format("{0:ddd MMM yyyy}", Eval("Author.UpdatedOn")).Substring(0,10)%>
            </p>
            <p>
                Posted in:
         <a href="<%# GetURL((string)Eval("LinkToMessage")) %>" target="_blank">
             <%# DataBinder.Eval(Container.DataItem, "Subject")%> </a>&nbsp;

            
            </p>
			</div>
        </div>
        <div class="thin-line">&nbsp;</div>
    </ItemTemplate>

</asp:Repeater>
<asp:Repeater ID="rptasList" runat="server">
    <HeaderTemplate>
        <h3>Discussions</h3>
    </HeaderTemplate>
    <ItemTemplate>
        <p>
            <a href="<%# DataBinder.Eval(Container.DataItem, "LinkToMessage")%>" target="_blank">
                <%# DataBinder.Eval(Container.DataItem, "Subject")%> </a>&nbsp;
        </p>
        <div class="thin-line">&nbsp;</div>
    </ItemTemplate>
    <FooterTemplate>

        <a href="//community.nutritioncare.org/home" class="button" target="_blank">More ASPENConnect</a>
    </FooterTemplate>
</asp:Repeater>
