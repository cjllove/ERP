﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsLotManage.aspx.cs" Inherits="ERPProject.ERPDictionary.GoodsLotManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelLot" runat="server" />
        <f:Panel ID="PanelLot" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" Position="Top" runat="server">
                    <Items>
                        <f:TriggerBox ID="trbSearch" LabelWidth="70px" Width="360px" runat="server" Label="查询信息" EmptyText="输入商品名称或商品批号查询" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server">
                        </f:ToolbarFill>
                        <f:Button ID="bntClear" runat="server" Icon="Erase" OnClick="bntClear_Click" Text=" 清 空" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                        <f:Button ID="btnDel" runat="server" Icon="PageCancel" OnClick="btnDel_Click" ConfirmText="此操作不可恢复，是否删除选中的批号信息?" Text="删 除" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                        <f:Button ID="btnSave" runat="server" Icon="Disk" OnClick="btnSave_Click" Text="保 存" ValidateForms="FormLot" EnableDefaultState="false" />
                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                        <f:Button ID="btExp" CssStyle="margin-left: 15px;" OnClick="btExp_Click" Icon="PageExcel" Text="导 出" EnableAjax="false" DisableControlBeforePostBack="false" runat="server" EnableDefaultState="false" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormLot" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px 0px 10px" ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbxGDNAME" runat="server" Label="商品名称" Required="true" ShowRedStar="true" EmptyText="请输入商品编码、名称或助记码">
                                </f:TextBox>
                                <f:TextBox ID="tbxHWTM" runat="server" Label="货物条码" Readonly="true">
                                </f:TextBox>
                                <f:TextBox ID="tbxPZWH" runat="server" Label="注册证号" ShowRedStar="true" Required="true" MaxLength="100">
                                </f:TextBox>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextBox ID="tbsPH" runat="server" Label="批号" Required="true" ShowRedStar="true" MaxLength="20">
                                </f:TextBox>
                                <f:DatePicker ID="dpkYXQZ" runat="server" Label="效期" Required="true" ShowRedStar="true">
                                </f:DatePicker>
                                <f:DatePicker ID="dpkRQ_SC" runat="server" Label="生产日期" Required="true" ShowRedStar="true">
                                </f:DatePicker>
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:HiddenField ID="hfdGDSEQ" runat="server" />

                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" EnableCollapse="true" AnchorValue="100% -60px"  EnableColumnLines="true"
                    DataKeyNames="PHID,GDSEQ" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowPaging="true" PageSize="100" OnPageIndexChange="GridLot_PageIndexChange">
                    <Columns>
                        <f:RowNumberField TextAlign="Center" Width="30px"></f:RowNumberField>
                        <f:BoundField DataField="PHID" HeaderText="批号内码" Hidden="true" />
                        <f:BoundField DataField="GDSEQ" HeaderText="商品编码" Width="115px" TextAlign="Center" />
                        <f:BoundField DataField="GDNAME" HeaderText="商品名称" Width="300px" ExpandUnusedSpace="true" />
                        <f:BoundField DataField="PH" HeaderText="批号" Width="100px" />
                        <f:BoundField DataField="YXQZ" HeaderText="效期" Width="120px" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                        <f:BoundField DataField="PZWH" HeaderText="注册证号" Width="120px" />
                        <f:BoundField DataField="RQ_SC" HeaderText="生产日期" Width="100px" DataFormatString="{0:yyyy-MM-dd}" TextAlign="Center" />
                        <f:BoundField DataField="HWTM" HeaderText="货物条码" Width="130px" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <%--<script type="text/javascript" src="../res/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="../res/js/jquery-ui.min.js"></script>
    <script type="text/javascript">
        var tbxGDNAMEID = '<%= tbxGDNAME.ClientID %>';

        F.ready(function () {
            var cache = {};

            $('#' + tbxGDNAMEID + ' input').autocomplete({
                minLength: 2,
                source: function (request, response) {
                    request.term = request.term + ";GoodsType";
                    var term = request.term;
                    if (term in cache) {
                        response($.map(cache[term], function (item) {
                            return {
                                label: item.split('#')[0],
                                val: item.split('#')[1]
                            }
                        }));

                        return;
                    }

                    $.getJSON("../captcha/AutoComplete.ashx", request, function (data, status, xhr) {
                        cache[term] = data;
                        response($.map(data, function (item) {
                            return {
                                label: item.split('#')[0],
                                val: item.split('#')[1]
                            }
                        }));
                    });
                },
                select: function (e, i) {
                    F('<%= hfdGDSEQ.ClientID%>').setValue(i.item.val.split('$')[0]);
                    F('<%= tbxHWTM.ClientID%>').setValue(i.item.val.split('$')[1]);
                }
            });
        });
    </script>--%>
</body>
</html>