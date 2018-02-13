﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexPrice.aspx.cs" Inherits="ERPProject.ERPWorkbench.IndexPrice" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../res/js/ichart.1.2.min.js" type="text/javascript"></script>
    <title>财务科长首页</title>
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

        #Panel1 .f-panel {
            border-radius: 6px;
        }


            #Panel1 .f-panel.panel-up {
                border-bottom: 0;
                border-bottom-left-radius: 0;
                border-bottom-right-radius: 0;
            }

            #Panel1 .f-panel.panel-down {
                border-top-left-radius: 0;
                border-top-right-radius: 0;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" AjaxLoadingType="Mask" EnableAjax="true" />
        <f:Timer ID="Timer1" Interval="60" Enabled="false" OnTick="Timer1_Tick" EnableAjaxLoading="false" runat="server" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" EnableCollapse="true"
            Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            BoxConfigChildMargin="0 0 0 0" BodyPadding="1px" ShowHeader="false">
            <Items>
                <f:Panel ID="Panel2" Title="左" BoxFlex="1" runat="server"
                    BodyPadding="5px" Margin="0 0 0 0" ShowBorder="false" ShowHeader="false" Layout="VBox">
                    <Items>
                        <f:Panel runat="server" Title="结算信息" BoxFlex="1" BodyPadding="5px" Layout="Fit">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool4">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openBW" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="true" ShowHeader="false">
                                    <div style="width: 100%;" id="echart3"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel8" ShowHeader="true" runat="server" Title="科室月耗排行" Layout="Fit" BoxFlex="2" BodyPadding="0px">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openDB" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:Grid ID="GridDept" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                                    DataKeyNames="SEQNO" EnableRowLines="true">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center"></f:RowNumberField>
                                        <f:BoundField DataField="NAME" DataFormatString="{0:f2}" HeaderText="科室名称" MinWidth="120px" ExpandUnusedSpace="true" />
                                        <f:BoundField DataField="SYJE" DataFormatString="{0:f2}" TextAlign="Right" HeaderText="使用金额" Width="200px" />
                                        <f:BoundField DataField="YSJE" DataFormatString="{0:f2}" TextAlign="Right" HeaderText="预算金额" Width="200px" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel3" Title="右" BoxFlex="1" runat="server"
                    Margin="5px 5px 5px 0" ShowBorder="false" ShowHeader="false" Layout="VBox">
                    <Items>
                        <f:Panel ID="PanelDelivery" runat="server" Margin="0 0 0 0" BoxFlex="1" Layout="Fit"
                            BodyPadding="5px" ShowBorder="true" ShowHeader="true" Title="库存损益情况" CssStyle="overflow-x:auto;border-bottom: 0px;border-bottom-left-radius: 0;border-bottom-right-radius: 0;">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool2">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openPS" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="true" ShowHeader="false">
                                    <div style="width: 100%;" id="echart"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel5" runat="server" Margin="0 0 0 0" AutoScroll="true" BoxFlex="2" CssStyle="border-top-left-radius: 0;border-top-right-radius: 0;"
                            BodyPadding="5px" ShowBorder="true" ShowHeader="true" RegionPosition="Left" Title="科室预算执行情况" Layout="Fit"
                            EnableIFrame="false">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool3">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openYS" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="true" ShowHeader="false">
                                    <div style="width: 100%;" id="echart2"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdRoleList" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdWcl" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdjs1" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdjs2" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdsy1" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdsy2" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdsy3" runat="server"></f:HiddenField>
        <f:Window ID="WindowMemo" Width="650px" Height="450px" Icon="TagBlue" Title="系统公告通知"
            EnableCollapse="false" runat="server" EnableResize="true" EnableClose="false"
            IsModal="true" AutoScroll="true" BodyPadding="10px" Hidden="true">
            <Content>
                <p><%=MyMemo %></p>
            </Content>
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Center">
                    <Items>
                        <f:HiddenField ID="hfdSeqNo" runat="server"></f:HiddenField>
                        <f:Button runat="server" ID="btnClose" OnClick="btnClose_Click" Text="我知道了" Icon="RosetteBlue"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script>
        var basePath = '<%= ResolveUrl("~") %>';
        //function openTODOLINK(id, url, name) {
        //    parent.addExampleTab.apply(null, [id, basePath + url, name, basePath + 'extjs/res/ext-theme-classic/images/tree/leaf.gif', true]);
        //}
        function openBW() {
            openTODOLINK('1403', '/ERPPayment/BillChecking_KS.aspx', '结算单查询')
        }
        function openDB() {
            openTODOLINK('1402', '/ERPQuery/ToDoQuery.aspx', '待办事宜查询')
        }
        function openPS() {
            openTODOLINK('7123', '/ERPQuery/TodayPs.aspx', '配送情况跟踪')
        }
        function openYS() {
            openTODOLINK('6709', '/ERPApply/DepartmentBudget.aspx', '科室预算分析')

        }
        function openTODOLINK(v_id, v_url, v_name) {
            top.F.addMainTab(
              parent.F(top.mainTabStripClientID),
                {
                    id: v_id,
                    iframeUrl: v_url,
                    title: v_name,
                    closable: true,
                    refreshWhenExist: false
                });
        }
    </script>
    <script type="text/javascript">
        function reloaddata(ysje, xsje) {
            $('#echart2').height($('#echart2').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart2')[0]);
            var option = {
                title: {
                    text: '科室预算汇总',
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow'
                    }
                },
                legend: {
                    data: ['预算金额', '销售金额']
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                yAxis: {
                    type: 'value',
                    boundaryGap: [0, 0.01]
                },
                xAxis: {
                    type: 'category',
                    data: []
                },
                series: [
                    {
                        name: '预算金额',
                        type: 'bar',
                        data: [ysje]
                    },
                    {
                        name: '销售金额',
                        type: 'bar',
                        data: [xsje]
                    }
                ]
            };
            eChart.setOption(option);
        }
        function getEcharsData() {
            $('#echart').height($('#echart').parents('.f-panel-body').height());
            var datatitl = F('<%= hfdsy1.ClientID%>').getValue().split(',');
            var datatit2 = F('<%= hfdsy2.ClientID%>').getValue().split(',');
            var datatit3 = F('<%= hfdsy3.ClientID%>').getValue().split(',');
            var eChart = echarts.init($('#echart')[0]);
            var option = {
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                        type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
                    }
                },
                legend: {
                    data: ['损耗', '益余']
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                yAxis: {
                    type: 'value'
                },
                xAxis: {
                    type: 'category',
                    data: datatitl
                },
                series: [
                    {
                        name: '损耗',
                        type: 'bar',
                        stack: '总量',
                        label: {
                            normal: {
                                show: true,
                                position: 'insideRight'
                            }
                        },
                        data: datatit2
                    },
                    {
                        name: '益余',
                        type: 'bar',
                        stack: '总量',
                        label: {
                            normal: {
                                show: true,
                                position: 'insideRight'
                            }
                        },
                        data: datatit3
                    }]
            };
            eChart.setOption(option);
        }
        function getEcharsData3() {
            $('#echart3').height($('#echart3').parents('.f-panel-body').height());
            var dataVal1 = F('<%= hfdjs1.ClientID%>').getValue().split(",");
            var dataVal2 = F('<%= hfdjs2.ClientID%>').getValue().split(",");
            var eChart = echarts.init($('#echart3')[0]);
            var option = {
                title: {
                    text: '年度结算信息',
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow'
                    }
                },
                legend: {
                    data: dataVal1
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                yAxis: {
                    type: 'value',
                    boundaryGap: [0, 0.01]
                },
                xAxis: {
                    type: 'category',
                    data: dataVal1
                },
                series: [
                    {
                        name: '结算金额',
                        type: 'bar',
                        data: dataVal2
                    }
                ]
            };
            eChart.setOption(option);
        };
    </script>
</body>
</html>