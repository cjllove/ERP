﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsWindow_Gather.aspx.cs" Inherits="ERPProject.ERPQuery.GoodsWindow_Gather" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" OnCustomEvent="PageManager1_CustomEvent" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" BodyPadding="0px" Layout="Anchor"
            ShowHeader="false">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnPostBack" Text="增 加" Icon="SystemSave" runat="server" Hidden="true" OnClick="btnPostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" EnableDefaultState="false" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" EnableDefaultState="false" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="93% 6%">
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label="查询信息" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridGoods" AnchorValue="100% -30" ShowBorder="true" ShowHeader="false" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" EnableColumnLines="true"
                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" CheckBoxSelectOnly="true" IsDatabasePaging="true"
                    PageSize="70" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC">
                    <Columns>
                        <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="UNIT" HeaderText="最小单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="40px" DataField="UNITNAME" SortField="UNIT" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装数量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="165px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCER" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" HeaderText="制品编号" EnableColumnHide="true" EnableHeaderMenu="false" />
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
                        <f:BoundField DataField="STR0" Hidden="true" HeaderText="上游供应商" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdDept" runat="server" />
        <f:HiddenField ID="hfdSupplier" runat="server" />
        <f:HiddenField ID="hfdGoodsType" runat="server" />
        <f:HiddenField ID="hfdSearch" runat="server" />
    </form>
    <f:Grid AllowPaging="true" ID="Grid2" Hidden="true" ShowBorder="true" PageSize="20" IsDatabasePaging="true" EnableColumnLines="true"
        ShowHeader="false" Title="表格" Width="780px" Height="300px" runat="server" EnableCollapse="true" OnPageIndexChange="Grid2_PageIndexChange"
        DataKeyNames="GDSEQ">
        <Columns>
            <f:RowNumberField runat="server" Width="30px"></f:RowNumberField>
            <f:BoundField Width="100px" ColumnID="LISGDSEQ" DataField="GDSEQ" HeaderText="商品编码" />
            <f:BoundField Width="150px" ColumnID="LISGDNAME" DataField="GDNAME" HeaderText="商品名称" />
            <f:BoundField Width="70px" ColumnID="LISZJM" DataField="ZJM" HeaderText="助记码" />
            <f:BoundField Width="80px" ColumnID="LISGDSPEC" DataField="GDSPEC" HeaderText="规格" />
            <f:BoundField Width="40px" ColumnID="LISUNIT" DataField="UNITNAME" HeaderText="单位" />
            <f:BoundField Width="130px" ColumnID="LISPIZNO" DataField="PIZNO" HeaderText="注册证号" />
            <f:BoundField Width="130px" ColumnID="LISPRODUCER" DataField="PRODUCERNAME" HeaderText="生产厂家" />
        </Columns>
        <PageItems>
            <f:Button ID="Button1" Text="取消" OnClientClick="showhide()" runat="server">
            </f:Button>
        </PageItems>
        <Toolbars>
            <f:Toolbar ID="Toolbar1" Hidden="true" runat="server">
                <Items>
                    <f:Button ID="Button4" Text="取消" OnClientClick="showhide()" runat="server">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Grid>
    <script type="text/javascript">
        function showhide() {
            F('<% =Grid2.ClientID%>').hide();
        }
        function tbxMyBox1_TriggerClick(t) {
            F.customEvent('Grid2_bind_');
            F('<% =Grid2.ClientID%>').hide();
            $('#Grid2_wrapper').css('top', $("#<% =trbSearch.ClientID %>-triggerWrap").offset().top + $("#<% =trbSearch.ClientID %>-triggerWrap").height());
            $('#Grid2_wrapper').css('left', $("#<% =trbSearch.ClientID %>-triggerWrap").offset().left);
            $('#Grid2_wrapper').css('position', 'fixed');
            $('#Grid2_wrapper').css('z-index', '9999');
            //显示方法
            F('<% =Grid2.ClientID%>').show(F('<% =trbSearch.ClientID %>').getEl(), function () {
                $(document).click(function (e) {
                    var target = $(e.target);
                    if (target.closest("#<% =Grid2.ClientID%>").length == 0 && !F('<% =Grid2.ClientID%>').isHidden() && target.closest("#<% =trbSearch.ClientID%>").length == 0) {
                        showhide();
                    }
                });
            });
        }
        var tbxMyBox1 = '<%= trbSearch.ClientID %>';

        // 页面AJAX回发后执行的函数
        F.ready(function () {
            F('<% =Grid2.ClientID%>').draggable = false;
            <%--F('<% =trbSearch.ClientID %>').on('change', function (field, newvalue, oldvalue) {
                if (window.tgbGDSEQ_down != 1) {
                    F.customEvent('trbSearch_change_' + newvalue);
                }
                window.tgbGDSEQ_down = 0;
            });--%>
            //项点击事件
            F('<%= Grid2.ClientID %>').on('itemmousedown', function (View, record, item, index, e) {
                var v = $(item).find('.x-grid-cell-LISGDSEQ div.x-grid-cell-inner').text();
                $("#<% =trbSearch.ClientID %>-inputCell").children("input").val(v);
            });
            F('<% =Grid2.ClientID%>').on('itemdblclick', function (grid, record, item, index) {
                F.customEvent('Grid2_click_' + index);
                F('<% =Grid2.ClientID%>').hide();
            });
        });



    </script>
</body>
</html>
