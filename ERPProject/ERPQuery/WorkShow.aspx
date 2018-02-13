﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkShow.aspx.cs" Inherits="ERPProject.ERPQuery.WorkShow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>展示看板</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" ShowTabHeader="false"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Layout="HBox" runat="server">
                    <Items>
                        <f:Panel runat="server" ShowBorder="false" ShowHeader="false" BoxFlex="2" Layout="VBox">
                            <Items>
                                <f:Panel ID="Panel5" Title="面板1" Height="400px" runat="server" Layout="Fit"
                                    BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                            <div style="width: 100%;" id="echart"></div>
                                        </f:ContentPanel>
                                    </Items>
                                </f:Panel>
                                <f:Panel ID="Panel6" Title="面板2" BoxFlex="1" Margin="0" Layout="Fit"
                                    runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                            <div style="width: 100%;" id="echart0"></div>
                                        </f:ContentPanel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" BoxFlex="3" Layout="VBox"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" Title="面板1" Height="400px" runat="server" Layout="Fit"
                                    BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                            <div style="width: 100%;" id="echart2"></div>
                                        </f:ContentPanel>
                                    </Items>
                                </f:Panel>
                                <f:Panel ID="Panel4" Title="面板2" BoxFlex="1" Margin="0" Layout="Fit"
                                    runat="server" BodyPadding="5px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                            <div style="width: 100%;" id="echart3"></div>
                                        </f:ContentPanel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel1" runat="server" Width="500px" ShowBorder="false" BodyPadding="0px" Layout="Fit"
                            ShowHeader="false">
                            <Items>
                                <f:Grid ID="gridPs" runat="server" CssStyle="border-top: 0px;" ShowHeader="false">
                                    <Columns>
                                        <f:ImageField ImageHeight="50px" runat="server" ColumnID="IMAGEURL" DataImageUrlField="IMAGEURL" Width="60px"></f:ImageField>
                                        <f:BoundField runat="server" ColumnID="DEPTNAME" DataField="DEPTNAME" HeaderText="科室" ExpandUnusedSpace="true"></f:BoundField>
                                        <f:BoundField runat="server" ColumnID="PRESULT" DataField="PRESULT" HeaderText="状态" ExpandUnusedSpace="true"></f:BoundField>
                                        <f:BoundField runat="server" ColumnID="PSTIME" DataField="PSTIME" HeaderText="时间"></f:BoundField>
                                        <f:TemplateField HeaderText="进度" Width="110px">
                                            <ItemTemplate>
                                                <div style="width: 100px; border-width: 1px; border-style: solid; height: 12px">
                                                    <div id="ycdiv" style="width: 2px; height: 10px; border-color: #999999; background-color: grey;"></div>
                                                </div>
                                            </ItemTemplate>
                                        </f:TemplateField>
                                        <f:BoundField runat="server" DataField="WC" ColumnID="WC" Hidden="true"></f:BoundField>
                                        <f:BoundField runat="server" DataField="YC" ColumnID="YC" Hidden="true"></f:BoundField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfd1" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfd21" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfd22" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfd3" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfd4" runat="server"></f:HiddenField>

    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function updatedata() {
            var grid = F('<%= gridPs.ClientID %>');
            var wc = 0;
            var yc = 0;
            grid.getRowEls().each(function (index, tr) {
                wc = grid.getCellValue(tr, 'WC');
                yc = grid.getCellValue(tr, 'YC');
                if (yc.length == 0) {
                    yc = 0;
                }
                $("ycdiv").width(Math.round(yc * 30 / wc * 100));


                //alert($("ycdiv").width);
                
              })

           

        }
        function getEcharsData_PSLY() {
            $('#echart').height($('#echart').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart')[0]);
            var hfd1 = F('<%= hfd1.ClientID%>').getValue().split(",");
            var dataVal = new Array();
            for (var i = 0; i < hfd1.length; i++) {
                dataVal.push(eval('(' + '{"value":"' + hfd1[i].split("$")[0] + '", "name":"' + hfd1[i].split("$")[1] + '"}' + ')'));
            }
            var option = {
                title: {
                    text: '本月入库配送来源',
                    top: 15
                },

                tooltip: {
                    trigger: 'item',
                    formatter: "{a} <br/>{b} : {c} ({d}%)"
                },

                //visualMap: {
                //    show: false,
                //    min: 1,
                //    max: 300

                //},
                series: [
                    {
                        name: '访问来源',
                        type: 'pie',
                        radius: '55%',
                        center: ['50%', '50%'],
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
        function getEcharsData_PSSL() {
            $('#echart0').height($('#echart0').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart0')[0]);
            var hfd21 = F('<%= hfd21.ClientID%>').getValue().split(",");
            var hfd22 = F('<%= hfd22.ClientID%>').getValue().split(",");
            hfd21arr = new Array();
            hfd22arr = new Array();
            for (var i = 0; i < hfd21.length; i++) {
                hfd21arr.push(hfd21[i]);
            }
            for (var i = 0; i < hfd22.length; i++) {
                hfd22arr.push(hfd22[i]);

            }
            var option = {
                title: {
                    text: '周配送科室量'
                },
                tooltip: {
                    trigger: 'axis',
                    axisPointer: {
                        type: 'shadow'
                    }
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '86px',
                    containLabel: true
                },
                yAxis: {
                    type: 'value',
                    name: '金额',
                    boundaryGap: [0, 0.01]
                },
                xAxis: {
                    type: 'category',
                    data: ['周一', '周二', '周三', '周四', '周五', '周六', '周日']
                },
                series: [

                    {
                        name: '计划配送科室量',
                        type: 'bar',
                        data: hfd21arr
                    },
                    {
                        name: '实际配送科室量',
                        type: 'bar',
                        data: hfd22arr
                    }
                ]
            };
            eChart.setOption(option);
        };
        function getEcharsData_WCQK() {
            $('#echart2').height($('#echart2').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart2')[0]);
            var hfd3 = F('<%= hfd3.ClientID%>').getValue().split(",");
            var myDate = new Date()
            var option = {
                title: [{
                    text: '配送完成情况'
                }, {
                    text: myDate.getFullYear() + "年" + (myDate.getMonth() + 1) + "月" + myDate.getDate() + "日  星期" + myDate.getDay(),
                    top: 30
                }],
                tooltip: {
                    formatter: "{a} <br/>{b} : {c}%"
                },
                series: [
                    {
                        name: '配送情况',
                        type: 'gauge',
                        radius: '85%',
                        detail: { formatter: '{value}%' },
                        data: [{ value: hfd3, name: '科室完成率' }],
                        axisLine: {            // 坐标轴线
                            lineStyle: {       // 属性lineStyle控制线条样式
                                width: 20,
                                color: [[hfd3 / 100, '#2F4554'], [1, '#CC0F1C']],
                                shadowColor: '#fff', //默认透明
                                shadowBlur: 10
                            }
                        }
                    }
                ]
            };
            eChart.setOption(option);
        };

        function getEcharsData_PSQK(dmap, datauser, datadept) {
            $('#echart3').height($('#echart3').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart3')[0]);
            var dataMap = {};
            var datadmap = new Array();
            var datauserarr = new Array();
            var datadeptarr = new Array();
            for (var i = 0; i < datauser.split(",").length; i++) {
                datauserarr.push(datauser.split(",")[i]);
            }
            for (var i = 0; i < datadept.split(",").length; i++) {
                datadeptarr.push(datadept.split(",")[i]);
            }
            function dataFormatter(obj) {
                return obj;
            }
            var hfd = F('<%= hfd4.ClientID%>').getValue().split("$");
            for (var i = 0; i <= hfd.length - 1; i++) {
                if (i == hfd.length - 1) {
                    datadmap.push(eval('i+1+": "+hfd[i]+" "'));

                }
                else {
                    datadmap.push(eval('i+1+": "+hfd[i]+", "'));

                }
            }
            dataMap.dataGDP = datadeptarr;
            //dataFormatter({

            //1: [11307.28, 24515.76, 11237.55, 14359.88, 22226.7, 10568.83, 12582, 19195.69, 49110.27, 32318.85],
            //2: [9224.46, 20394.26, 9200.86, 11672, 18457.27, 8667.58, 10368.6, 17165.98, 41425.48, 27722.31],
            //3: [7521.85, 17235.48, 7358.31, 9740.25, 15212.49, 7278.75, 8587, 15046.45, 34457.3, 22990.35],
            //4: [6719.01, 16011.97, 7315.4, 8496.2, 13668.58, 6426.1, 8314.37, 14069.87, 30981.98, 21462.69],
            //5: [5252.76, 13607.32, 6024.45, 6423.18, 11164.3, 5284.69, 7104, 12494.01, 26018.48, 18753.73],
            //6: [4462.74, 11467.6, 4878.61, 4944.25, 9304.52, 4275.12, 6211.8, 10572.24, 21742.05, 15718.47],
            //7: [3905.64, 10012.11, 4230.53, 3905.03, 8047.26, 3620.27, 5513.7, 9247.66, 18598.69, 13417.68],
            //8: [3110.97, 8477.63, 3571.37, 3041.07, 6672, 3122.01, 4750.6, 8072.83, 15003.6, 11648.7],
            //9: [2578.03, 6921.29, 2855.23, 2388.38, 6002.54, 2662.08, 4057.4, 6694.23, 12442.87, 9705.02],
            //10: [2150.76, 6018.28, 2324.8, 1940.94, 5458.22, 2348.54, 3637.2, 5741.03, 10606.85, 8003.67],
            //11: [1107.28, 24515.76, 1137.55, 14359.88, 2226.7, 1068.83, 1282, 19195.69, 49110.27, 32318.85],
            //12: [9224.46, 2094.26, 900.86, 11672, 18457.27, 8667.58, 10368.6, 1765.98, 41425.48, 2722.31]
            // });
            option = {
                baseOption: {
                    timeline: {
                        // y: 0,
                        axisType: 'category',
                        // realtime: false,
                        // loop: false,
                        autoPlay: true,
                        // currentIndex: 2,
                        playInterval: 2000,
                        data: datauserarr
                        //[
                        //    '干乐', '曹丹', '于明辉',

                        //    '王岩玲'
                        //]
                    },
                    tooltip: {},
                    legend: {
                        x: 'right',
                        data: ['GDP'],
                    },
                    calculable: true,
                    grid: {
                        top: 80,
                        bottom: 100
                    },
                    xAxis: [
                        {
                            'type': 'category',
                            'axisLabel': { 'interval': 0 },
                            'data':
                                //[
                            //    '内科急诊', '\n手术室', '病理科', '\n介入室', '呼吸内科', '\n消化内科', '心胸外科', '\n神经内科',
                            //    '骨外科', '\n普外科', '产科'
                            //]
                            datadeptarr,
                            splitLine: { show: false }
                        }
                    ],
                    yAxis: [
                        {
                            type: 'value',
                            name: '科室配送金额'
                        }
                    ],
                    series: [
                        { name: '金额', type: 'bar' },
                    ]
                },
                options: [
                    {
                        title: { text: '配送情况' },
                        series: [
                            { data: dataMap.dataGDP['1'] }
                        ]
                    },
                    {
                        title: { text: '配送情况' },
                        series: [
                            { data: dataMap.dataGDP['2'] },
                        ]
                    },
                    {
                        title: { text: '配送情况' },
                        series: [
                            { data: dataMap.dataGDP['3'] },
                        ]
                    },
                    {
                        title: { text: '配送情况' },
                        series: [
                            { data: dataMap.dataGDP['4'] },
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['5'] },
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['6'] }
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['7'] },
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['8'] },
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['9'] },
                        ]
                    },
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['10'] },
                        ]
                    }
                    ,
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['11'] },
                        ]
                    }
                    ,
                    {
                        title: { text: '科室消耗' },
                        series: [
                            { data: dataMap.dataGDP['12'] },
                        ]
                    }
                ]
            };
            eChart.setOption(option);
        };
    </script>
</body>
</html>
