﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeptWorkShow.aspx.cs" Inherits="ERPProject.ERPQuery.DeptWorkShow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title>科室展示看板</title>
    <link href="../res/fullcalendar/fullcalendar.css" rel="stylesheet" />
    <link href="../res/fullcalendar/fullcalendar.print.css" rel="stylesheet" />
    <link href="../res/fullcalendar/theme.css" rel="stylesheet" />
    <style type="text/css">
    </style>
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
                                <f:Panel ID="Panel5" Title="面板1" BoxFlex="1" runat="server" Layout="HBox"
                                    BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:Panel runat="server" ShowHeader="false" ShowBorder="false" Layout="Fit" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                                    <div style="width: 100%;" id="echart"></div>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                        <f:Panel runat="server" ShowHeader="false" ShowBorder="false" Layout="Fit" BoxFlex="1">
                                            <Items>
                                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false" CssStyle="padding-top:60px">
                                                    <marquee scrollamount="2" align="absmiddle" direction="up">
                                                       <font size=+3 color=red>Hello, World</font>
                                                    </marquee>
                                                </f:ContentPanel>
                                            </Items>
                                        </f:Panel>
                                    </Items>
                                </f:Panel>
                                <f:Panel ID="Panel6" Title="面板2" BoxFlex="2" Margin="0" Layout="Fit"
                                    runat="server" BodyPadding="0px" ShowBorder="false" ShowHeader="false">
                                    <Items>
                                        <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                            <div style="width: 100%;" id="calendar"></div>
                                        </f:ContentPanel>
                                    </Items>
                                </f:Panel>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="Panel1" runat="server" Width="650px" ShowBorder="false" BodyPadding="0px" Layout="Fit"
                            ShowHeader="false">
                            <Items>
                                <f:Grid ID="GridPs" runat="server" CssStyle="border-top: 0px;" EnableColumnLines="true" ShowHeader="false" OnRowDataBound="gridPs_RowDataBound">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="30px"></f:RowNumberField>
                                        <f:BoundField runat="server" DataField="GDNAME" HeaderText="商品" MinWidth="120px" ExpandUnusedSpace="true"></f:BoundField>
                                        <f:BoundField runat="server" DataField="GDSPEC" HeaderText="规格" Width="110px"></f:BoundField>
                                        <f:BoundField runat="server" TextAlign="Center" DataField="KCSL" ColumnID="KCSL" HeaderText="库存数" Width="60px"></f:BoundField>
                                        <f:BoundField runat="server" TextAlign="Center" DataField="DSNUM" HeaderText="定数数量" Width="70px"></f:BoundField>
                                        <f:BoundField runat="server" TextAlign="Center" DataField="NUM1" HeaderText="定数含量" Width="70px"></f:BoundField>
                                        <f:BoundField runat="server" TextAlign="Center" DataField="ZGKC" ColumnID="ZGKC" HeaderText="库存上限" Width="70px"></f:BoundField>
                                        <f:BoundField runat="server" TextAlign="Center" DataField="ZDKC" ColumnID="ZDKC" HeaderText="库存下限" Width="70px"></f:BoundField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdArray" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdSJXH" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdYSXH" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdPS" runat="server"></f:HiddenField>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script src="../res/fullcalendar/jquery-ui-1.10.2.custom.min.js"></script>
    <script src="../res/fullcalendar/fullcalendar.min.js"></script>
    <script type="text/javascript">
        function getEcharsData() {
            $('#echart').height($('#echart').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart')[0]);
            var datatitl = F('<%= hfdArray.ClientID%>').getValue().split(",");
            var hfd21 = F('<%= hfdYSXH.ClientID%>').getValue().split(",");
            var hfd22 = F('<%= hfdSJXH.ClientID%>').getValue().split(",");
            hfd21arr = new Array();
            hfd22arr = new Array();
            for (var i = 0; i < hfd21.length; i++) {
                hfd21arr.push(hfd21[i]);
            }
            for (var i = 0; i < hfd22.length; i++) {
                hfd22arr.push(hfd22[i]);

            }
            option = {
                title: {
                    text: '月度申领、出库'
                },
                tooltip: {
                    trigger: 'axis'
                },
                legend: {
                    data: ['实际消耗金额', '预算消耗金额']
                },
                grid: {
                    left: '3%',
                    right: '4%',
                    bottom: '3%',
                    containLabel: true
                },
                xAxis: {
                    type: 'category',
                    boundaryGap: false,
                    data: ['1月', '2月', '3月', '4月', '5月', '6月', '7月', '8月', '9月', '10月', '11月', '12月']
                },
                yAxis: {
                    type: 'value'
                },
                series: [
                    {
                        name: '实际消耗金额',
                        type: 'line',
                        stack: '金额',
                        data: hfd22
                    },
                    {
                        name: '预算消耗金额',
                        type: 'line',
                        stack: '金额',
                        data: hfd21
                    }
                ]
            };
            eChart.setOption(option);
        };
        function calendar() {
            var date = new Date();
            var d = date.getDate();
            var m = date.getMonth();
            var y = date.getFullYear();
            var day = new Date(y, m + 1, 0);
            var arr = [];
            var hfdps = F('<%= hfdPS.ClientID%>').getValue();
            $.get("/captcha/deptWorkShow.ashx", { "type": "3021" }, function (result) {
                $('#calendar').height($('#calendar').parents('.f-panel-body').height());
                var arr1 = eval(result);
                for (var j = 0; j < arr1.length; j++) {
                    arr.push(eval({ title: "已配送【" + arr1[j].HSJE + "元】商品", start: new Date(y, m, parseInt(arr1[j].DD)) }));
                }
                console.log(hfdps);
                for (var i = d; i < day.getDate() ; i++) {
                    console.log((new Date(y, m + 1, i)).getDay());
                    if ((new Date(y, m + 1, i + 1)).getDay() == hfdps) {
                        arr.push(eval({ title: "定数待配送", start: new Date(y, m, i + 1) }));
                    }
                }

                $('#calendar').fullCalendar({
                    theme: false,
                    header: {
                        left: 'today',
                        center: 'title',
                        right: 'month'
                    },
                    editable: false,
                    events: arr
                });
            });
        };
    </script>
</body>
</html>
