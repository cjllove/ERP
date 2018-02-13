<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageAcnRk.aspx.cs" Inherits="ERPProject.ERPApply.StorageAcnRk" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>调拨入库</title>
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
    </style>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申请库房" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="申请员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -120" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" KeepCurrentSelection="true" EnableCheckBoxSelect="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="115px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" HeaderText="申请库房" TextAlign="Center" SortField="DEPTNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTOUTNAME" HeaderText="出库库房" TextAlign="Center" SortField="DEPTOUTNAME" />
                                        <f:BoundField Width="80px" DataField="XSRQ" HeaderText="申请日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="XSRQ" />
                                        <f:BoundField Width="80px" DataField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" SortField="CATID" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="80px" DataField="SLRNAME" HeaderText="申请人" TextAlign="Center" SortField="SLRNAME" />
                                        <f:BoundField Width="80px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" SortField="SHRNAME" />
                                        <f:BoundField Width="80px" DataField="str2" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="str2" />
                                        <f:BoundField MinWidth="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="商 品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" />
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申请人" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申请库房" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库库房" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true"
                                                            CompareType="String" CompareControl="docDEPTID" CompareOperator="NotEqual" CompareMessage="出库与入库部门不能相同" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" />
                                                        <f:DropDownList ID="docSTR1" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSTR2" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -123" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit" EnableColumnLines="true">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="115px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblGDSEQ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="申请数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblBZSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="XSSL" DataField="XSSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="到货数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblXSSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="申请数量（最小单位）">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="LblPH" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" FieldType="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" Hidden="true"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" Hidden="true"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" Hidden="true"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="CODEINFO" ColumnID="CODEINFO" HeaderText="商品赋码信息" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comCODEINFO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="cellclick" Handler="onCellClick" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
    </form>
    <script language="javascript" type="text/javascript">
        function onCellClick(grid, rowIndex, columnIndex, e) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",N,R").indexOf(flag) > 0)
                return true;
            else
                return false;
        }
        var TXDNDG;
        var TXDDG;
        var USERXMID = "";
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            //ReportViewer.ReportURL = "/grf/cksld.grf?timestamp=" + new Date().getTime();
            //TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            //TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            //TXDNDG.OnInitialize = TXDNDGLoading;
            //TXDDG.OnInitialize = TXDDGLoading;
            ReportViewer.ReportURL = "/grf/DBD.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbrksjd&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);

            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function TXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbrksj&osid=" + billNo;
            //载入子报表数据
            TXDNDG.LoadDataFromURL(dataurl);
            field = TXDNDG.FieldByName("DT");
            //console.log(field.Value);
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader1')
            }
        }
        function TXDDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbrksjDG&osid=" + billNo;
            //载入子报表数据
            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }
        }
        <%--function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbrksj&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }--%>
    </script>
</body>
</html>

