﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HighValueGoodsReturn.aspx.cs" Inherits="ERPProject.ERPApply.HighValueGoodsReturn" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>高值商品销售退回</title>
    <script src="/res/js/CreateControl.js" type="text/javascript"></script>
    <script src="/res/js/GRInstall.js" type="text/javascript"></script>
    <script src="/res/js/jquery.ymh.js" type="text/javascript"></script>
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

        .x-grid-row.highlight td {
            background-color: lightgreen;
            background-image: none;
        }

        .x-grid-row.highyellowlight td {
            background-color: yellow;
            background-image: none;
        }

        .x-grid-row.highredlight td {
            background-color: red;
            background-image: none;
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
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
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="75px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" ShowTrigger="false" OnTriggerClick="lstBILLNO_TriggerClick" />
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstTYPE" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbXSbill" runat="server" MaxLength="20" Label="销售单号" ShowTrigger="false" OnTriggerClick="lstBILLNO_TriggerClick"></f:TriggerBox>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -120" ShowBorder="false" ShowHeader="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" SortField="BILLNO" HeaderText="单据编号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAG" SortField="FLAG" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" SortField="DEPTID" HeaderText="使用科室" />
                                        <f:BoundField Width="80px" DataField="XSRQ" SortField="XSRQ" HeaderText="使用日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="金额合计" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="LRY" SortField="LRY" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHR" SortField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SPR" HeaderText="审批员" TextAlign="Center" SortField="SHR" />
                                        <f:BoundField Width="80px" DataField="SPRQ" HeaderText="审批日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="120px" DataField="STR1" SortField="STR1" HeaderText="销售单号" TextAlign="Center" />
                                        <f:BoundField Width="170px" DataField="MEMO" SortField="MEMO" HeaderText="备注" TextAlign="Center" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：退货数量为负数" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnablePostBack="true" ConfirmText="是否删除此单据?" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" Enabled="false" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnScan_Click" />
                                                <f:Button ID="btnCommit" Icon="UserTick" Text="提交" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnCommit_Click" ConfirmText="是否提交此单据？" Enabled="false" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnAdt" Icon="UserTick" Text="审 批" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" EnableDefaultState="false" OnClick="btnAdt_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnablePostBack="false" runat="server" EnableDefaultState="false" OnClientClick="btnPrint_Bill()" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnablePostBack="true" runat="server" ConfirmText="是否确认复制此单据信息?" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnablePostBack="true" runat="server" ConfirmText="是否确认导出此单据信息?" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnNext" Icon="ForwardBlue" Text="下 页" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnBef" Icon="RewindBlue" Text="上 页" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="btnBill_Click" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnablePostBack="true" runat="server" EnableDefaultState="false" OnClick="tgbTHDH_TriggerClick" />
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
                                                        <f:DropDownList ID="docDEPTID" runat="server" Label="申退科室" Required="true" ShowRedStar="true" ForceSelection="true" EnableEdit="true"/>
                                                        <f:DropDownList ID="docSTR2" runat="server" Label="退货原因" Required="true" ShowRedStar="true"></f:DropDownList>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" EmptyText="系统自动生成" Enabled="false" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DatePicker ID="docXSRQ" runat="server" Label="退货日期" Required="true" ShowRedStar="true" />
                                                        <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" />
                                                        <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />

                                                        <f:DatePicker ID="docSHRQ" runat="server" Label="审核日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TriggerBox ID="tgbSTR1" runat="server" Label="销售单号" Required="true" ShowRedStar="true" EmptyText="输入销售单号" ShowTrigger="false" OnTriggerClick="tgbTHDH_TriggerClick"></f:TriggerBox>
                                                        <f:DropDownList ID="ddlSPR" runat="server" Label="审批员" Enabled="false" />
                                                        <f:DropDownList ID="docSHR" runat="server" Label="审核员" Enabled="false" />
                                                        <f:DatePicker ID="dpkSPRQ" runat="server" Label="审批日期" Enabled="false" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox runat="server" ID="docMEMO" Label="备注" MaxLength="40"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -175" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit">
                                    <Columns>
                                        <f:RowNumberField Width="50px" TextAlign="Center"></f:RowNumberField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:Label ID="lblGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="商品条码" Hidden="true">
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
                                        <f:RenderField Width="80px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
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
                                        <f:RenderField Width="60px" ColumnID="NUM2" DataField="NUM2" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="可退数量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblNUM2" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="60px" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" TextAlign="Center"
                                            HeaderText="使用数量">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Auto" TextAlign="Center"
                                            HeaderText="退货数">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" NoDecimal="true" runat="server" DecimalPrecision="1" MaxValue="0" MaxLength="8" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="Auto"
                                            HeaderText="含税进价" TextAlign="Right">
                                            <Editor>
                                                <f:Label runat="server" ID="lblHSJJ"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="HSJE" DataField="HSJE" HeaderText="含税金额" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="NUM3NAME" DataField="NUM3NAME" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String"
                                            HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPH" runat="server"></f:Label>
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
                                                <f:Label ID="lblEditorHWID" runat="server" />
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
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" ColumnID="MEMO" DataField="MEMO" FieldType="String"
                                            HeaderText="备注">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" />
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
                                        <f:RenderField Width="0px" ColumnID="JXTAX" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="税率" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="STR1" DataField="STR1" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="退货行号" TextAlign="Center" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblSTR1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="NUM3" DataField="NUM3" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="赠品标志" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comNUM3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="STR3" DataField="STR3" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="配送商编码" Hidden="true" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblSTR3" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                           <f:RenderField Width="0px" ColumnID="ONECODE" DataField="ONECODE" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="高值码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblONECODE" runat="server" />
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
        <f:HiddenField ID="hfdGDSEQ" runat="server" />
        <f:HiddenField ID="highlightRows" runat="server" />
        <f:HiddenField ID="highlightRowYellow" runat="server"></f:HiddenField>
        <f:HiddenField ID="highlightRowRed" runat="server"></f:HiddenField>
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
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
                        <f:Button ID="btnRejectSubmit" Text="确定" Icon="SystemSave" runat="server" OnClick="btnRejectSubmit_Click">
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
                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
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
                        <f:TriggerBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="75px" Width="350px" EmptyText="扫描或输入追溯码" ShowRedStar="true" OnTriggerClick="zsmScan_TextChanged"></f:TriggerBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" EnableDefaultState="false" />
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
                    DataKeyNames="GDSEQ,ROWNO,KCSL,NUM3NAME,PSSID,JXCNO">
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
                        <f:BoundField Width="90px" DataField="NUM3NAME" HeaderText="赠品标志" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Hidden="true" DataField="PSSID" HeaderText="供应商编码" />
                        <f:BoundField Width="150px" DataField="PSSNAME" HeaderText="供应商" />
                        <f:BoundField Width="150px" DataField="PRODUCERNAME" HeaderText="生产厂家" />
                        <f:BoundField Width="150px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="ROWNO" HeaderText="行号" TextAlign="Center" />
                        <f:BoundField Width="40px" DataField="JXCNO" HeaderText="进销存行号" TextAlign="Center" />
                        <f:BoundField DataField="ONECODE" Hidden="true"></f:BoundField>
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar6" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="GodSure" Text="确定" Icon="SystemSave" runat="server" OnClick="GodSure_Click">
                        </f:Button>
                        <f:Button ID="GodClose" Text="关闭" Icon="SystemClose" runat="server" OnClick="GodClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <%--  <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
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
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" Icon="SystemClose" runat="server">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>--%>
    </form>

    <script type="text/javascript">
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
             ReportViewer.ReportURL = "/grf/xsthdx.grf?time=" + new Date().getTime();
            var dataurl = "/captcha/PrintReport.aspx?Method=GetXSDataBill_RtnGZ&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>