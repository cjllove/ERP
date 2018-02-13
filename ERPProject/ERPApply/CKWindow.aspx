﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CKWindow.aspx.cs" Inherits="ERPProject.ERPApply.CKWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnUpt" Text="修改出库数" EnableDefaultState="false" Icon="Disk" runat="server" Hidden="true" OnClick="btnUpt_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" EnableDefaultState="false" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Grid ID="GridGoods" AnchorValue="100% -1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true" EnableColumnLines="true"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="SEQNO,ROWNO,GDSEQ" PageSize="20"  IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <f:RowNumberField runat="server" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="SEQNO" HeaderText="单据编号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Hidden="true" DataField="ROWNO" HeaderText="行号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="80px" DataField="BZSL" ColumnID="BZSL" HeaderText="出库数量" TextAlign="Right" />
                        <f:BoundField Width="70px" DataField="JXTAX" HeaderText="税率" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                        <f:BoundField Width="80px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                        <f:BoundField Width="110px" DataField="BARCODE" HeaderText="商品条码" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="80px" DataField="UNITNAME" HeaderText="包装单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="ZPBH" HeaderText="制品编号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="170px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="HWID" HeaderText="货位" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SUPID" HeaderText="供应商" Hidden="true" />
                        <f:BoundField Hidden="true" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdDept" runat="server" />
        <f:HiddenField ID="hfdSupplier" runat="server" />
        <f:HiddenField ID="hfdSearch" runat="server" />
        <f:Window ID="WinLot" Title="商品预占信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="260px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" ClicksToEdit="1" AllowCellEditing="true"
                    DataKeyNames="KCSL,XSSL_OLD,BZHL">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField MinWidth="120px" DataField="GDNAME" HeaderText="商品名称" ExpandUnusedSpace="true" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="XSSL_OLD" HeaderText="原始预占数量" TextAlign="Center" Hidden="true" />
                        <f:RenderField Width="80px" DataField="XSSL" ColumnID="XSSL" HeaderText="预出数<font color='red'>*</font>" TextAlign="Center" FieldType="Int">
                            <Editor>
                                <f:NumberBox ID="comXSSL" CssClass="ColBlue" Required="true" runat="server" MinValue="0" DecimalPrecision="2" MaxValue="99999999" />
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" EnableDefaultState="false" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="Button1" Text="取 消" Icon="SystemClose" EnableDefaultState="false" runat="server" OnClick="Button1_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
</body>
</html>