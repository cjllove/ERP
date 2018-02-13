<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApplyForSure.aspx.cs" Inherits="ERPProject.ERPApply.ApplyForSure" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室申领确定</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
</head>
<body>
    <div style="width: 0px; height: 0px">
        <script type="text/javascript">
            CreateDisplayViewerEx("0%", "0%", "", "", false, "");
        </script>
    </div>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="true" ID="PageManager1" AutoSizePanelID="TabStrip1" EnableAjax="true"
            runat="server" OnCustomEvent="PageManager1_CustomEvent" />
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
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExp" runat="server" Icon="DatabaseGo" EnableAjax="false"
                                                    OnClick="btExport_Click" EnablePostBack="true" Text="导出" EnableDefaultState="false" ConfirmText="是否确认导出科室申领信息?" />
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
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入单据编号" />
                                                        <f:TextBox ID="tbxSTR2" runat="server" Label="销售单号" EmptyText="输入销售单号" MaxLength="20" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态">
                                                            <f:ListItem Text="---请选择---" Value="" />
                                                            <f:ListItem Text="已分配" Value="S" />
                                                            <f:ListItem Text="已出库" Value="Y" />
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="申领日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -107" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" DataKeyNames="SEQNO,FLAG" CssStyle="border-top: 1px solid #99bce8;"
                                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound"
                                    AllowPaging="true" IsDatabasePaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" HeaderText="单据状态" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAG_CN" ColumnID="FLAG_CN" SortField="FLAG_CN" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="DEPTOUT" SortField="DEPTOUT" HeaderText="出库部门" MinWidth="120px" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="120px" DataField="DEPTID" SortField="DEPTID" HeaderText="申领科室" />
                                        <f:BoundField Width="80px" DataField="XSRQ" SortField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="CATID" SortField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SLR" SortField="SLR" HeaderText="申领人" Hidden="true" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="STR2" SortField="STR2" HeaderText="销售单号" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="PRINT" SortField="PRINT" HeaderText="打印状态" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="FUNCTIME" SortField="FUNCTIME" HeaderText="打印次数" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="OPERTIME" SortField="OPERTIME" HeaderText="打印日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd HH:mm}" />
                                        <f:BoundField Width="100px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：根据分配情况，销减库存！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="出库确认" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" ValidateForms="FormDoc" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnBill" Icon="Printer" Text="打印拣货单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btn_Bill()" Enabled="false" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印同行单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" Enabled="false" />
                                                <f:Button ID="btnPBQ" Icon="Printer" Text="打印标签" EnableDefaultState="false" runat="server" OnClick="btnPBQ_Click" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行信息?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnGoods" Icon="ZoomIn" Text="追加赠品" EnableDefaultState="false" ValidateForms="FormDoc" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                                        <f:TextBox ID="docSTR1" runat="server" Hidden="true" />
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申领人" Hidden="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docNUM1" runat="server" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docSTR2" runat="server" Label="销售单号" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态">
                                                            <f:ListItem Text="---请选择---" Value="" />
                                                            <f:ListItem Text="已分配" Value="S" />
                                                            <f:ListItem Text="已出库" Value="Y" />
                                                        </f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" EmptyText="自动生成" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申领日期" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="40" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -112" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblGDNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Auto" EnableHeaderMenu="false" TextAlign="Center"
                                            HeaderText="拣货数">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" MinValue="0" MaxValue="99999999" DecimalPrecision="2" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="出库单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="XSSL" DataField="XSSL" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="拣货数量(最小单位)">
                                            <Editor>
                                                <f:Label ID="lblXSSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITSMALLNAME" DataField="UNITSMALLNAME" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="最小包装单位">
                                            <Editor>
                                                <f:Label ID="lblUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Center" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJE" DataField="HSJE" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Center" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="NUM1NAME" DataField="NUM1NAME" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorPH" EmptyText="可输入[\]选择批次" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="lblEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" FieldType="Date"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注" ExpandUnusedSpace="true">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="DHSL" DataField="DHSL" FieldType="Auto" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="申领数量(最小单位)">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BARCODE" DataField="BARCODE" FieldType="String"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="STR1" ColumnID="STR1" HeaderText="对应的领用单" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblSTR1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="STR3" ColumnID="STR3" HeaderText="供应商" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblSTR3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="NUM1" DataField="NUM1" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISJF" runat="server" />
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
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="750px" Height="360px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH,HWID">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="170px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Hidden="true" DataField="HWID" HeaderText="货位ID" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="680px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="140px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="90px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="55px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批次" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="210px" DataField="ONECODE" HeaderText="商品追溯码" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="STR1" HeaderText="本位码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描追溯码" LabelWidth="95px" Width="350px" EmptyText="扫描追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                        <f:Button ID="btnLabelPrint" Text="追溯码打印" EnableDefaultState="false" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:HiddenField ID="GoodsType" runat="server" />
        <f:HiddenField ID="hfdCurrent" runat="server" />
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            if (F('<%= docFLAG.ClientID%>').getValue() == "N" || F('<%= docFLAG.ClientID%>').getValue() == "S")
                return true;
            else
                return false;
        }
        var docBILLNO = ('<%= docBILLNO.ClientID%>');
        var docSEQNO = '<%= docSEQNO.ClientID%>';
        var docFLAG = '<%= docFLAG.ClientID%>';
        var docSTR2 = '<%= docSTR2.ClientID%>';
        var GoodsType = '<%= GoodsType.ClientID%>';
        function btnPrint_BQ() {

            var LCD_BILLNO = F(docBILLNO).getValue();
            var billNo = F(docSEQNO).getValue();
            var billState = F(docFLAG).getValue();
            var billNo2 = F(docSTR2).getValue();
            var billGoodsType = F(GoodsType).getValue();
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            if (billGoodsType == "试剂") {
                ReportViewer.ReportURL = "/grf/onecode_SJ.grf";
                var dataurl = "/captcha/PrintReport.aspx?Method=GoodsSJ&osid=" + LCD_BILLNO;
            }
            else {
                if (billNo2 != "") {
                    ReportViewer.ReportURL = "/grf/TraceCode.grf?timestamp=" + new Date().getTime();
                    var dataurl = "/captcha/PrintReport.aspx?Method=GoodsXsGz&osid=" + billNo2;
                }
                else {
                    if (billNo == "") {
                        F.alert("请选择要打印的单据信息！");
                        return;
                    }
                    ReportViewer.ReportURL = "/grf/TraceCode.grf?timestamp=" + new Date().getTime();
                    var dataurl = "/captcha/PrintReport.aspx?Method=GoodsCkGz&osid=" + billNo;
                }
            }
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        function PrintLable() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未确认出库,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/F_barcode.grf?time=201604121432";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_F&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        var TXDNDG;
        var TXDDG;
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billNo2 = F('<%= docSTR2.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();

            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未确认出库,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd;
            var dataurl = "";
            if (billNo2 != "") {
                ReportViewer.ReportURL = "<%=SHTXD%>?timestamp=" + new Date().getTime();
                TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
                TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
                TXDNDG.OnInitialize = TXDNDGLoading;
                TXDDG.OnInitialize = TXDDGLoading;
                //ReportViewer.ReportURL = "/grf/spxsd.grf";
                //dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill&osid=" + billNo2;
                //ReportViewer.Report.LoadDataFromURL(dataurl);
            }
            else {
                //ReportViewer.ReportURL = "/grf/Fds_Shtx.grf";
                ReportViewer.ReportURL = "<%=FDS_SHTXD%>?timestamp=" + new Date().getTime();
                dataurl = "/captcha/PrintReport.aspx?Method=GetData_FcksldAll&osid='" + billNo + "'";
                ReportViewer.Report.LoadDataFromURL(dataurl);
            }
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function TXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billNo2 = F('<%= docSTR2.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetXSDNDG&osid='" + billNo + "'";
            //载入子报表数据
            TXDNDG.LoadDataFromURL(dataurl);

            field = TXDNDG.FieldByName("DT");
            //console.log(field.Value);
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader1')
            }
        }
        function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=EidtPrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }

        function TXDDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billNo2 = F('<%= docSTR2.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetXSDYDG&osid='" + billNo + "'";
            //载入子报表数据

            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");

            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }

        }
        function btn_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld.grf?time=201604251603";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_Jh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
