<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexGZ.aspx.cs" Inherits="ERPProject.ERPWorkbench.IndexGZ" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        .x-grid-body {
            border-top-width: 0px;
        }

        * {
            margin: 0;
            padding: 0;
        }

        .panel-main div {
            background: none;
            position: absolute;
            z-index: 100;
        }

        .label-text span {
            font-family: "Microsoft YaHei";
            color: #393939;
            font-weight: 700;
            font-size: 16px;
            text-align: right;
        }

        .progress-bar-body {
            transition: width 2s;
            -moz-transition: width 2s; /* Firefox 4 */
            -webkit-transition: width 2s; /* Safari 和 Chrome */
            -o-transition: width 2s; /* Opera */
            width: 0;
            height: 50px;
            position: absolute;
            z-index: 99;
            background-color: #B2F13A;
        }

        .bz span {
            font-size: 16px;
            text-align: right;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" AjaxLoadingType="Mask" EnableAjax="true" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="True" EnableCollapse="true"
            Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start" BodyPadding="1px" ShowHeader="false">
            <Items>
                <f:Panel ID="Panel2" Title="左" BoxFlex="1" runat="server" ShowBorder="false" ShowHeader="false" Layout="VBox">
                    <Items>
                        <f:Grid ID="GridStockEarlyWarning" BoxFlex="2" ShowBorder="true" ShowHeader="true" Title="库存上下限预警" EnableColumnLines="true"
                            AutoScroll="true" runat="server" DataKeyNames="GDSEQ,SUPNAME" EnableHeaderMenu="true" EnableTextSelection="true"
                            AllowPaging="true" IsDatabasePaging="true" PageSize="100" OnPageIndexChange="GridStockEarlyWarning_PageIndexChange">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                <f:BoundField Width="80PX" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="150PX" DataField="GDNAME" HeaderText="商品名称" />
                                <f:BoundField Width="120PX" DataField="GDSPEC" HeaderText="商品规格" />
                                <f:BoundField Width="70PX" DataField="KCSL" HeaderText="库存数量" TextAlign="Right" />
                                <f:BoundField Width="70PX" DataField="ZDKC" HeaderText="最低库存" TextAlign="Right" />
                                <f:BoundField Width="70PX" DataField="ZGKC" HeaderText="最高库存" TextAlign="Right" />
                                <f:BoundField Width="180PX" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                <f:BoundField Width="50PX" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="70PX" DataField="HSJJ" HeaderText="价格" TextAlign="Right" />
                                <f:BoundField Width="150PX" DataField="PIZNO" HeaderText="批准文号" />
                                <f:BoundField Width="180PX" DataField="SUPNAME" HeaderText="供应商" />
                            </Columns>
                        </f:Grid>
                        <f:Grid ID="GridOrderArrival" BoxFlex="3" ShowBorder="true" ShowHeader="true" Title="订单到货情况" EnableColumnLines="true"
                            AutoScroll="true" runat="server" DataKeyNames="SEQNO,SUPNAME" EnableHeaderMenu="true" EnableTextSelection="true"
                            EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridOrderArrival_RowDoubleClick" AllowPaging="true" IsDatabasePaging="true"
                            OnPageIndexChange="GridOrderArrival_PageIndexChange" PageSize="100">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField Width="100PX" DataField="SEQNO" HeaderText="订单编号" TextAlign="Center" />
                                <f:BoundField Width="75PX" DataField="SUBNUM" HeaderText="订货条目数" TextAlign="Center" />
                                <f:BoundField Width="70PX" DataField="DHSL" HeaderText="订货数量" TextAlign="Right" />
                                <f:BoundField Width="75PX" DataField="RKTMS" HeaderText="入库条目数" TextAlign="Center" />
                                <f:BoundField Width="70PX" DataField="RKSL" HeaderText="入库数量" TextAlign="Right" />
                                <f:BoundField Width="80px" DataField="XDRQ" HeaderText="订货日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="85px" DataField="DHRQ" HeaderText="要求到货日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="60PX" DataField="LRY" HeaderText="录入人" TextAlign="Center" />
                                <f:BoundField Width="180PX" DataField="RKD" HeaderText="入库单号" />
                                <f:BoundField Width="180PX" DataField="SUPNAME" HeaderText="供应商" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel3" Title="右" BoxFlex="1" runat="server" ShowBorder="true" ShowHeader="false" Layout="VBox">
                    <Items>
                        <f:Grid ID="GridEffectiveEarlyWarning" BoxFlex="2" ShowBorder="false" ShowHeader="true" AutoScroll="true" runat="server"
                            DataKeyNames="SEQNO" Title="效期预警" EnableTextSelection="true" PageSize="100">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                <f:BoundField Width="80PX" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="150PX" DataField="GDNAME" HeaderText="商品名称" />
                                <f:BoundField Width="120PX" DataField="GDSPEC" HeaderText="商品规格" />
                                <f:BoundField Width="50PX" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="70PX" DataField="KCHSJJ" HeaderText="价格" TextAlign="Right" />
                                <f:BoundField Width="80PX" DataField="KCSL" HeaderText="数量" TextAlign="Right" />
                                <f:BoundField Width="70px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="90PX" DataField="PH" HeaderText="生产批号" TextAlign="Center" />
                                <f:BoundField Width="100PX" DataField="CATNAME" HeaderText="商品类别" TextAlign="Center" />
                                <f:BoundField Width="180PX" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                <f:BoundField Width="180PX" DataField="SUPNAME" HeaderText="供应商" />
                            </Columns>
                        </f:Grid>
                        <f:Grid ID="GridConsume" BoxFlex="3" ShowBorder="false" ShowHeader="true" AutoScroll="true" runat="server"
                            DataKeyNames="GDSEQ" Title="使用情况" EnableTextSelection="true" PageSize="100">
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />
                                <f:BoundField Width="80PX" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                <f:BoundField Width="150PX" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                                <f:BoundField Width="120PX" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" />
                                <f:BoundField Width="50PX" DataField="UNIT" ColumnID="UNIT" HeaderText="单位" TextAlign="Center" />
                                <f:BoundField Width="70PX" DataField="HSJJ" ColumnID="HSJJ" HeaderText="价格" TextAlign="Right" />
                                <f:BoundField Width="80PX" DataField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Right" />
                                <f:BoundField Width="100PX" DataField="CATNAME" ColumnID="CATNAME" HeaderText="商品类别" TextAlign="Center" />
                                <f:BoundField Width="180PX" DataField="PRODUCERNAME" ColumnID="PRODUCERNAME" HeaderText="生产厂家" />
                                <f:BoundField Width="180PX" DataField="SUPNAME" ColumnID="SUPNAME" HeaderText="供应商" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
    <script type="text/javascript">
        var basePath = '<%= ResolveUrl("~") %>';
        function openTODOLINK(id, url, name) {
            parent.addExampleTab.apply(null, [id, basePath + url, name, basePath + 'res/images/leaf.gif', true]);
        }
    </script>
</body>
</html>
