﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ERPGoodsMatching.aspx.cs" Inherits="ERPProject.WeiGo.ERPGoodsMatching" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品匹配管理</title>
    <%--<script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>--%>
    <%--<style type="text/css" media="all">
        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }
    </style>--%>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX" ShowHeader="false">
            <Items>
                <f:Panel ID="PanelDetail" ShowBorder="false" BoxFlex="4" AnchorValue="100% -300px" BodyPadding="0px" Layout="HBox"
                    BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" BoxFlex="2" runat="server" BodyPadding="0" ShowBorder="false" ShowHeader="false" CssStyle="border-right: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                            Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyStyle="background-color:#d9e7f8;">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar4" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText4" CssStyle="" Text="Excel操作信息！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill4" runat="server" />
                                                <f:FileUpload runat="server" ID="fuDocument" Label="Excel" EmptyText="请选择导入Excel文件" Width="300px" LabelWidth="60px"></f:FileUpload>
                                                <f:DropDownList runat="server" Hidden="true" ID="ddlHis" ShowRedStar="true" Required="true" Label="医院" LabelWidth="60px">
                                                    <f:ListItem Value="" />
                                                </f:DropDownList>
                                                <f:Button runat="server" ID="btnIns" Text="导 入" AjaxLoadingType="Default" Icon="CogGo" OnClick="btnIns_Click" EnableDefaultState="false"></f:Button>
                                                <f:Button ID="BtnErr" runat="server" Text="错 误" Icon="ErrorGo" OnClick="BtnErr_Click" EnableDefaultState="false"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Grid ID="GridExcel" ShowBorder="false" ShowHeader="false" AllowSorting="false"
                                    BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="HISCODE,SEQNO" PageSize="10" IsDatabasePaging="true" AnchorValue="100%" EnableColumnLines="true"
                                    AllowPaging="true" OnPageIndexChange="GridExcel_PageIndexChange" OnRowDoubleClick="GridExcel_RowDoubleClick" EnableRowDoubleClickEvent="true">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:TriggerBox ID="tgbExlBill" EmptyText="请输入导入单号" Hidden="true" Width="170px" runat="server" TriggerIcon="Search" Label="单号" LabelWidth="50px" OnTriggerClick="btnExlSch_Click"></f:TriggerBox>
                                                <f:TriggerBox ID="tgbExlGoods" EmptyText="请输入商品名称" Width="170px" runat="server" TriggerIcon="Search" Label="商品" LabelWidth="50px" OnTriggerClick="btnExlSch_Click"></f:TriggerBox>
                                                <f:Button ID="btnExlDel" runat="server" Icon="Delete" ConfirmText="是否确定将选中行的信息删除？" Text="删 行" OnClick="btnExlDel_Click" EnableDefaultState="false"></f:Button>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button runat="server" ID="btnMH" Text="模糊匹配" Icon="ZoomOut" OnClick="btnMH_Click" EnableDefaultState="false"></f:Button>
                                                <f:Button runat="server" ID="btnJQ" Text="精确匹配" Icon="ZoomIn" OnClick="btnJQ_Click" EnableDefaultState="false"></f:Button>
                                                <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                                <f:Button ID="btnExlSch" runat="server" Text="查 询" Icon="SystemSearch" OnClick="btnExlSch_Click" EnableDefaultState="false"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField runat="server"></f:RowNumberField>
                                        <f:BoundField Width="150px" DataField="MEMO" ColumnID="MEMO" HeaderText="错误信息" Hidden="true" />
                                        <f:BoundField Width="180px" DataField="HISNAME" HeaderText="HIS名称" />
                                        <f:BoundField Width="120px" DataField="HISSPEC" HeaderText="HIS规格" />
                                        <f:BoundField Width="55px" DataField="HISUNIT" HeaderText="HIS单位" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="PRODUCER" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="YWRQ" HeaderText="业务日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" Hidden="true"></f:BoundField>
                                        <f:BoundField Width="150px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="HOSNAME" HeaderText="医院" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="SEQNO" HeaderText="导入单号" TextAlign="Center" Hidden="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel3" Width="5px" runat="server" ShowBorder="false" ShowHeader="false" BodyStyle="background-color:#d9e7f8;">
                        </f:Panel>
                        <f:Panel ID="Panel5" BoxFlex="2" runat="server" BodyPadding="0" ShowBorder="false" ShowHeader="false" CssStyle="border-left: 1px solid #99bce8;border-bottom: 1px solid #99bce8;"
                            Layout="VBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyStyle="background-color:#d9e7f8;">
                            <Items>
                                <f:Panel ID="Panel7" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar5" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText5" CssStyle="" Text="操作信息：系统商品信息！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill5" runat="server" />
                                                <f:Button ID="btnMatch" Icon="Add" Text="匹 配" runat="server" OnClick="btnMatch_Click" EnableDefaultState="false"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Grid ID="GridGoods" ShowBorder="false" ShowHeader="false" AllowSorting="false" BoxFlex="1"
                                    AutoScroll="true" runat="server" DataKeyNames="GDSEQ,FLAG" EnableRowDoubleClickEvent="true" EnableColumnLines="true" OnRowDoubleClick="GridGoods_RowDoubleClick"
                                    PageSize="20" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" >
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:DropDownList ID="ddlGoodsState" runat="server" Label="商品状态" LabelWidth="80px" Width="170px">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="已下传" Value="Y" />
                                                    <f:ListItem Text="未下传" Value="N" />
                                                </f:DropDownList>
                                                <f:TriggerBox ID="trbSearch" LabelWidth="80px" Width="300px" MaxLength="20" Label="商品信息" runat="server" EmptyText="可输入ERP编码或ERP编码或商品名称" TriggerIcon="Search" OnTriggerClick="btnSrch_Click" />
                                                <f:Button ID="btnSrch" Icon="SystemSearch" Text="查 询" DisableControlBeforePostBack="false" runat="server" OnClick="btnSrch_Click" EnableDefaultState="false"/>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField runat="server"></f:RowNumberField>
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="ERP名称" />
                                        <f:BoundField Width="0px" DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="50px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="PROCEDURENAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="150px" DataField="PIZNO" HeaderText="注册证号" />
                                        <f:BoundField Width="120px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel9" Height="5px" runat="server" ShowBorder="false" ShowHeader="false" BodyStyle="background-color:#d9e7f8;">
                </f:Panel>
                <f:Panel ID="Panel6" ShowBorder="false" BoxFlex="3" AnchorValue="100% -255" BodyPadding="0" Layout="VBox" CssStyle="border-top: 1px solid #99bce8;"
                    BoxConfigAlign="Stretch" BoxConfigPosition="Start" ShowHeader="False" runat="server">
                    <Items>
                        <f:Panel ID="Panel8" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar6" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText3" CssStyle="" Text="操作提示：已匹配数据" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill6" runat="server" />
                                        <f:Button ID="btnDel" runat="server" Icon="Delete" ConfirmText="是否确定将选中行的匹配信息删除,重新匹配？" Text="删 行" OnClick="btnDel_Click" EnableDefaultState="false"></f:Button>
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnSys" runat="server" Icon="PackageStart" Text="导入系统" ConfirmText="是否将已经批次的信息导入到系统中?" OnClick="btnSys_Click" EnableDefaultState="false"></f:Button>
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnExp" runat="server" Text="模板下载" Icon="DiskDownload" ConfirmText="是否下载数据匹配模板?"  DisableControlBeforePostBack="false" OnClick="btnExp_Click" EnableDefaultState="false"></f:Button>
                                        <f:Button ID="btnExlOut" runat="server" Text="导出信息" Icon="PageExcel" ConfirmText="是否将匹配信息导出?" OnClick="btnExlOut_Click" EnableDefaultState="false"></f:Button>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                        </f:Panel>
                        <f:Grid ID="GridOut" ShowBorder="false" ShowHeader="false" AllowSorting="false" BoxFlex="1"
                            AutoScroll="true" runat="server" DataKeyNames="SEQNO,HISCODE" EnableColumnLines="true"
                            PageSize="50" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridOut_PageIndexChange">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                        <f:DropDownList runat="server" ID="ddlHisp" Label="医院" Hidden="true" LabelWidth="60px">
                                            <f:ListItem Value="" />
                                        </f:DropDownList>
                                        <f:DropDownList ID="ddlMode" runat="server" Label="匹配模式" EnableEdit="true" ForceSelection="true" LabelWidth="80px">
                                            <f:ListItem Text="--请选择--" />
                                            <f:ListItem Text="精确匹配" Value="1" />
                                            <f:ListItem Text="模糊匹配" Value="2" />
                                            <f:ListItem Text="手动匹配" Value="3" />
                                        </f:DropDownList>
                                        <f:TriggerBox Hidden="true" ID="TgbBill" EmptyText="请输入导入单号" Width="270px" runat="server" TriggerIcon="Search" Label="单号" LabelWidth="60px" OnTriggerClick="btnGridOut_Click"></f:TriggerBox>
                                        <f:TriggerBox ID="tgbGoods" LabelWidth="80px" Width="300px" Label="商品信息" runat="server" EmptyText="可输入ERP编码或ERP编码或商品名称" TriggerIcon="Search" OnTriggerClick="btnGridOut_Click" />
                                        <f:Button ID="btnGridOut" Icon="SystemSearch" Text="查 询" runat="server" OnClick="btnGridOut_Click" EnableDefaultState="false"/>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Columns>
                                <f:RowNumberField runat="server"></f:RowNumberField>
                                <f:BoundField Width="0px" DataField="SEQNO" Hidden="true" />
                                <f:BoundField Width="120px" DataField="HOSNAME" HeaderText="医院名称" />
                                <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="ERP编码" TextAlign="Center" />
                                <f:BoundField Width="180px" DataField="GDNAME" HeaderText="ERP名称" />
                                <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格" />
                                <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                <f:BoundField Width="150px" DataField="PZWH" HeaderText="注册证号" />
                                <f:BoundField Width="100px" DataField="EAS_CODE" HeaderText="ERP编码" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="HISCODE" HeaderText="HIS编码" TextAlign="Center" />
                                <f:BoundField Width="180px" DataField="HISNAME" HeaderText="HIS名称" />
                                <f:BoundField Width="60px" DataField="FLAGNAME" HeaderText="状态" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="PPMODENAME" HeaderText="匹配模式" TextAlign="Center" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <%--<f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>--%>
    </form>
    <script type="text/jscript">
       <%-- var highlightRowsClientID = '<%= highlightRows.ClientID %>';
        var gridClientID = '<%= GridGoods.ClientID %>';--%>
        //function highlightRows() {
        //    // 增加延迟，等待HiddenField更新完毕
        //    window.setTimeout(function () {
        //        var highlightRows = F(highlightRowsClientID);
        //        var grid = F(gridClientID);

        //        $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');

        //        $.each(highlightRows.getValue().split(','), function (index, item) {
        //            if (item !== '') {
        //                var row = grid.getView().getNode(parseInt(item, 10));
        //                $(row).addClass('highlight');
        //            }
        //        });
        //    }, 100);
        //}

        // 页面第一个加载完毕后执行的函数
        //F.ready(function () {
        //    var grid = F(gridClientID);

        //    grid.on('columnhide', function () {
        //        highlightRows();
        //    });

        //    grid.on('columnshow', function () {
        //        highlightRows();
        //    });

        //    grid.getStore().on('refresh', function () {
        //        highlightRows();
        //    });

        //    highlightRows();

        //});
    </script>
</body>
</html>