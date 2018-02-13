﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllDeptExecuAnaly.aspx.cs" Inherits="ERPProject.ERPQuery.AllDeptExecuAnaly" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>全院科室月度预算执行报表</title>
    <style type="text/css" media="all">
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="科室预算与执行排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
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
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btClear_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" EnableDefaultState="false" Icon="DatabaseGo" EnableAjax="false"
                                                    OnClick="btExpor_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出此科室预算执行信息?" DisableControlBeforePostBack="false" />
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
                                <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px" Layout="HBox">
                                    <Items>
                                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px; border: none" id="echart"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" ShowBorder="false" ShowHeader="false" AnchorValue="100% -284"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableSummary="true" SummaryPosition="Bottom"
                                    DataKeyNames="DEPTID" EnableColumnLines="true" EnableMultiSelect="true" BoxFlex="3"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="TOTAL" OnSort="GridList_Sort" SortDirection="DESC"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange" OnRowClick="GridList_RowClick" EnableRowClickEvent="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridList_RowDoubleClick" OnRowDataBound="GridList_RowDataBound">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="180px" DataField="DEPTNAME" HeaderText="部门名称" SortField="DEPTNAME" ColumnID="DEPTNAME" TextAlign="Left" />
                                        <f:GroupField runat="server" HeaderText="预算金额" TextAlign="Center">
                                            <Columns>

                                                <f:BoundField Width="80px" DataField="YSTOTAL" HeaderText="本月金额" SortField="YSTOTAL" ColumnID="YSTOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="80px" DataField="TYSTOTAL" HeaderText="同比金额" SortField="TYSTOTAL" ColumnID="TYSTOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="80px" DataField="HYSTOTAL" HeaderText="环比金额" SortField="HYSTOTAL" ColumnID="HYSTOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="TYSPERRATE" ColumnID="TYSPERRATE" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HYSPERRATE" ColumnID="HYSPERRATE" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="执行金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="80px" DataField="TOTAL" HeaderText="本月金额" SortField="TOTAL" ColumnID="ITOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="80px" DataField="TSJJE" HeaderText="同比金额" SortField="TSJJE" ColumnID="TSJJE" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="80px" DataField="HSJJE" HeaderText="环比金额" SortField="HSJJE" ColumnID="HSJJE" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="TPERRATE" ColumnID="TPERRATE" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" HeaderTextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HPERRATE" ColumnID="HPERRATE" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" HeaderTextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField Width="90px" DataField="perRate" HeaderText="占比<br>本月执行/本月预算</br>" TextAlign="Center" ColumnID="perRate" DataFormatString="{0:p2}" ExpandUnusedSpace="true" MinWidth="90px" />
                                        <f:BoundField Width="180px" DataField="DEPTID" HeaderText="部门编码" SortField="DEPTID" ColumnID="DEPTID" TextAlign="Left" Hidden="true" />
                                    </Columns>
                                </f:Grid>


                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单品预算与执行排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel4" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：按单品使用日期倒序排列！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnCl" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExpt" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" ConfirmText="是否确认导出单品预算执行分析?" DisableControlBeforePostBack="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSch" Icon="Magnifier" Text="查 询" OnClick="btnSch_Click" runat="server" EnablePostBack="true" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TriggerBox ID="tbxMonth2" Required="true" ShowRedStar="True" Label="选择月份" EmptyText="请选择日期和时间" TriggerIcon="Date" runat="server"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Panel runat="server" ShowBorder="false" Height="200px" ShowHeader="false" BodyPadding="5px">
                                    <Items>
                                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%; height: 190px;" id="echart2"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>

                                <f:Grid ID="GridGoods" AnchorValue="100% -284" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="YSTOTAL,TOTAL" SortDirection="DESC" PageSize="50" OnSort="GridGoods_Sort" OnPageIndexChange="GridGoods_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true" OnRowClick="GridGoods_RowClick" EnableRowClickEvent="true" OnRowDataBound="GridGoods_RowDataBound">
                                    <Columns>
                                        <f:RowNumberField Width="40px"></f:RowNumberField>
                                        <f:BoundField Width="110px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField Width="220px" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                                        <f:GroupField runat="server" HeaderText="预算金额" TextAlign="Center">
                                            <Columns>

                                                <f:BoundField Width="120px" DataField="YSTOTAL" HeaderText="本月" SortField="YSTOTAL" ColumnID="YSJE" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="TYSPERRATE" ColumnID="TYSPERRATE" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HYSPERRATE" ColumnID="HYSPERRATE" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="执行金额" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="150px" DataField="TOTAL" HeaderText="本月" SortField="TOTAL" ColumnID="TOTAL" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="TPERRATE" ColumnID="TPERRATE" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="HPERRATE" ColumnID="HPERRATE" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField Width="90px" DataField="perRate" HeaderText="占比<br>本月执行/本月预算</br>" TextAlign="Center" DataFormatString="{0:p2}" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>


                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArray2" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal2" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalje" runat="server"></f:HiddenField>
    </form>
    <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
    <script src="../res/js/echarts.min.js" type="text/javascript"></script>

    <script type="text/javascript">
        var tbxMyBoxClientID = '<%= tbxMonth.ClientID %>';
        var tbxMyBoxClientID2 = '<%= tbxMonth2.ClientID %>';
        F.ready(function () {
            var tbxMyBox = F(tbxMyBoxClientID);
            tbxMyBox.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID + '-inputEl',
                    dateFmt: 'yyyy-MM'
                });
            };
            var tbxMyBox2 = F(tbxMyBoxClientID2);
            tbxMyBox2.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID2 + '-inputEl',
                    dateFmt: 'yyyy-MM'
                });
            };
        });
        function updateDate() {
            var me = F('<%= GridList.ClientID %>'), Total = 0, TbTotal = 0, HbTotal = 0;
            var Totalsl = F('<%= Totalsl.ClientID%>').getValue();
            var Totalje = F('<%= Totalje.ClientID%>').getValue();
            console.log(Totalsl);
            me.getRowEls().each(function (index, tr) {
                Total = me.getCellValue(tr, 'YSTOTAL');
                TbTotal = me.getCellValue(tr, 'TYSTOTAL');
                HbTotal = me.getCellValue(tr, 'HYSTOTAL');
                me.updateCellValue(index, 'TYSPERRATE', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'HYSPERRATE', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                /*   me.updateCellValue(index, 'perRate', Total == 0 ? '0%' : ((Total / Totalsl) * 100).toFixed(2) + '%', true);*/
                Total = me.getCellValue(tr, 'ITOTAL');

                TbTotal = me.getCellValue(tr, 'TSJJE');
                HbTotal = me.getCellValue(tr, 'HSJJE');
                me.updateCellValue(index, 'TPERRATE', TbTotal == 0 ? '0%' : ((Total / TbTotal - 1) * 100).toFixed(2) + '%', true);
                me.updateCellValue(index, 'HPERRATE', HbTotal == 0 ? '0%' : ((Total / HbTotal - 1) * 100).toFixed(2) + '%', true);
                //me.updateCellValue(index, 'perRate', Totalsl == 0 ? '0%' : ((Total / Totalsl).toFixed(2) * 100) + '%', true);

            });
        }
        function getEcharsData() {
            var hfdArrayTite = F('<%= hfdArray.ClientID%>').getValue();
            var hfdArrayVal2 = F('<%= hfdArrayVal.ClientID%>').getValue();
            var dataTitl = new Array();
            for (var i = 0; i < hfdArrayTite.split(",").length; i++) {
                dataTitl.push(hfdArrayTite.split(",")[i]);
            }
            var dataVal1 = new Array();
            var dataVal2 = new Array();
            for (var i = 0; i < hfdArrayVal2.split(",").length; i++) {
                // dataVal.push(eval('(' + '{"data":[' + hfdArrayVal2.split(",")[i].split("$")[0].replace('|', ',') + '],"type":"bar", "name":"' + hfdArrayVal2.split(",")[i].split("$")[1] + '"}' + ')'));
                dataVal1.push(parseFloat(hfdArrayVal2.split(",")[i].split("|")[0]));
                dataVal2.push(parseFloat(hfdArrayVal2.split(",")[i].split("|")[1]));
            }
            var eChart = echarts.init($('#echart')[0]);

            var option = {
                color: ['#3398DB', '#91C7AE'],
                title: {
                    text: '科室预算执行情况分析',
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: ['预算金额', '执行金额'],
                    show: true
                },
                xAxis: [
                    {
                        type: 'category',
                        data: dataTitl
                    }
                ],
                yAxis: [
                    {
                        type: 'value',
                        name: '金额',
                        min: 'auto',
                        axisLabel: {
                            formatter: '{value} 元'
                        }
                    }
                ],
                series: [
                             {
                                 name: '预算金额',
                                 type: 'bar',
                                 data: dataVal1,

                             },
                             {
                                 name: '执行金额',
                                 type: 'bar',
                                 data: dataVal2
                             }
                ]
            };
            eChart.setOption(option);
        };
        function getEcharsData2() {
            var hfdArrayTite = F('<%= hfdArray2.ClientID%>').getValue();
            var hfdArrayVal2 = F('<%= hfdArrayVal2.ClientID%>').getValue();
            var dataTitl = new Array();
            for (var i = 0; i < hfdArrayTite.split(",").length; i++) {
                dataTitl.push(hfdArrayTite.split(",")[i]);
            }
            var dataVal1 = new Array();
            var dataVal2 = new Array();
            for (var i = 0; i < hfdArrayVal2.split(",").length; i++) {
                // dataVal.push(eval('(' + '{"data":[' + hfdArrayVal2.split(",")[i].split("$")[0].replace('|', ',') + '],"type":"bar", "name":"' + hfdArrayVal2.split(",")[i].split("$")[1] + '"}' + ')'));
                dataVal1.push(parseFloat(hfdArrayVal2.split(",")[i].split("|")[0]));
                dataVal2.push(parseFloat(hfdArrayVal2.split(",")[i].split("|")[1]));
            }
            var eChart2 = echarts.init($('#echart2')[0]);
            var option = {
                color: ['#E5516A', '#51AFEF'],
                title: {
                    text: '单品预算执行情况分析',
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: ['预算金额', '执行金额'],
                    show: true
                },
                xAxis: [
                    {
                        type: 'category',
                        data: dataTitl,
                        axisLabel: {
                            interval: 0,
                            formatter: function (params) {
                                var newParamsName = "";
                                var paramsNameNumber = params.length;
                                var provideNumber = 4;
                                var rowNumber = Math.ceil(paramsNameNumber / provideNumber);
                                if (paramsNameNumber > provideNumber) {
                                    for (var p = 0; p < rowNumber; p++) {
                                        var tempStr = "";
                                        var start = p * provideNumber;
                                        var end = start + provideNumber;
                                        if (p == rowNumber - 1) {
                                            tempStr = params.substring(start, paramsNameNumber);
                                        } else {
                                            tempStr = params.substring(start, end) + "\n";
                                        }
                                        newParamsName += tempStr;
                                    }

                                } else {
                                    newParamsName = params;
                                }
                                return newParamsName
                            }
                        }
                    }
                ],
                yAxis: [
                    {
                        type: 'value',
                        name: '金额',
                        min: 'auto',
                        axisLabel: {
                            formatter: '{value} 元'
                        }
                    }
                ],
                series: [
                             {
                                 name: '预算金额',
                                 type: 'bar',
                                 data: dataVal1
                             },
                             {
                                 name: '执行金额',
                                 type: 'bar',
                                 data: dataVal2
                             }
                ]

            };
            eChart2.setOption(option);


        };
    </script>
</body>
</html>
