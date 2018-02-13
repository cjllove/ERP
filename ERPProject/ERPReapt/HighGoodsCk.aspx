<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighGoodsCk.aspx.cs" Inherits="ERPProject.ERPReapt.HighGoodsCk" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>高值直接出库</title>
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
            EnableTabCloseMenu="false" ActiveTabIndex="0"
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
                                                <f:Button ID="btnEpt" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnableAjax="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此信息?" DisableControlBeforePostBack="false" OnClick="btnEpt_Click" />
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
                                                        <f:DropDownList ID="ddlDEPTOUT" runat="server" Label="出库部门" EnableEdit="true" ForceSelection="true" />
                                                        <f:TriggerBox ID="tgbBILLNO" runat="server" Label="单据编号" EmptyText="输入单据信息" ShowTrigger="false" OnTriggerClick="tgbBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态">
                                                            <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                            <f:ListItem Text="新单" Value="M" />
                                                            <f:ListItem Text="已审核" Value="Y" />
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="出库日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -106" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                    EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" SortField="BILLNO" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="70px" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="使用科室" TextAlign="Center" SortField="DEPTID" />
                                        <f:BoundField Width="150px" DataField="DEPTOUT" HeaderText="出库库房" TextAlign="Center" SortField="DEPTOUT" />
                                        <f:BoundField Width="90px" DataField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="XSRQ" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="金额合计" TextAlign="Right" SortField="SUBSUM" DataFormatString="{0:f2}" />
                                        <f:BoundField Width="80px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="80px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="400px" DataField="MEMO" HeaderText="备注" TextAlign="Center" SortField="MEMO" />
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
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除整张单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSubmit" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认提交此单据?" runat="server" OnClick="btnSubmit_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" Hidden="true" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCancel" Hidden="true" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCancel_Click" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" Hidden="true" OnClick="btnScan_Click" />
                                                <f:Button ID="btnPrt" Icon="Printer" Text="打印拣货单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btn_Bill()" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印同行单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnExport" Hidden="true" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Hidden="true" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
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
                                                        <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库库房" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="docSLR" runat="server" Label="出库员" Required="true" ShowRedStar="true"></f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="自动生成" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="出库日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TriggerBox ID="docGZM" runat="server" Label="高值条码"   MaxLength="30" EmptyText="扫描高值条码" ShowRedStar="true"   TriggerIcon="Search" ShowTrigger="false"  OnTriggerClick="docGZM_TextChanged"></f:TriggerBox>
                                                        
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审批员" Enabled="false" />
                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审批日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                       <f:TextBox runat="server" ID="docMEMO" Label="备注" MaxLength="80"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -143" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" AllowColumnLocking="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true">
                                    <Columns>
                                        <f:RenderField Width="35px" ColumnID="ROWNO" DataField="ROWNO" Locked="true" EnableLock="true" FieldType="String" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false" Locked="true" EnableLock="true">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" Locked="true" EnableLock="true" Hidden="true"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" Locked="true" EnableLock="true"
                                            HeaderText="商品名称">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" Locked="true" EnableLock="true"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="230" ColumnID="STR2" DataField="STR2" FieldType="String" EnableLock="true"
                                            HeaderText="高值条码">
                                            <Editor>
                                                <f:Label ID="lblSTR2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Auto" TextAlign="Center"
                                            HeaderText="出库数">
                                            <Editor>
                                                <f:Label ID="lblBZSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="JXTAX" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="Auto" RendererFunction="round4"
                                            HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Center" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ZPBH" DataField="ZPBH" FieldType="String"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String"
                                            HeaderText="生产厂家" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="Label3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="MEMO" DataField="MEMO" FieldType="String"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="40" />
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
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISJF" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
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
        <f:HiddenField ID="hdfIndex" runat="server" />
        <f:HiddenField ID="hdfOper" runat="server" />
        <f:HiddenField ID="hdfZP" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowScan" Title="赋码扫描" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="620px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" DataKeyNames="ONECODE" EnableTextSelection="true" 
                    OnRowDataBound="GridSacn_RowDataBound" >
                    <Columns>
                        <f:CheckBoxField Width="40px" RenderAsStaticField="true" DataField="FLAGNAME" TextAlign="Center" />
                        <f:RowNumberField runat="server" Width="35px" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="53px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="50px" DataField="BZSL" HeaderText="入库数" TextAlign="Center" Hidden="true" />
                        <f:RenderField Width="230px" DataField="ONECODE" HeaderText="商品追溯码">
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
                        <f:TriggerBox ShowTrigger="false" ID="zsmERP" runat="server" Label="ERP条码" LabelWidth="80px" EmptyText="扫描ERP追溯码追溯码" MaxLength="40" ShowRedStar="true" OnTriggerClick="zsmERP_TriggerClick" >
                        </f:TriggerBox>
                        <f:Button ID="zsmALL" Icon="TableRowInsert" Text="一键入库" EnableDefaultState="false" runat="server" ConfirmText="是否将全部追溯码入库？" OnClick="zsmALL_Click"></f:Button>
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
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" />
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
    </form>
    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            return false;
        }
        function btnPrint_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y" && billState != "G") {
                F.alert("选择单据未审核,不允许打印！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/Fds_Shtx.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_Fcksld&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function btn_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "/grf/cksld.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_Jh&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>
