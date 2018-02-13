﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TreasuryQuantityInvoicingDetails.aspx.cs" Inherits="ERPProject.ERPQuery.TreasuryQuantityInvoicingDetails" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" EnableAjax="true" EnableAjaxLoading="false" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0"
            runat="server">
            <Tabs>
                <f:Tab Title="库房进销汇总" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText" runat="server" Text="操作信息：收支情况分析主界面！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="btnAudit" Icon="ArrowRotateClockwise" Text="清 空" EnablePostBack="true" EnableDefaultState="false" runat="server" OnClick="btClear_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" ConfirmText="是否导出当前商品库存信息?" EnableDefaultState="false" DisableControlBeforePostBack="false" Enabled="true"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" OnClick="btExport_Click" Hidden="true" />
                                                <f:Button ID="BtnPrintKC" Icon="Printer" Text="打印库存" EnablePostBack="false" runat="server" EnableDefaultState="false" OnClientClick="btnPrint_BillKC()" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btSearch" Icon="Magnifier" Text="查 询" OnClick="btSearch_Click" EnableDefaultState="false" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormQuery" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="RQXZ" runat="server" Label="查询期间" TriggerIcon="Date"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="库房" EnableEdit="true" ForceSelection="true" />
                                                        <f:TriggerBox ID="tbxGOODS" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlCATID" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true" />

                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:HiddenField ID="USERID" runat="server" />
                                                        <f:HiddenField ID="HiddenField1" runat="server" />
                                                        <f:HiddenField ID="HiddenField2" runat="server" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="false" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="GDSEQ,XQ" EnableColumnLines="true" EnableTextSelection="true" SortField="GDNAME" SortDirection="ASC"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange" EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="30px" />
                                        <f:BoundField DataField="GDSEQ" SortField="GDSEQ" ColumnID="GDSEQ" Width="115px" HeaderText="商品编码" />
                                        <f:BoundField DataField="GDNAME" SortField="GDNAME" Width="180px" HeaderText="商品名称" />
                                        <f:BoundField DataField="GDSPEC" SortField="GDSPEC" Width="100px" HeaderText="规格" />
                                        <f:BoundField DataField="UNIT" SortField="UNIT" Width="90px" HeaderText="单位" />
                                        <f:BoundField DataField="QCKCSL" ColumnID="QCKCSL" SortField="QCKCSL" Width="120px" HeaderText="&nbsp;月期初库存数量<br/>（上月末结存数量）" />
                                        <f:GroupField HeaderText="入库数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="CGRK" ColumnID="CGRK" SortField="CGRK" HeaderText="外购入库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="KSTH" ColumnID="KSTH" SortField="KSTH" HeaderText="科室退货"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="PYRK" ColumnID="PYRK" SortField="PYRK" HeaderText="盘盈入库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="DBRK" ColumnID="DBRK" SortField="DBRK" HeaderText="调拨入库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="RKHJ" ColumnID="RKHJ" SortField="RKHJ" HeaderText="入库合计"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField HeaderText="出库数量" TextAlign="Center">
                                            <Columns>

                                                <f:BoundField Width="100px" DataField="KFCK" ColumnID="KFCK" SortField="KFCK" HeaderText="科室出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="THCK" ColumnID="THCK" SortField="THCK" HeaderText="退货出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="PKCK" ColumnID="PKCK" SortField="PKCK" HeaderText="盘亏出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="DBCK" ColumnID="DBCK" SortField="DBCK" HeaderText="调拨出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="CKHJ" ColumnID="CKHJ" SortField="CKHJ" HeaderText="出库合计"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField DataField="QMKCSL" ColumnID="QMKCSL" SortField="QMKCSL" Width="90px" HeaderText="本月结余<br/>&nbsp;数量" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>

                <f:Tab Title="科室进销汇总" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" runat="server" Text="操作信息：收支情况分析主界面！"></f:ToolbarText>
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnClearDept" Icon="ArrowRotateClockwise" Text="清 空" EnablePostBack="true" EnableDefaultState="false" runat="server" OnClick="btnClearDept_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExportDept" Icon="PageExcel" Text="导 出" ConfirmText="是否导出当前商品库存信息?" EnableDefaultState="false" DisableControlBeforePostBack="false" Enabled="true"
                                                    EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" OnClick="btnExportDept_Click" Hidden="true" />
                                                <f:Button ID="btnPrintDept" Icon="Printer" Text="打印库存" EnablePostBack="false" runat="server" EnableDefaultState="false" OnClientClick="btnPrintDept_BillKC()" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSearchDept" Icon="Magnifier" Text="查 询" OnClick="btnSearchDept_Click" EnableDefaultState="false" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="RQXZ2" runat="server" Label="查询期间" TriggerIcon="Date"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlDEPTID2" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:TriggerBox ID="tbxGOODS2" runat="server" Label="查询商品" EmptyText="可输入编码、名称或助记码" TriggerIcon="Search" OnTriggerClick="btSearch_Click"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlCATID2" runat="server" Label="商品分类" EnableEdit="true" ForceSelection="true" />

                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridListDept" AnchorValue="100% -75" ShowBorder="false" ShowHeader="false"
                                    AllowSorting="false" AutoScroll="false" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="GDSEQ,XQ" EnableColumnLines="true" EnableTextSelection="true" SortField="GDNAME" SortDirection="ASC"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridListDept_PageIndexChange" EnableSummary="true" SummaryPosition="Bottom">
                                    <Columns>
                                        <f:RowNumberField Width="30px" />
                                        <f:BoundField DataField="GDSEQ" SortField="GDSEQ" ColumnID="GDSEQ" Width="115px" HeaderText="商品编码" />
                                        <f:BoundField DataField="GDNAME" SortField="GDNAME" Width="180px" HeaderText="商品名称" />
                                        <f:BoundField DataField="GDSPEC" SortField="GDSPEC" Width="100px" HeaderText="规格" />
                                        <f:BoundField DataField="UNIT" SortField="UNIT" Width="90px" HeaderText="单位" />
                                        <f:BoundField DataField="QCKCSL" ColumnID="QCKCSL" SortField="QCKCSL" Width="120px" HeaderText="&nbsp;月期初库存数量<br/>（上月末结存数量）" />
                                        <f:GroupField HeaderText="入库数量" TextAlign="Center">
                                            <Columns>
                                                <f:BoundField Width="100px" DataField="LCDRK" ColumnID="LCDRK" SortField="LCDRK" HeaderText="科室申领"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="DSCRK" ColumnID="DSCTH" SortField="DSCRK" HeaderText="定数出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="CKDRK" ColumnID="CKDRK" SortField="CKDRK" HeaderText="库房出库"
                                                    TextAlign="Center" />     
                                                <f:BoundField Width="100px" DataField="DBRK" ColumnID="DBRK" SortField="DBRK" HeaderText="调拨入库"
                                                    TextAlign="Center" />                                             
                                                <f:BoundField Width="100px" DataField="PYRK" ColumnID="PYRK" SortField="PYRK" HeaderText="盘盈入库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="RKHJ" ColumnID="RKHJ" SortField="RKHJ" HeaderText="入库合计"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:GroupField HeaderText="出库数量" TextAlign="Center">
                                            <Columns>

                                                <f:BoundField Width="100px" DataField="LTDCK" ColumnID="LTDCK" SortField="LTDCK" HeaderText="科室申退"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="XSTCK" ColumnID="XSTCK" SortField="XSTCK" HeaderText="商品销售退"
                                                    TextAlign="Center" />     
                                                <f:BoundField Width="100px" DataField="DBCK" ColumnID="DBCK" SortField="DBRK" HeaderText="调拨出库"
                                                    TextAlign="Center" />  
                                                <f:BoundField Width="100px" DataField="PKCK" ColumnID="PKCK" SortField="PKCK" HeaderText="盘亏出库"
                                                    TextAlign="Center" />
                                                <f:BoundField Width="100px" DataField="CKHJ" ColumnID="CKHJ" SortField="CKHJ" HeaderText="出库合计"
                                                    TextAlign="Center" />
                                            </Columns>
                                        </f:GroupField>
                                        <f:BoundField DataField="QMKCSL" ColumnID="QMKCSL" SortField="QMKCSL" Width="90px" HeaderText="本月结余<br/>&nbsp;数量" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>

    </form>
    <script src="../res/my97/WdatePicker.js"></script>
    <script type="text/javascript">
        var tbxMyBoxClientID = '<%= RQXZ.ClientID %>';
        var tbxMyBox2ClientID = '<%= RQXZ2.ClientID %>';

        F.ready(function () {

            var tbxMyBox = F(tbxMyBoxClientID);
            var tbxMyBox2 = F(tbxMyBox2ClientID);

            tbxMyBox.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBoxClientID + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        tbxMyBox.validate();
                    }
                });
            };
            tbxMyBox2.onTriggerClick = function () {
                WdatePicker({
                    el: tbxMyBox2ClientID + '-inputEl',
                    dateFmt: 'yyyy-MM',
                    onpicked: function () {
                        // 确认选择后，执行触发器输入框的客户端验证
                        tbxMyBox2.validate();
                    }
                });
            };


        });
        function IsDate(strDate) {
            var strSeparator = "-"; //日期分隔符
            var strDateArray;
            var intYear;
            var intMonth;
            var intDay;
            var boolLeapYear;
            strDateArray = strDate.split(strSeparator);
            if (strDateArray.length != 2) return false;
            intYear = parseInt(strDateArray[0], 10);
            intMonth = parseInt(strDateArray[1], 10);
            if (isNaN(intYear) || isNaN(intMonth)) return false;
            if (intMonth > 12 || intMonth < 1) return false;
            return true;
        }

        function btnPrint_BillKC() {

            var GrfKSKF = F('<%= ddlDEPTID.ClientID%>').getValue();
            var GrfGOODS = F('<%= tbxGOODS.ClientID%>').getValue();
            var GrfCATID = F('<%= ddlCATID.ClientID%>').getValue();
            var GrfUSER = F('<%= USERID.ClientID%>').getValue();
            var Grftime = F('<%= RQXZ.ClientID%>').getValue();
            if (IsDate(Grftime) == false) {
                alert("日期格式不正确！");
                return;
            }

            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/TJYKD_AL_kfspsljxcmxb.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsKCSPJXCMXB&RQXZ=" + Grftime + "&KSKF=" + GrfKSKF + "&GOODS=" + GrfGOODS + "&CATID=" + GrfCATID + "&USER=" + GrfUSER;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            var print = getCookie("WEGOERP_TJYKD_AL_kfspsljxcmxb_SPKCXX");
            if (print != null && print != "") {
                setCookie("WEGOERP_TJYKD_AL_kfspsljxcmxb_SPKCXX", print);
                ReportViewer.Report.Printer.PrinterName = print;
            }
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

    </script>
</body>
</html>