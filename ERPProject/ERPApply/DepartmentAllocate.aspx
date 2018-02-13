<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DepartmentAllocate.aspx.cs" Inherits="ERPProject.ERPApply.DepartmentAllocate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室商品调拨</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
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
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" EmptyText="输入单据编号信息" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="调出科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="操作员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" MaxLength="15" EmptyText="输入商品信息" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="调入科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -112" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" EnableTextSelection="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="0px" Hidden="true" DataField="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="80px" ColumnID="FLAGNAME" DataField="FLAGNAME" SortField="FLAGNAME" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" SortField="DEPTOUT" HeaderText="调出科室" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="调入科室" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="XSRQ" SortField="XSRQ" HeaderText="调出时间" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="CATID" SortField="CATID" HeaderText="条目数" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="调出金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SLR" SortField="SLR" HeaderText="操作员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="80px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：请首先维护单据头!" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确定删除此单据?" Enabled="false" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确定删除选中行?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认保存并审核此单据?" OnClick="btnBill_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印单据" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" ConfirmText="是否确定复制单据信息?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnableAjax="false" DisableControlBeforePostBack="true" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnGoods_Click" ValidateForms="FormDoc" />
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
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="调出科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="docSLR" runat="server" Label="操作员" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="调入科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="调拨日期" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <%--<f:TextBox ID="tbxINSERT" Label="扫描条码" runat="server" EmptyText="输入或扫描条码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="tbxINSERT_TextChanged"></f:TextBox>--%>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -142" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableLock="true" Locked="true"
                                            HeaderText="商品编码" EnableHeaderMenu="false" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BarCode" DataField="BarCode" FieldType="String" EnableLock="true" Locked="true" Hidden="true"
                                            HeaderText="商品条码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableLock="true" Locked="true" Hidden="true"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true"
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
                                        <f:RenderField Width="90px" DataField="BZSL" ColumnID="BZSL" FieldType="Float" HeaderText="调拨数量<font color='red'>*</font>" TextAlign="Center" EnableLock="true" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" MinValue="0" Required="true" NoDecimal="True" MaxValue="99999999" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="DSCODE" DataField="DSCODE" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="定数条码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblDSCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Right" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" Hidden="true" HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家" TextAlign="Center">
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
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxPH" runat="server" MaxLength="20"></f:TextBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false" FieldType="Date"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="dpkEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" EnableColumnHide="false" FieldType="Date"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:DatePicker ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPIZNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" FieldType="String" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISDS" ColumnID="ISDS" HeaderText="是否定数" FieldType="String" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="lblISDS" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="DSBZSL" ColumnID="DSBZSL" HeaderText="定数含量" FieldType="String" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="DSBZSL" runat="server" />
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
        <f:HiddenField ID="hfdDsBzsl" runat="server"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="510px" Height="360px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableTextSelection="true" EnableMultiSelect="false" EnableCheckBoxSelect="true"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH">
                    <Columns>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
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
        <f:Window runat="server" ID="WindowGoods" Title="商品信息" Hidden="true" EnableIFrame="false"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Anchor" Width="850px" Height="460px">
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
                    DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" IsDatabasePaging="true" KeepCurrentSelection="true"
                    PageSize="70" AllowPaging="true" OnPageIndexChange="GoodsInfo_PageIndexChange" OnSort="GoodsInfo_Sort" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GoodsInfo_RowDoubleClick" SortField="GDNAME" SortDirection="ASC">
                    <Columns>
                        <f:RowNumberField Width="50px" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField Width="70px" DataField="ISDS" SortField="ISDS" HeaderText="是否定数" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="GDSEQ" SortField="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="GDNAME" SortField="GDNAME" HeaderText="商品名称" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="120px" DataField="GDSPEC" SortField="GDSPEC" HeaderText="商品规格" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="KSSPSL" SortField="KSSPSL" HeaderText="数量" EnableColumnHide="true" EnableHeaderMenu="false" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="DSCODE" SortField="DSCODE" HeaderText="定数条码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="UNIT" HeaderText="包装单位" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="60px" DataField="UNITNAME" SortField="UNITNAME" HeaderText="单位" TextAlign="Center" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装含量" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="165px" DataField="PZWH" SortField="PZWH" HeaderText="注册证号" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" SortField="PRODUCERNAME" HeaderText="生产厂家" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField Width="100px" DataField="ZPBH" SortField="ZPBH" Hidden="true" HeaderText="制品编号" />
                        <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" EnableColumnHide="true" EnableHeaderMenu="false" />
                        <f:BoundField DataField="HSJJ" Hidden="true" HeaderText="含税进价" />
                        <f:BoundField DataField="HSJE" Hidden="true" HeaderText="含税金额" />
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
                        <f:BoundField DataField="DSBZSL" Hidden="true" HeaderText="定数包装数量" />
                    </Columns>
                </f:Grid>


            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
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
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",N").indexOf(flag) > 0)
                return true;
            else
                return false;
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'DHSL', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0, DHSLTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += parseInt(me.getCellValue(tr, 'BZSL'));
                    DHSLTotal += me.getCellValue(tr, 'DHSL');
                    HSJETotal += BZSL * HSJJ;
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('DHSL', DHSLTotal, true);
            }
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
            ReportViewer.ReportURL = "/grf/ksdbd.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetKDDataBill&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
