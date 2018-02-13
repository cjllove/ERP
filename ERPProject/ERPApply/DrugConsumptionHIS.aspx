﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrugConsumptionHIS.aspx.cs" Inherits="ERPProject.ERPApply.DrugConsumptionHIS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>使用信息管理</title>
    <script src="../res/js/CreateControl.js" type="text/javascript"></script>
    <script src="../res/js/GRInstall.js" type="text/javascript"></script>
    <style type="text/css" media="all">
        .f-grid-colheader-Major .f-grid-colheader-text {
            white-space: normal;
            word-break: break-all;
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
        <f:PageManager ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
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
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px" ColumnWidth="45% 25% 10% 10% 10%"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入单据编号信息" />
                                                <f:DropDownList ID="lstDEPTID" runat="server" Label="使用科室" EnableEdit="true" ForceSelection="true" />
                                                <f:DropDownList ID="ddlFLag" runat="server" Label="状态" EnableEdit="true" ForceSelection="true">
                                                    <f:ListItem Text="--请选择--" Value="" Selected="true" />
                                                    <f:ListItem Text="新单" Value="N" />
                                                    <f:ListItem Text="已审核" Value="Y" />
                                                    <f:ListItem Text="已完结" Value="G" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlUser" runat="server" Label="录入员" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="lstLRRQ1" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="　　至" LabelSeparator="" Required="true" ShowRedStar="true" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableTextSelection="true"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" OnRowDataBound="GridList_RowDataBound"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField DataField="SEQNO" Hidden="true" SortField="SEQNO" />
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="单据编号" SortField="BILLNO" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="FLAG" ColumnID="FLAG" HeaderText="单据状态" SortField="FLAG" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTID" HeaderText="使用科室" SortField="DEPTID" />
                                        <f:BoundField Width="90px" DataField="XSRQ" HeaderText="使用日期" SortField="XSRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="CATID" HeaderText="商品种类" SortField="CATID" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" SortField="SUBNUM" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="LRY" HeaderText="录入员" SortField="LRY" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="LRRQ" HeaderText="录入日期" SortField="LRRQ" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="SUPNAME" HeaderText="供应商" SortField="SUPNAME" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SHR" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" SortField="MEMO" TextAlign="Center" ExpandUnusedSpace="true" />
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
                                        <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：科室使用录入，用于销减科室库存！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                        <f:Button ID="btnExtraction" Icon="ApplicationDouble" Text="提取领用单" runat="server" OnClick="btnExtraction_Click" Hidden="true" />
                                        <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除此单据?" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                        <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" ValidateForms="FormDoc" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" Hidden="true" EnablePostBack="true" ConfirmText="是否确认保存并审核此单据?" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" Enabled="false" />

                                        <f:Button ID="btnTemplate" EnablePostBack="false" runat="server" Text="打印单据" Icon="Printer" EnableDefaultState="false">
                                            <Menu ID="Menu1" runat="server">
                                                <f:MenuButton ID="btnPrint" Icon="Printer" Text="打印普耗销售单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill('ph')">
                                                </f:MenuButton>
                                                <f:MenuButton ID="btnPrintPJ" Icon="Printer" Text="打印配件销售单" EnablePostBack="false" runat="server" OnClientClick="btnPrint_Bill('pj')">
                                                </f:MenuButton>
                                            </Menu>
                                        </f:Button>
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
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
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="使用科室" Required="true" ShowRedStar="true"  EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="docXSRQ" runat="server" Label="使用日期" Required="true" ShowRedStar="true" />
                                                <f:TextBox ID="docBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="系统自动生成" />
                                                <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" Enabled="false">
                                                    <f:ListItem Text="--请选择--" Value="" />
                                                    <f:ListItem Text="新单" Value="N" />
                                                    <f:ListItem Text="已审核" Value="Y" />
                                                    <f:ListItem Text="已完结" Value="G" />
                                                </f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                                <f:TriggerBox ID="tbxBARCODE" runat="server" EmptyText="输入或扫描条码" ShowRedStar="true" Label="条码扫描" OnTriggerClick="tbxBARCODE_TextChanged"></f:TriggerBox>
                                                <f:TextBox ID="tbxNUM" runat="server" ShowRedStar="true" Label="扫描个数" Text="0" Enabled="false"></f:TextBox>
<%--                                                <f:DropDownList ID="docLRY" runat="server" Label="录入员" Enabled="false" EnableEdit="true" ForceSelection="true" />--%>
                                               <f:TextBox ID="tbxLRY" runat="server" Label="录入员" Enabled="false"  />

                                                <f:DatePicker ID="docLRRQ" runat="server" Label="录入日期" Enabled="false" Required="true" />
                                            </Items>
                                        </f:FormRow>
                                        <f:FormRow>
                                            <Items>
                                           <f:TriggerBox ID="docCUSTID" runat="server" Label="患者信息" MaxLength="20" ShowTrigger="false" />
                                            <f:TriggerBox ID="docNUM2" runat="server" Label="住院次数" MaxLength="20" ShowTrigger="false" />
                                               <f:TextBox runat="server" ID="docDOCTOR" Label="医生工号" MaxLength="40"></f:TextBox>
                                                <f:TextBox runat="server" ID="docSTR8" Label="医生名称" MaxLength="40"></f:TextBox>
                                          
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" 
                                    DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1" EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit" EnableRowLines="true">
                                    <Columns>
                                        <f:RenderField Width="35px" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" TextAlign="Center">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="BARCODE" DataField="BARCODE" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
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
                                        <f:RenderField Width="120px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="包装单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
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
                                        <f:RenderField Width="220px" DataField="STR1" ColumnID="STR1" HeaderText="非定数条码" EnableColumnHide="false" EnableHeaderMenu="false" TextAlign="Center">
                                            <Editor>
                                                <f:Label runat="server" ID="lblSTR1"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="XSSL" DataField="XSSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="确认数量(最小包装)">
                                            <Editor>
                                                <f:Label ID="lblXSSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="BZSL" DataField="BZSL" FieldType="Auto" TextAlign="Center" HeaderText="使用数<font color='red'>*</font>">
                                            <Editor>
                                                <f:NumberBox ID="nbxBZSL" runat="server" NoDecimal="false" MinValue="0" MaxValue="99999999" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="KCSL" DataField="KCSL" EnableHeaderMenu="false" HeaderText="库存数" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="JXTAX" DataField="JXTAX" FieldType="Float" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="税率" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorJXTAX" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" ColumnID="HSJJ" DataField="HSJJ" FieldType="Float" HeaderText="含税进价" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="XHSJE" HeaderText="含税金额" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" Hidden="true" ColumnID="DHSL" DataField="DHSL" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="使用数量(最小包装)">
                                            <Editor>
                                                <f:Label ID="lblDHSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="UNITSMALLNAME" DataField="UNITSMALLNAME" HeaderText="最小包装单位" FieldType="String" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblUNITSMALLNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PH" DataField="PH" FieldType="String" HeaderText="批号<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorPH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="YXQZ" DataField="YXQZ"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
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
                                        <f:RenderField Width="110px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="货位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="PZWH" DataField="PZWH" FieldType="String"
                                            HeaderText="注册证号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="RQ_SC" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd" FieldType="Date">
                                            <Editor>
                                                <f:Label ID="lblEditorRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="100px" ColumnID="MEMO" DataField="MEMO" FieldType="String"
                                            HeaderText="备注<font color='red'>*</font>" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="tbxEditorMEMO" runat="server" />
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
        <f:HiddenField ID="hfdDgAudit" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True"
            Width="820px" Height="480px" OnClose="Window1_Close">
        </f:Window>
        <f:Window ID="WindowBill" Title="领用单信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="360px">
            <Items>
                <f:Grid ID="GridBill" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server"
                    DataKeyNames="SEQNO" EnableCheckBoxSelect="true" EnableMultiSelect="true" CheckBoxSelectOnly="true" IsDatabasePaging="true"
                    PageSize="20" AllowPaging="true" OnPageIndexChange="GridBill_PageIndexChange" OnSort="GridBill_Sort" EnableColumnLines="true">
                    <Columns>
                        <f:BoundField Width="100px" DataField="SEQNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="FLAG_CN" HeaderText="单据状态" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="XSRQ" HeaderText="申领日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="DEPTOUT" HeaderText="出库部门" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="SLR" HeaderText="申领人" TextAlign="Center" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar4" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:Button ID="btnBillSave" Text="确定" Icon="SystemSave" runat="server" OnClick="btnBillSave_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
        <f:Window ID="WindowLot" Title="商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="750px" Height="400px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="PH,YXQZ,RQ_SC,PZWH,KCSL,HWID">
                    <Columns>
                        <f:RowNumberField Width="35px" runat="server"></f:RowNumberField>
                        <f:BoundField Width="70px" DataField="GDSEQ" Hidden="true" />
                        <f:BoundField Width="160px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="90px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="90px" DataField="YXQZ" HeaderText="效期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="90px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy/MM/dd}" />
                        <f:BoundField Width="80px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>'></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="80px" DataField="TYPE" HeaderText="供应类型" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Width="0px" DataField="HWID" HeaderText="货位" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="0px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Center" Hidden="true" />
                    </Columns>
                </f:Grid>
            </Items>
            <Toolbars>
                <f:Toolbar ID="Toolbar3" runat="server" Position="Bottom" ToolbarAlign="Right">
                    <Items>
                        <f:HiddenField ID="hfdRowIndex" runat="server" />
                        <f:Button ID="btnClosePostBack" Text="确定" Icon="SystemSave" EnableDefaultState="false" runat="server" OnClick="btnClosePostBack_Click">
                        </f:Button>
                        <f:Button ID="btnClose" Text="关闭" Icon="SystemClose" EnableDefaultState="false" runat="server" OnClick="btnClose_Click">
                        </f:Button>
                    </Items>
                </f:Toolbar>
            </Toolbars>
        </f:Window>
    </form>
    <script type="text/javascript">
        function PosGoodsQt(Info) {
            F('<%= hfdValue.ClientID%>').setValue(Info);
            F.customEvent('GoodsAdd');
        
        }
        var arrEditor = ["BZSL","PH"];//Gird中可编辑的表格

        function onGridBeforeEdit(event, value, params) {
            var me = this; rowId = params.rowId; var str1v=me.getCellValue(rowId, 'STR1');
            if ((F('<%= docFLAG.ClientID%>').getValue() == "N") && (str1v == null || str1v == undefined || str1v == '')) {
                       if (arrEditor.indexOf(params.columnId) >= 0) {
                           return true;
                       }
                       else {
                           return false;
                       }
                   }
                <%-- else if (F('<%= docFLAG.ClientID%>').getValue() == "N" ) {
                return true;
            }--%>
            else
                return false;
        }
        function onBeforeEdit(a, b, c) {
            if (c.columnId == 'PH' && b == "\\") {
                F.customEvent("PHWindow$" + c.rowId);
            }
        }

        function btnPrint_Bill(flag) {
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
            if (flag == "pj") {
                ReportViewer.ReportURL = "/ERPXM/XJ_CJRM/GRF/spxsd_wxpj.grf";
            } else {
                ReportViewer.ReportURL = "<%=SPXSD%>?timestamp=" + new Date().getTime();
            }
            var dataurl = "<%=REPORT%>?Method=GetXSData&osid=" + billNo + "&flag=" + flag;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }
    </script>
</body>
</html>