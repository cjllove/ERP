<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageAcnCk.aspx.cs" Inherits="ERPProject.ERPApply.StorageAcnCk" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>调拨出库</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" OnCustomEvent="PageManager1_CustomEvent"
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
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
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
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申请科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="申请人" EnableEdit="true" ForceSelection="true" />
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
                                <f:Grid ID="GridList" AnchorValue="100% -112" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableCheckBoxSelect="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" KeepCurrentSelection="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="140px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAG" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" HeaderText="申请库房" TextAlign="Center" SortField="DEPTNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTOUTNAME" HeaderText="出库库房" TextAlign="Center" SortField="DEPTOUTNAME" />
                                        <f:BoundField Width="180px" DataField="XSRQ" HeaderText="申请日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="XSRQ" />
                                        <f:BoundField Width="70px" DataField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" SortField="CATID" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="80px" DataField="SLRNAME" HeaderText="申请人" TextAlign="Center" SortField="SLRNAME" />
                                        <f:BoundField Width="80px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="180px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="80px" DataField="SPRNAME" HeaderText="审核员" TextAlign="Center" SortField="SPRNAME" />
                                        <f:BoundField Width="180px" DataField="SPRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SPRQ" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：无高值商品时，请先执行【分配库存】" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnablePostBack="true" runat="server" OnClick="btnScan_Click" EnableDefaultState="false" />
                                                <f:Button runat="server" ID="btnFp" Icon="LockAdd" Text="分配库存" ConfirmText="是否确认进行库存分配?" EnablePostBack="true" OnClick="btnFp_Click" EnableDefaultState="false"></f:Button>
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserMature" Text="审 核" Enabled="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnCancel" Enabled="false" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印拣货单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Billjh()" EnableDefaultState="false" />
                                                <f:Button ID="Button2" Icon="Printer" Text="打印缺货单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Billqh()" EnableDefaultState="false" />
                                                <f:Button ID="Button1" Icon="Printer" Text="打印同行单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" EnableDefaultState="false" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="商 品" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
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
                                                        <f:TextBox ID="txbTHTYPE" runat="server" Label="分配" Hidden="true" />
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
                                                        <f:DropDownList ID="docSPR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSPRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" TextAlign="Center" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
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
                                        <f:RenderField Width="70px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="KCSL" DataField="KCSL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="库存数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="BZSL" DataField="BZSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="申请数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblBZSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="XSSL" DataField="XSSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="出库数<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:NumberBox ID="nblXSSL" runat="server" MinValue="0" MaxValue="99999999" NoDecimal="true" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJE" DataField="HSJE" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="申请数量（最小包装）">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
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
                                        <f:RenderField Width="80px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" DataField="STR1" ColumnID="STR1" HeaderText="追溯码" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false"
                                            HeaderText="生产日期" TextAlign="Center" FieldType="Date" Renderer="Date" RendererArgument="yyyy-MM-dd">
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
                                        <f:RenderField Width="0px" DataField="STR2" ColumnID="STR2" HeaderText="入库单" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="Label3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="beforeedit" Handler="onGridBeforeEdit" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="360px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click" EnableDefaultState="false">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="false">
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="false">
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="80px" MaxLength="80" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click" ValidateForms="Form2"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" Layout="Fit" Width="680px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ONECODE,STR1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="55px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="180px" DataField="ONECODE" HeaderText="商品追溯码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="STR1" HeaderText="本位码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <%--<f:Button ID="btnLabelPrint" Text="标签打印" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()">
                        </f:Button>--%>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="75px" Width="350px" EmptyText="扫描或输入追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script language="javascript" type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            //高值不允许编辑
            var me = this, rowId = params.rowId;
            var isgz = me.getCellValue(rowId, 'ISGZ');
            if (isgz == "Y") {
                return false;
            }
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",A").indexOf(flag) > 0)
                return true;
            else
                return false;
        }
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState == "A" || billState == "R") {
                F.alert("此单据未确定出库,请检查！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/DBD.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDb&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        function btnPrint_Billjh() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/dbjhd.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbjh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        function btnPrint_Billqh() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld_qh.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsDbqh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>

