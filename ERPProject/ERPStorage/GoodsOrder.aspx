<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsOrder.aspx.cs" Inherits="ERPProject.ERPStorage.GoodsOrder" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品订货管理</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        @media screen and (-webkit-min-device-pixel-ratio:0) {
            .x-grid3-cell, /* Normal grid cell */
            .x-grid3-gcell /* Grouped grid cell (esp. in head)*/ {
                box-sizing: border-box;
            }
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
        <f:PageManager OnCustomEvent="PageManager1_CustomEvent" ID="PageManager1" EnableAjaxLoading="false" AutoSizePanelID="TabStrip1" runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="1" runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="Anchor"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel3" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar2" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText2" Text="操作信息：双击打开单据明细！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 空" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAllCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认提交选中单据？" OnClick="btnAllCommit_Click" />
                                                <f:ToolbarSeparator runat="server" ID="ListLine2" />
                                                <f:Button ID="btnAuditBatch" Icon="UserTick" Text="批量审核" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认审核选中单据？" OnClick="btnAuditBatch_Click" />
                                                <f:Button ID="btnBatchPrint" Icon="Printer" Text="批量打印" runat="server" EnableDefaultState="false" OnClick="btnBatchPrint_Click" />
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
                                                        <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" TriggerIcon="Search" MaxLength="15" OnTriggerClick="lstBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstLRY" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="收货地点" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstCGY" runat="server" Label="采购员" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -113" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG,DEPTID,LRY,FLAG_CN" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="70px" DataField="SEQNO" SortField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="FLAG_CN" ColumnID="FLAG_CN" HeaderText="单据状态" SortField="FLAG_CN" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="ISSENDNAME" HeaderText="传输状态" SortField="ISSENDNAME" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="DEPTID" HeaderText="收货地点" SortField="DEPTID" TextAlign="Left" />
                                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="送货商名称" SortField="SUPNAME" TextAlign="Left" />
                                        <f:BoundField Width="90px" DataField="XDRQ" HeaderText="订货日期" SortField="XDRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="40px" DataField="PRINTNUM" ColumnID="PRINTNUM" SortField="PRINTNUM" HeaderText="打印" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="条目数" SortField="SUBNUM" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="订货金额" SortField="SUBSUM" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="CGY" HeaderText="采购员" SortField="CGY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRY" HeaderText="录入员" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="SHR" HeaderText="审核员" SortField="SHR" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" SortField="SHRQ" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField MinWidth="600px" DataField="MEMO" HeaderText="备注" ExpandUnusedSpace="true" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据操作主界面！" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btn_Auto" runat="server" EnablePostBack="true" Hidden="true" Icon="BasketEdit" Text="自动订货" EnableDefaultState="false" OnClick="btn_Auto_Click"></f:Button>
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" Hidden="true" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine1" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认保存并提交此单据?" runat="server" OnClick="btnCommit_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine2" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" Hidden="true" EnableDefaultState="false" ConfirmText="是否确定已经保存数据并复制单据?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" ConfirmText="是否确认删除选中行？" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine3" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" ID="WebLine4" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" EnableDefaultState="false" ConfirmText="是否确定已经保存数据并导出数据?" EnableAjax="false" DisableControlBeforePostBack="false" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnableDefaultState="false" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnableDefaultState="false" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <%--<f:Button ID="btntext" Text="test" runat="server" OnClientClick="zd()"></f:Button>--%>
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                </f:Panel>
                                <f:Panel ID="Panel4" BodyPadding="0px" RegionSplit="true" EnableCollapse="true" RegionPosition="Top" ShowBorder="false"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                                        <f:DropDownList ID="docISSEND" runat="server" Label="发送状态" Enabled="false" EnableEdit="true" ForceSelection="true" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="订货部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docXDRQ" runat="server" Label="订货日期" ShowRedStar="true" Required="true" />
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="15" EmptyText="系统自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>

                                                <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="docPSSID" runat="server" Label="送货商" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="dpkDHRQ" runat="server" Label="到货日期" ShowRedStar="true" Required="true" CompareOperator="GreaterThanEqual" CompareMessage="到货日期不能小于订货日期" CompareType="String" CompareControl="docXDRQ" AutoPostBack="true" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="10% 15% 25% 25% 25%">
                                                    <Items>
                                                        <f:CheckBox ID="cbxISYX" runat="server" Label="优先供货"></f:CheckBox>
                                                        <f:DropDownList ID="docDHLX" runat="server" Label="订货类型" ShowRedStar="true" Required="true"></f:DropDownList>
                                                        <f:TextBox ID="docMEMO" runat="server" Label="备注" MaxLength="80" EmptyText="备注信息" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList Hidden="true" ID="docCGY" runat="server" Label="操作员" Required="true" ShowRedStar="true" MaxLength="8" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" OnPageIndexChange="GridGoods_PageIndexChange"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowPaging="false" IsDatabasePaging="false"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="false" AllowColumnLocking="true" EnableTextSelection="true">
                                    <Toolbars>
                                        <f:Toolbar runat="server">
                                            <Items>
                                                <f:TriggerBox ID="tgxGoods" Label="商品信息" LabelWidth="75px" runat="server" CssStyle="margin-left:30px;" TriggerIcon="Search" MaxLength="20" EmptyText="输入商品信息过滤查询" OnTriggerClick="tgxGoods_TriggerClick"></f:TriggerBox>
                                                <f:Button runat="server" ID="btn_Search" EnablePostBack="true" Text="过 滤" EnableDefaultState="false" CssStyle="margin-left:20px" Icon="Magnifier" OnClick="tgxGoods_TriggerClick">
                                                </f:Button>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDNAME" BoxConfigAlign="Middle" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="商品规格" FieldType="String" EnableHeaderMenu="false" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="BZSL" ColumnID="BZSL" HeaderText="订货包装数<font color='red'>*</font>" TextAlign="Center" FieldType="Int" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" MinValue="0" NoDecimal="false" MaxValue="99999999" DecimalPrecision="2" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="订货单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="DHS" ColumnID="DHS" HeaderText="订货数(最小包装)" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comDHS" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="DHSL" ColumnID="DHSL" HeaderText="入库数" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="含税金额" TextAlign="Right" EnableHeaderMenu="false" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="KCSL" DataField="KCSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="库存数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SPZTSL" DataField="SPZTSL" FieldType="Int" EnableHeaderMenu="false"
                                            HeaderText="在途数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comSPZTSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ZPBH" ColumnID="ZPBH" HeaderText="制品编号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TextBox ID="comZPBH" Required="true" MaxLength="15" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="200px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SUPNAME" DataField="SUPNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="NUM2" ColumnID="NUM2" HeaderText="供货周期" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:NumberBox ID="comNUM2" runat="server" NoDecimal="true" MinValue="1" MaxLength="2"></f:NumberBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" DataField="MEMO" ColumnID="MEMO" HeaderText="备注" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="JXTAX" ColumnID="JXTAX" HeaderText="税率" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="PH" ColumnID="PH" HeaderText="批号" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:TextBox ID="comPH" Required="true" runat="server" MaxLength="20" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:DatePicker ID="comYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false"
                                            Renderer="Date" RendererArgument="yyyy-MM-dd" NullDisplayText="">
                                            <Editor>
                                                <f:Label ID="comRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
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
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="SUPID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="供应商">
                                            <Editor>
                                                <f:Label ID="lblSUPID" runat="server" />
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
                        <f:HiddenField ID="ColorForGridGoods" runat="server"></f:HiddenField>
                        <f:HiddenField ID="hdfGL" runat="server"></f:HiddenField>
                        <f:HiddenField ID="hdfBH" runat="server"></f:HiddenField>
                        <f:HiddenField ID="USERID" runat="server"></f:HiddenField>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdSave" runat="server" />
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowRed" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdOper" runat="server"></f:HiddenField>
        <f:HiddenField ID="hfdFid" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdftest" runat="server" />
        <f:HiddenField ID="hfdCurrent" runat="server" />
        <f:HiddenField ID="hfdBills" runat="server" Label="需要打印的单据号"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowScan" Title="追溯码扫描信息" Hidden="true" EnableIFrame="false" runat="server" AutoScroll="true"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="630px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ONECODE" AllowCellEditing="true">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="56px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
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
                        <f:Button ID="btnLabelPrint" Text="标签打印" Icon="Printer" runat="server" OnClientClick="btnPrint_BQ()"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
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
    </form>
    <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="200px">
        <Items>
            <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                ShowHeader="False" LabelWidth="75px" runat="server">
                <Rows>
                    <f:FormRow>
                        <Items>
                            <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                        </Items>
                    </f:FormRow>
                    <f:FormRow>
                        <Items>
                            <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="80px" MaxLength="80" />
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnRejectSubmit" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <f:Window ID="WinAuto" Title="自动订货信息" Hidden="true" EnableIFrame="false" runat="server"
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
                            <f:DropDownList runat="server" ID="ddlDeptOrder" Label="订货部门" ShowRedStar="true" Required="true"></f:DropDownList>
                        </Items>
                    </f:FormRow>
                    <f:FormRow>
                        <Items>
                            <f:DatePicker runat="server" ID="dbpOrder1" Label="销售日期"></f:DatePicker>
                            <f:DatePicker runat="server" ID="dbpOrder2" Label="至" LabelWidth="30px"></f:DatePicker>
                        </Items>
                    </f:FormRow>
                </Rows>
                <Rows>
                    <f:FormRow>
                        <Items>
                            <f:Label runat="server" Text="公式说明（考虑订货包装）："></f:Label>
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
                    <f:Button ID="btn_Sure" Text="生成订单" EnableDefaultState="false" Icon="SystemSave" ValidateForms="Form3" runat="server" OnClick="btn_Sure_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
    <f:Grid AllowPaging="true" ID="Grid2" Hidden="true" ShowBorder="true" PageSize="20" IsDatabasePaging="true"
        ShowHeader="false" Width="660px" Height="210px" runat="server" EnableCollapse="false" OnPageIndexChange="Grid2_PageIndexChange"
        DataKeyNames="GDSEQ">
        <Columns>
            <f:RowNumberField runat="server" Width="20px"></f:RowNumberField>
            <f:BoundField Width="90px" ColumnID="LISGDSEQ" DataField="GDSEQ" HeaderText="商品编码" />
            <f:BoundField Width="140px" ColumnID="LISGDNAME" DataField="GDNAME" HeaderText="商品名称" />
            <f:BoundField Width="80px" ColumnID="LISZJM" DataField="ZJM" HeaderText="助记码" />
            <f:BoundField Width="80px" ColumnID="LISGDSPEC" DataField="GDSPEC" HeaderText="规格" />
            <f:BoundField Width="35px" ColumnID="LISUNIT" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
            <f:BoundField Width="100px" ColumnID="LISPIZNO" DataField="PIZNO" HeaderText="注册证号" />
            <f:BoundField Width="150px" ColumnID="LISPRODUCER" DataField="PRODUCERNAME" HeaderText="生产厂家" />
        </Columns>
        <PageItems>
            <f:Button ID="Button1" Text="取消" OnClientClick="showhide()" runat="server">
            </f:Button>
        </PageItems>
        <Toolbars>
            <f:Toolbar ID="Toolbar4" Hidden="true" runat="server">
                <Items>
                    <f:Button ID="Button4" Text="取消" OnClientClick="showhide()" runat="server">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Grid>

    <script type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function PrintDHD(flag) {
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var GrfUSER = F('<%= USERID.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (flag == "1") {
                if ((",N,Y").indexOf(billState) < 0) {
                    F.alert("选择单据未审核或已被结算,不允许打印！");
                    return;
                }
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd();
            ReportViewer.ReportURL = "<%=GoodOrder%>?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetOrderData&osid=" + billNo + "&USER=" + GrfUSER;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue();
            $.post("/captcha/PrintReport.aspx?Method=EidtPrintNum", { seqno: billNo, user: lry, oper: 'P' });
            F.customEvent("Search");
        }
        function btnPrint_BQ() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if ((',Y,G').indexOf(billState) < 0) {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            ReportViewer.ReportURL = "/grf/onecode_GZ.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsOrderGz&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btnPrint_onclick() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if ((',Y,G').indexOf(billState) < 0) {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd();
            ReportViewer.ReportURL = "<%=GoodOrder%>?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetOrderData&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

        function onGridBeforeEdit(event, value, params) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            var opervalue = F('<%= hfdOper.ClientID%>').getValue();
            if ((",M,R,N").indexOf(flag) > 0) {
                if (flag == "N" && opervalue == "input")
                    return false;
                else {
                    true
                }
            }
            else
                return false;
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId === 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'DHS', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += me.getCellValue(tr, 'BZSL');
                    HSJETotal += me.getCellValue(tr, 'BZSL') * me.getCellValue(tr, 'HSJJ');
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
            }
        }
    </script>
</body>
</html>
