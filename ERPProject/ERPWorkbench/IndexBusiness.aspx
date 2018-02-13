﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IndexBusiness.aspx.cs" Inherits="ERPProject.ERPWorkbench.IndexBusiness" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <script src="../res/js/ichart.1.2.min.js" type="text/javascript"></script>
    <title></title>
    <style>
        .x-grid-body {
            border-top-width: 0px;
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

        .f-grid-row.color-yellow,
        .f-grid-row.color-yellow .ui-icon,
        .f-grid-row.color-yellow a {
            background-color: #edb400;
            color: #fff;
        }

        .f-grid-row.color-red,
        .f-grid-row.color-red .ui-icon,
        .f-grid-row.color-red a {
            background-color: #C23531;
            color: #fff;
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
                    BodyPadding="5px" Margin="0 0 0 0" ShowBorder="false" ShowHeader="false" Layout="Anchor">
                    <Items>
                        <f:Grid ID="GridList" AnchorValue="100% 40%" ShowBorder="True" ShowHeader="True" EnableColumnLines="true" CssClass="panel-up"
                            AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="SEQNO,WARINGRQ,TITLE,MEMO,MYSTATUS" Title="备忘录"
                            AllowSorting="true" EnableHeaderMenu="true" SortField="WARINGRQ" OnSort="GridList_Sort" SortDirection="DESC">
                            <Tools>

                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool4">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openBW" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Columns>
                                <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                <f:BoundField DataField="SEQNO" Hidden="true" />
                                <f:BoundField Width="80px" DataField="WARINGRQ" HeaderText="提醒时间" SortField="WARINGRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                <f:BoundField Width="140px" DataField="TITLE" HeaderText="备忘主题" SortField="TITLE" TextAlign="Center" ExpandUnusedSpace="true" />
                                <f:BoundField Width="80px" DataField="STATUS" HeaderText="状态" SortField="STATUS" TextAlign="Center" DataFormatString="" />
                                <f:BoundField Width="80px" DataField="LRNAME" HeaderText="录入人" SortField="LRNAME" TextAlign="Center" />
                                <f:BoundField Width="100px" DataField="LOOKPER" HeaderText="范围" SortField="LOOKPER" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" TextAlign="Center" Hidden="true" />
                                <f:BoundField Width="100px" DataField="MYSTATUS" HeaderText="当前状态" SortField="MYSTATUS" Hidden="true" />
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
                        <f:Panel ID="Panel5" AnchorValue="100% 60%" runat="server" Margin="0 0 0 0" Title="科室预算执行情况" CssClass="panel-down"
                            BodyPadding="5px" ShowBorder="true" ShowHeader="true" AutoScroll="true" Layout="Fit"
                            EnableIFrame="false">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool2">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openYS" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Items>
                                <f:ContentPanel runat="server" ShowBorder="false" ShowHeader="false">
                                    <div style="width: 100%;" id="echart2"></div>
                                </f:ContentPanel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
                <f:Panel ID="Panel3" Title="右" BoxFlex="1" runat="server"
                    Margin="5px 5px 5px 0" ShowBorder="true" ShowHeader="false" Layout="Fit">

                    <Items>
                        <f:Grid ID="GridGoodsList" ShowBorder="false" ShowHeader="true" Title="科室配置商品" AutoScroll="true" runat="server" PageSize="100"
                            DataKeyNames="GDSEQ" EnableRowLines="true" AllowPaging="true" IsDatabasePaging="true" OnPageIndexChange="GridGoodsList_PageIndexChange" OnRowDataBound="GridGoodsList_RowDataBound">
                            <Tools>
                                <f:Tool runat="server" EnablePostBack="false" Text="更多..." MarginRight="5" CssClass="tabtool" ToolTip="更多" ID="tool1">
                                    <Listeners>
                                        <f:Listener Event="click" Handler="openKC" />
                                    </Listeners>
                                </f:Tool>
                            </Tools>
                            <Columns>
                                <f:RowNumberField Width="30px" EnablePagingNumber="true" TextAlign="Center"></f:RowNumberField>
                                <f:BoundField DataField="GDSEQ" HeaderText="商品编码" Width="100px" TextAlign="Center" Hidden="true" />
                                <f:BoundField DataField="GDNAME" HeaderText="商品名称" Width="170px" />
                                <f:BoundField DataField="GDSPEC" HeaderText="商品规格" Width="120px" />
                                <f:BoundField DataField="UNITNAME" HeaderText="单位" Width="40px" TextAlign="Center" />
                                <f:BoundField DataField="ISDS" HeaderText="定数" Width="40px" TextAlign="Center" />
                                <f:BoundField DataField="KCSL" ColumnID="KCSL" HeaderText="库存" Width="60px" TextAlign="Right" />
                                <f:BoundField DataField="ZGKC" ColumnID="ZGKC" HeaderText="上限" Width="60px" TextAlign="Right" />
                                <f:BoundField DataField="ZDKC" ColumnID="ZDKC" HeaderText="下限" Width="60px" TextAlign="Right" />
                                <f:BoundField DataField="HSJJ" HeaderText="价格" Width="60px" TextAlign="Right" />
                            </Columns>
                        </f:Grid>
                    </Items>
                </f:Panel>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdRoleList" runat="server"></f:HiddenField>
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
        function openBW() {
            openTODOLINK('1403', '/ERPAssist/MyMemo.aspx', '我的备忘录')
        }

        function openKC() {
            openTODOLINK('7101', '/ERPQuery/StockSearch.aspx', '商品库存查询')
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
        //function openTODOLINK(id, url, name) {
        //    parent.addExampleTab.apply(null, [id, basePath + url, name, basePath + 'extjs/res/ext-theme-classic/images/tree/leaf.gif', true]);
        //}
        F.ready(function () {
            F('<% =GridList.ClientID %>').addTool({
                id: 'tool_1',
                renderTpl: '<span style="color:blue;text-decoration: underline;font-weight:bold">更多....</span>',
                width: 50,
                handler: function (event) {
                    openTODOLINK('1315', 'ERPAssist/MyMemo.aspx', '备忘录');
                }

            })

            top.F(top.mainTabStripClientID).on('tabchange', function (event, tab) {
                if (tab.id.indexOf("RegionPanel_mainTabStrip_mainTab") > -1) {
                    setTimeout(function () {
                        reloaddata();
                    }, 1000)
                }
            })
        })
    </script>
    <script type="text/javascript">
        function reloaddata(ysje, xsje) {
            $('#echart2').height($('#echart2').parents('.f-panel-body').height());
            var eChart = echarts.init($('#echart2')[0]);
            var option = {
                title: {
                    text: '科室预算情况',
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
    </script>
</body>
</html>