﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentReturns.aspx.cs" Inherits="ERPProject.ERPReapt.DepartmentReturns" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室申退管理</title>
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" EnableAjax="true" EnableAjaxLoading="false" runat="server" />
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
                                                <f:Button ID="bntSearch" Icon="Magnifier"  Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="请输入单据信息" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申退科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="申退员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="收货部门" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -95" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="FLAG" ColumnID="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" SortField="DEPTOUT" HeaderText="收货部门" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="申退科室" />
                                        <f:BoundField Width="90px" DataField="XSRQ" SortField="XSRQ" HeaderText="申退日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="条目数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SLR" SortField="SLR" HeaderText="申退人" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Region"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel8" BodyPadding="0px" RegionSplit="false" EnableCollapse="false" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：库存数指的是申退科室的库存数" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" AjaxLoadingType="Mask" DisableControlBeforePostBack="true" />
                                                <f:Button ID="btnSumit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" Enabled="false" runat="server" ValidateForms="FormDoc" ConfirmText="是否确认提交此单据?" OnClick="btnSumit_Click" AjaxLoadingType="Mask" DisableControlBeforePostBack="true" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBack" Icon="ApplicationGo" Text="退至供应商" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBack_Click"/>
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行信息?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" Hidden="true" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server"  >
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申退人" Hidden="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="收货部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docTHTYPE" runat="server" Label="申退原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申退科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申退日期" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tbxBARCODE" runat="server" EmptyText="输入或扫描条码" Label="条码扫描" OnTriggerClick="tbxBARCODE_TextChanged"></f:TriggerBox>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="申退说明" MaxLength="80" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -146" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ,ROWNO" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" EnableColumnLines="true" >
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false" HeaderText="序号">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" Locked="true" EnableLock="true"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false" TextAlign="Center" Locked="true" EnableLock="true"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" EncodeText="true" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Int" EnableHeaderMenu="false" TextAlign="Center" EnableLock="true"
                                            HeaderText="申退数<font color='red'>*</font>">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" MinValue="0" DecimalPrecision="6" MaxLength="10" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="退货数量(最小单位)">
                                            <Editor>
                                                <f:Label ID="lblEditorDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="KCSL" DataField="KCSL" FieldType="Int" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="库存数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="UNITSMALLNAME" DataField="UNITSMALLNAME" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" EnableLock="true"
                                            HeaderText="最小包装单位">
                                            <Editor>
                                                <f:Label ID="lblUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Hidden="true"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="82px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="含税进价" TextAlign="Right" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="含税金额" TextAlign="Right" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Hidden="true"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="220px" DataField="STR2" ColumnID="STR2" HeaderText="条码信息" EnableLock="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comONECODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="批号<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorPH" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableLock="true" FieldType="Date"
                                            HeaderText="有效期至<font color='red'>*</font>" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" EnableLock="true" FieldType="Date"
                                            HeaderText="生产日期<font color='red'>*</font>" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false" EnableLock="true"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BARCODE" DataField="BARCODE" EnableLock="true" Hidden="true" FieldType="String" HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableLock="true" Hidden="true" HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" HeaderText="生产厂家编码" TextAlign="Center" EnableLock="true" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" Hidden="true" EnableLock="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISGZ" ColumnID="ISGZ" HeaderText="是否贵重" EnableLock="true" Hidden="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableLock="true" Hidden="true" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISJF" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="beforeedit" Handler="onGridBeforeEdit" />
                                        <f:Listener Event="afteredit" Handler="onGridAfterEdit" />
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
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="360px">
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
                        <f:BoundField Hidden="true" DataField="KCSL" HeaderText="库存数量" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowScan" Title="追溯码扫描信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="620px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:CheckBoxField Width="40px" RenderAsStaticField="true" DataField="FLAGNAME" TextAlign="Center" />
                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="BZHL" HeaderText="包装含量" Hidden="true" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:RenderField Width="220px" DataField="ONECODE" HeaderText="商品追溯码">
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
                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TriggerBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="75px" Width="350px" EmptyText="扫描追溯码" ShowRedStar="true" AutoPostBack="true" OnTriggerClick="zsmScan_TriggerClick" ShowTrigger="false"></f:TriggerBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmALL" Icon="TableRowInsert" Text="一键入库" EnableDefaultState="false" runat="server" ConfirmText="是否将全部追溯码入库？" OnClick="zsmALL_Click"></f:Button>
                        <f:Button ID="zsmDelete" Hidden="true" Icon="Delete" Text="删 除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
          <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="470px" Height="260px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="100px" runat="server">
                    <Rows>
                        <f:FormRow >
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow >
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="140px" MaxLength="80"   />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowBack" Title="退至供应商-商品信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="710px" Height="360px">
            <Items>
                <f:Grid ID="GridReturn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="ROWNO,GDSEQ,ROWNUM" EnableCheckBoxSelect="true" EnableMultiSelect="true" KeepCurrentSelection="true" EnableColumnLines="true">
                    <Columns>
                        <f:BoundField Width="70px" DataField="ROWNO" Hidden="true" />
                        <f:BoundField Width="70px" DataField="ROWNUM" Hidden="true" />
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="80px" DataField="GDSEQ" HeaderText="商品编号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="GDSPEC" HeaderText="规格" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="SL" HeaderText="申退数" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="包装单位" TextAlign="Center" />
                        <f:RenderField Width="80px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" RendererFunction="round2"/>
                        <f:RenderField Width="80px" DataField="SUMHSJE" HeaderText="含税金额" TextAlign="Right" RendererFunction="round2"/>
                        <f:BoundField Width="90px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="240px" DataField="STR2" HeaderText="条码信息" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="YXQZ" HeaderText="有效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="90px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="HiddenField1" runat="server" />
                        <f:Button ID="Button1" Text="退至供应商" Icon="SystemSave" runat="server" OnClick="btnReturn_Click">
                        </f:Button>
                        <f:Button ID="Button2" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            var grid = this, rowData = grid.getRowData(params.rowId);
            if (rowData.values['STR2'] && encodeURIComponent(rowData.values['STR2']).length > 9)
                return false;
            else if (F('<%= docFLAG.ClientID%>').getValue() == "M")
                return true;
            else
                return false;
    }
    var LTDNDG;
    var LTDDG;
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
        ReportViewer.ReportURL = "/grf/GoodsReturnISGZ.grf?timestamp=" + Date.parse(new Date());
        //LTDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
        //LTDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
        //LTDNDG.OnInitialize = LTDNDGLoading;
        //LTDDG.OnInitialize = LTDDGLoading;
        LTDNGZ = ReportViewer.Report.ControlByName("FGZ").AsSubReport.Report;
        LTDGZ = ReportViewer.Report.ControlByName("GZ").AsSubReport.Report;
        LTDNGZ.OnInitialize = LTDNGZLoading;
        LTDGZ.OnInitialize = LTDGZLoading;
        ReportViewer.Start();
        ReportViewer.Report.PrintPreview(true);
        ReportViewer.Stop();
    }
    function LTDNGZLoading() {
        var billNo = F('<%= docBILLNO.ClientID%>').getValue();
        var billState = F('<%= docFLAG.ClientID%>').getValue();
        var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
        var dataurl = "/captcha/PrintReport.aspx?Method=GoodsReturnNGZ&osid=" + billNo;

        //载入子报表数据
        LTDNGZ.LoadDataFromURL(dataurl);
        var field = LTDNGZ.FieldByName("DT");
        if (field.IsNull || field.Value == '') {
            ReportViewer.Report.DeleteReportHeader('ReportHeader1')
        }
    }
    function LTDGZLoading() {
        var billNo = F('<%= docBILLNO.ClientID%>').getValue();
        var billState = F('<%= docFLAG.ClientID%>').getValue();
        var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
        var dataurl = "/captcha/PrintReport.aspx?Method=GoodsReturnYGZ&osid=" + billNo;

        //载入子报表数据
        LTDGZ.LoadDataFromURL(dataurl);
        var field = LTDGZ.FieldByName("DT");
        if (field.IsNull || field.Value == '') {
            ReportViewer.Report.DeleteReportHeader('ReportHeader2')
        }
    }
    function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var lry = F('<%= UserAction.UserID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=EidtPrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
    function LTDNDGLoading() {
        var billNo = F('<%= docBILLNO.ClientID%>').getValue();
        var billState = F('<%= docFLAG.ClientID%>').getValue();
        var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
        var dataurl = "/captcha/PrintReport.aspx?Method=GoodsReturnNDG&osid=" + billNo + "&SHBM=" + billDEPTID;

        //载入子报表数据
        LTDNDG.LoadDataFromURL(dataurl);
        var field = LTDNDG.FieldByName("DT");
        if (field.IsNull || field.Value == '') {
            ReportViewer.Report.DeleteReportHeader('ReportHeader1')
        }
    }
    function LTDDGLoading() {
        var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsReturnYDG&osid=" + billNo + "&SHBM=" + billDEPTID;
            //载入子报表数据
            LTDDG.LoadDataFromURL(dataurl);
            var field = LTDDG.FieldByName("DT");
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'DHSL', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0, DHSLTotal = 0, KCSLTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += me.getCellValue(tr, 'BZSL');
                    DHSLTotal += me.getCellValue(tr, 'DHSL');
                    KCSLTotal += me.getCellValue(tr, 'KCSL');
                    HSJETotal += BZSL * HSJJ;
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('KCSL', KCSLTotal, true);
                me.updateSummaryCellValue('DHSL', DHSLTotal, true);
            }
        }
    </script>
</body>
</html>