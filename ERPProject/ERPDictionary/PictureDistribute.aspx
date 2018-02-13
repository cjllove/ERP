﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PictureDistribute.aspx.cs" Inherits="ERPProject.ERPDictionary.PictureDistribute" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品图片批量发布</title>
    <style>
        .w1000 {
            margin: 0 auto;
            width: 90%;
        }

            .w1000 img {
                width: 100%;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelDetail" runat="server" />
        <f:Panel ID="PanelDetail" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BoxConfigChildMargin="0" ShowHeader="False">
            <Items>
                <f:Panel ID="PanelPicture" runat="server" ShowHeader="false" BoxFlex="2" ShowBorder="false"
                    BodyPadding="0px" Layout="Fit">
                    <Items>
                        <f:Grid ID="gridPic" ShowBorder="false" ShowHeader="false" AllowSorting="false" EnableCheckBoxSelect="true"
                            AutoScroll="true" runat="server" EnableRowClickEvent="true" PageSize="15" AllowPaging="true" KeepCurrentSelection="true"
                            IsDatabasePaging="true" DataKeyNames="GDSEQ,ROWNO,FLAG" OnRowClick="gridPic_RowClick" OnPageIndexChange="gridPic_PageIndexChange">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <%--<f:TriggerBox ID="trbSearch" LabelWidth="70px" Width="360px" runat="server" Label="查询信息" EmptyText="按商品编码,条码,助记码,商品名称查询" TriggerIcon="Search" OnTriggerClick="btnSearch_Click" />--%>
                                        <f:TextBox ID="trbSearch" LabelWidth="70px" Width="360px" runat="server" Label="查询信息" EmptyText="按商品编码,条码,助记码,商品名称查询"></f:TextBox>
                                        <f:CheckBox ID="cbxNonPic" ShowLabel="false" runat="server" CssStyle="margin-left:5px;" Text="已发布" />
                                        <f:CheckBox ID="cbxNonPic1" ShowLabel="false" runat="server" CssStyle="margin-left:5px;" Text="已撤回" />


                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnSearch" runat="server" EnablePostBack="true" Text="查 询" Icon="SystemSearch" OnClick="btnSearch_Click" />
                                        <f:Button ID="btnDistribute" runat="server" EnablePostBack="true" Text="发 布" Icon="Accept" OnClick="btnDistribute_Click" />
                                        <f:Button ID="btnBack" runat="server" EnablePostBack="true" Icon="PageBack" Text="撤 回" OnClick="btnBack_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:BoundField DataField="GDSEQ" Width="88px" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField DataField="ROWNO" Width="37px" Hidden="true" HeaderText="系号" TextAlign="Center" />
                                <f:BoundField DataField="FLAG" Width="56px" HeaderText="图片状态" TextAlign="Center" />
                                <f:BoundField DataField="GDNAME" Width="150px" HeaderText="商品名称" TextAlign="Center" />
                                <f:BoundField Width="90px" DataField="ZJM" HeaderText="助记码" TextAlign="Center" />
                                <f:BoundField DataField="GDPICT" Width="130px" HeaderText="图片名称" TextAlign="Center" />
                                <f:BoundField DataField="STR1" Width="96px" HeaderText="图片别名" TextAlign="Center" />
                                <f:BoundField DataField="GDSPEC" Width="80px" HeaderText="规格" TextAlign="Center" />
                                <f:BoundField DataField="PRODUCERNAME" Width="155px" HeaderText="产地" TextAlign="Center" />
                                <f:BoundField DataField="PIZNO" Width="188px" HeaderText="注册证号" TextAlign="Center" />
                            </Columns>
                        </f:Grid>

                    </Items>
                </f:Panel>
                <f:Panel ID="sfmImage" runat="server" CssStyle="border-left: 1px solid #99bce8;" BoxFlex="1"
                    AutoScroll="true" ShowBorder="false" ShowHeader="false" BodyPadding="0px">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:HyperLink ID="lnkImage" runat="server" CssStyle="margin-left:10px;" Text="看大图" Target="_blank" />
                                <f:Label runat="server" CssStyle="padding-left:15px;" Text="(请双击左边单元行查看大图)" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormPic" ShowBorder="false" AutoScroll="false" CssStyle="padding:5px;" ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:Image ID="imgBMPPATH" CssClass="w1000" Height="580px" runat="server" ImageUrl="~/res/images/model.png" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>

</body>
</html>
