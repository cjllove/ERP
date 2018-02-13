<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentUseQuery.aspx.cs" Inherits="ERPProject.ERPQuery.DepartmentUseQuery" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室使用信息查询</title>
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
            background-color: lightgreen;
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
                <f:Tab Title="按科室汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                        <%--<f:Button ID="Button3" Icon="DatabaseGo" Text="导 出" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />--%>
                                        <f:Button ID="btnPrint2" Icon="Printer" Text="打 印" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" OnClick="btnPrint2_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnExportSum" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnExportSum_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnQuery" Icon="Magnifier" Text="查 询" OnClick="btnQuery_Click" EnableDefaultState="false" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlDept" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
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
                                <f:Grid ID="GridDeptKC" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="DEPTID"
                                    EnableSummary="true" SummaryPosition="Bottom" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridDeptKC_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" />
                                        <f:BoundField Hidden="true" DataField="DEPTID" ColumnID="DEPTID" HeaderText="科室编码" />
                                        <f:BoundField Width="210px" DataField="DEPTNAME" ColumnID="DEPTNAME" HeaderText="科室名称" />
                                        <%--<f:BoundField Width="100px" DataField="CKSL" ColumnID="CKSL" HeaderText="收货数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="CKJE" ColumnID="CKJE" HeaderText="收货金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYSL" ColumnID="SYSL" HeaderText="使用数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="SYJE" ColumnID="SYJE" HeaderText="使用金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                         <f:BoundField Width="100px" DataField="KCSL" ColumnID="KCSL" HeaderText="库存数量" TextAlign="Right" />
                                        <f:BoundField Width="100px" DataField="KCJE" ColumnID="KCJE" HeaderText="库存金额" DataFormatString="{0:F2}" TextAlign="Right" />--%>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="按品规汇总" Icon="Table" Layout="Fit" runat="server">
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
                                                <f:Button ID="btnClear1" Icon="ArrowRotateClockwise" Text="清 空" EnableDefaultState="false" OnClick="btnClear1_Click" EnablePostBack="true" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint1" Icon="Printer" Text="打 印" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint1_onclick()" />
                                                <f:Button ID="btnExport1" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnExport1_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
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
                                                        <f:DropDownList ID="docDHLX1" runat="server" Label="商品类型" ShowRedStar="true" Required="true"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridCom" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true" AutoScroll="true" runat="server" DataKeyNames="GDSEQ"
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
                                        <f:BoundField Width="120px" DataField="HISCODE" SortField="HISCODE" HeaderText="医院物资编码" TextAlign="Center" Hidden="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="按科室品规汇总" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" EnableDefaultState="false" OnClick="btClear_Click" EnablePostBack="true" runat="server" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" DisableControlBeforePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" runat="server" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" OnClick="btSearch_Click" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormUser" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                    ShowHeader="False" LabelWidth="75px" runat="server">
                                    <Rows>
                                        <f:FormRow >
                                            <Items>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="查询期间" Required="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                                <f:TriggerBox ID="tbxGOODS" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                 <f:DropDownList ID="docDHLX2" runat="server" Label="商品类型" ShowRedStar="true" Required="true"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="查询科室" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="ddlCATID" runat="server" Label="商品类别" EnableEdit="true" ForceSelection="true" />
                                                <f:Label ID="lblSUBSUM" runat="server" Label="合计数量" CssClass="ColBlue"></f:Label>
                                                <f:Label ID="lblSUBNUM" runat="server" Label="合计金额" CssClass="ColBlue"></f:Label>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" CssStyle="border-top: 1px solid #99bce8;"
                                    PageSize="100" IsDatabasePaging="true" AllowPaging="true" OnPageIndexChange="GridGoods_PageIndexChange" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" DataKeyNames="DEPTNAME,GDSEQ" OnSort="GridGoods_Sort" SortField="DEPTNAME" SortDirection="ASC"
                                    EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="50px" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="科室名称" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="HSID" SortField="HSID" HeaderText="收费代码" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="ISSF" SortField="ISSF" HeaderText="是否收费" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="CATTYPE" SortField="CATTYPE" ColumnID="CATTYPE" HeaderText="商品类别" TextAlign="Center" />
                                        <f:BoundField Width="200px" DataField="GDNAME" SortField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="150px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="规格·容量" />
                                        <f:BoundField Width="50px" DataField="UNIT" SortField="UNIT" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HSJJ" SortField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" TextAlign="Right" />
                                        <f:BoundField Width="80px" DataField="SL" SortField="SL" ColumnID="SL" HeaderText="数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="JE" SortField="JE" ColumnID="JE" HeaderText="金额" DataFormatString="{0:F2}" TextAlign="Right" />
                                        <f:BoundField Width="170px" DataField="PRODUCER" SortField="PRODUCER" HeaderText="生产厂家" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
    </form>
    <script type="text/javascript">
        function btnPrint_onclick() {
            if (F('<%= lstLRRQ1.ClientID%>').lastValue > F('<%= lstLRRQ2.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }

            var deptid = F('<%= ddlDEPTID.ClientID%>').getValue();
            var gdseq = F('<%= tbxGOODS.ClientID%>').getValue();
            var begrq = F('<%= lstLRRQ1.ClientID%>').getText();
            var endrq = F('<%= lstLRRQ2.ClientID%>').getText();

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/kssymx.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetDeptStockOutMX&deptid=" + deptid + "&gdseq=" + gdseq + "&b=" + begrq + "&e=" + endrq;
            <%--if (deptid != "") {
                Report.ParameterByName("DEPT").AsString = F('<%= ddlDEPTID.ClientID%>').getText();
            }
            else {
                Report.ParameterByName("DEPT").AsString = "全部出库科室";
            }--%>

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
            var dh = F('<%= docDHLX1.ClientID%>').getValue();
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/ckdhz.grf?201601062022";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockOutData1&gdseq=" + gdseq + "&b=" + begrq + "&e=" + endrq + "&d=" + dh;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint2_onclick() {
            if (F('<%= dpkRQ1.ClientID%>').lastValue > F('<%= dpkRQ2.ClientID%>').lastValue) {
                F.util.alert('开始日期大于结束日期', F.util.formAlertTitle, Ext.MessageBox.INFO);
                return;
            }
            var begrq = F('<%= dpkRQ1.ClientID%>').getText();
            var endrq = F('<%= dpkRQ2.ClientID%>').getText();
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/ckdhz1.grf?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetStockOutData2&b=" + begrq + "&e=" + endrq;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        <%-- var tbxSELECTSUPNAME = '<%= SELECTSUPNAME.ClientID %>';
        var tbxSELECTSUPID = '<%= SELECTSUPID.ClientID %>';

        var tbxSELECTPRODUCERNAME = '<%= SELECTPRODUCERNAME.ClientID %>';
        var tbxSELECTPRODUCERID = '<%= SELECTPRODUCERID.ClientID %>';
        F.ready(function () {
            //getDocInfo(tbxSELECTSUPNAME, tbxSELECTSUPID, "供应商");
            getDocInfo(tbxSELECTPRODUCERNAME, tbxSELECTPRODUCERID, "生产厂家");
        });--%>
    </script>
</body>
</html>
