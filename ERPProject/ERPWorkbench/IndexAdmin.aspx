﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexAdmin.aspx.cs" Inherits="ERPProject.ERPWorkbench.IndexAdmin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        .panel-radius {
            border-radius: 6px;
        }

        .panal-top {
            border-bottom-left-radius: 0;
            border-bottom-right-radius: 0;
            border-bottom: 0;
        }

        .panal-down {
            border-top-left-radius: 0;
            border-top-right-radius: 0;
        }


        /*-------------------系统更新样式------------------------*/
        h1 {
            font-family: 微软雅黑,黑体;
        }

            h1 span.lead {
                font-size: 14px;
                padding-left: 20px;
                color: #333;
            }

        ul.timeline {
            list-style-type: none;
            background: url("/ERPUpload/Wiki/images/version_line.png") repeat-y scroll 120px 0 transparent;
            margin: 20px 0;
            padding: 0;
        }

            ul.timeline li {
                position: relative;
                margin-bottom: 20px;
            }

                ul.timeline li .time {
                    position: absolute;
                    width: 90px;
                    text-align: right;
                    left: 0;
                    top: 10px;
                    color: #999;
                }

                ul.timeline li .version {
                    position: absolute;
                    width: 290px;
                    text-align: right;
                    left: -200px;
                    top: 30px;
                    font-size: 40px;
                    line-height: 50px;
                    color: #3594cb;
                    overflow: hidden;
                }

                ul.timeline li .number {
                    position: absolute;
                    background: url("/ERPUpload/Wiki/images/version_dot.png") no-repeat scroll 0 0 transparent;
                    width: 56px;
                    height: 56px;
                    left: 97px;
                    line-height: 56px;
                    text-align: center;
                    color: #fff;
                    font-size: 18px;
                }

                ul.timeline li.alt .number {
                    background-image: url("/ERPUpload/Wiki/images/version_dot_alt.png");
                }

                ul.timeline li .content {
                    margin-left: 180px;
                }

                    ul.timeline li .content pre {
                        background-color: #3594cb;
                        padding: 20px;
                        color: #fff;
                        font-size: 13px;
                        line-height: 20px;
                    }

                ul.timeline li.alt .content pre {
                    background-color: #43B1F1;
                }

        ul.timeline {
            list-style-type: none;
            background: url("/ERPUpload/Wiki/images/version_line.png") repeat-y scroll 120px 0 transparent;
            margin: 20px 0;
            padding: 0;
        }

            ul.timeline li {
                position: relative;
                margin-bottom: 20px;
            }

                ul.timeline li .time {
                    position: absolute;
                    width: 90px;
                    text-align: right;
                    left: 0;
                    top: 10px;
                    color: #999;
                }

                ul.timeline li .version {
                    position: absolute;
                    width: 290px;
                    text-align: right;
                    left: -200px;
                    top: 30px;
                    font-size: 40px;
                    line-height: 50px;
                    color: #3594cb;
                    overflow: hidden;
                }

                    ul.timeline li .version span {
                        color: #666;
                        font-size: 12px;
                        line-height: 20px;
                        display: block;
                        text-align: right;
                    }

                        ul.timeline li .version span i {
                            color: #333;
                            font-style: normal;
                        }

                ul.timeline li .number {
                    position: absolute;
                    background: url("/ERPUpload/Wiki/images/version_dot.png") no-repeat scroll 0 0 transparent;
                    width: 56px;
                    height: 56px;
                    left: 97px;
                    line-height: 56px;
                    text-align: center;
                    color: #fff;
                    font-size: 18px;
                }

                ul.timeline li.alt .number {
                    background-image: url("/ERPUpload/Wiki/images/version_dot_alt.png");
                }

                ul.timeline li .content {
                    margin-left: 180px;
                }

                    ul.timeline li .content pre {
                        background-color: #3594cb;
                        padding: 20px;
                        color: #fff;
                        font-size: 13px;
                        line-height: 20px;
                    }

                ul.timeline li.alt .content pre {
                    background-color: #43B1F1;
                }

        .footer {
            width: 150px;
            margin: 0 auto 40px;
        }

            .footer a {
                text-decoration: none;
                text-align: center;
            }

        .btn {
            display: inline-block;
            padding: 4px 12px;
            margin-bottom: 0;
            font-size: 14px;
            line-height: 20px;
            vertical-align: middle;
            cursor: pointer;
            border-radius: 4px;
            box-shadow: inset 0 1px 0 rgba(255,255,255,0.2),0 1px 2px rgba(0,0,0,0.05);
        }

        .btn-info {
            color: #fff;
            text-shadow: 0 -1px 0 rgba(0,0,0,0.25);
            background-color: #43B1F1;
            background-image: linear-gradient(to bottom,#43B1F1,#3594cb);
            background-repeat: repeat-x;
            border-color: rgba(0,0,0,0.1) rgba(0,0,0,0.1) rgba(0,0,0,0.25);
            padding: 11px 19px;
            font-size: 17.5px;
        }

        #contentUpdate {
            padding: 0 30px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" runat="server" AutoSizePanelID="Panel2" />
        <f:Panel ID="Panel2" runat="server" ShowBorder="false"
            Layout="VBox" ShowHeader="false" BodyPadding="10">
            <Items>
                <f:Panel ID="PanelLeft" BoxFlex="1" runat="server"
                    ShowBorder="false" ShowHeader="false" Layout="HBox">
                    <Items>
                        <%-- 表空间检查 --%>
                        <f:Panel ID="PanelSpace" BoxFlex="1" runat="server" ShowBorder="true" ShowHeader="true" Title="数据空间使用情况" MarginRight="10" CssClass="panel-radius panal-top" Layout="Fit">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" IconFont="Refresh" MarginRight="5" CssClass="tabtool" ToolTip="刷新本页" ID="toolRefresh">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="getMonitorData" />
                                    </Listeners>
                                </f:Tool>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool4">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openSpace" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%;" id="echart"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                        <%-- 系统操作日志 --%>
                        <f:Panel ID="PanelLog" BoxFlex="1" runat="server" ShowBorder="true" ShowHeader="true" Title="系统操作日志" CssClass="panel-radius  panal-top" Layout="Fit">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" IconFont="Refresh" MarginRight="5" CssClass="tabtool" ToolTip="刷新本页" ID="tool1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="getLogData" />
                                    </Listeners>
                                </f:Tool>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool5">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openLog" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:Grid ID="GridLog" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="userid" EnableColumnLines="true" EnableTextSelection="true">
                                    <Columns>
                                        <%--<f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" />--%>
                                        <f:BoundField Width="145px" DataField="RQSJ" ColumnID="RQSJ" HeaderText="操作时间" />
                                        <f:BoundField Width="90px" DataField="USERID" ColumnID="USERID" HeaderText="登录账号" />
                                        <f:BoundField Width="80px" DataField="FUNCID" ColumnID="FUNCID" HeaderText="功能" />
                                        <f:BoundField Width="140px" DataField="MEMO" ColumnID="MEMO" HeaderText="信息描述" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="130px" DataField="STATION" ColumnID="STATION" HeaderText="IP地址" Hidden="true" />
                                        <%--<f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                                        <ItemTemplate>
                                            <div class="expander">
                                                <p>
                                                    <strong>备注内容：</strong>
                                                    <p><%# Eval("MEMO") %></p>
                                                </p>
                                            </div>
                                        </ItemTemplate>
                                    </f:TemplateField>--%>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Panel ID="PanelRight" BoxFlex="1" runat="server"
                    ShowBorder="false" ShowHeader="false" Layout="HBox">
                    <Items>
                        <f:Panel ID="PanelInf" BoxFlex="1" runat="server" ShowBorder="true" ShowHeader="true" Title="接口运行情况" MarginRight="10" CssClass="panel-radius  panal-down" Layout="Fit">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" IconFont="Refresh" MarginRight="5" CssClass="tabtool" ToolTip="刷新本页" ID="tool2">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="getInfData" />
                                    </Listeners>
                                </f:Tool>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool6">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openInf" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:Grid ID="GridInf" ShowBorder="false" ShowHeader="false" AutoScroll="true"
                                    runat="server" DataKeyNames="INFTYPE,EXECRQ,EXECMEMO"
                                    EnableTextSelection="true">
                                    <Columns>
                                        <f:BoundField ColumnID="EXECRQ" ID="SEQNO" Width="140px" DataField="EXECRQ" HeaderText="执行时间" HtmlEncode="false" DataFormatString="{0:yyyy年MM月dd日}" />
                                        <f:BoundField ColumnID="EXECCOUNT" DataField="EXECCOUNT" Width="65px" TextAlign="Center" HeaderText="数据/条" />
                                        <f:BoundField ColumnID="EXECTIME" DataField="EXECTIME" Width="65px" TextAlign="Center" HeaderText="用时/秒" />
                                        <f:BoundField ColumnID="EXECMEMO" DataField="EXECMEMO" Width="145px" ExpandUnusedSpace="True" HeaderText="备注 " />
                                        <f:BoundField ColumnID="PARARQ" DataField="PARARQ" Width="95px" HeaderText="参数时间 " />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                        <f:Panel ID="PanelUpdate" BoxFlex="1" runat="server" ShowBorder="true" ShowHeader="true" Title="系统更新日志" CssClass="panel-radius  panal-down" Layout="Fit">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" IconFont="Refresh" MarginRight="5" CssClass="tabtool" ToolTip="刷新本页" ID="tool3">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="getUpdateData" />
                                    </Listeners>
                                </f:Tool>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool7">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openUpdate" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowHeader="false" ShowBorder="false" AutoScroll="true">
                                    <div id="contentUpdate">
                                    </div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
    </form>
    <script src="/res/js/echarts.min.js"></script>
    <script>
        var GridLog = '<%=GridLog.ClientID%>';
        var GridInf = '<%=GridInf.ClientID%>';
        var basePath = '<%= ResolveUrl("~/") %>';
        function openSpace() {
            openTODOLINK('1313', '/XTFrame/TableSpaceMonitor.aspx', '数据空间检查')
        }
        function openLog() {
            openTODOLINK('1301', '/ERPQuery/OperationLog.aspx', '系统操作日志')
        }
        function openInf() {
            openTODOLINK('1312', '/XTFrame/DataUpdateClient.aspx', '数据更新查看')
        }
        function openUpdate() {
            openTODOLINK('1120', '/XTFrame/version.aspx', '版本更新日志')
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
        var option = {
            tooltip: {
                trigger: 'axis',
                axisPointer: {            // 坐标轴指示器，坐标轴触发有效
                    type: 'shadow'        // 默认为直线，可选为：'line' | 'shadow'
                }
            },
            legend: {
                data: ['已使用空间', '剩余空间']
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
                data: []
                //data: ['INDX', 'SYSAUX', 'SYSTEM', 'UNDOTBS1', 'USERS']
            },
            series: [{
                // 根据名字对应到相应的系列
                name: '已使用空间',
                type: 'bar',
                stack: '总量',
                label: {
                    normal: {
                        show: true,
                        position: 'insideRight'
                    }
                },
                data: []
                //data: [0,0,0,0,0]
            }, {
                name: '剩余空间',
                type: 'bar',
                stack: '总量',
                label: {
                    normal: {
                        show: true,
                        position: 'insideRight'
                    }
                },
                data: []
                //data: [0,0,0,0,0]
            }]
        };
        function bindGrid(grid, data) {
            data = eval('(' + data + ')');
            F(grid).clearData();
            F(grid).addNewRecords(data);
        }
        function getMonitorData() {
            $('#echart').height($('#echart').parents('.f-panel-body').eq(0).height());
            var myChart = echarts.init($('#echart')[0]);
            myChart.setOption(option);
            $.ajax('/XTFrame/TableSpaceMonitor.aspx?data=1', {

                success: function (data) {

                    data = eval('(' + data + ')');
                    // 填入数据

                    myChart.setOption({
                        xAxis: {
                            type: 'category',
                            data: data.category
                        },
                        series: [{
                            // 根据名字对应到相应的系列
                            name: '已使用空间',
                            type: 'bar',
                            stack: '总量',
                            label: {
                                normal: {
                                    show: true,
                                    position: 'insideRight'
                                }
                            },
                            data: data.data[0]
                        }, {
                            name: '剩余空间',
                            type: 'bar',
                            stack: '总量',
                            label: {
                                normal: {
                                    show: true,
                                    position: 'insideRight'
                                }
                            },
                            data: data.data[1]
                        }]
                    });
                }
            });
        }

        function getLogData() {
            $.ajax('/ERPquery/operationlog.aspx?data=1', {

                success: function (data) {
                    bindGrid(GridLog, data);
                }
            })
        }

        function getInfData() {
            $.ajax('/XTFrame/DataUpdateClient.aspx?indexAdmin=1', {

                success: function (data) {
                    bindGrid(GridInf, data);
                }
            })

        }
        function getUpdateData() {
            $.ajax('/XTFrame/version.aspx?indexAdmin=1', {
                success: function (data) {
                    $('#contentUpdate').html('');
                    data = eval('(' + data + ')');
                    for (var i = 0; i < data.length; i++) {
                        var d = data[i];
                        var ul = $('<ul class="timeline"></ul>');
                        var li = $('<li></li>');
                        var divTime = $('<div class="time">' + data[i]['UPTDATE'].split('T')[0] + '</div>');
                        var divVersion = $('<div class="version">v' + data[i]['VERSION'] + '<span>由<i>' + data[i]['UPTPERNAME'] + '</i>更新</span></div>');
                        var divNubmer = $('<div class="number">' + (i + 1) + '</div>');
                        var divContent = $('<div class="content"> <pre>' + data[i]['UPTMEMO'] + '</pre></div>');
                        li.append(divTime).append(divVersion).append(divNubmer).append(divContent);
                        ul.append(li)

                        $('#contentUpdate').append(ul);
                    }
                    //$(contentUpdate).html(data);
                }
            })
        }
        F.ready(function () {

            getMonitorData();
            getLogData();
            getInfData();
            getUpdateData();
            setInterval(getMonitorData, 60000)
            setInterval(getLogData, 60000)
            setInterval(getInfData, 60000)
            setInterval(getUpdateData, 60000)

            //top.F(top.mainTabStripClientID).on('tabchange', function (event, tab) {
            //	if (tab.id.indexOf("RegionPanel_mainTabStrip_mainTab") > -1) {
            //		setTimeout(function () {
            //			getMonitorData();
            //		}, 1000)
            //	}
            //})
        })
    </script>
</body>
</html>