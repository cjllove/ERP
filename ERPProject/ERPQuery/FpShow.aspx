﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FpShow.aspx.cs" Inherits="ERPProject.ERPQuery.FpShow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>发票查询</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <link href="../res/css/jquery-ui.min.css" rel="stylesheet" />
    <link href="../res/css/jquery-ui.theme.min.css" rel="stylesheet" />
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
                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" Hidden="true" OnClick="btExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="25% 25% 25% 25% ">
                                    <Items>
                                        <f:TriggerBox ID="tbxBILLNO" runat="server" Label="发票单号" EmptyText="输入发票单号" MaxLength="20" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:TriggerBox ID="tbxSEQNO" runat="server" Label="结算单号" EmptyText="输入结算单号" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                        <f:DatePicker ID="dpkout1" runat="server" Label="操作日期" Required="true" ShowRedStar="true" LabelWidth="70px"></f:DatePicker>
                                        <f:DatePicker ID="dpkout2" runat="server" Label="至" LabelWidth="30px" Required="true" CompareControl="dpkout1" CompareOperator="GreaterThanEqual" CompareMessage="结束日期应大于开始日期!" ShowRedStar="true"></f:DatePicker>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridGoods" AnchorValue="100% -64" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC"
                    EnableSummary="true" SummaryPosition="Bottom">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="100px" DataField="SEQNO" SortField="SEQNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="FROMORGNAME" SortField="FROMORGNAME" HeaderText="开具发票部门" />
                        <f:BoundField Width="120px" DataField="TOORGNAME" SortField="TOORGNAME" HeaderText="接受发票部门" />
                        <f:BoundField Width="100px" DataField="INVOICENO" SortField="INVOICENO" HeaderText="发票单号" />
                        <f:BoundField Width="90px" DataField="INVOICESUM" SortField="INVOICESUM" HeaderText="发票金额" />
                        <f:BoundField Width="70px" DataField="LRYNAME" SortField="LRYNAME" HeaderText="录入员" />
                        <f:BoundField Width="80px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" />
                        <f:BoundField Width="100px" DataField="PRECODE" SortField="PRECODE" HeaderText="结算单号" />
                        <f:BoundField Width="60px" DataField="ROWNO" SortField="ROWNO" HeaderText="行号" />

                        <f:BoundField Width="105px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="DHS" HeaderText="数量" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="含税金额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="PH" SortField="PH" HeaderText="批次" />
                        <f:BoundField Width="80px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" />
                        <f:BoundField Width="80px" DataField="YXQZ" SortField="YXQZ" HeaderText="有效期至" />
                        <f:BoundField Width="170px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="170px" DataField="PZWH" SortField="PZWH" HeaderText="注册证号" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>