<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexWarehouse.aspx.cs" Inherits="ERPProject.ERPWorkbench.IndexWarehouse" %>

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

        #Panel1 .f-panel {
            border-radius: 6px;
            /*border-top-right-radius:4px;
            border-bottom-left-radius:4px;
            border-bottom-right-radius:4px;*/
        }


            #Panel1 .f-panel.panel-up {
                /*border-radius:0;*/
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
        <f:Timer ID="Timer1" Interval="60" OnTick="Timer1_Tick" runat="server" Enabled="false" EnableAjaxLoading="false" />
        <f:Panel ID="Panel1" runat="server" ShowBorder="false" EnableCollapse="true"
            Layout="HBox" BoxConfigAlign="Stretch" BoxConfigPosition="Start"
            BoxConfigChildMargin="0 0 0 0" BodyPadding="1px" ShowHeader="false">
            <Items>
                <f:Panel ID="Panel2" Title="左" BoxFlex="1" runat="server"
                    BodyPadding="5px" Margin="0 0 0 0" ShowBorder="false" ShowHeader="false" Layout="Anchor">
                    <Items>
                        <f:Grid ID="GridList" AnchorValue="100% 50%" ShowBorder="true" ShowHeader="true" Title="备忘录" EnableColumnLines="true" CssClass="panel-up"
                            AutoScroll="true" runat="server" DataKeyNames="WARINGRQ,TITLE"
                            AllowSorting="true" EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC">
                            <Tools>

                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool4">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openBW" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField Width="80px" DataField="WARINGRQ" HeaderText="提醒时间" SortField="WARINGRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="140px" DataField="TITLE" HeaderText="备忘主题" SortField="TITLE" ExpandUnusedSpace="true" />
                                <f:BoundField Width="80px" DataField="STATUS" HeaderText="状态" SortField="STATUS" TextAlign="Center" DataFormatString="" />
                                <f:BoundField Width="80px" DataField="LRNAME" HeaderText="录入人" SortField="LRNAME" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="LOOKPER" HeaderText="范围" SortField="LOOKPER" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注说明" SortField="MEMO" TextAlign="Center" Hidden="true" />
                                <f:TemplateField ColumnID="expander" RenderAsRowExpander="true">
                                    <ItemTemplate>
                                        <div class="expander">
                                            <div style="line-height: 25px;"><strong>查看范围：</strong><%# getPersonNamesString(Eval("lookper").ToString()) %></div>
                                            <div style="line-height: 25px;">
                                                <strong>备忘录内容：</strong>
                                                <%# Eval("MEMO") %>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </f:TemplateField>
                            </Columns>
                        </f:Grid>
                        <f:Panel ID="PanelDelivery" AnchorValue="100% 50%" runat="server" Margin="0 0 0 0" Title="当日配送情况" CssClass="panel-down" Layout="Fit"
                            BodyPadding="5px" ShowBorder="True" ShowHeader="True" AutoScroll="false" CssStyle="overflow-x:auto">
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
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel3" Title="右" BoxFlex="1" runat="server"
                    Margin="5px 5px 5px 0" ShowBorder="true" ShowHeader="false" Layout="Fit">
                    <Items>
                        <f:Panel ID="Panel8" ShowHeader="true" ShowBorder="false" runat="server" Title="待办事宜" Layout="Anchor" AnchorValue="100% 100%" BodyPadding="0px">
                            <Tools>

                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openDB" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:Grid ID="GridToDoList" ShowBorder="false" CssStyle="border-width:0px;border-color:red;" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                                    DataKeyNames="SEQNO" ShowGridHeader="false" OnRowCommand="GridList_RowCommand" AnchorValue="100% 80%" EnableRowLines="false">
                                    <Columns>
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField DataField="FUNCID" Hidden="true" />
                                        <f:BoundField DataField="FUNCNAME" Hidden="true" />
                                        <f:BoundField DataField="RUNWHAT" Hidden="true" />
                                        <f:BoundField DataField="PARA" Hidden="true" />
                                        <f:BoundField DataField="EXECTYPE" Hidden="true" />
                                        <f:BoundField DataField="RUNWHAT" Hidden="true" />
                                        <f:BoundField DataField="PARA" Hidden="true" />
                                        <f:LinkButtonField DataTextField="INSTRUCTIONS" Width="200px" ExpandUnusedSpace="true" CommandName="TODOLINK" />
                                        <f:BoundField DataField="SCRQ" Width="150px" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" />
                                    </Columns>
                                </f:Grid>
                                <f:Grid ID="GridYuJing" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableRowLines="false"
                                    DataKeyNames="PARA,INSTRUCTIONS" ShowGridHeader="false" OnRowCommand="GridList_RowCommand" AnchorValue="100% 20%">
                                    <Columns>
                                        <f:BoundField DataField="PARA" Hidden="true" />
                                        <f:LinkButtonField DataTextField="INSTRUCTIONS" Width="200px" ExpandUnusedSpace="true" CommandName="YUJING" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
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
        <f:HiddenField ID="hfdWcl" runat="server"></f:HiddenField>
    </form>
    <script src="/res/js/echarts.min.js" type="text/javascript"></script>
    <script>
        var basePath = '<%= ResolveUrl("~/") %>';
        function openBW() {
            openTODOLINK('1403', '/ERPAssist/MyMemo.aspx', '我的备忘录')
        }
        function openDB() {
            openTODOLINK('1402', '/ERPQuery/ToDoQuery.aspx', '待办事宜查询')
        }
        function openPS() {
            openTODOLINK('7123', '/ERPQuery/TodayPs.aspx', '配送情况跟踪')
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
        F.ready(function () {
            F('<% =GridList.ClientID %>').addTool({
                id: 'tool_1',
                renderTpl: '<span style="color:blue;text-decoration: underline;font-weight:bold">更多....</span>',
                width: 50,
                handler: function (event) {
                    openTODOLINK('1315', 'ERPAssist/MyMemo.aspx', '备忘录');
                }
            })
            F('<% =PanelDelivery.ClientID %>').addTool({
                id: 'tool_2',
                renderTpl: '<span style="color:blue;text-decoration: underline;font-weight:bold">更多....</span>',
                width: 50,
                handler: function (event) {
                    openTODOLINK('7123', 'ERPQuery/TodayPs.aspx', '配送情况跟踪');
                }
            })
        })
        function getEcharsData() {
            $('#echart').height($('#echart').parents('.f-panel-body').height());
            var datatitl = F('<%= hfdWcl.ClientID%>').getValue().split(",")
            var eChart = echarts.init($('#echart')[0]);
            var option = {
                tooltip: {
                    formatter: "{a} <br/>{b} : {c}%"
                },
                series: [
                    {
                        name: '配送情况',
                        type: 'gauge',
                        radius: '90%',
                        detail: { formatter: '{value}%' },
                        data: [{ value: datatitl, name: '科室完成率' }],
                        axisLine: {            // 坐标轴线
                            lineStyle: {       // 属性lineStyle控制线条样式
                                width: 8,
                                color: [[datatitl / 100, '#1475BB'], [1, '#CC0F1C']],
                                shadowColor: '#fff', //默认透明
                                shadowBlur: 8
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
