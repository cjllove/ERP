<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighPreUse.aspx.cs" Inherits="ERPProject.ERPApply.HighPreUse" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>跟台商品使用</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" OnCustomEvent="PageManager1_CustomEvent"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
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
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" EmptyText="输入单据编号" MaxLength="20" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" ForceSelection="true" EnableEdit="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAGNAME" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="DEPTID" SortField="DEPTID" HeaderText="使用科室" TextAlign="Center" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="90px" DataField="XSRQ" SortField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="金额合计" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="80px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：跟台高值商品的使用后录入界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />

                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnBill_Click" Enabled="false" />

                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印使用单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />

                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" DisableControlBeforePostBack="true" EnableAjax="false" ConfirmText="是否导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除选中行?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" ValidateForms="FormDoc" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
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
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="使用科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false"></f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="docSTR1" runat="server" Label="预入库单" MaxLength="100" EmptyText="输入跟台预入库单号" ShowRedStar="true" Required="true" TriggerIcon="Search" OnTriggerClick="docSTR1_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="docSLR" runat="server" Label="跟台员" Required="true" ShowRedStar="true"></f:DropDownList>
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="docNUM2" runat="server" Label="患者姓名" MaxLength="40" ShowTrigger="false" />
                                                        <f:TriggerBox ID="docNUM3" runat="server" Label="手术ID " MaxLength="40" ShowTrigger="false" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox runat="server" ID="tbxSTR5" Label="住院号" MaxLength="40"></f:TextBox>
                                                        <f:TextBox runat="server" ID="tbxSTR6" Label="流水号" MaxLength="40"></f:TextBox>                                                        
                                                        <f:TriggerBox ID="docNUM4" runat="server" Label="病床号" MaxLength="40" ShowTrigger="false" />
                                                        <f:TextBox runat="server" EmptyText="自动填写,原定数回收单号" ID="docSTR2" Label="使用单号" Enabled="false" MaxLength="40"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:TextBox runat="server" ID="tbxSTR7" Label="主治医生" MaxLength="40"></f:TextBox>
                                                        <f:TextBox runat="server" ID="tbxSTR8" Label="手术医生" MaxLength="40"></f:TextBox>
                                                        <f:DatePicker runat="server" ID="dphSTR9" Label="手术日期"></f:DatePicker>
                                                        <f:TextBox runat="server" ID="docMEMO" Label="备注" MaxLength="40"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -176" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" ColumnID="ROWNO" DataField="ROWNO" EnableLock="true" Locked="true" FieldType="String" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:Label runat="server" ID="lblGDSEQ"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Locked="true" Hidden="true"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品名称">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Hidden="true"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableLock="true"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Int" TextAlign="Center" EnableLock="true"
                                            HeaderText="使用数<font color='red'>*</font>">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" Required="true" MinValue="0" NoDecimal="true" MaxLength="10" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="DHSL" DataField="DHSL" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="预入库数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="JXTAX" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Hidden="true"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableLock="true"
                                            HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Center" EnableLock="true">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" EnableLock="true"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ" EnableLock="true" NullDisplayText=""
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lplYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" EnableLock="true" NullDisplayText=""
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="170px" DataField="SUPNAME" ColumnID="SUPNAME" HeaderText="供应商" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableLock="true"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="STR1" DataField="STR1" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="本位码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblSTR1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Hidden="true"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableLock="true"
                                            HeaderText="注册证号">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableLock="true"
                                            HeaderText="备注<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="40" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ISLOT" DataField="ISLOT" FieldType="String" EnableColumnHide="false" EnableHeaderMenu="false" EnableLock="true" Hidden="true"
                                            HeaderText="批号管理" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否高值" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="SUPID" ColumnID="SUPID" HeaderText="供应商编码" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorSUPID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Hidden="true"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableLock="true" Hidden="true"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="STR3" DataField="STR3" FieldType="String" EnableLock="true"
                                            HeaderText="" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblSTR3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true" EnableLock="true">
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
        <f:HiddenField ID="hfdSave" runat="server" />
        <f:HiddenField ID="hdfIndex" runat="server" />
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="680px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1" EnableColumnLines="true">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:RenderField Width="220px" DataField="ONECODE" HeaderText="商品追溯码">
                            <Editor>
                                <f:Label ID="lalONECODE" runat="server" />
                            </Editor>
                        </f:RenderField>
                        <f:BoundField Width="100px" DataField="STR1" HeaderText="本位码" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnLabelPrint" Text="追溯码打印" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()">
                        </f:Button>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" Hidden="true" LabelWidth="70px" Width="350px" EmptyText="扫描或输入追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" Hidden="true" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" Hidden="true" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WinBillno" Title="单号查询" Hidden="true" EnableIFrame="false" runat="server" EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="350px">
            <Items>
                <f:Grid ID="GridBill" ShowBorder="false" EnableTextSelection="true" OnPageIndexChange="GridBill_PageIndexChange" EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    ShowHeader="false" IsDatabasePaging="true" AllowPaging="true" OnRowDoubleClick="GridBill_RowDoubleClick" PageSize="10" AllowSorting="false" CheckBoxSelectOnly="false"
                    AutoScroll="true" runat="server" EnableColumnLines="true" DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true">
                    <Columns>
                        <f:RowNumberField Width="30px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单据编号" />
                        <f:BoundField Width="140px" DataField="DEPTNAME" HeaderText="入库库房" />
                        <f:BoundField Width="140px" DataField="PSSNAME" HeaderText="配送商" />
                        <f:BoundField Width="95px" DataField="SHRQ" HeaderText="入库日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <f:DropDownList ID="ddlDEPTIN" runat="server" Label="入库部门" Required="true" ShowRedStar="true" LabelWidth="75px" Width="170px"></f:DropDownList>
                        <f:DatePicker ID="dbkTime1" runat="server" Label="入库日期" ShowRedStar="true" Required="true" LabelWidth="75px" Width="170px" />
                        <f:DatePicker ID="dbkTime2" LabelWidth="40px" Width="140px" runat="server" Label="至" ShowRedStar="true" Required="true" CompareControl="dbkTime1" CompareOperator="GreaterThanEqual" CompareType="String" CompareValue="dbkTime1" />
                        <f:Button runat="server" Text="查 询" ID="btnSrchBill" EnableDefaultState="false" OnClick="btnSrchBill_Click"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Toolbars>
                <f:Toolbar runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnBillSure" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnBillSure_Click"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>

        <f:Window runat="server" ID="WindowGoods" Title="商品信息" Hidden="true" EnableIFrame="false"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Anchor" Width="850px" Height="420px">
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="false"
                    ShowHeader="False" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label="查询信息" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" LabelWidth="75px" Width="800px" CssStyle="margin:10px 20px;" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GoodsInfo" ShowBorder="false" ShowHeader="false" AllowSorting="true" AutoScroll="true" runat="server" AnchorValue="100% -40" CssStyle="border-top:1px solid #ccc;"
                    DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true" IsDatabasePaging="true" EnableColumnLines="true"
                    PageSize="70" AllowPaging="true" OnPageIndexChange="GoodsInfo_PageIndexChange" OnSort="GoodsInfo_Sort" EnableRowDoubleClickEvent="false" OnRowDoubleClick="GoodsInfo_RowDoubleClick" SortField="GDNAME" SortDirection="ASC">
                    <Columns>
                        <f:RowNumberField Width="30px" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField Width="110px" DataField="SEQNO" SortField="SEQNO" HeaderText="单据编号" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="UNIT" HeaderText="包装单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="BZHL" HeaderText="包装含量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="80px" DataField="SSSL" HeaderText="实收数量" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" DataFormatString="{0:F4}" />
                        <f:BoundField Width="80px" DataField="HSJE" HeaderText="含税金额" Hidden="true" TextAlign="Right" />
                        <f:BoundField Width="70px" DataField="PH" SortField="PH" HeaderText="批号" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="YXQZ" SortField="YXQZ" HeaderText="有效期至" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="165px" DataField="PZWH" SortField="PZWH" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="90px" DataField="RQ_SC" SortField="RQ_SC" HeaderText="生产日期" EnableColumnHide="true" EnableHeaderMenu="false" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Hidden="true" DataField="SUPID" HeaderText="供应商编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="供应商" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" Hidden="true" HeaderText="制品编号" />
                        <f:BoundField Width="100px" DataField="STR1" HeaderText="本位码" />
                        <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                        <f:BoundField DataField="SUPPLIER" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="SUPPLIERNAME" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" />
                        <f:BoundField DataField="ZJM" Hidden="true" HeaderText="助记码" />
                        <f:BoundField DataField="DEPTID" Hidden="true" HeaderText="管理部门" />
                        <f:BoundField DataField="FLAG" Hidden="true" HeaderText="状态" />
                        <f:BoundField DataField="CATID" Hidden="true" HeaderText="类别" />
                        <f:BoundField DataField="ISLOT" Hidden="true" HeaderText="批号管理" />
                        <f:BoundField DataField="ISGZ" Hidden="true" HeaderText="是否贵重" />
                        <f:BoundField DataField="ISJF" Hidden="true" HeaderText="是否计费" />
                        <f:BoundField DataField="DSBZSL" Hidden="true" HeaderText="定数包装数量" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="HiddenField1" runat="server" />
                        <f:Button ID="btnSure" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSure_Click">
                        </f:Button>
                        <f:Button ID="btnClose1" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btnClose1_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            if (F('<%= docFLAG.ClientID%>').getValue() == "N" || F('<%= docFLAG.ClientID%>').getValue() == "M")
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
            if (billState != "Y") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/HighPreUse.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGoodsGT&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint_BQ() {
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
            //ReportViewer.ReportURL = "/grf/HighPreUse_ZSM.grf?timestamp=" + new Date().getTime();
            //var dataurl = "/captcha/PrintReport.aspx?Method=GetOnecodeZSM&osid=" + billNo;
            ReportViewer.ReportURL = "/grf/TraceCode.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetOnecodeZSM&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
