﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ArrivalAnalysis.aspx.cs" Inherits="ERPProject.ERPStorage.ArrivalAnalysis" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>订单到货分析</title>
    <meta name="sourcefiles" content="~/ERPQuery/WebForm1.aspx" />
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" runat="server" EnablePostBack="true" OnClick="btSearch_Click" />
                <f:TextBox ID="tbxDept" runat="server" Label="部门">
                </f:TextBox>
            </Items>
        </f:Panel>

        <f:Window ID="Window1" runat="server" IsModal="true" Hidden="true" Target="Parent"
            EnableIFrame="true" Width="820px" Height="480px" AutoScroll="false">
        </f:Window>
    </form>
</body>
</html>