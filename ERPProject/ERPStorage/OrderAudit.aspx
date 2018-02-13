﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderAudit.aspx.cs" Inherits="ERPProject.ERPStorage.OrderAudit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品订货审核</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1"
            runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" runat="server" OnClick="btnAudit_Click" Icon="UserTick" ConfirmText="是否确认审核通过此信息?" Text="审核通过" EnablePostBack="true" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" runat="server" OnClick="btSearch_Click" Icon="Magnifier" Text="查 询" EnablePostBack="true" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:TextBox ID="tbxDept" runat="server" Label="部门">
                                        </f:TextBox>
                                        <f:DropDownList ID="ddlGoodsType" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true">
                                        </f:DropDownList>
                                        <f:DatePicker ID="dpkOrderDate" runat="server" Label="订货时间"></f:DatePicker>
                                        <f:DropDownList ID="ddlOrderType" runat="server" Label="订货分类" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="定时" Value="1" />
                                            <f:ListItem Text="临时" Value="2" />
                                            <f:ListItem Text="批量" Value="3" />
                                        </f:DropDownList>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -20" ShowBorder="true" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="180px" DataField="NAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="100px" DataField="GDMODE" HeaderText="商品型号" />
                        <f:BoundField Width="70px" DataField="GDMODE" HeaderText="包装单位" />
                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装数量" />
                        <f:BoundField Width="100px" DataField="ZPBH" HeaderText="制品编号" />
                        <f:BoundField Width="100px" DataField="SPLB" HeaderText="库存分类" />
                        <f:BoundField Width="180px" DataField="CDID" HeaderText="产地" />
                        <f:BoundField Width="180px" DataField="SUPID" HeaderText="供应商" />
                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="单价" />
                        <f:BoundField Width="90px" DataField="DHS" HeaderText="订货数" />
                        <f:BoundField Width="90px" DataField="KCSL" HeaderText="库存数" />
                        <f:BoundField Width="100px" DataField="LOT" HeaderText="批号" />
                        <f:BoundField Width="100px" DataField="BZRQ" HeaderText="效期" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>