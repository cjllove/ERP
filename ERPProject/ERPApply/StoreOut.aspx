﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StoreOut.aspx.cs" Inherits="ERPProject.ERPApply.StoreOut" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>库房直接出库管理</title>
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
            runat="server" OnCustomEvent="PageManager1_CustomEvent" />
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
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                                <f:Button ID="btnEpt" Icon="DatabaseGo" Text="导 出" EnableAjax="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此信息?" DisableControlBeforePostBack="false" OnClick="btnEpt_Click" EnableDefaultState="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnBatchAudit" Icon="UserTick" Text="批量审核" ConfirmText="是否确认批量审核选中单据？" EnableDefaultState="false" OnClick="btnBatchAudit_Click" runat="server" />
                                                <f:Button ID="btnBatchPrint" Icon="Printer" Text="批量打印" runat="server" EnableDefaultState="false" OnClick="btnBatchPrint_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ShowHeader="False" LabelWidth="75px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbBILLNO" runat="server" Label="单据编号" TriggerIcon="Search" EmptyText="可模糊输入单号" OnTriggerClick="tgbBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="单据状态"></f:DropDownList>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="出库日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -80" ShowBorder="false" ShowHeader="false" EnableColumnLines="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO,FLAG,DEPTOUT,DEPTID" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true"
                                    EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true"
                                    EnableMultiSelect="true" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="BILLNO" HeaderText="单据编号" TextAlign="Center" SortField="BILLNO" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="70px" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="120px" DataField="DEPTOUT" HeaderText="出库库房" SortField="DEPTOUT" />
                                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="使用科室" SortField="DEPTID" />
                                        <f:BoundField Width="80px" DataField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="XSRQ" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Right" SortField="SUBNUM" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" HeaderText="金额合计" TextAlign="Right" SortField="SUBSUM" />
                                        <f:BoundField Width="40px" DataField="PRINTNUM" ColumnID="PRINTNUM" HeaderText="打印" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" HeaderText="录入员" TextAlign="Center" SortField="LRY" />
                                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="70px" DataField="SHR" HeaderText="审核员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="80px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" SortField="MEMO" />
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
                                        <%--<f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：单据标题中星号*标记,单击可编辑!" runat="server" />--%>
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                            <f:Button ID="btnTemplate" EnablePostBack="false" Icon="BookOpen" EnableDefaultState="false" runat="server" Text="模板">
                                                    <Menu ID="Menu1" runat="server">
                                                        <f:MenuButton ID="btnSaveTemplate" Icon="TableSave" EnablePostBack="true" runat="server" OnClick="btnSaveTemplate_Click" Text="保存模板">
                                                        </f:MenuButton>
                                                        <f:MenuButton ID="btnLoadTemplate" Icon="TableAdd" EnablePostBack="true" runat="server" OnClick="btnLoadTemplate_Click" Text="载入模板">
                                                        </f:MenuButton>                                                       
                                                    </Menu>
                                                </f:Button>
                                        <f:Button ID="btn_Auto" runat="server" EnablePostBack="true" Icon="BasketEdit" Text="自动出库" EnableDefaultState="false" OnClick="btn_Auto_Click"></f:Button>
                                        <f:Button ID="btnUnit" Icon="PageWhiteEdit" Text="单位变更" EnablePostBack="true" runat="server" OnClick="btnUnit_Click" EnableDefaultState="false"></f:Button>
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnablePostBack="true" ConfirmText="是否删除整张单据?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                        <f:Button ID="btnCommit" Icon="UserTick" Text="提 交" EnablePostBack="true" runat="server" OnClick="btnCommit_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                        <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnablePostBack="true" runat="server" OnClick="btnScan_Click" EnableDefaultState="false" />
                                        <f:Button ID="mbtnPrint" runat="server" Icon="Printer" Text="打 印" EnableDefaultState="false">
                                            <Menu runat="server">
                                                <f:MenuButton ID="btnPrt" Icon="Printer" Text="打印拣货单" EnablePostBack="false" runat="server" OnClientClick="btn_Bill()" />
                                                <f:MenuButton ID="btnPrint" Icon="Printer" Text="打印同行单" EnablePostBack="true" runat="server" OnClick="btnPrint_Click"  />
                                                <f:MenuButton ID="btnPBQ" Icon="Printer" Text="打印标签" EnablePostBack="false" runat="server" OnClientClick="btnPrint_onclick()" Hidden="true" />
                                            </Menu>
                                        </f:Button>
                                        <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" runat="server" ConfirmText="是否确认复制此单据信息?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                        <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" OnClick="btnBill_Click" EnableDefaultState="false" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                        <f:Button ID="btnGoodsZP" Icon="ZoomIn" Text="追加赠品" EnablePostBack="true" Hidden="true" runat="server" OnClick="btnGoodsZP_Click" ValidateForms="FormDoc" EnableDefaultState="false" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="85px" runat="server">
                                    <Rows>
                                        <f:FormRow Hidden="true">
                                            <Items>
                                                <f:TextBox ID="docSEQNO" runat="server" Hidden="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="25% 25% 25% 25%">
                                            <Items>
                                                <f:DropDownList ID="docDEPTOUT" runat="server" Label="出库库房" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" AutoPostBack="true" OnSelectedIndexChanged="docDEPTOUT_SelectedIndexChanged" />
                                                <f:DropDownList ID="docSLR" runat="server" Label="出库员" Required="true" ShowRedStar="true"></f:DropDownList>
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="申领科室" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" >
                                                </f:DropDownList>
                                                <f:DatePicker ID="docXSRQ" runat="server" Label="出库日期" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:CheckBox ID="ckbCKJSY" runat="server" Label="出库即用"></f:CheckBox>
                                                <f:TextBox ID="docMEMO" runat="server" Label="备     注" MaxLength="40"></f:TextBox>
                                                <f:DropDownList ID="docSHR" runat="server" Label="审批员" Enabled="false" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审批日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false" TextAlign="Center" HeaderText="序号">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="labGDSEQ" runat="server" BoxConfigAlign="Middle">
                                                </f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String"
                                            HeaderText="商品名称">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
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
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装单位编码" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Auto" TextAlign="Center"
                                            HeaderText="出库数<font color='red'>*</font>" >
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" DecimalPrecision="2" Required="true" MinValue="0" MaxLength="8" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="JXTAX" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJJ" DataField="HSJJ" FieldType="Auto"
                                            HeaderText="含税进价" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="DHSL" DataField="DHSL" FieldType="string" TextAlign="Right" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="出库数量(最小单位)">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" DataField="UNITSMALLNAME" ColumnID="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="Auto" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="NUM1NAME" DataField="NUM1NAME" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorPH" runat="server" MaxLength="50" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ" NullDisplayText="" FieldType="Date"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="KCSL" DataField="KCSL" FieldType="Auto" TextAlign="Center"
                                            HeaderText="库存数">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="生产厂家编码" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String"
                                            HeaderText="制品编号" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="货位">
                                            <Editor>
                                                <f:Label ID="lblHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String"
                                            HeaderText="注册证号" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" Hidden="true">
                                            <Editor>
                                                <f:Label ID="Label3" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="180px" ColumnID="MEMO" DataField="MEMO" FieldType="String"
                                            HeaderText="备注<font color='red'>*</font>">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" MaxLength="40" />
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
                                        <f:RenderField Width="0px" ColumnID="NUM1" DataField="NUM1" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="赠品标志" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comNUM1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISJF" ColumnID="ISJF" HeaderText="是否计费" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comISJF" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="afteredit" Handler="onGridAfterEdit" />
                                        <f:Listener Event="beforeedit" Handler="onBeforeEdit" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowRed" runat="server"></f:HiddenField>
        <f:HiddenField ID="hdfIndex" runat="server" />
        <f:HiddenField ID="hdfOper" runat="server" />
        <f:HiddenField ID="hdfZP" runat="server" />
          <f:HiddenField ID="hfdCurrent" runat="server" />
        <f:HiddenField ID="hfdBills" runat="server" Label="需要打印的单据号"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Self"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="Window2" Title="赠品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Self"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" Layout="Fit" Width="370px" Height="200px">
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
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" MaxLength="50" />
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
                        </f:Button>
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
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="70px" Width="350px" EmptyText="扫描或输入追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>

        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" Layout="Fit" Width="750px" Height="400px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH">
                    <Columns>
                        <f:RowNumberField Width="50px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="CKKCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" Hidden="true"/>
                        <f:BoundField Width="60px" DataField="TYPE" HeaderText="是否直送" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
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
        <f:Window ID="WindowUnit" Title="商品单位更改" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="550px" Height="250px">
            <Items>
                <f:Grid ID="GridUnit" EnableCollapse="true" ShowBorder="false" ShowHeader="false"
                    runat="server" EnableCheckBoxSelect="false" DataKeyNames="UNIT,UNITNAME,BZHL,HSJJ" EnableMultiSelect="false"
                    EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridUnit_RowDoubleClick">
                    <Columns>
                        <f:RowNumberField />
                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="0px" DataField="UNIT" HeaderText="单位" Hidden="true" />
                        <f:BoundField Width="80px" DataField="UNITNAME" HeaderText="单位" />
                        <f:BoundField Width="60px" DataField="HSJJ" HeaderText="价格" TextAlign="Right" />
                        <f:BoundField Width="80px" DataField="BZHL" HeaderText="包装含量" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hdfSelctIndex" runat="server" />
                        <f:Button ID="btnUnitSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnUnitSubmit_Click">
                        </f:Button>
                        <f:Button ID="btnUnitClose" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnUnitClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WinAuto" Title="自动出库信息" Hidden="true" EnableIFrame="false" runat="server" ShowHeader="true" ShowBorder="true"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="580px" Height="250px">
            <Items>
                <f:Panel ID="Panel5" runat="server" ShowBorder="false" ShowHeader="false" Layout="Region">
                    <Items>
                        <f:Panel runat="server" ID="panelLeftRegion" RegionPosition="Center" RegionSplit="false" EnableCollapse="false" CssStyle="border-bottom: 0px solid #99bce8;"
                            Width="390px" Title="自动出库" ShowBorder="false" ShowHeader="false">
                            <Items>
                                <f:Form ID="Form3" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                                    ShowHeader="False" LabelWidth="75px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:RadioButtonList ID="rblTYPE" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rblTYPE_SelectedIndexChanged">
                                                    <f:RadioItem Text="历史出库生成" Value="XS" Selected="true" />
                                                    <f:RadioItem Text="库存参数生成" Value="KC" />
                                                </f:RadioButtonList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList runat="server" ID="ddlDeptOrder" Label="出库库房" ShowRedStar="true" Required="true"></f:DropDownList>
                                                <f:DropDownList runat="server" ID="ddlDeptid" Label="入库科室" ShowRedStar="true" Required="true"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DatePicker runat="server" ID="dbpOrder1" Label="销售日期"></f:DatePicker>
                                                <f:DatePicker runat="server" ID="dbpOrder2" Label="至" LabelWidth="30px" CompareControl="dbpOrder1" CompareOperator="GreaterThanEqual" CompareType="String" CompareMessage="日期维护不正确"></f:DatePicker>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:Label runat="server" EncodeText="false" Text="<strong>公式说明（考虑订货包装）:</strong>"></f:Label>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:Label ID="memo" runat="server" Text="出库量 = 销售期间的销售量 - 科室库存 - 在途库存"></f:Label>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Panel>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar7" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btn_Sure" Text="生成计划单" EnableDefaultState="false" Icon="SystemSave" ValidateForms="Form3" runat="server" OnClick="btnSure_Click">
                        </f:Button>
                        <f:Button ID="btn_close" Text="关闭" EnableDefaultState="false" Icon="SystemClose" ValidateForms="Form3" runat="server" OnClick="btn_close_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
           <f:Window ID="Window3" Title="保存模板信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="270px" Height="120px">
        <Items>
            <f:Form ID="Form4" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                ShowHeader="False" LabelWidth="75px" runat="server">
                <Rows>
                    <f:FormRow>
                        <Items>
                            <f:TextBox ID="tbsFileName" runat="server" Label="模板名称" MaxLength="20" ShowRedStar="true" Required="true" EmptyText="保存模板的名称"></f:TextBox>
                        </Items>
                    </f:FormRow>
                </Rows>
            </f:Form>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar8" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnSaveTemplateClose" Text="保存确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSaveTemplateClose_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window> <f:Window ID="Window4" Title="加载模板信息" Hidden="true" EnableIFrame="false" runat="server"
        EnableMaximize="false" EnableResize="false" IsModal="True" Layout="Fit" Width="470px" Height="320px">
        <Items>
            <f:Grid ID="GridTemplate" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableMultiSelect="false"
                DataKeyNames="GROUPID,GROUPNAME" EnableColumnLines="true" OnRowCommand="GridTemplate_RowCommand"
                EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridTemplate_RowDoubleClick">
                <Columns>
                    <f:RowNumberField runat="server" Width="30" TextAlign="Center" />
                    <f:BoundField DataField="GROUPID" EnableColumnHide="true" EnableHeaderMenu="false" Hidden="true" />
                    <f:BoundField DataField="GROUPNAME" HeaderText="模板名称" EnableColumnHide="true" EnableHeaderMenu="false" ExpandUnusedSpace="true" />
                    <f:BoundField DataField="USERNAME" HeaderText="添加人" Width="90px" EnableColumnHide="true" EnableHeaderMenu="false" />
                    <f:LinkButtonField HeaderText="操作" Width="50px" TextAlign="Center" CommandName="FileDelete" Text="删除" ConfirmText="确定要删除该模板么？" />
                </Columns>
            </f:Grid>
        </Items>
        <Toolbars>
            <f:Toolbar ID="Toolbar9" runat="server" Position="Bottom" ToolbarAlign="Right">
                <Items>
                    <f:Button ID="btnLoadTemplateClose" Text="保存确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="btnSaveTemplateClose_Click">
                    </f:Button>
                </Items>
            </f:Toolbar>
        </Toolbars>
    </f:Window>
                <f:HiddenField ID="hfdTemplateName" runat="server" />

    </form>
    <script type="text/javascript">
      <%--  function PrintTXD() {
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd();
            ReportViewer.ReportURL = "/grf/Fds_Shtx_2.grf?timestamp=" + new Date().getTime();
            PLTXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            PLTXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            PLTXDNDG.OnInitialize = PLTXDNDGLoading;
            PLTXDDG.OnInitialize = PLTXDDGLoading;
            //var dataurl = "/captcha/PrintReport.aspx?Method=GetData_Fcksld&osid=" + billNo;
            //ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
         function PLTXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetData_Fcksld&osid=" + billNo;
            //载入子报表数据
            PLTXDNDG.LoadDataFromURL(dataurl);

            field = PLTXDNDG.FieldByName("DT");
            //console.log(field.Value);
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader1')
            }
        }
        function PLTXDDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetData_FcksldDG&osid=" + billNo;
            //载入子报表数据

            PLTXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");

            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }

        }--%>
        function btnPrint_Bill() {
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            //if (billState != "Y" && billState != "G") {
            //    F.alert("选择单据未审核,不允许打印！");
            //    return;
            //}
            var Report = ReportViewer.Report;
            Report.OnPrintEnd = OnPrintEnd();
            ReportViewer.ReportURL = "/grf/Fds_Shtx_2.grf?timestamp=" + new Date().getTime();
            //TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            //TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            //TXDNDG.OnInitialize = TXDNDGLoading;
            //TXDDG.OnInitialize = TXDDGLoading;
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_FcksldN&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        <%--function TXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetData_Fcksld&osid=" + billNo;
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
            dataurl = "/captcha/PrintReport.aspx?Method=GetData_FcksldDG&osid=" + billNo;
            //载入子报表数据

            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");

            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }

        }--%>
        function btn_Bill() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            var Report = ReportViewer.Report;
            ReportViewer.ReportURL = "<%=cksld%>?timestamp=" + new Date().getTime();
            console.log(ReportViewer.ReportURL);
            TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            TXDNDG.OnInitialize = TXDNDGJHLoading;
            TXDDG.OnInitialize = TXDDGJHLoading;
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function TXDNDGJHLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
             var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue(); 
            dataurl = "/captcha/PrintReport.aspx?Method=GetData_JHFDG&osid=" + billNo;
             //载入子报表数据
             TXDNDG.LoadDataFromURL(dataurl);

             field = TXDNDG.FieldByName("DT");
             //console.log(field.Value);
             if (field.IsNull || field.Value == '') {
                 ReportViewer.Report.DeleteReportHeader('ReportHeader1')
             };
         }
         function TXDDGJHLoading() {
             var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
             dataurl = "/captcha/PrintReport.aspx?Method=GetData_JHDG&osid=" + billNo;
            //载入子报表数据

            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");

            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }

        }
        function btnPrint_onclick() {
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
           // ReportViewer.ReportURL = "/grf/F_barcode.grf";
            ReportViewer.ReportURL = "/grf/F_barcode.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetData_F&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        }
        function OnPrintEnd() {
            //打印数据已经成功发送到打印机数据缓冲池触发本事件
            var billNo = F('<%= hfdBills.ClientID%>').getValue();
            var lry = F('<%= hfdCurrent.ClientID%>').getValue(); 
              $.post("/captcha/PrintReport.aspx?Method=EidtPrintNum", { seqno: billNo, user: lry, oper: 'P' });
        }
        function onBeforeEdit(a,b,c) {
            if (c.columnId == 'PH' && b=="\\") {
                F.customEvent("PHWindow$" + c.rowId);
            }
        }
        function onGridAfterEdit(event, value, params) {
            var me = this, columnId = params.columnId, rowId = params.rowId;
            if (columnId == 'BZSL') {
                var BZSL = me.getCellValue(rowId, 'BZSL');
                var BZHL = me.getCellValue(rowId, 'BZHL');
                var HSJJ = me.getCellValue(rowId, 'HSJJ');
                me.updateCellValue(rowId, 'DHSL', BZSL * BZHL);
                me.updateCellValue(rowId, 'HSJE', BZSL * HSJJ);
                var BZSLTotal = 0, HSJETotal = 0, DHSLTotal = 0;
                me.getRowEls().each(function (index, tr) {
                    BZSLTotal += parseInt(me.getCellValue(tr, 'BZSL'));
                    DHSLTotal += parseInt(me.getCellValue(tr, 'DHSL'));
                    HSJETotal += me.getCellValue(tr, 'BZSL') * me.getCellValue(tr, 'HSJJ');
                });
                me.updateSummaryCellValue('GDNAME', "本页合计", true);
                me.updateSummaryCellValue('BZSL', BZSLTotal, true);
                me.updateSummaryCellValue('HSJE', HSJETotal, true);
                me.updateSummaryCellValue('DHSL', DHSLTotal, true);
            }
        }
    </script>
</body>
</html>
