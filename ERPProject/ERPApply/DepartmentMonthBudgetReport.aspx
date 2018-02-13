﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentMonthBudgetReport.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentMonthBudgetReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>全院科室月度预算执行报表</title>
    <style type="text/css" media="all">
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
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" EnableDefaultState="false" Icon="DatabaseGo" EnableAjax="false"
                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出全部" ConfirmText="是否确认导出此科室申领信息?" Enabled="true" DisableControlBeforePostBack="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow>
                                    <Items>
                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                        <%--<f:DropDownList ID="ddlMonth" runat="server" Label="选择月份" EnableEdit="true" ForceSelection="true">
                                            <f:ListItem Text="请选择月份" Value="" />
                                            <f:ListItem Text="1月" Value="01" />
                                            <f:ListItem Text="2月" Value="02" />
                                            <f:ListItem Text="3月" Value="03" />
                                            <f:ListItem Text="4月" Value="04" />
                                            <f:ListItem Text="5月" Value="05" />
                                            <f:ListItem Text="6月" Value="06" />
                                            <f:ListItem Text="7月" Value="07" />
                                            <f:ListItem Text="8月" Value="08" />
                                            <f:ListItem Text="9月" Value="09" />
                                            <f:ListItem Text="10月" Value="10" />
                                            <f:ListItem Text="11月" Value="11" />
                                            <f:ListItem Text="12月" Value="12" />
                                        </f:DropDownList>--%>
                                        <f:TriggerBox ID="tbxMonth" Required="true" ShowRedStar="True" Label="选择月份" EmptyText="请选择日期和时间" TriggerIcon="Date" runat="server"></f:TriggerBox>
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" AnchorValue="100% -77" ShowBorder="false" ShowHeader="false"
                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableSummary="true" SummaryPosition="Bottom"
                    DataKeyNames="DEPTID" EnableColumnLines="true" EnableMultiSelect="true" EnableTextSelection="true"
                    AllowSorting="true" EnableHeaderMenu="true" SortField="DEPTNAME" OnSort="GridList_Sort" SortDirection="ASC"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="180px" DataField="DEPTNAME" HeaderText="部门名称" SortField="DEPTNAME" ColumnID="DEPTNAME" TextAlign="Left" />
                        <f:BoundField Width="120px" DataField="YSTOTAL" HeaderText="预算金额" SortField="YSTOTAL" ColumnID="YSJE" TextAlign="Right" DataFormatString="{0:f2}" />
                        <f:BoundField Width="150px" DataField="TOTAL" HeaderText="执行金额" SortField="TOTAL" ColumnID="TOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                        <f:BoundField Width="90px" DataField="perRate" HeaderText="占比" TextAlign="Center" SortField="perRate" DataFormatString="{0:p2}" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        var tbxMyBoxClientID = '<%= tbxMonth.ClientID %>';
        F.ready(function () {
            var tbxMyBox = F(tbxMyBoxClientID);
            tbxMyBox.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID + '-inputEl',
                    dateFmt: 'yyyy-MM'
                });
            };
        });
    </script>
</body>
</html>