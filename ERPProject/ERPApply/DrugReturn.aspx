﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrugReturn.aspx.cs" Inherits="ERPProject.ERPApply.DrugReturn" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>销售退货管理</title>
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
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="Button3" Icon="DatabaseGo" Text="导出全部" OnClick="btExport_Click" ConfirmText="是否导出当前商品退货信息?" DisableControlBeforePostBack="false"
                                            EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" EnableDefaultState="false" />
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
                                                <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入单据编号查询" ShowTrigger="false" OnTriggerClick="lstBILLNO_TriggerClick" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="lstTYPE" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="tgbXSbill" runat="server" MaxLength="20" Label="销售单号" EmptyText="输入销售单号查询" ShowTrigger="false" OnTriggerClick="lstBILLNO_TriggerClick"></f:TriggerBox>
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort" EnableTextSelection="true">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAG" ColumnID="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="使用科室" />
                                        <f:BoundField Width="80px" DataField="XSRQ" SortField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="金额合计" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SPR" HeaderText="审批员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="80px" DataField="SPRQ" HeaderText="审批日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="STR1" SortField="STR1" HeaderText="销售单号" TextAlign="Center" />
                                        <f:BoundField MinWidth="120px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" />
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
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：退货数量为负数" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除此单据?" runat="server" OnClick="btnBill_Click" Enabled="false" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" Hidden="true" />
                                        <f:Button ID="btnCommit" Icon="UserTick" Text="提交" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnCommit_Click" ConfirmText="是否保存并提交此单据？" Enabled="false" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                        <f:Button ID="btnAdt" Icon="UserTick" Text="审 批" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnAdt_Click" ValidateForms="FormDoc" />
                                        <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill()" />
                                        <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="tgbTHDH_TriggerClick" ValidateForms="FormDoc" />
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

                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <%--tgbTHDH_TriggerClick--%>
                                                <f:TriggerBox ID="tgbSTR1" runat="server" Label="销售单号" Required="true" ShowRedStar="true" EmptyText="输入销售单号" TriggerIcon="Search" ShowTrigger="true" OnTriggerClick="tgbSTR1_TriggerClick"></f:TriggerBox>
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" EmptyText="系统自动生成" MaxLength="15" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="申退科室" EnableEdit="true" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="docSTR2" runat="server" Label="退货原因" Required="true" ShowRedStar="true"></f:DropDownList>
                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTOUT" runat="server" Label="回收库房" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docXSRQ" runat="server" Label="退货日期" Required="true" ShowRedStar="true" />
                                                <f:DropDownList ID="ddlSPR" runat="server" Label="审批员" Enabled="false" />
                                                <f:DatePicker ID="dpkSPRQ" runat="server" Label="审批日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow ColumnWidths="50% 25% 25%">
                                            <Items>
                                                <f:TextBox runat="server" ID="docMEMO" Label="备注" MaxLength="40"></f:TextBox>
                                                <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" />
                                                <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" AllowColumnLocking="true"
                                    DataKeyNames="GDSEQ,PH" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String" EnableLock="true" Locked="true" TextAlign="Center"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:Label ID="lblGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BARCODE" EnableLock="true" Locked="true" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="商品条码">
                                            <Editor>
                                                <f:Label ID="lblEditorBARCODE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblEditorName" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="GDSPEC" EnableLock="true" Locked="true" DataField="GDSPEC" FieldType="String"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" EnableLock="true" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="UNITNAME" EnableLock="true" DataField="UNITNAME" FieldType="String"
                                            HeaderText="包装单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNITNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZHL" EnableLock="true" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="NUM2" EnableLock="true" DataField="NUM2" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="可退数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblNUM2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" EnableLock="true" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="使用数量">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" EnableLock="true" DataField="BZSL" FieldType="Auto" TextAlign="Center"
                                            HeaderText="退货数<font color='red'>*</font>">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" NoDecimal="true" runat="server" DecimalPrecision="1" MaxValue="0" MaxLength="8" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" EnableLock="true" FieldType="Auto"
                                            HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label runat="server" ID="lblHSJJ"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" EnableLock="true" DataField="HSJE" HeaderText="含税金额" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="PH" EnableLock="true" DataField="PH" FieldType="String"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPH" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ" EnableLock="true"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="PRODUCER" EnableLock="true" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="生产厂家编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="ZPBH" EnableLock="true" DataField="ZPBH" FieldType="String"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="PRODUCERNAME" EnableLock="true" DataField="PRODUCERNAME" FieldType="String"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblEditorPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="HWID" EnableLock="true" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="150px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableLock="true"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="RQ_SC" DataField="RQ_SC" EnableLock="true"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableLock="true"
                                            HeaderText="备注">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <%-- <f:RenderField Width="180px" ColumnID="MEMO1" DataField="MEMO1" FieldType="String" EnableLock="true" Hidden="true"
                                            HeaderText="备注">
                                            <Editor>
                                                <f:Label ID="labEditorMEMO" runat="server" />
                                            </Editor>
                                        </f:RenderField>--%>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISLOT" ColumnID="ISLOT" EnableLock="true" HeaderText="批号管理" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" DataField="ISGZ" ColumnID="ISGZ" EnableLock="true" HeaderText="是否贵重" EnableColumnHide="false" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comISGZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="JXTAX" DataField="JXTAX" EnableLock="true" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="STR1" DataField="STR1" EnableLock="true" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="退货行号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblSTR1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="NUM3" DataField="NUM3" EnableLock="true" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="NUM1" DataField="NUM1" EnableLock="true" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="进销存系号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM1" runat="server" />
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
        <f:Window ID="winXSD" Title="销售单信息单查询" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="400px">
            <Toolbars>
                <f:Toolbar ID="Toolbar9" runat="server" Position="top">
                    <Items>
                        <f:TextBox ID="WinGOODS" runat="server" Label="商品名称" LabelWidth="80px" MaxLength="28" EmptyText="输入商品信息" />
                        <f:TextBox ID="WinPH" runat="server" Label="商品批号" LabelWidth="80px" MaxLength="28" EmptyText="输入商品批号" />
                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                        <f:Button ID="Button1" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="winSearch_Click" RegionPosition="Right" />
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Items>
                <f:Grid ID="GridWin" ShowBorder="false" ShowHeader="false" EnableColumnLines="true" EnableRowDoubleClickEvent="true" OnRowDoubleClick="WinlistRow_DoubleClick"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="GDSEQ,MEMO,BZSL,YRKSL,DDSL">
                    <Columns>
                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="销售单号" />
                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="80px" DataField="BZSL" HeaderText="销售数量" />
                        <f:BoundField Width="80px" DataField="LRRQ" HeaderText="销售日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar10" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="HiddenField1" runat="server" />
                        <f:ToolbarText Text="操作提示：通过双击选择相应商品添加到前台界面。" runat="server" ID="tbxShuoMing"></f:ToolbarText>
                        <f:ToolbarFill ID="ToolbarFill4" runat="server" />
                        <f:Button ID="btnClose2" Text="关闭" Icon="SystemClose" runat="server" OnClick="btnClose2_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowReject" Title="驳回信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="370px" Height="230px">
            <Items>
                <f:Form ID="Form2" ShowBorder="false" AutoScroll="false" BodyPadding="5px 10px"
                    ShowHeader="False" LabelWidth="70px" runat="server">
                    <Rows>
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="100px" MaxLength="30" />
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
                    DataKeyNames="PH,YXQZ,RQ_SC,PIZNO,KCSL">
                    <Columns>
                        <f:RowNumberField runat="server" Width="50px"></f:RowNumberField>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="60px" DataField="TYPE" HeaderText="供应类型" TextAlign="Center" />
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

        <f:Window ID="Win_Rtn" Title="退货单据信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="830px" Height="450px">
            <Items>
                <f:Grid ID="GrdRtn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="GDSEQ,SEQNO,KCSL,ISDG,PH,ROWNO">
                    <Columns>
                        <f:RowNumberField runat="server" Width="50px"></f:RowNumberField>
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="100px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Left" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="KCSL" HeaderText="可退数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="退货数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNum" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="80px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="DH" HeaderText="单号" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="ISDG" HeaderText="是否代管" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供应商" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="SEQNO" HeaderText="序号" TextAlign="Center" />
                        <f:BoundField Width="0px" Hidden="true" DataField="ROWNO" HeaderText="行号" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar11" runat="server" Position="Top" ToolbarAlign="Left">
                    <Items>
                        <f:TriggerBox ID="trbSearch" runat="server" Width="400px" ShowTrigger="false" EmptyText="可输入商品名称、助记码、商品编码或流水码" />
                        <f:Button runat="server" Text="查 询" ID="Button2" EnableDefaultState="false" OnClick="trbSearch_TriggerClick"></f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="GodSure" Text="确定" EnableDefaultState="false" Icon="SystemSave" runat="server" OnClick="GodSure_Click">
                        </f:Button>
                        <f:Button ID="GodClose" Text="关闭" EnableDefaultState="false" Icon="SystemClose" runat="server" OnClick="GodClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WinBillno" Title="销售单号查询" Hidden="true" EnableIFrame="false" runat="server" EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="350px">
            <Items>
                <f:Grid ID="GridBill" ShowBorder="false" EnableTextSelection="true" OnPageIndexChange="GridBill_PageIndexChange" EnableMultiSelect="false" EnableCheckBoxSelect="true" KeepCurrentSelection="true"
                    ShowHeader="false" IsDatabasePaging="true" AllowPaging="true" OnRowDoubleClick="GridBill_RowDoubleClick" PageSize="10" AllowSorting="false" CheckBoxSelectOnly="false"
                    AutoScroll="true" runat="server" EnableColumnLines="true" DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true">
                    <Columns>
                        <f:RowNumberField Width="30px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="单据编号" />
                        <f:BoundField Width="140px" DataField="DEPTNAME" HeaderText="使用科室" ExpandUnusedSpace="true" />
                        <%--<f:BoundField Width="140px" DataField="PSSNAME" HeaderText="出库库房" />--%>
                        <f:BoundField Width="95px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar7" runat="server" Position="Top" ToolbarAlign="Left">
                    <Items>
                        <f:TriggerBox ID="tgbBillNo" runat="server" Label="销售单号" LabelWidth="75px" Width="220px" EmptyText="输入销售单号查询" OnTriggerClick="btnSrchBill_Click" />
                        <f:TriggerBox ID="tgbGoods" runat="server" Label="商品信息" LabelWidth="75px" Width="310px" EmptyText="销售单商品名称、编码、简称查询" OnTriggerClick="btnSrchBill_Click" />
                        <f:Button runat="server" Text="查 询" ID="btnSrchBill" EnableDefaultState="false" OnClick="btnSrchBill_Click"></f:Button>
                    </Items>
                </f:Toolbar>
                <f:Toolbar ID="Toolbar8" runat="server" Position="Top" ToolbarAlign="Left">
                    <Items>
                        <f:DatePicker ID="dbkTime1" runat="server" Label="审核日期" ShowRedStar="true" Required="true" LabelWidth="75px" Width="220px" />
                        <f:DatePicker ID="dbkTime2" LabelWidth="40px" Width="185px" runat="server" Label="至" ShowRedStar="true" Required="true" CompareControl="dbkTime1" CompareOperator="GreaterThanEqual" CompareType="String" CompareValue="dbkTime1" />
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
    </form>

    <script type="text/javascript">
        function onGridBeforeEdit(event, value, params) {
            var flag = F('<%= docFLAG.ClientID%>').getValue();
            if ((",R,M").indexOf(flag) > 0)
                return true;
            else
                return false;
        }

        var TXDNDG;
        var TXDDG;
        var USERXMID = "";
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
            <%--ReportViewer.ReportURL = "<%=XSTHD%>?timestamp=" + new Date().getTime();
            TXDNDG = ReportViewer.Report.ControlByName("FDG").AsSubReport.Report;
            TXDDG = ReportViewer.Report.ControlByName("DG").AsSubReport.Report;
            TXDNDG.OnInitialize = TXDNDGLoading;
            TXDDG.OnInitialize = TXDDGLoading;--%>
            ReportViewer.ReportURL = "/grf/xsthdx.grf?timestamp=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill_Rtn&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
        function TXDNDGLoading() {
            var billNo = F('<%= docBILLNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            var billDEPTID = F('<%= docDEPTID.ClientID%>').getValue();
            dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill_Rtn&osid=" + billNo;
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
            dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill_RtnDG&osid=" + billNo;
            //载入子报表数据
            TXDDG.LoadDataFromURL(dataurl);
            field = TXDDG.FieldByName("DT");
            if (field.IsNull || field.Value == '') {
                ReportViewer.Report.DeleteReportHeader('ReportHeader2')
            }
        }
    </script>
</body>
</html>
