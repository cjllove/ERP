<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkPanal.aspx.cs" Inherits="ERPProject.ERPQuery.WorkPanal" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>配送面板</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" ShowTabHeader="false"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Layout="HBox" runat="server">
                    <Items>
                        <f:Panel runat="server" Width="200px" ShowBorder="false" ShowHeader="false" BoxFlex="1" Layout="Fit">
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="true" ShowHeader="false">
                                    <div style="width: 100%;" id="echart2"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" BoxFlex="2" Layout="Fit"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar runat="server">
                                    <Items>
                                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                                        <f:DatePicker ID="dpkTime" runat="server" Label="配送日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                        <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" runat="server" EnableDefaultState="false" OnClick="btnSearch_Click" />
                                        <f:Button runat="server" Icon="ArrowIn" Text="配送汇总" ID="btnHz" Hidden="true" OnClick="btnHz_Click"></f:Button>
                                        <f:Button runat="server" ID="ChgShow" Text="详细信息" Icon="ArrowTurnRight" OnClick="ChgShow_Click" EnableDefaultState="false"></f:Button>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%;" id="echart"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="HBox" AutoScroll="true"
                            ShowHeader="false">
                            <Items>
                                <f:Grid ID="grdWeak" ShowBorder="false" ShowHeader="false" CssStyle="border-right: 1px solid #99bce8;" BoxFlex="1"
                                    EnableRowDoubleClickEvent="true" AllowSorting="true" AutoScroll="true" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:CheckBox ID="ckbAll" Text="所有科室" runat="server" LabelWidth="75px" EnableAjaxLoading="true" Checked="false" AutoPostBack="true" OnCheckedChanged="ckbAll_CheckedChanged"></f:CheckBox>
                                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" Hidden="true"></f:DropDownList>
                                                <f:DatePicker ID="lisDATE" runat="server" Label="配送日期" Required="true" ShowRedStar="true" Hidden="true"></f:DatePicker>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button runat="server" ID="Chag" Text="视图" Icon="ArrowTurnLeft" OnClick="Chag_Click" EnableDefaultState="false"></f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="NAME" SortField="NAME" HeaderText="科室" ExpandUnusedSpace="true" MinWidth="120px" />
                                        <f:BoundField DataField="PSNAME" SortField="PSNAME" HeaderText="配送员" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ1" HeaderText="星期一" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ2" HeaderText="星期二" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ3" HeaderText="星期三" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ4" HeaderText="星期四" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ5" HeaderText="星期五" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ6" HeaderText="星期六" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:CheckBoxField DataField="XQ7" HeaderText="星期日" RenderAsStaticField="true" Width="80px" TextAlign="Center" />
                                        <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                                            <ItemTemplate>
                                                <div class="expander">
                                                    <p>
                                                        <strong>【<%# Eval("PSNAME") %>】配送详细信息：</strong>
                                                    </p>
                                                    <p>
                                                        <strong>待配送：</strong><%# Eval("DSXQ") %>
                                                    </p>
                                                    <p>
                                                        <strong>配送中：</strong><%# Eval("PSXQ") %>
                                                    </p>
                                                </div>
                                            </ItemTemplate>
                                        </f:TemplateField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <%-- <f:Tab Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel3" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Fit"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar runat="server">
                                    <Items>
                                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                                        <f:Button runat="server" ID="btnMX" Text="配送明细" Icon="ArrowInout" OnClick="btnMX_Click" EnableDefaultState="false"></f:Button>
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%;" id="echart2"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>--%>
            </Tabs>
        </f:TabStrip>

        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdVal1" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdVal2" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdVal3" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdVal4" runat="server"></f:HiddenField>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function getEcharsData() {
            $('#echart').height($('#echart').parents('.f-panel-body').height());
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var dataVal1 = F('<%= hfdVal1.ClientID%>').getValue().split(",");
            var dataVal2 = F('<%= hfdVal2.ClientID%>').getValue().split(",");
            var dataVal3 = F('<%= hfdVal3.ClientID%>').getValue().split(",");
            var eChart = echarts.init($('#echart')[0]);
            var option = {
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                        type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
                    }
                },
                legend: {
                    orient: 'vertical',
                    left: 'left',
                    top: '2%',
                    data: ['待配送品规', '配送中品规', '已配送品规']
                },
                grid: {
                    left: '120',
                    right: '4%',
                    bottom: '2%',
                    top: '2%',
                    containLabel: true
                },
                xAxis: {
                    type: 'value'
                },
                yAxis: [{
                    type: 'category',
                    data: datatitl
                }
                ],
                series: [
                    {
                        name: '待配送品规',
                        type: 'bar',
                        stack: '总量',
                        label: {
                            normal: {
                                show: true,
                                position: 'inside'
                            }
                        },
                        itemStyle: {
                            normal: { color: '#62B057' }
                        },
                        data: dataVal1
                    },
            {
                name: '配送中品规',
                type: 'bar',
                stack: '总量',
                label: {
                    normal: {
                        show: true,
                        position: 'inside'
                    }
                },
                data: dataVal2
            },
        {
            name: '已配送品规',
            type: 'bar',
            stack: '总量',
            label: {
                normal: {
                    show: true,
                    position: 'inside'
                }
            },
            data: dataVal3
        }
                ]
            };
            eChart.setOption(option);
        };
        function getEcharsData2(var1) {
            $('#echart2').height($('#echart2').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart2')[0]);
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var option = {
                tooltip: {
                    formatter: "{a} <br/>{b} : {c}%"
                },
                series: [
                    {
                        name: '配送情况',
                        type: 'gauge',
                        radius: '85%',
                        detail: { formatter: '{value}%' },
                        data: [{ value: var1, name: '科室完成率' }],
                        axisLine: {            // 坐标轴线
                            lineStyle: {       // 属性lineStyle控制线条样式
                                width: 20,
                                color: [[var1 / 100, '#1475BB'], [1, '#CC0F1C']],
                                shadowColor: '#fff', //默认透明
                                shadowBlur: 10
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
