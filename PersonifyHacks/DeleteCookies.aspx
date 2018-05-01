<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Cookies Deleted</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        Cookies Deleted.
    </div>
    </form>
</body>
</html>

<script type="C#" runat="server">

    protected void Page_Load(object sender, EventArgs e)
    {
        HttpCookie aCookie;
        string cookieName;
        int limit = HttpContext.Current.Request.Cookies.Count;
        for (int i=0; i<limit; i++)
        {
            cookieName = HttpContext.Current.Request.Cookies[i].Name;
            aCookie = new HttpCookie(cookieName);
            aCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }
    }

</script>