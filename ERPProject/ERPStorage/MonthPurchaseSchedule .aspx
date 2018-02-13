<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthPurchaseSchedule .aspx.cs" Inherits="ERPProject.ERPStorage.MonthPurchaseSchedule" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>月度采购进度查询</title>
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <link href="../res/css/jquery-ui.min.css" rel="stylesheet" />
    <link href="../res/css/jquery-ui.theme.min.css" rel="stylesheet" />
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../../res/js/jquery.ymh.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .ColBlue {
            font-size: 12px;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell-inner {
            font-weight: bold;
            color: blue;
        }

        .x-grid-row-summary .x-grid-cell {
            background-color: #fff !important;
        }

        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.highRedwlight td {
            background-color: red;
            background-image: none;
        }

        .x-grid-row.highyellowlight td {
            background-color: yellow;
            background-image: none;
        }

        .x-grid-row.SelectColor td {
            background-color: #B8CFEE;
            background-image: none;
        }
    </style>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="报表查询" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnMySerarch_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 5px 5px 5px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="33% 33% 34%">
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" />
                                                         <f:TextBox ID="txtGDSEQ" runat="server" Label="商品编码" MaxLength="15" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="订货部门" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="33% 33% 34%">
                                                    <Items>
                                                    <f:TriggerBox ID="tbxJHYF" runat="server" Label="计划月份" TriggerIcon="Date"></f:TriggerBox>
                                                    <f:TextBox ID="txtProducter" runat="server" Label="供应商" MaxLength="15" />
                                                    <f:ToolbarFill ID="toolbarfill" runat="server" ></f:ToolbarFill>
                                                        <%--<f:DatePicker ID="lstBEGRQ" runat="server" Label="开始月份" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstENDRQ" runat="server" Label="结束月份" LabelSeparator="" ShowRedStar="true" />--%>
                                                        <f:HiddenField ID="hfdHiddenField" runat="server"></f:HiddenField>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,BILLNO," EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    EnableColumnLines="true" EnableMultiSelect="false" CheckBoxSelectOnly="true"
                                    AllowSorting="true" EnableHeaderMenu="true" 
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="50" OnPageIndexChange="GridList_PageIndexChange">
                                    <Listeners>
                                        <f:Listener Event="cellmousedown" Handler="onMouseDown" />
                                    </Listeners>
                                    <Columns>
                                      																																					
                                       <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="100px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDNAME" HeaderText="商品名称" SortField="GDNAME" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAG" HeaderText="规格" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="DEPTID" HeaderText="单位" SortField="DEPTID" TextAlign="Center" />
                                         <f:BoundField Width="80px" DataField="CATID" HeaderText="分类" TextAlign="Center" SortField="YSRQ"  />
                                         <f:BoundField Width="120px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" SortField="HSJJ" />
                                         <f:BoundField Width="70px" DataField="DHS" HeaderText="计划量" TextAlign="Center" SortField="DHS" />
                                        <f:BoundField Width="80px" DataField="DHSL" HeaderText="入库量" TextAlign="Center"  SortField="DHSL" />
                                          <f:BoundField Width="70px" DataField="HSJE" HeaderText="计划金额" TextAlign="Center" SortField="HSJE" />
                                        <f:BoundField Width="80px" DataField="DHJE" HeaderText="入库金额" TextAlign="Center"  SortField="DHJE" />
                                        <f:BoundField Width="80px" DataField="PerRate" HeaderText="达成率" TextAlign="Center"  SortField="PerRate" />
                                         <f:BoundField Width="80px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center"  SortField="PZWH" />
                                        <f:BoundField Width="80px" DataField="SPUID" HeaderText="供应商" TextAlign="Center"  SortField="SPUID" />
                                          <f:BoundField Width="80px" DataField="PRODUCER" HeaderText="生产厂家" TextAlign="Center"  SortField="PRODUCER" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" ExpandUnusedSpace="true" />
                                    </Columns>
                                    <%--                                    <Listeners>
                                        <f:Listener Event="cellkeydown" Handler="onCellClick" />
                                        <f:Listener Event="cellmousedown" Handler="onMouseDown" />
                                    </Listeners>--%>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="图形分析" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                            
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                               
                                                  <f:Button ID="btnSearchChart" Icon="DatabaseGo" Text="查  询" OnClick="btnSearchChart_Click"    EnablePostBack="true"  runat="server" />

                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 5px 5px 5px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                              
                                                
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="dptStartDate" runat="server" Label="开始日期" Enabled="false" LabelWidth="90px" />
                                                        <f:DatePicker ID="dptEndDate" runat="server" Label="结束日期" Enabled="false" LabelWidth="90px" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                               <f:Panel ID="PanelChart" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server" >
                                   <Content>

                                   </Content>
                               </f:Panel>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdoper" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highyellowRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highredRows" runat="server"></f:HiddenField>
    </form>
    <script src="../res/my97/WdatePicker.js" type="text/javascript"></script>
    <script src="../res/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript">
      <%--  function btnPrint_Bill() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("单据不存在！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/monthBudget.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=getYSBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }--%>


        var highlightRowsClientID = '<%= highlightRows.ClientID %>';
        var highyellowRowsClientID = '<%= highyellowRows.ClientID %>';
        var highredRowsClientID = '<%= highredRows.ClientID %>';
        var gridClientID = '<%= GridList.ClientID %>';
        function highlightRows() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highlightRowsClientID);
                var grid = F(gridClientID);

                $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highlight');

                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highlight');
                    }
                });
            }, 100);
        }
        function highyellowRows() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highyellowRowsClientID);
                var grid = F(gridClientID);

                $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highyellowlight');

                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highyellowlight');
                    }
                });
            }, 100);
        }
        function highredRows() {
            // 增加延迟，等待HiddenField更新完毕
            window.setTimeout(function () {
                var highlightRows = F(highredRowsClientID);
                var grid = F(gridClientID);

                $(grid.el.dom).find('.x-grid-row.highlight').removeClass('highRedwlight');

                $.each(highlightRows.getValue().split(','), function (index, item) {
                    if (item !== '') {
                        var row = grid.getView().getNode(parseInt(item, 10));
                        $(row).addClass('highRedwlight');
                    }
                });
            }, 100);
        }

        // 页面第一个加载完毕后执行的函数
        F.ready(function () {
            var redstarID = "";
            $redstarID = $('#YSSL');
            $redstarID.addClass("redstar");
            var grid = F(gridClientID);

            grid.on('columnhide', function () {
                highlightRows();
                highyellowRows();
                highredRows();
            });

            grid.on('columnshow', function () {
                highlightRows();
                highyellowRows();
                highredRows();
            });

            grid.getStore().on('refresh', function () {
                highlightRows();
                highyellowRows();
                highredRows();
            });

            highlightRows();
            highyellowRows();
            highredRows();

        });

    </script>

    <script>
        function onMouseDown(grid, rowIndex, columnIndex, e, e1, e2, e3) {
            if (!$(e3.target).hasClass('x-grid-row-checker')) {
                $(e1).find('.x-grid-row-checker').click();
            }
        }
    </script>

    <script language="javascript" type="text/javascript">
      <%--  var arrEditor = [5];//可编辑表格的索引号  
        var gridGoods = '<%=GridList.ClientID %>';
        function onEditorCellClick(grid, rowIndex, columnIndex, e) {
            //改变背景色
            var grid = F(gridGoods);
            $(grid.el.dom).find('.x-grid-row.SelectColor').removeClass('SelectColor');
            $.each((F(gridGoods).f_getSelectedCell()[0] + ',').split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10));
                    $(row).addClass('SelectColor');
                }
            });
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",S,M,N,R").indexOf(flag) > 0) {
                if (flag == "N" || flag == "S")
                    return false;
                else {
                    if (arrEditor.indexOf(columnIndex) >= 0) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            else
                return false;
        }--%>
    </script>

    <script language="javascript" type="text/javascript">

        <%--var tbxMyBoxClientID = '<%= ksyf.ClientID %>';
        var tbxMyBoxClientID2 = '<%= jsyf.ClientID %>';
        F.ready(function () {

            var tbxMyBox = F(tbxMyBoxClientID);
            var tbxMyBox2 = F(tbxMyBoxClientID2);
            tbxMyBox.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        tbxMyBox.validate();
                    }
                });
            };
            tbxMyBox2.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID2 + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        tbxMyBox2.validate();
                    }
                });
            };
        });--%>

    </script>
</body>
</html>
