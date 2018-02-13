<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlyReport.aspx.cs" Inherits="ERPProject.ERPQuery.MonthlyReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>医疗物资库房月报明细表</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <script src="../res/js/jquery.ymh.js" type="text/javascript"></script>
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
    </style>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="PanelMain" runat="server" />
        <f:Panel ID="PanelMain" ShowBorder="false" BodyPadding="0px" Layout="VBox" ShowHeader="False" runat="server">
            <Toolbars>
                <f:Toolbar ID="Toolbar2" runat="server">
                    <Items>
                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：" runat="server" />
                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" EnableDefaultState="false" runat="server" OnClick="bntClear_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                        <f:Button ID="btnPrintAll" Icon="Printer" Text="打 印" runat="server" EnableDefaultState="false" OnClick="btnPrintAll_Click" />
                        <f:ToolbarSeparator runat="server" />
                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" EnableDefaultState="false" runat="server" OnClick="bntSearch_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow >
                            <Items>
                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" />
                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                <f:DropDownList ID="lstSUPID" runat="server" Label="供应商" ForceSelection="true" EnableEdit="true" />
                                <f:DropDownList ID="docDHLX" runat="server" Label="商品类型" ShowRedStar="true" Required="true"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridMonthly" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="SUPID"
                    EnableSummary="true" SummaryPosition="Bottom" EnableRowDoubleClickEvent="true">
                    <Columns>
                        <f:RowNumberField Width="30px" TextAlign="Center" />
                        <f:BoundField Hidden="true" DataField="CATEGORYID" ColumnID="CATEGORYID" HeaderText="类别编码" />
                        <f:BoundField Width="180px" DataField="CATEGORYNAME" ColumnID="CATEGORYNAME" HeaderText="类别名称" EnableLock="true" Locked="true" />
                        <f:BoundField Width="130px" DataField="QCJE" ColumnID="QCJE" HeaderText="上月余额" DataFormatString="{0:F2}" TextAlign="Right" />
                        <f:GroupField HeaderText="入库金额" TextAlign="Center">
                            <Columns>
                                <f:BoundField Width="120px" DataField="BYRKJE" ColumnID="BYRKJE" SortField="BYRKJE" HeaderText="本月入库金额"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                                <f:BoundField Width="100px" DataField="PYJE" ColumnID="PYJE" SortField="PYJE" HeaderText="盘盈"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                                <f:BoundField Width="120px" DataField="BNLJRKJE" ColumnID="BNLJRKJE" SortField="BNLJRKJE" HeaderText="本年累计入库"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                            </Columns>
                        </f:GroupField>
                        <f:GroupField HeaderText="出库金额" TextAlign="Center">
                            <Columns>
                                <f:BoundField Width="120px" DataField="BYCKJE" ColumnID="BYCKJE" SortField="BYCKJE" HeaderText="本月出库金额"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                                <f:BoundField Width="100px" DataField="PKJE" ColumnID="PKJE" SortField="PKJE" HeaderText="盘亏"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                                <f:BoundField Width="100px" DataField="BSJE" ColumnID="BSJE" SortField="BSJE" HeaderText="报损"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                                <f:BoundField Width="120px" DataField="BNLJCKJE" ColumnID="BNLJCKJE" SortField="BNLJCKJE" HeaderText="本年累计出库"
                                    DataFormatString="{0:F2}" TextAlign="Right" />
                            </Columns>
                        </f:GroupField>
                        <f:BoundField Width="130px" DataField="QMJE" ColumnID="QMJE" HeaderText="期末余额" DataFormatString="{0:F2}" TextAlign="Left" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
    </form>
    <script type="text/javascript">
        function PrintYDBB() {
            var beg = F('<%= lstLRRQ1.ClientID%>').getText();
            var end = F('<%= lstLRRQ2.ClientID%>').getText();
            var sup = F('<%= lstSUPID.ClientID%>').getValue();

            ReportViewer.ReportURL = "/grf/yyydbb.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetYDBB&beg=" + beg + "&end=" + end + "&sup=" + sup;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
