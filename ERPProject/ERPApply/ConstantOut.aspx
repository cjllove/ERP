<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConstantOut.aspx.cs" Inherits="ERPProject.ERPApply.ConstantOut" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>定数出库管理</title>
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
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开单据明细！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnCmt" Icon="UserTick" Text="提 交" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据?" OnClick="btnCmt_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="70px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" EmptyText="输入单据编号模糊查询" ShowTrigger="false" MaxLength="15" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="申领科室" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="新单" Value="M" />
                                                    <f:ListItem Text="已提交" Value="N" />
                                                    <f:ListItem Text="已分配" Value="S" />
                                                    <f:ListItem Text="缺货中" Value="B" />
                                                    <f:ListItem Text="调拨中" Value="D" />
                                                    <f:ListItem Text="调拨完成" Value="W" />
                                                    <f:ListItem Text="已出库" Value="Y" />
                                                </f:DropDownList>
                                                <f:DropDownList ID="lstSLR" runat="server" Label="申领人" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25% ">
                                            <Items>

                                                <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                                <f:TriggerBox ID="tbxGDSEQ" runat="server" Label="商品信息" ShowTrigger="false" EmptyText="输入商品信息查询" MaxLength="80" />
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true" EnableTextSelection="true"  SortDirection="ASC"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="60px" DataField="FLAG" ColumnID="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" SortField="DEPTOUT" HeaderText="出库部门" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="申领科室" />
                                        <f:BoundField Width="80px" DataField="XSRQ" SortField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="CATID" SortField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="CHJE" HeaderText="金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="CHSL" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SLR" SortField="SLR" HeaderText="申领人" TextAlign="Center" />
                                        <f:BoundField Width="40px" DataField="PRINTNUM" ColumnID="PRINTNUM" SortField="PRINTNUM" HeaderText="打印" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="180px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
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
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" Enabled="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="提 交" EnablePostBack="true" runat="server" ConfirmText="是否确认保存并提交此单据?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnAudit2" Icon="UserTick" Text="审 核" EnablePostBack="true" runat="server" ConfirmText="是否确认审核此单据?" OnClick="btnAudit2_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnRtn" Text="回 撤(有问题呵呵)" EnableDefaultState="false" Hidden="true" Icon="ArrowUndo" EnablePostBack="true" runat="server" OnClick="btnRtn_Click" ConfirmText="是否确认将此单据撤回？"></f:Button>
                                        <f:Button ID="BtnPrintJh" Icon="Printer" Text="打印拣货单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_BillJh()" EnableDefaultState="false" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打印同行单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" EnableDefaultState="false" />
                                        <f:Button ID="btnPrintBQ" Icon="Printer" Text="打印标签" EnablePostBack="false" runat="server" OnClientClick="btnPrint_onclick()" EnableDefaultState="false" />

                                        <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" runat="server" ConfirmText="是否确认删除选中行信息?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加定数" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="75px" runat="server">
                                    <Rows>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="docSLR" runat="server" Label="申领人" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" Enabled="false" EmptyText="自动生成" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="新单" Value="M" />
                                                    <f:ListItem Text="已提交" Value="N" />
                                                    <f:ListItem Text="已分配" Value="S" />
                                                    <f:ListItem Text="缺货中" Value="B" />
                                                    <f:ListItem Text="调拨中" Value="D" />
                                                    <f:ListItem Text="调拨完成" Value="W" />
                                                    <f:ListItem Text="已出库" Value="Y" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DatePicker ID="docXSRQ" runat="server" Label="申领日期" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:TextBox ID="docMEMO" runat="server" Label="备注说明" MaxLength="80" />
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核人" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableRowSelectEvent="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="30px" TextAlign="Center" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblROWNO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="115px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" HeaderText="商品条码">
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
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="HISCODE" DataField="HISCODE" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="HIS编码">
                                            <Editor>
                                                <f:Label ID="lblEditorHISCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String"
                                            HeaderText="包装单位编码" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="定数含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="CH" DataField="CH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="出货定数" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblCH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="190px" ColumnID="STR2" DataField="STR2" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="定数条码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorSTR2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="税率" TextAlign="Right" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="CHJE" DataField="CHJE" FieldType="Float" EnableHeaderMenu="false"
                                            HeaderText="出货金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="Label2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJE" DataField="HSJE" FieldType="Float" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="制品编号" TextAlign="Center" Hidden="true">

                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String"
                                            HeaderText="生产厂家编码" TextAlign="Center" Hidden="true">
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
                                                <f:Label ID="lblEditorPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" FieldType="Date"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false" FieldType="Date"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="lblEditorYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ISJF" DataField="ISJF" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="是否计费" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblISJF" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="DHSL" DataField="DHSL" FieldType="Float" EnableHeaderMenu="false"
                                            HeaderText="出货定数" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BZSL" DataField="BZSL" Hidden="true" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblBZSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightyel" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdCurrent" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="false" runat="server" Layout="VBox"
            EnableMaximize="false" EnableResize="false" Target="Self"
            Width="820px" Height="480px" OnClose="Window1_Close">
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnClosePostBack" Text="确 定" Icon="SystemSave" runat="server" EnableDefaultState="false" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关 闭" Icon="SystemClose" EnableDefaultState="false" OnClick="btnClose_Click" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Form ID="FormSearch" ShowBorder="false" AutoScroll="true" BodyPadding="5px 30px 0px 30px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:TriggerBox ID="trbSearch" runat="server" Label="查询信息" TriggerIcon="Search" OnTriggerClick="trbSearch_TriggerClick" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
                <f:Grid ID="GridCom" BoxFlex="1" ShowBorder="true" ShowHeader="false" EnableRowDoubleClickEvent="false" OnRowDoubleClick="GridCom_RowDoubleClick" EnableColumnLines="true"
                    AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="GDSEQ" EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    OnPageIndexChange="GridCom_PageIndexChange">
                    <Columns>
                        <%-- <f:RowNumberField runat="server"></f:RowNumberField>--%>
                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="200px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="0px" Hidden="true" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="SL" HeaderText="库存可出定数" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="NUM_XS" HeaderText="定数含量" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="DSNUM" HeaderText="定数个数" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="NUM_DS" HeaderText="待收定数" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="sum_num" HeaderText="应出定数" TextAlign="Center" />
                        <f:BoundField Width="120px" DataField="GDSPEC" HeaderText="规格·容量" />
                        <f:BoundField Width="90px" DataField="UNIT" HeaderText="包装单位" Hidden="true" />
                        <f:BoundField Width="90px" DataField="UNITNAME" HeaderText="包装单位" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="BZHL" HeaderText="包装数量" Hidden="true" />
                        <f:BoundField DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" />
                        <f:BoundField Width="100px" DataField="ZPBH" HeaderText="制品编号" Hidden="true" />
                        <f:BoundField Width="100px" DataField="PIZNO" HeaderText="注册证号" />
                        <f:BoundField Hidden="true" DataField="PRODUCER" HeaderText="生产厂家编码" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="100px" DataField="BARCODE" HeaderText="商品条码" Hidden="true" />
                        <f:BoundField DataField="JXTAX" Hidden="true" HeaderText="税率" />
                        <f:BoundField DataField="YBJ" Hidden="true" HeaderText="医保价" />
                        <f:BoundField DataField="UNIT_ORDER" Hidden="true" HeaderText="订货单位" />
                        <f:BoundField DataField="UNIT_ORDER_NAME" Hidden="true" HeaderText="订货单位名称" />
                        <f:BoundField DataField="UNIT_SELL" Hidden="true" HeaderText="出库单位" />
                        <f:BoundField DataField="UNIT_SELL_NAME" Hidden="true" HeaderText="出库单位名称" />
                        <f:BoundField DataField="SUPPLIER" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="SUPPLIERNAME" Hidden="true" HeaderText="供应商" />
                        <f:BoundField DataField="HWID" Hidden="true" HeaderText="货位ID" />
                        <f:BoundField DataField="ZJM" Hidden="true" HeaderText="助记码" />
                        <f:BoundField DataField="DEPTID" Hidden="true" HeaderText="管理部门" />
                        <f:BoundField DataField="UNIT_DABZ" Hidden="true" HeaderText="大包装单位" />
                        <f:BoundField DataField="UNIT_DABZ_NAME" Hidden="true" HeaderText="大包装单位" />
                        <f:BoundField DataField="UNIT_ZHONGBZ" Hidden="true" HeaderText="中包装单位" />
                        <f:BoundField DataField="UNIT_ZHONGBZ_NAME" Hidden="true" HeaderText="中包装单位" />
                        <f:BoundField DataField="BARCODE_DABZ" Hidden="true" HeaderText="大包装厂家条码" />
                        <f:BoundField DataField="BARCODE_ZHONGBZ" Hidden="true" HeaderText="中包装厂家条码" />
                        <f:BoundField DataField="NUM_DABZ" Hidden="true" HeaderText="大包装系数" />
                        <f:BoundField DataField="NUM_ZHONGBZ" Hidden="true" HeaderText="中包装系数" />
                        <f:BoundField DataField="YCODE" Hidden="true" HeaderText="原编码" />
                        <f:BoundField DataField="NAMEJC" Hidden="true" HeaderText="商品通用名" />
                        <f:BoundField DataField="NAMEEN" Hidden="true" HeaderText="英文名" />
                        <f:BoundField DataField="STRUCT" Hidden="true" HeaderText="主要构成" />
                        <f:BoundField DataField="FLAG" Hidden="true" HeaderText="状态" />
                        <f:BoundField DataField="CATID" Hidden="true" HeaderText="类别" />
                        <f:BoundField DataField="JX" Hidden="true" HeaderText="剂形" />
                        <f:BoundField DataField="YX" Hidden="true" HeaderText="药效" />
                        <f:BoundField DataField="BAR1" Hidden="true" HeaderText="药监码" />
                        <f:BoundField DataField="BAR2" Hidden="true" HeaderText="统计码" />
                        <f:BoundField DataField="BAR3" Hidden="true" HeaderText="其它编码" />
                        <f:BoundField DataField="HISCODE" Hidden="true" HeaderText="商品HIS 代码" />
                        <f:BoundField DataField="HISNAME" Hidden="true" HeaderText="商品HIS 名称" />
                        <f:BoundField DataField="ISLOT" Hidden="true" HeaderText="批号管理" />
                        <f:BoundField DataField="ISGZ" Hidden="true" HeaderText="是否贵重" />
                        <f:BoundField DataField="ISJF" Hidden="true" HeaderText="是否计费" />
                    </Columns>
                </f:Grid>

            </Items>
        </f:Window>

    </form>
    <script type="text/javascript">
        var GridGoods = '<%=GridGoods.ClientID%>';
        function getStr2s() {
            var str2 = "";
            var data = F(GridGoods).data;
            var selected = F(GridGoods).getSelectedRows();
            for (var i = 0; i < data.length; i++) {
                var values = data[i].values;
                for (var j = 0; j < selected.length; j++) {
                    if (data[i].id == selected[j]) {
                        str2 += values["STR2"]+",";
                    }
                }
            }
            str2 = str2.substring(0, str2.length - 1)
            return str2;
        }
       function OnStr2PrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
           var billNo = getStr2s();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=PrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
        function btnPrint_onclick() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnStr2PrintEnd();
            ReportViewer.ReportURL = "/grf/lable.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetDSData&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            //指定打印机 cneg 20150630
            //var print = getCookie("WEGOERP_PRINT_3");
            //if (print != null && print != "") {
            //    setCookie("WEGOERP_PRINT_3", print);
            //    ReportViewer.Report.Printer.PrinterName = print;
            //}
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        var TXDNDG;
        var TXDDG;
        var USERXMID = "";
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var Report = ReportViewer.Report;

     
            //console.log(USERXMID)

            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核已被结算,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "<%=DS_SHTXD%>?timestamp=" + new Date().getTime();
            //TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            //TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            //TXDNDG.OnInitialize = TXDNDGLoading;
            //TXDDG.OnInitialize = TXDDGLoading;
            var dataurl = "/captcha/PrintReport.aspx?Method=GetDSXSD&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            Report.OnPrintEnd = OnPrintEnd();

            //指定打印机 cneg 20150630
            //var print = getCookie("WEGOERP_PRINT_2");
            //if (print != null && print != "") {
            //    setCookie("WEGOERP_PRINT_2", print);
            //    ReportViewer.Report.Printer.PrinterName = print;
            //}
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=PrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
        function TXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetDSXSDNDG&osid=" + billNo;
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
            dataurl = "/captcha/PrintReport.aspx?Method=GetDSXSDYDG&osid=" + billNo;
            //载入子报表数据

            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");

            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }

        }

        function btnPrint_BillJh() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld_Ds.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_DsJh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            //var print = getCookie("WEGOERP_PRINT_1");
            //if (print != null && print != "") {
            //    setCookie("WEGOERP_PRINT_1", print);
            //    ReportViewer.Report.Printer.PrinterName = print;
            //}
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
