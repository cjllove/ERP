<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CatFX.aspx.cs" Inherits="ERPProject.ERPQuery.CatFX" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品类别查询</title>
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>

</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server" AutoScroll="true">
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="" runat="server" />
                                <f:DropDownList ID="lstCAT" runat="server" Label="商品类型" EnableEdit="true" ForceSelection="true" Hidden="true" />
                                <f:DatePicker ID="dpkBEGRQ"  runat="server" Label="开始时间"></f:DatePicker>
                                <f:DatePicker ID="dpkENDRQ"  runat="server" Label="结束时间"></f:DatePicker>

                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                <f:ToolbarSeparator runat="server" />

                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" EnableDefaultState="false" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" EnableAjax="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />

                                   </Items>
                        </f:Toolbar>
                    </Toolbars>
                   
                </f:Form>
                <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px" Layout="HBox">
                    <Items>
                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%; height: 190px;" id="echart"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%; height: 190px;" id="echart2"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Grid ID="GridList" ShowBorder="false" ShowHeader="false" SortDirection="DESC" OnSort="GridList_Sort" AnchorValue="100% -252"
                    AllowSorting="true" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="GDSEQ,XQ" EnableColumnLines="true" EnableTextSelection="true" SortField="SYJE"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="50" OnPageIndexChange="GridList_PageIndexChange"
                    OnRowDataBound="GridList_RowDataBound">
                    <Columns>
                        <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                            <ItemTemplate>
                            </ItemTemplate>
                        </f:TemplateField>
                        <%--<f:RowNumberField Width="80px" runat="server" HeaderText="序号" TextAlign="Center" Locked="true" />--%>
                        <f:BoundField Width="80px" DataField="CODE" HeaderText="类别编码" TextAlign="Left" ColumnID="CODE"  Locked="true" />
                        <f:BoundField MinWidth="100px" ExpandUnusedSpace="true" DataField="CATEGORY" HeaderText="商品类别" TextAlign="Center" ColumnID="CATEGORY" Locked="true" />
                        <f:GroupField EnableLock="true" HeaderText="使用数量" TextAlign="Center">
                            <Columns>
                                <f:BoundField EnableLock="true" Width="80px" DataField="SYSL" SortField="SYSL" HeaderText="使用数量" ColumnID="SYSL"
                                    TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="SLZB" SortField="SLZB" HeaderText="数量占比"
                                    TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="HBZZSL" SortField="HBZZSL" HeaderText="环比增长"
                                    TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="TBZZSL" SortField="TBZZSL" HeaderText="同比增长"
                                    TextAlign="Center" />
                            </Columns>
                        </f:GroupField>
                        <f:GroupField EnableLock="true" HeaderText="使用金额" TextAlign="Center">
                            <Columns>
                                <f:BoundField EnableLock="true" Width="80px" DataField="SYJE" SortField="SYJE" HeaderText="使用金额" ColumnID="SYJE"
                                  DataFormatString="{0:f2}"  TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="JEZB" SortField="JEZB" HeaderText="金额占比"
                                    TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="HBZZJE" SortField="HBZZJE" HeaderText="环比增长"
                                    TextAlign="Center" />
                                <f:BoundField EnableLock="true" Width="80px" DataField="TBZZJE" SortField="TBZZJE" HeaderText="同比增长"
                                    TextAlign="Center" />
                            </Columns>
                        </f:GroupField>
                    </Columns>
                    <Listeners>
                        <f:Listener Event="rowexpanderexpand" Handler="onRowExpanderExpand" />
                        <f:Listener Event="rowexpandercollapse" Handler="onRowExpanderCollapse" />
                    </Listeners>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
    <f:HiddenField ID="highyellowlight" runat="server"></f:HiddenField>
    <f:HiddenField ID="highredlight" runat="server"></f:HiddenField>
    <script type="text/javascript">
        var highyellowlight = '<%= highyellowlight.ClientID %>';
        var highredlight = '<%= highredlight.ClientID %>';
        var gridClientID = '<%= GridList.ClientID %>';
        function highYellowlight() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highyellowlight);
                var grid = F(gridClientID);
                $(grid.el.dom).find('.x-grid-row.highyellowlight').removeClass('highyellowlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highyellowlight');
                    }
                });
            }, 100);
        }
        function highRedlight() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highredlight);
                var grid = F(gridClientID);
                $(grid.el.dom).find('.x-grid-row.highredlight').removeClass('highredlight');
                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highredlight');
                    }
                });
            }, 100);
        }
        // 页面第一个加载完毕后执行的函数

    </script>
    <script type="text/javascript">

        function showpie(text, subtext, data1, data2) {
            var myChart = echarts.init(document.getElementById('echart'));

            option = {
                title: {
                    text: text,
                    subtext: subtext,
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    x: 'left',
                    data: data1
                },
                calculable: true,
                series: [
                    {
                        name: '访问来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data:
                            data2

                    }
                ]
            };


            myChart.setOption(option);
        }
        function showpie2(text, subtext, data1, data2) {
            var myChart = echarts.init(document.getElementById('echart2'));

            option = {
                title: {
                    text: text,
                    subtext: subtext,
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    x: 'left',
                    data: data1
                },
                calculable: true,
                series: [
                    {
                        name: '访问来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '60%'],
                        data:
                            data2

                    }
                ]
            };


            myChart.setOption(option);
        }
    </script>
    <script>

        var grid1 = '<%= GridList.ClientID %>';
        function onRowExpanderExpand(event, rowId) {
            var begrq = F('<%= dpkBEGRQ.ClientID%>').getText();
            var endrq = F('<%= dpkENDRQ.ClientID%>').getText();
            var grid = this, rowEl = grid.getRowEl(rowId), rowData = grid.getRowData(rowId);

            var tplEl = rowEl.find('.f-grid-rowexpander-details .f-grid-tpl');
            if (!tplEl.text()) {

                F.create({
                    type: 'grid',
                    renderTo: tplEl,
                    header: false,
                    columnMenu: false,
                    columnResizing: false,
                    cls: 'gridinrowexpander',
                    fields: ['type', 'SYSL', 'SLZB', 'HBZZSL', 'TBZZSL', 'SYJE', 'JEZB', 'HBZZJE', 'TBZZJE'],
                    columns: [{
                        text: '', field: 'type', width: 180
                    }, {
                        text: '使用数量', field: 'SYSL', width: 80
                    }, {
                        text: '数量占比', field: 'SLZB', width: 80
                    }, {
                        text: '环比增长', field: 'HBZZSL', width: 80
                    }, {
                        text: '同比增长', field: 'TBZZSL', width: 80
                    }, {
                        text: '使用金额', field: 'SYJE', width: 80
                    }, {
                        text: '金额占比', field: 'JEZB', winth: 80
                    }, {
                        text: '环比增长', field: 'HBZZJE', winth: 80
                    }, {
                        text: '同比增长', field: 'TBZZJE', width: 80,
                    }],
                    dataUrl: './grid_rowexpander_grid_data.ashx?id=' + rowId + '&name=' + encodeURIComponent(rowData.values['CODE']) + '&begrq=' + begrq + '&endrq=' + endrq, // 这里可传递行中任意数据（rowData）
                    listeners: {
                        dataload: function () {
                            rowExpandersDoLayout();
                        }
                    }
                });

            }
        }

        function onRowExpanderCollapse(event, rowId) {
            rowExpandersDoLayout();
        }

        // 重新布局表格和行扩展列中的表格（解决出现纵向滚动条时的布局问题）
        function rowExpandersDoLayout() {
            var grid1Cmp = F(grid1);

            grid1Cmp.doLayout();

            $('.f-grid-row:not(.f-grid-rowexpander-collapsed) .gridinrowexpander').each(function () {
                var gridInside = F($(this).attr('id'));
                gridInside.doLayout();
            });
        }

    </script>
</body>
</html>
