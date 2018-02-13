<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StorageAllocation.aspx.cs" Inherits="ERPProject.ERPApply.StorageAllocation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>库存调拨申请</title>
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
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" OnCustomEvent="PageManager1_CustomEvent"
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
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAuditBatch" Icon="UserTick" Text="提 交" ConfirmText="确认提交选中单据?" EnablePostBack="true" runat="server" OnClick="btnAuditBatch_Click" EnableDefaultState="false" />
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
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="15" EmptyText="系统自动生成" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="申请库房" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="lstSLR" runat="server" Label="申请员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="出库库房" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -115" ShowBorder="false" ShowHeader="false" EnableCheckBoxSelect="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" KeepCurrentSelection="true" EnableTextSelection="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound"
                                    EnableColumnLines="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="180px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField DataField="FLAG" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAG" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" HeaderText="申请库房" TextAlign="Center" SortField="DEPTNAME" />
                                        <f:BoundField Width="150px" DataField="DEPTOUTNAME" HeaderText="出库库房" TextAlign="Center" SortField="DEPTOUTNAME" />
                                        <f:BoundField Width="140px" DataField="XSRQ" HeaderText="申请日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="XSRQ" />
                                        <f:BoundField Width="80px" DataField="CATID" HeaderText="商品种类" TextAlign="Center" Hidden="true" SortField="CATID" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="80px" DataField="SUBSUM" HeaderText="合计金额" TextAlign="Center" SortField="SUBSUM"></f:BoundField>
                                        <f:BoundField Width="80px" DataField="SLRNAME" HeaderText="申请人" TextAlign="Center" SortField="SLRNAME" />
                                        <f:BoundField Width="80px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="140px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="80px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" SortField="SHRNAME" />
                                        <f:BoundField Width="140px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="100px" DataField="MEMO" HeaderText="备注" SortField="MEMO" ExpandUnusedSpace="true" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据操作主界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btn_Auto" runat="server" EnablePostBack="true" Icon="BasketEdit" Text="自动调拨" EnableDefaultState="false" OnClick="btn_Auto_Click"></f:Button>
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnablePostBack="true" Enabled="false" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="提 交" Enabled="false" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" ConfirmText="是否确认复制此单据信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" ConfirmText="是否确认导出此单据信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" Hidden="true" EnableDefaultState="false" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" runat="server" ValidateForms="FormDoc" ConfirmText="是否确认删除选中行?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnablePostBack="true" runat="server" ValidateForms="FormDoc" OnClick="btnBill_Click" EnableDefaultState="false" />
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
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="申请日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docSLR" runat="server" Label="申请人" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="系统自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申请库房" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库库房" ShowRedStar="true" Required="true" EnableEdit="true"
                                                            CompareType="String" CompareControl="docDEPTID" CompareOperator="NotEqual" CompareMessage="出库与入库部门不能相同" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
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
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -146" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" TextAlign="Center" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <%--<f:RenderField Width="115px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TriggerBox ID="trbEditorGDSEQ" Required="true" runat="server" OnTriggerClick="trbEditorGDSEQ_TriggerClick" TriggerIcon="Search" MaxLength="15"></f:TriggerBox>
                                            </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="100px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
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
                                        <f:RenderField Width="90px" ColumnID="NUM1" DataField="NUM1" FieldType="Auto" EnableHeaderMenu="false"
                                            HeaderText="可出库存" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="BZSL" DataField="BZSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="申请数" TextAlign="Center">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" Required="true" runat="server" MinValue="0" MaxValue="99999999" NoDecimal="true" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="税率" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Right" RendererFunction="round4">
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
                                        <f:RenderField Width="0px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="LblPH" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
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
                                        <f:RenderField Width="0px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false" Hidden="true"
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
                                        <f:RenderField Width="80px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
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
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <%--<f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
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
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click" EnableDefaultState="false" >
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>--%>
        <f:Window ID="WinAuto" Title="自动调拨信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="400px" Height="230px">
            <Items>
                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="75px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:RadioButtonList ID="rblTYPE" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblTYPE_SelectedIndexChanged">
                                    <f:RadioItem Text="按历史销售生成" Value="XS" Selected="true" />
                                    <f:RadioItem Text="按库存参数生成" Value="KC" />
                                </f:RadioButtonList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList runat="server" ID="ddlDeptOrder1" Label="调出部门" ShowRedStar="true" Required="true"></f:DropDownList>
                                <f:DropDownList runat="server" ID="ddlDeptOrder2" Label="调入部门" ShowRedStar="true" Required="true"></f:DropDownList>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:DatePicker runat="server" ID="dbpOrder1" Label="销售日期"></f:DatePicker>
                                <f:DatePicker runat="server" ID="dbpOrder2" Label="至" LabelWidth="30px" CompareOperator="GreaterThanEqual" CompareControl="dbpOrder1" CompareType="String"></f:DatePicker>
                            </Items>
                        </f:FormRow>
                    </Rows>
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:Label runat="server" Text="公式说明（注意包装信息）："></f:Label>
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:Label ID="memo" runat="server" Text="订货量 =（备货天数+订货周期)*日均用量 -库房库存"></f:Label>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar7" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btn_Sure" Text="生成调拨单" EnableDefaultState="false" Icon="SystemSave" ValidateForms="Form3" runat="server" OnClick="btn_Sure_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow Hidden="true">
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" MaxLength="80" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click" EnableDefaultState="false">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script language="javascript" type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function onCellClick(grid, rowIndex, columnIndex, e) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",N,R").indexOf(flag) > 0)
                return true;
            else
                return false;
        }
    </script>
</body>
</html>

