﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupxhFx.aspx.cs" Inherits="ERPProject.ERPQuery.SupxhFx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商情况分析</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="供应商消耗排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：到货情况分析查询主界面！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false" DisableControlBeforePostBack="false"
                                                    OnClick="btExport_Click" Text="导出" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstSUPPLIER" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="查询期间" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label=" 至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px" Layout="HBox">
                                    <Items>
                                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                        <%--  <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart2"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>--%>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -284" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" OnSort="GridGoods_Sort" SortDirection="DESC" SortField="SYJE" PageSize="20"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SUPID" ColumnID="SUPID" SortField="SUPID" HeaderText="供应商编码" Hidden="true" />
                                        <f:BoundField DataField="SUPNAME" ColumnID="SUPNAME" SortField="SUPNAME" HeaderText="供应商" ExpandUnusedSpace="true" MinWidth="150px" />
                                        <f:GroupField runat="server" HeaderText="消耗数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="SYSL" SortField="SYSL" ColumnID="SYSL" HeaderText="消耗数量" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="SLZB"  SortField="SLZB" ColumnID="SLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="SLZB" SortField="SYSL" ColumnID="SLZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="SLHBZ" ColumnID="SLHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="SLTBZ" ColumnID="SLTBZ" Hidden="true"></f:BoundField>
                                                <%-- <f:BoundField Width="90px" DataField="SLHB" SortField="SLHB" ColumnID="SLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="SLTB" SortField="SLTB" ColumnID="SLTB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="SLHB" SortField="SLHBZ" ColumnID="SLHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="SLTB" SortField="SLTBZ" ColumnID="SLTB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="消耗金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="SYJE" SortField="SYJE" ColumnID="SYJE" HeaderText="消耗金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <%--<f:BoundField Width="90px" DataField="JEZB" SortField="JEZB"  ColumnID="JEZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="JEZB" SortField="SYJE" ColumnID="JEZB" HeaderText="占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="JEHBZ" ColumnID="JEHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="JETBZ" ColumnID="JETBZ" Hidden="true"></f:BoundField>
                                                <%--<f:BoundField Width="90px" DataField="JEHB" SortField="JEHB" ColumnID="JEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="JETB" SortField="JETB" ColumnID="JETB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="JEHB" SortField="JEHBZ" ColumnID="JEHB" HeaderText="环比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="JETB" SortField="JETBZ" ColumnID="JETB" HeaderText="同比增长" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="供应商消耗明细排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：按订货日期倒序排列！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnCl" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExpt" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false" DisableControlBeforePostBack="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSch" Icon="Magnifier" Text="查 询" OnClick="btnSch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormList" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlSUPPLIER" Label="供应商" EnableEdit="true" runat="server"></f:DropDownList>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstISGZ" runat="server" Label="是否高值">
                                                            <f:ListItem Text="--请选择--" Value="" />
                                                            <f:ListItem Text="是" Value="Y" />
                                                            <f:ListItem Text="否" Value="N" />
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="lisDATE1" runat="server" Label="查询期间" Required="true" ShowRedStar="true"></f:DatePicker>
                                                        <f:DatePicker ID="lisDATE2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <%--                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="HSJE" SortDirection="DESC" PageSize="50" OnSort="GridList_Sort" OnPageIndexChange="GridList_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true">--%>
                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="SYSL" SortDirection="DESC" PageSize="50" OnSort="GridList_Sort" OnPageIndexChange="GridGoods_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="40px" EnablePagingNumber="true"></f:RowNumberField>
                                        <f:BoundField Width="110px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField Width="220px" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="110px" DataField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:GroupField runat="server" HeaderText="数量分析" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="SYSL" ColumnID="SYSL" SortField="SYSL" HeaderText="消耗数量" DataFormatString="{0:f2}" TextAlign="Right" />
                                                <%-- <f:BoundField Width="90px" DataField="SLZB" ColumnID="SLZB" SortField="SLZB" HeaderText="数量占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="SLTB" ColumnID="SLTB" SortField="SLTB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="SLHB" ColumnID="SLHB" SortField="SLHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="90px" DataField="SLZB" ColumnID="SLZB" SortField="SYSL" HeaderText="数量占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="SLTB" ColumnID="SLTB" SortField="SLTBZ" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="SLHB" ColumnID="SLHB" SortField="SLHBZ" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="SLHBZ" ColumnID="SLHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="SLTBZ" ColumnID="SLTBZ" Hidden="true"></f:BoundField>
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="金额分析" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="110px" DataField="SYJE" ColumnID="SYJE" HeaderText="消耗金额" DataFormatString="{0:f2}" TextAlign="Right" />
                                                <%--<f:BoundField Width="100px" DataField="JEZB" SortField="JEZB" ColumnID="JEZB" HeaderText="金额占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="JETB" SortField="JETB" ColumnID="JETB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="JEHB" SortField="JEHB" ColumnID="JEHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />--%>
                                                <f:BoundField Width="100px" DataField="JEZB" SortField="SYJE" ColumnID="JEZB" HeaderText="金额占比" TextAlign="Right" DataFormatString="{0:p}" />
                                                <f:BoundField Width="110px" DataField="JETB" SortField="JETBZ" ColumnID="JETB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="110px" DataField="JEHB" SortField="JEHBZ" ColumnID="JEHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField DataField="JEHBZ" ColumnID="JEHBZ" Hidden="true"></f:BoundField>
                                                <f:BoundField DataField="JETBZ" ColumnID="JETBZ" Hidden="true"></f:BoundField>
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                                        <f:BoundField Width="180px" DataField="PIZNO" HeaderText="批准文号" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hdfsupid" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalje" runat="server"></f:HiddenField>
        <f:HiddenField ID="TotalslMX" runat="server"></f:HiddenField>
        <f:HiddenField ID="TotaljeMX" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal2" runat="server"></f:HiddenField>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function updateDate() {
            var me = F('<%= GridGoods.ClientID %>'), Total = 0, TbTotal = 0, HbTotal = 0;
            var Totalsl = F('<%= Totalsl.ClientID%>').getValue();
            var Totalje = F('<%= Totalje.ClientID%>').getValue();
            me.getRowEls().each(function (index, tr) {
                Total = me.getCellValue(tr, 'SYSL');
                TbTotal = me.getCellValue(tr, 'SLTBZ');
                HbTotal = me.getCellValue(tr, 'SLHBZ');
                me.updateCellValue(index, 'SLTB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'SLHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'SLZB', Total == 0 ? '0%' : ((Total / Totalsl) * 100).toFixed(2) + '%', true);
                Total = me.getCellValue(tr, 'SYJE');
                TbTotal = me.getCellValue(tr, 'JETBZ');
                HbTotal = me.getCellValue(tr, 'JEHBZ');
                me.updateCellValue(index, 'JETB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'JEHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'JEZB', Total == 0 ? '0%' : ((Total / Totalje) * 100).toFixed(2) + '%', true);
            });
        }
        function updateDateMX() {
            var me = F('<%= GridList.ClientID %>'), Total = 0, TbTotal = 0, HbTotal = 0;
            var Totalsl = F('<%= TotalslMX.ClientID%>').getValue();
            var Totalje = F('<%= TotaljeMX.ClientID%>').getValue();
            me.getRowEls().each(function (index, tr) {
                Total = me.getCellValue(tr, 'SYSL');
                TbTotal = me.getCellValue(tr, 'SLTBZ');
                HbTotal = me.getCellValue(tr, 'SLHBZ');
                me.updateCellValue(index, 'SLTB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'SLHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'SLZB', Total == 0 ? '0%' : ((Total / Totalsl) * 100).toFixed(2) + '%', true);
                Total = me.getCellValue(tr, 'SYJE');
                TbTotal = me.getCellValue(tr, 'JETBZ');
                HbTotal = me.getCellValue(tr, 'JEHBZ');
                me.updateCellValue(index, 'JETB', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'JEHB', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'JEZB', Total == 0 ? '0%' : ((Total / Totalje) * 100).toFixed(2) + '%', true);
            });
        }
        function showpie() {
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var hfdArrayVal = F('<%= hfdArrayVal.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfdArrayVal.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfdArrayVal[i].split("$")[0] + '", "name":"' + hfdArrayVal[i].split("$")[1] + '"}' + ')'));
            }
            var myChart = echarts.init(document.getElementById('echart'));
            var option = {
                title: {
                    text: '供应商消耗数量占比',
                    subtext: '',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    data: datatitl
                },
                series: [
                    {
                        name: '来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['70%', '60%'],
                        data: dataVal,
                        itemStyle: {
                            emphasis: {
                                shadowBlur: 10,
                                shadowOffsetX: 0,
                                shadowColor: 'rgba(0, 0, 0, 0.5)'
                            }
                        }
                    }
                ]
            };
            myChart.setOption(option);
        };
        //    function getEcharsData2(data1,dqje, tbje, hbje) {
        //    var myChart = echarts.init(document.getElementById('echart2'));
        //    var option = {
        //        tooltip: {
        //            trigger: 'axis'
        //        },
        //        legend: {
        //            data: ['当前金额', '同比金额', '环比金额']
        //        },
        //        xAxis: [
        //            {
        //                type: 'category',
        //                data: data1
        //            }
        //        ],
        //        yAxis: [
        //            {
        //                type: 'value',
        //                name: '金额',
        //                min: 0,
        //                axisLabel: {
        //                    formatter: '{value} 元'
        //                }
        //            }
        //        ],
        //        series: [
        //            {
        //                name: '当前金额',
        //                type: 'bar',
        //                data: dqje
        //            },
        //            {
        //                name: '同比金额',
        //                type: 'bar',
        //                data: tbje
        //            },
        //            {
        //                name: '环比金额',
        //                type: 'bar',
        //                data: hbje
        //            }
        //        ]
        //    };
        //    myChart.setOption(option);
        //};
    </script>
</body>
</html>
