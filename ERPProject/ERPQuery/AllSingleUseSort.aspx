﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllSingleUseSort.aspx.cs" Inherits="ERPProject.ERPQuery.AllSingleUseSort" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>全院单品消耗情况排名</title>
    <style type="text/css">
        .f-grid-row[data-color=color1],
        .f-grid-row[data-color=color1] .ui-icon,
        .f-grid-row[data-color=color1] a {
            color: #f00;
        }

        .f-grid-row[data-color=color2],
        .f-grid-row[data-color=color2] .ui-icon,
        .f-grid-row[data-color=color2] a {
            color: #0f0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="单品使用分析" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：医院单品使用分析查询主界面！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" DisableControlBeforePostBack="false"/>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="10px 0px 10px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="30% 30% 20% 20%">
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" LabelWidth="60px" Width="200px"></f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="开始时间" Required="true" ShowRedStar="true" LabelWidth="80px" Width="220px"></f:DatePicker>
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label=" 至" LabelSeparator="" Required="true" ShowRedStar="true" LabelWidth="40px" Width="180px"></f:DatePicker>
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
                                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridGoods_RowDoubleClick" OnSort="GridGoods_Sort" SortDirection="DESC" SortField="HSJE" PageSize="20"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridGoods_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="120px"  DataField="GDSEQ" ColumnID="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField MinWidth="150px" ExpandUnusedSpace="true" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                                        <f:GroupField runat="server" HeaderText="数量分析" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="110px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="使用数量" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="ZBSL" SortField="ZBSL" HeaderText="数量占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="SLTB" SortField="SLTB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="SLHB" SortField="SLHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField runat="server" HeaderText="金额分析" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="110px" DataField="HSJE" SortField="HSJE" ColumnID="HSJE" HeaderText="使用金额" TextAlign="Right" DataFormatString="{0:f2}" />
                                                <f:BoundField Width="90px" DataField="ZBJE" SortField="ZBJE" HeaderText="金额占比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="JETB" SortField="JETB" HeaderText="同比" DataFormatString="{0:p2}" TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="JEHB" SortField="JEHB" HeaderText="环比" DataFormatString="{0:p2}" TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="商品使用排行" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="False" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：按单品使用日期倒序排列！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnCl" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExpt" runat="server" CssStyle="margin-left: 10px;" Icon="DatabaseGo" EnableAjax="false" EnableDefaultState="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" DisableControlBeforePostBack="false"/>
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
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:TextBox ID="lisGDSEQ" runat="server" Label="商品信息" EmptyText="请输入商品信息" MaxLength="20" />
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
                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowPaging="true" IsDatabasePaging="true" SortField="HSJE" SortDirection="DESC" PageSize="50" OnSort="GridList_Sort" OnPageIndexChange="GridList_PageIndexChange"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField Width="40px" EnablePagingNumber="true"></f:RowNumberField>
                                        <f:BoundField Width="110px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" />
                                        <f:BoundField Width="220px" DataField="GDNAME" ColumnID="GDNAME" SortField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="110px" DataField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />

                                        <f:BoundField Width="100px" DataField="SL" ColumnID="SL2" SortField="SL" HeaderText="使用数" DataFormatString="{0:f2}" TextAlign="Right" />

                                        <f:BoundField Width="110px" DataField="HSJE" ColumnID="HSJE2" HeaderText="使用金额" DataFormatString="{0:f2}" TextAlign="Right" />

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
        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdArrayVal2" runat="server"></f:HiddenField>
           <f:HiddenField ID="Totalsl" runat="server"></f:HiddenField>
        <f:HiddenField ID="Totalje" runat="server"></f:HiddenField>               
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script type="text/javascript">
          function getEcharsData() {
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var hfdArrayVal = F('<%= hfdArrayVal2.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfdArrayVal.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfdArrayVal[i].split("$")[0] + '", "name":"' + hfdArrayVal[i].split("$")[1] + '"}' + ')'));
            }
            var eChart = echarts.init($('#echart')[0]);
            var option = {
                title: {
                    text: '科室数量占比',
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
            eChart.setOption(option);
        };
        function getEcharsData2() {
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var hfdArrayVal = F('<%= hfdArrayVal.ClientID%>').getValue().split(",");
            var hfdArrayVal2 = F('<%= hfdArrayVal2.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfdArrayVal.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfdArrayVal[i].split("$")[0] + '", "name":"' + hfdArrayVal[i].split("$")[1] + '"}' + ')'));
            }
            //  var eChart = echarts.init($('#echart2')[0]);

            var eChart = echarts.init($('#echart2')[0]);
            var option = {
                title: {
                    text: '科室金额占比',
                    subtext: '',
                    x: 'center'
                },
                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },
                legend: {
                    orient: 'center',
                    left: 'left',
                    data: datatitl
                },
                series: [
                    {
                        name: '来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['80%', '60%'],
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
            eChart.setOption(option);
        };
    </script>
</body>
</html>
