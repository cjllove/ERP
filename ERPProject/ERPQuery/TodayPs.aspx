﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TodayPs.aspx.cs" Inherits="ERPProject.ERPQuery.TodayPs" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../res/js/jquery.ymh.js" type="text/javascript"></script>
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
            margin: 0,auto;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Items>
                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar2" runat="server">
                            <Items>
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="btnClear" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btnExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" Enabled="false" EnableDefaultState="false"
                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" OnClick="btnSearch_Click" runat="server" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="ddlDept" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" ></f:DropDownList>
                                        <f:DropDownList ID="ddlUser" runat="server" Label="配送员"></f:DropDownList>
                                        <f:DatePicker ID="dpkTime" runat="server" Label="配送日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridCom" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="CODE"
                    EnableSummary="true" SummaryPosition="Bottom" OnSort="GridCom_Sort" SortField="PSYNAME" SortDirection="DESC">
                    <Columns>
                        <f:RowNumberField Width="30px" TextAlign="Center" />
                        <f:BoundField Width="140px" DataField="PSYNAME" ColumnID="PSYNAME" SortField="PSYNAME" HeaderText="配送员" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="CODE" SortField="CODE" HeaderText="科室编码" Hidden="true" />
                        <f:BoundField Width="220px" DataField="NAME" ColumnID="NAME" SortField="NAME" HeaderText="科室名称" />
                        <f:BoundField Width="150px" DataField="FLAGNAME" ColumnID="FLAGNAME" SortField="FLAGNAME" HeaderText="配送状态" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="TIMENAME" ColumnID="TIMENAME" SortField="TIMENAME" HeaderText="上次配送时间" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>