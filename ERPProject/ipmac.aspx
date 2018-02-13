<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ Register TagPrefix="f" Namespace="FineUIPro" Assembly="FineUIPro" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <title>获取服务器 IP+MAC 地址</title>
    <script runat="server">
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string[] ipmac = PageManager1.GetRequestIPMAC();
                tbxIP.Text = ipmac[0];
                tbxMAC.Text = ipmac[1];
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server"></f:PageManager>
        <f:SimpleForm runat="server" Width="350px" LabelWidth="50px" 
            BodyPadding="10px" Title="服务器的IP+MAC地址">
            <Items>
                <f:TextBox runat="server" Label="IP" ID="tbxIP" ReadOnly="true"></f:TextBox>
                <f:TextBox runat="server" Label="MAC" ID="tbxMAC" ReadOnly="true"></f:TextBox>
            </Items>
        </f:SimpleForm>
    </form>
</body>
</html>