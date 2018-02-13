﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SystemInventoryComparison.aspx.cs" Inherits="ERPProject.ERPQuery.SystemInventoryComparison" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>系统库存对比</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
            <Items>
                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                    Layout="Anchor" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：查询ERP系统与EAS系统库存差异情况" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:CheckBox ID="ckbOnlyDifference" runat="server" Text="仅显示有差异的数据"></f:CheckBox>
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
                    <Items>
                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                            ShowHeader="False" LabelWidth="80px" runat="server">
                            <Rows>
                                <f:FormRow ColumnWidths="50% 50%">
                                    <Items>
                                        <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品编码" />
                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                            <f:ListItem Text="--请选择--" Value="" /> 
                                            <f:ListItem Text="是" Value="Y" /> 
                                            <f:ListItem Text="否" Value="N" /> 
                                        </f:DropDownList>
                                        <f:DatePicker ID="lstSJ" runat="server" Label="开始时间" />
                                    </Items>
                                </f:FormRow>
                            </Rows>
                        </f:Form>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="SEQNO" EnableTextSelection="true" EnableColumnLines="true"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                        <f:BoundField Width="250px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="KFKC" HeaderText="库存数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="KEKCSL" HeaderText="科室库存数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="WSCSL" HeaderText="未上传数量" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="100px" DataField="WJSSL" HeaderText="未结算数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SYSL" HeaderText="盘点损益数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="EASSL" HeaderText="EAS库存数量" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="CY" HeaderText="差异" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="ISGZ" SortField="ISGZ" HeaderText="是否高值" Hidden="false" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
</body>
</html>