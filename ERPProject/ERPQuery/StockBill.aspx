﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockBill.aspx.cs" Inherits="ERPProject.ERPQuery.StockBill" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>库存预占查询</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }

        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.highRedwlight td {
            background-color: red;
            background-image: none;
        }
    </style>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="25% 25% 25% 25% ">
                                    <Items>
                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                        <f:TriggerBox ID="tbxBILLNO" runat="server" Label="单据编号" EmptyText="可模糊输入入库单号" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:TriggerBox ID="tbxGOODS" runat="server" Label="商品信息" EmptyText="可输入编码、名称或助记码" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:TriggerBox ID="tbxPHID" runat="server" Label="批号信息" EmptyText="请输入批次号" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC"
                    EnableSummary="true" SummaryPosition="Bottom">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="100px" DataField="LOCKBILLNO" SortField="LOCKBILLNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="DEPTIDNAME" SortField="DEPTIDNAME" HeaderText="科室名称" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="LOCKROWNO" SortField="LOCKROWNO" HeaderText="行号" TextAlign="Center" />
                        <f:BoundField Width="105px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="50px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="LOCKKCSL" SortField="LOCKKCSL" ColumnID="LOCKKCSL" HeaderText="预占库存数" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="含税金额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:BoundField Width="120px" DataField="BAR3" SortField="BAR3" HeaderText="ERP编码" />
                        <f:BoundField Width="170px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="170px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Width="105px" DataField="phid" SortField="phid" HeaderText="批号" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>