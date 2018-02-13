﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Contant_GoodsWindow_His.aspx.cs" Inherits="ERPProject.ERPQuery.Contant_GoodsWindow_His" %>

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
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" EnableDefaultState="false" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" EnableDefaultState="false" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label="查询信息" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -35" ShowBorder="true" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" EnableColumnLines="true"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    OnPageIndexChange="GridGoods_PageIndexChange">
                    <Columns>
                        <%-- <f:RowNumberField runat="server"></f:RowNumberField>--%>
                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="200px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="0px" Hidden="true" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SL" HeaderText="库存可出定数" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="NUM_XS" HeaderText="定数含量" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="DSNUM" HeaderText="定数个数" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="NUM_DS" HeaderText="待收定数" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="sum_num" HeaderText="应出定数" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="90px" DataField="UNIT" HeaderText="包装单位" Hidden="true" />
                        <f:BoundField Width="90px" DataField="UNITNAME" HeaderText="包装单位" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="BZHL" HeaderText="包装数量" Hidden="true" />
                        <f:BoundField DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="ZPBH" HeaderText="制品编号" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" />
                        <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                        <f:BoundField DataField="YBJ" Hidden="true" HeaderText="医保价" />
                        <f:BoundField DataField="UNIT_ORDER" Hidden="true" HeaderText="订货单位" />
                        <f:BoundField DataField="UNIT_ORDER_NAME" Hidden="true" HeaderText="订货单位名称" />
                        <f:BoundField DataField="UNIT_SELL" Hidden="true" HeaderText="出库单位" />
                        <f:BoundField DataField="UNIT_SELL_NAME" Hidden="true" HeaderText="出库单位名称" />
                        <f:BoundField DataField="SUPPLIER" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="SUPPLIERNAME" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" />
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
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdDept" runat="server" />
        <f:HiddenField ID="hfdSupplier" runat="server" />
        <f:HiddenField ID="hfdSearch" runat="server" />
    </form>
</body>
</html>
