﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplierInvoicing.aspx.cs" Inherits="ERPProject.ERPQuery.SupplierInvoicing" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>供应商商品出入库数据查询</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
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

        .x-grid-row.highlight td {
            background-color: yellow;
            background-image: none;
        }

        .x-grid-row.highRedwlight td {
            background-color: red;
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="按供应商汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <f:CheckBox ID="ckbAll" runat="server" Label="包含零库存"></f:CheckBox>
                                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                        <f:Button ID="btnExportSum" Icon="DatabaseGo" Text="导 出" OnClick="btnExportSum_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" EnableDefaultState="false" />
                                        <f:Button ID="btnPrintAll" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" EnableDefaultState="false" OnClientClick="PrintAll()" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnQuery" Icon="Magnifier" Text="查 询" EnableDefaultState="false" OnClick="btnQuery_Click" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docPSSID" runat="server" Label="供 应 商" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="ddlISGZ" runat="server" Label="是否高值">
                                                    <f:ListItem Selected="true" Text="---请选择---" Value="" />
                                                    <f:ListItem Text="高值" Value="Y" />
                                                    <f:ListItem Text="非高值" Value="N" />
                                                </f:DropDownList>
                                                <f:DatePicker ID="dpkRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="dpkRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="docDHLX" runat="server" Label="商品类型" ShowRedStar="true" Required="true"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridDeptKC" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="SUPID" OnRowDataBound="GridDeptKC_RowDataBound"
                                    EnableSummary="true" SummaryPosition="Bottom" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridDeptKC_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" />
                                        <f:BoundField Hidden="true" DataField="SUPID" ColumnID="SUPID" HeaderText="供应商编码" />
                                        <f:BoundField Width="210px" DataField="SUPNAME" ColumnID="SUPNAME" HeaderText="供应商名称" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="100px" DataField="QCSL" ColumnID="QCSL" HeaderText="期初数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="QCJE" ColumnID="QCJE" HeaderText="期初金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="RKSL" ColumnID="RKSL" HeaderText="备货数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="RKJE" ColumnID="RKJE" HeaderText="备货金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SHSL" ColumnID="SHSL" HeaderText="损耗数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SHJE" ColumnID="SHJE" HeaderText="损耗金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="CKSL" ColumnID="CKSL" HeaderText="出库数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="CKJE" ColumnID="CKJE" HeaderText="出库金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="KFKCSL" ColumnID="KFKCSL" SortField="KFKCSL" HeaderText="库房库存"
                                            TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="KFKCJE" ColumnID="KFKCJE" SortField="KFKCJE" HeaderText="库房金额"
                                            DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYSL" ColumnID="SYSL" HeaderText="使用数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYJE" ColumnID="SYJE" HeaderText="使用金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="KSKCSL" ColumnID="KSKCSL" SortField="KSKCSL" HeaderText="科室库存"
                                            TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="KSKCJE" ColumnID="KSKCJE" SortField="KSKCJE" HeaderText="科室金额"
                                            DataFormatString="{0:F2}" TextAlign="Right" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="按品规汇总" Icon="Table" Layout="Fit" runat="server" Hidden="true">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnClear1" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btnClear1_Click" EnablePostBack="true" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint1" Icon="Printer" Text="打 印" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint1_onclick()" />
                                                <f:Button ID="btnExport1" Icon="DatabaseGo" Text="导 出" OnClick="btnExport1_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearch1" Icon="Magnifier" Text="查 询" EnableDefaultState="false" OnClick="btnSearch1_Click" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 50%">
                                                    <Items>
                                                        <f:DatePicker ID="dpkBegRQ" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="dpkEndRQ" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                        <f:TriggerBox ID="trbSearch" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -65" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ"
                                    EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="200px" DataField="GDNAME" SortField="GDNAME" ColumnID="COUNTTITLE" HeaderText="商品名称" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="230px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SL" SortField="SL" ColumnID="HJSL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="JE" SortField="JE" ColumnID="HJJE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="按供应商品规汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" OnClick="btClear_Click" EnablePostBack="true" runat="server" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" EnableDefaultState="false" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" EnableDefaultState="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" runat="server" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlPSSID" runat="server" Label="供应商" EnableEdit="true" ForceSelection="true" />
                                                <f:TriggerBox ID="tbxGOODS" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="lstISGZ" runat="server" Label="是否高值">
                                                    <f:ListItem Text="---请选择---" Value="" />
                                                    <f:ListItem Text="非高值" Value="N" Selected="true" />
                                                    <f:ListItem Text="高值" Value="Y" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="docDHLX1" runat="server" Label="商品类型" ShowRedStar="true" Required="true"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" OnSort="GridGoods_Sort" SortField="GDNAME" SortDirection="ASC"
                                    EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true">
                                    <Columns>
                                        <f:RowNumberField Width="50px" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="170px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="130px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" EnableLock="true" Locked="true" />
                                        <f:BoundField Width="210px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商" Hidden="true" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="PIZNO" SortField="PIZNO" HeaderText="注册证号" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="RKSL" ColumnID="G_RKSL" HeaderText="入库数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="RKJE" ColumnID="G_RKJE" HeaderText="入库金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SHSL" ColumnID="G_SHSL" HeaderText="损耗数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SHJE" ColumnID="G_SHJE" HeaderText="损耗金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="CKSL" ColumnID="G_CKSL" HeaderText="出库数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="CKJE" ColumnID="G_CKJE" HeaderText="出库金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SYSL" ColumnID="G_SYSL" HeaderText="使用数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="SYJE" ColumnID="G_SYJE" HeaderText="使用金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:GroupField HeaderText="库存数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="90px" DataField="KSKCSL" ColumnID="G_KSKCSL" SortField="KSKCSL" HeaderText="科室库存"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="KSKCJE" ColumnID="G_KSKCJE" SortField="KSKCJE" HeaderText="科室金额"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="KFKCSL" ColumnID="G_KFKCSL" SortField="KFKCSL" HeaderText="库房库存"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="90px" DataField="KFKCJE" ColumnID="G_KFKCJE" SortField="KFKCJE" HeaderText="库房金额"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdUser" runat="server"></f:HiddenField>
    </form>
    <script type="text/javascript">
        function PrintAll() {
            if (F('<%= dpkRQ1.ClientID%>').lastValue > F('<%= dpkRQ2.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }

            var gys = F('<%= docPSSID.ClientID%>').getValue();
            var isgz = F('<%= ddlISGZ.ClientID%>').getValue();
            var begrq = F('<%= dpkRQ1.ClientID%>').getText();
            var endrq = F('<%= dpkRQ2.ClientID%>').getText();
            var user = F('<%= hfdUser.ClientID%>').getValue();
            var all = F('<%= ckbAll.ClientID%>').getValue();
            var dhlx = F('<%= docDHLX.ClientID%>').getValue();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/gysjxc.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockGysData&gys=" + gys + "&isgz=" + isgz + "&b=" + begrq + "&e=" + endrq + "&u=" + user + "&a=" + all + "&dh=" + dhlx;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint1_onclick() {
            if (F('<%= dpkBegRQ.ClientID%>').lastValue > F('<%= dpkEndRQ.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }

            var gdseq = F('<%= trbSearch.ClientID%>').getValue();
            var begrq = F('<%= dpkBegRQ.ClientID%>').getText();
            var endrq = F('<%= dpkEndRQ.ClientID%>').getText();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/ckdhz.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockOutData1&gdseq=" + gdseq + "&b=" + begrq + "&e=" + endrq;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint_onclick() {
            if (F('<%= lstLRRQ1.ClientID%>').lastValue > F('<%= lstLRRQ2.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }

            var gys = F('<%= ddlPSSID.ClientID%>').getValue();
            var gdseq = F('<%= tbxGOODS.ClientID%>').getValue();
            var begrq = F('<%= lstLRRQ1.ClientID%>').getText();
            var endrq = F('<%= lstLRRQ2.ClientID%>').getText();
            var isgz = F('<%= lstISGZ.ClientID%>').getValue();
            var user = F('<%= hfdUser.ClientID%>').getValue();
            var dhlx = F('<%= docDHLX1.ClientID%>').getValue();
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/gysjxchz.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockOutData&gys=" + gys + "&gdseq=" + gdseq + "&b=" + begrq + "&e=" + endrq + "&isgz=" + isgz + "&u=" + user + "&dh=" + dhlx;

            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>