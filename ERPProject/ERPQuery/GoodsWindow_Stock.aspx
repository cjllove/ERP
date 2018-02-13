<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsWindow_Stock.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsWindow_Stock" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" OnCustomEvent="PageManager1_CustomEvent" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="VBox"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnPostBack" Text="追 加" Icon="Add" runat="server" OnClick="btnPostBack_Click" Hidden="true">
                        </f:Button>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click" EnableDefaultState="false">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" EnableDefaultState="false" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="90% 9%">
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label="商品信息" EmptyText="可输入商品名称、助记码、商品编码、ERP编码或HIS编码" MaxLength="20" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                                <f:Button ID="btnSearch" runat="server" Text="查询" EnableDefaultState="false" Icon="Magnifier" OnClick="trbSearch_TriggerClick"></f:Button>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="true" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" EnableColumnLines="true" KeepCurrentSelection="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" IsDatabasePaging="true" EnableTextSelection="true"
                    PageSize="50" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC" ClearSelectedRowsAfterPaging="false">
                    <Columns>
                        <f:BoundField Width="120px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="GDMODE" SortField="GDMODE" HeaderText="型号·尺码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="UNIT" HeaderText="最小单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="UNITNAME" SortField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="KCSL" SortField="KCSL" HeaderText="库存数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="PH" SortField="PH" HeaderText="批号" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="YXQZ" SortField="YXQZ" HeaderText="有效期至" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="90px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生成日期" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装数量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="165px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCER" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="ZPBH" Hidden="true" HeaderText="制品编号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                        <f:BoundField DataField="HSJJ" Hidden="true" HeaderText="含税进价" />
                        <f:BoundField DataField="YBJ" Hidden="true" HeaderText="医保价" />
                        <f:BoundField DataField="UNIT_ORDER" Hidden="true" HeaderText="订货单位" />
                        <f:BoundField DataField="UNIT_ORDER_NAME" Hidden="true" HeaderText="订货单位名称" />
                        <f:BoundField DataField="BZHL_ORDER" Hidden="true" HeaderText="订货单位包装含量" />
                        <f:BoundField DataField="UNIT_SELL" Hidden="true" HeaderText="出库单位" />
                        <f:BoundField DataField="UNIT_SELL_NAME" Hidden="true" HeaderText="出库单位名称" />
                        <f:BoundField DataField="BZHL_SELL" Hidden="true" HeaderText="出库单位包装含量" />
                        <f:BoundField DataField="SUPID" Hidden="true" HeaderText="供应商" />
                        <f:BoundField Width="120px" DataField="SUPNAME" HeaderText="供应商" />
                        <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" runat="server" />
                        <f:BoundField DataField="ZJM" Hidden="true" HeaderText="助记码" />
                        <f:BoundField DataField="DEPTID" Hidden="true" HeaderText="管理部门" />
                        <f:BoundField DataField="UNIT_DABZ" Hidden="true" HeaderText="大包装单位" />
                        <f:BoundField DataField="UNIT_DABZ_NAME" Hidden="true" HeaderText="大包装单位" />
                        <f:BoundField DataField="UNIT_ZHONGBZ" Hidden="true" HeaderText="中包装单位" />
                        <f:BoundField DataField="UNIT_ZHONGBZ_NAME" Hidden="true" HeaderText="中包装单位" />
                        <f:BoundField DataField="BARCODE_DABZ" Hidden="true" HeaderText="大包装厂家条码" />
                        <f:BoundField DataField="BARCODE_ZHONGBZ" Hidden="true" HeaderText="中包装厂家条码" />
                        <f:BoundField DataField="NUM_DABZ" Hidden="true" HeaderText="大包装系数" />
                        <f:BoundField DataField="NUM_ZHONGBZ" Hidden="true" HeaderText="中包装系数" />
                        <f:BoundField DataField="YCODE" Hidden="true" HeaderText="原编码" />
                        <f:BoundField DataField="NAMEJC" Hidden="true" HeaderText="商品通用名" />
                        <f:BoundField DataField="NAMEEN" Hidden="true" HeaderText="英文名" />
                        <f:BoundField DataField="STRUCT" Hidden="true" HeaderText="主要构成" />
                        <f:BoundField DataField="FLAG" Hidden="true" HeaderText="状态" />
                        <f:BoundField DataField="CATID" Hidden="true" HeaderText="类别" />
                        <f:BoundField DataField="JX" Hidden="true" HeaderText="剂形" />
                        <f:BoundField DataField="YX" Hidden="true" HeaderText="药效" />
                        <f:BoundField DataField="BAR1" Hidden="true" HeaderText="药监码" />
                        <f:BoundField DataField="BAR2" Hidden="true" HeaderText="统计码" />
                        <f:BoundField DataField="BAR3" Hidden="true" HeaderText="其它编码" />
                        <f:BoundField DataField="HISCODE" Hidden="true" HeaderText="商品HIS 代码" />
                        <f:BoundField DataField="HISNAME" Hidden="true" HeaderText="商品HIS 名称" />
                        <f:BoundField DataField="ISLOT" Hidden="true" HeaderText="批号管理" />
                        <f:BoundField DataField="ISGZ" Hidden="true" HeaderText="是否贵重" />
                        <f:BoundField DataField="ISJF" Hidden="true" HeaderText="是否计费" />
                        <f:BoundField DataField="STR0" Hidden="true" HeaderText="上游供应商" />
                        <f:BoundField DataField="PZWH" SortField="PZWH" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdDeptOut" runat="server" />
        <f:HiddenField ID="hfdDeptIn" runat="server" />
        <f:HiddenField ID="hfdSupplier" runat="server" />
        <f:HiddenField ID="hfdProducer" runat="server" />
        <f:HiddenField ID="hfdProvider" runat="server" />
        <f:HiddenField ID="hfdShs" runat="server" />
        <f:HiddenField ID="hfdGoodsType" runat="server" />
        <f:HiddenField ID="hfdSearch" runat="server" />
        <f:HiddenField ID="hfdType" runat="server" />
        <f:HiddenField ID="hfdISDG" runat="server" />
        <f:HiddenField ID="hfdisGZ" runat="server" />
        <f:HiddenField ID="hfdSum" runat="server" />
    </form>
    <script type="text/javascript">
        function PosGoods(hfdPosValue) {
            if (hfdPosValue != "") {
                var activeWindow = F.wnd.getActiveWindow();
                activeWindow.window.PosGoodsQt(hfdPosValue);
            }
        }
    </script>
</body>
</html>
