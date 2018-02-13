﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="High-valueConsumable.aspx.cs" Inherits="ERPProject.ERPApply.High_valueConsumable" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值耗材使用管理</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnTjAll" Icon="PageEdit" Text="登 记" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认登记选中单据?" runat="server" OnClick="btnTjAll_Click" />
                                        <f:Button ID="bntClear" Icon="Erase" EnableDefaultState="false" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" EnableDefaultState="false" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" ForceSelection="true" EnableEdit="true" />
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="lstDOCTOR" runat="server" Label="主治医生" />
                                                <f:TextBox ID="lstOPTTABLE" runat="server" Label="手术台号" />
                                                <f:DatePicker ID="lstOPTDATE1" runat="server" Label="手术日期" />
                                                <f:DatePicker ID="lstOPTDATE2" runat="server" Label="　　至" LabelSeparator="" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
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
                                        <f:BoundField Width="90px" DataField="CATID" SortField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="合计金额" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="DOCTOR" SortField="DOCTOR" HeaderText="主治医生" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="OPTTABLE" SortField="OPTTABLE" HeaderText="手术台号" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="OPTDATE" SortField="OPTDATE" HeaderText="手术日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar1" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：高值商品的使用后录入界面！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" Hidden="true" />
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                        <f:Button ID="btnTJ" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnTJ_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除此单据?" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除选中行?" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                        <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ConfirmText="是否确认驳回此单据？" Enabled="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnDJ" Icon="PageEdit" Text="登 记" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认登记此单据?" runat="server" OnClick="btnDJ_Click" Enabled="false" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打印登记单" EnablePostBack="false" runat="server" EnableDefaultState="false" OnClientClick="PrintUseBill()" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" ValidateForms="FormDoc" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" />
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
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" EmptyText="自动生成" MaxLength="20" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:TriggerBox ID="trbBARCODE" Label="高值条码" runat="server" EmptyText="扫描或输入高值码信息" ShowRedStar="true" OnTriggerClick="trbBARCODE_TriggerClick"></f:TriggerBox>
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="docCUSTID" runat="server" Label="患者姓名" MaxLength="20" ShowTrigger="false" />
                                                <f:TriggerBox ID="docOPTID" runat="server" Label="手术ID " MaxLength="20" ShowTrigger="false" />
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox runat="server" ID="docSTR7" Label="住院号" MaxLength="40"></f:TextBox>
                                                <f:TriggerBox ID="docOPTTABLE" runat="server" Label="病床号" MaxLength="40" ShowTrigger="false" />
                                                <f:DatePicker runat="server" ID="docOPTDATE" Label="手术日期"></f:DatePicker>
                                                <f:TextBox runat="server" ID="docSTR9" Label="条码板号" Enabled="false"></f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 50%">
                                            <Items>
                                                <f:TextBox runat="server" ID="docDOCTOR" Label="主治医生" MaxLength="40"></f:TextBox>
                                                <f:TextBox runat="server" ID="docSTR8" Label="手术医生" MaxLength="40"></f:TextBox>
                                                <f:TextBox runat="server" ID="docMEMO" Label="备注" MaxLength="40"></f:TextBox>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TriggerBox Hidden="true" ID="docSTR2" runat="server" Label="身份证号" MaxLength="20" ShowTrigger="false" OnTriggerClick="docSTR2_TriggerClick" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="true" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:Label ID="lblGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true" Locked="true"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String"
                                            HeaderText="商品名称" EnableLock="true" Locked="true" TextAlign="Center">
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
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
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
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="190px" TextAlign="Center" EnableLock="true" DataField="ONECODE" ColumnID="ONECODE" HeaderText="商品追溯码" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblONECODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="DHSL" EnableLock="true" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="使用数量">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" EnableLock="true" ColumnID="BZSL" DataField="BZSL" FieldType="Int" TextAlign="Center"
                                            HeaderText="使用数">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" MinValue="0" DecimalPrecision="2" MaxLength="10" Enabled="false" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="KCSL" EnableLock="true" DataField="KCSL" EnableHeaderMenu="false"
                                            HeaderText="库存数" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="JXTAX" EnableLock="true" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String"
                                            HeaderText="含税进价" TextAlign="Center" EnableLock="true">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" EnableLock="true" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNITSMALLNAME" EnableLock="true" DataField="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="String" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" EnableLock="true"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ" EnableLock="true" NullDisplayText=""
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableLock="true"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableLock="true"
                                            HeaderText="生产厂家" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableLock="true"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" EnableLock="true" NullDisplayText=""
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableLock="true"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="40" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ISLOT" DataField="ISLOT" FieldType="String" EnableColumnHide="false" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="批号管理" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" EnableLock="true" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="STR2" ColumnID="STR2" EnableLock="true" HeaderText="是否越库" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comSTR2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="CUSTID" DataField="CUSTID" FieldType="String" EnableLock="true"
                                            HeaderText="患者ID">
                                            <Editor>
                                                <f:TextBox ID="txbEditorCUSTID" Required="true" runat="server" MaxLength="20"></f:TextBox>
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
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="520px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="50px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:RenderField Width="180px" DataField="ONECODE" HeaderText="商品追溯码">
                            <Editor>
                                <f:Label ID="lalONECODE" runat="server" />
                            </Editor>
                        </f:RenderField>
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar5" runat="server" Position="Top" ToolbarAlign="Right">
                    <Items>
                        <%--<f:Button ID="btnLabelPrint" Text="标签打印" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()">
                        </f:Button>--%>
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="70px" Width="350px" EmptyText="扫描或输入追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>

        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="750px" Height="400px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="PH,YXQZ,RQ_SC,KCSL,GDSEQ" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridLot_RowDoubleClick">
                    <Columns>
                        <f:RowNumberField Width="50px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="TYPE" HeaderText="供应类型" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:HiddenField ID="saveOnecode" runat="server" />
                        <f:HiddenField ID="savecustid" runat="server" />
                        <f:HiddenField ID="save" runat="server" />
                        <f:Button ID="btnClose" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>

        <f:Window ID="WindowHis" Title="高值HIS使用信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="550px" Height="300px">
            <Items>
                <f:Grid ID="GridHis" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ITEM_CODE,OPER_CODE,NAME,ID_NO,DOCTOR" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridHis_RowDoubleClick">
                    <Columns>
                        <f:RowNumberField Width="30px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="90px" DataField="ITEM_CODE" HeaderText="手术ID" TextAlign="Left" />
                        <f:BoundField Width="70px" DataField="OPER_CODE" HeaderText="手术台号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="NAME" HeaderText="患者姓名" TextAlign="Center" />
                        <f:BoundField Width="130px" DataField="ID_NO" HeaderText="身份证号" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="DOCTOR" HeaderText="主治医生" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnCse" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="btnCse_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            if (F('<%= docFLAG.ClientID%>').getValue() == "M")
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
            if (billState != "Y" && billState!="J") {
                F.alert("选择单据状态不正确,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/gzckd.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetGZXSDataBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function PrintUseBill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y" && billState != "J") {
                F.alert("选择单据状态不正确,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/RecordSheet.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetSYDataBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>