﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockSearchByGdSeq.aspx.cs" Inherits="ERPProject.ERPQuery.StockSearchByGdSeq" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>商品库存按品规统计查询</title>
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
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar1" runat="server">
                    <Items>
                        <f:CheckBox ID="cbxKC" runat="server" Text="显示有库存信息" CssStyle="margin-left:10px;" Checked="true"></f:CheckBox>
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前商品库存信息?" DisableControlBeforePostBack="false"
                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="25% 25% 50%">
                            <Items>
                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="查询部门" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                    <f:ListItem Text="--请选择--" Value="" />
                                    <f:ListItem Text="高值" Value="Y" />
                                    <f:ListItem Text="非高值" Value="N" />
                                </f:DropDownList>
                                <f:DropDownList ID="docCategory" runat="server" Label="商品类别" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                 <f:DropDownList ID="ddlSHSID" runat="server" Label="供 应 商" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                                <f:DropDownList ID="ddlPRODUCER" runat="server" Label="生产厂家" EnableEdit="true" ForceSelection="true">
                                </f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                               <f:TriggerBox ID="tbxGOODS" runat="server" Label="商品信息" EmptyText="可输入商品信息、助记码或货位编码" ShowTrigger="false" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                <f:Label ID="lblSUBNUM" runat="server" Label="合计数量"></f:Label>
                                <f:Label ID="lblSUBSUM" runat="server" Label="合计金额"></f:Label>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true" AllowColumnLocking="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ,DEPTID" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC"
                    EnableSummary="true" SummaryPosition="Bottom">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" EnableLock="true" Locked="true" />
                        <f:BoundField Width="100px" DataField="DEPTIDNAME" SortField="DEPTIDNAME" HeaderText="存货地点" EnableLock="true" Locked="true" />
                        <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableLock="true" Locked="true" />
                        <f:BoundField Width="170px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                        <f:BoundField Width="130px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="40px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="BZHL" Hidden="true" HeaderText="包装含量" />
                        <f:BoundField Width="70px" DataField="HWID" SortField="HWID" HeaderText="货位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ZDKC" SortField="ZDKC" ColumnID="ZDKC" HeaderText="库存下限" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="ZGKC" SortField="ZGKC" ColumnID="ZGKC" HeaderText="库存上限" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="KCSL" SortField="KCSL" ColumnID="KCSL" HeaderText="库存数" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="CATID" SortField="CATID" HeaderText="商品分类" TextAlign="Center" />
                        <f:BoundField Width="190px" DataField="SUPID" SortField="SUPID" HeaderText="供应商" />
                        <f:BoundField Width="190px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="190px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Width="190px" DataField="PSSID" SortField="PSSID" HeaderText="配送商" />
                        <f:BoundField Width="60px" DataField="ISDG" SortField="ISDG" HeaderText="是否代管" Hidden="true" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="是否高值" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="DEPTID" SortField="DEPTID" HeaderText="科室编码" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>
