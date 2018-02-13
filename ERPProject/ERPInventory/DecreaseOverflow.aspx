﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DecreaseOverflow.aspx.cs" Inherits="ERPProject.ERPInventory.DecreaseOverflow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品损益管理</title>
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
            EnableTabCloseMenu="false" ActiveTabIndex="1"
            runat="server">
            <Tabs>
                <f:Tab Title="单据列表" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
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
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="Formlist" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="损益单号" />
                                                        <f:DropDownList ID="lisFLAG" runat="server" Label="单据状态"></f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="   至" Required="true" ShowRedStar="true" CompareType="String" CompareControl="lstLRRQ1" CompareOperator="GreaterThanEqual" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableTextSelection="true"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" OnRowDataBound="GridList_RowDataBound" EnableColumnLines="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField Width="30px" />
                                        <f:BoundField Width="150px" DataField="SEQNO" HeaderText="损益单号" TextAlign="Center" SortField="SEQNO" />
                                        <f:BoundField Width="80px" DataField="BILLTYPE" HeaderText="单据类别" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="FLAG" ColumnID="FLAG" HeaderText="单据状态" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="FLAGNAME" ColumnID="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="80px" DataField="DEPTID" HeaderText="损益科室" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="DEPTIDNAME" HeaderText="损益科室" TextAlign="Center" SortField="DEPTIDNAME" />
                                        <f:BoundField Width="80px" DataField="SYTYPE" HeaderText="损益类别" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="SYTYPENAME" HeaderText="损益类别" TextAlign="Center" SortField="SYTYPENAME" />
                                        <f:BoundField Width="90px" DataField="SUBSUM" HeaderText="损益金额" TextAlign="Right" DataFormatString="{0:F2}" SortField="SUBSUM" />
                                        <f:BoundField Width="90px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="110px" DataField="LRY" HeaderText="录入员" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="130px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="110px" DataField="SHR" HeaderText="审核员" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" SortField="SHRNAME" />
                                        <f:BoundField Width="130px" DataField="SHRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SHRQ" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="损益原因" TextAlign="Center" SortField="MEMO" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" Text="操作信息：商品损益操作" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnScan" Icon="TableConnect" Text="追溯码" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnScan_Click" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打 印" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnTj" Icon="UserTick" Text="提 交" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否保存并提交单据？" OnClick="btnTj_Click" Enabled="false" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" EnableDefaultState="false" ConfirmText="是否确定已经保存数据并复制单据?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" EnableDefaultState="false" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" Hidden="true" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnGoods" Icon="Magnifier" Text="追加商品" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow ColumnWidths="25% 50% 25%">
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="损益部门" EnableEdit="true" ForceSelection="true" ShowRedStar="true" Required="true">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlKCTYPE" runat="server" Label="损益类别" ShowRedStar="true" Required="true"></f:DropDownList>
                                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="损益单号" EmptyText="不需填写，系统自动生成"></f:TextBox>
                                                        <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态" Enabled="false">
                                                        </f:DropDownList>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:NumberBox ID="nbxSUBSUM" runat="server" Label="损益金额" Enabled="false"></f:NumberBox>
                                                        <f:DropDownList ID="ddlLRY" runat="server" Label="录入员" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="损益原因"></f:TextBox>
                                                        <f:DropDownList ID="ddlSHR" runat="server" Label="审核人" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkSHRQ" runat="server" Label="审核日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>

                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" EnableColumnLines="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="GDSEQ" AllowCellEditing="true" ClicksToEdit="1"
                                    EnableAfterEditEvent="true" OnAfterEdit="GridGoods_AfterEdit" EnableTextSelection="true">
                                    <Columns>
                                        <f:RenderField Hidden="true" runat="server" ColumnID="ISFLOAT" DataField="ISFLOAT"></f:RenderField>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="GDSEQ" ColumnID="GDSEQ" HeaderText="商品编码" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="tgbGDSEQ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="170px" DataField="NAMEJC" ColumnID="NAMEJC" HeaderText="商品通用名" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblNAMEJC" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="170px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblGDNAME" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="BARCODE" ColumnID="BARCODE" HeaderText="商品条码" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblBARCODE" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="UNIT" ColumnID="UNIT" HeaderText="单位" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblUNIT" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="UNITNAME" ColumnID="UNITNAME" HeaderText="单位" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblUNITNAME" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="GDSPEC" ColumnID="GDSPEC" HeaderText="规格" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="comGDSPEC" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="BZHL" ColumnID="BZHL" HeaderText="包装含量" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblBZHL" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="KCSL" ColumnID="KCSL" HeaderText="库存数量" TextAlign="Right" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="SYSL" ColumnID="SYSL" HeaderText="损益数量" TextAlign="Right" EnableHeaderMenu="false" FieldType="Float">
                                            <Editor>
                                                <f:NumberBox ID="comSYSL" runat="server" NoDecimal="false" MaxLength="6"></f:NumberBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="KCHSJE" ColumnID="KCHSJE" HeaderText="库存含税金额" TextAlign="Right" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblKCHSJE" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" DataField="HSJJ" ColumnID="HSJJ" HeaderText="含税进价" TextAlign="Right" EnableHeaderMenu="false" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" DataField="HSJE" ColumnID="HSJE" HeaderText="损益含税金额" TextAlign="Right" EnableHeaderMenu="false" EnableColumnHide="false" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="comHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="BHSJJ" ColumnID="BHSJJ" HeaderText="不含税进价" TextAlign="Right" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comBHSJJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="BHSJE" ColumnID="BHSJE" HeaderText="损益不含税金额" TextAlign="Right" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comBHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="LSJ" ColumnID="LSJ" HeaderText="零售价" TextAlign="Right" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comLSJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="LSJE" ColumnID="LSJE" HeaderText="损益零售金额" TextAlign="Right" FieldType="Int" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="comLSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" ColumnID="PH" DataField="PH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" HeaderText="批号" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox runat="server" ID="tbxPH" MaxLength="20"></f:TextBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="PZWH" ColumnID="PZWH" HeaderText="注册证号" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false">
                                            <Editor>
                                                <f:Label ID="comPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="RQ_SC" ColumnID="RQ_SC" HeaderText="生产日期" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false"
                                             Renderer="Date" FieldType="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="comRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" DataField="YXQZ" ColumnID="YXQZ" HeaderText="有效期" TextAlign="Center" EnableHeaderMenu="false" EnableColumnHide="false"
                                             Renderer="Date" FieldType="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="comYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="JXTAX" ColumnID="JXTAX" HeaderText="进项税" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblJXTAX" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false" Hidden="true"
                                            HeaderText="制品编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="comZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="170px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false" EnableColumnHide="false"
                                            HeaderText="备注" TextAlign="Center">
                                            <Editor>
                                                <f:TextBox ID="comMEMO" runat="server" MaxLength="80"></f:TextBox>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" DataField="HWID" ColumnID="HWID" HeaderText="货位" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblHWID" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="GDMODE" ColumnID="GDMODE" HeaderText="型号" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblGDMODE" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" DataField="ISLOT" ColumnID="ISLOT" HeaderText="批号管理" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblISLOT" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                         <f:RenderField Width="105px" DataField="SUPID" ColumnID="SUPID" HeaderText="供应商" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblSUPID" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" DataField="PSSID" ColumnID="PSSID" HeaderText="配送商" TextAlign="Center" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblPSSID" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" DataField="SUPNAME" ColumnID="SUPNAME" HeaderText="供应商" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblSUPNAME" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" DataField="PSSNAME" ColumnID="PSSNAME" HeaderText="配送商" TextAlign="Center" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblPSSNAME" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="35px" EnableLock="true" Locked="true" ColumnID="ISGZ" DataField="ISGZ" FieldType="String" EnableHeaderMenu="false" Hidden="true">
                                            <Editor>
                                                <f:Label ID="lblISGZ" runat="server"></f:Label>
                                            </Editor>
                                        </f:RenderField>
                                    </Columns>
                                    <Listeners>
                                        <f:Listener Event="cellclick" Handler="onCellClick" />
                                        <f:Listener Event="afteredit" Handler="OnGridBeforeEdit" />
                                    </Listeners>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:Window ID="WindowLot" Title="损益商品批号信息" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="true" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="850px" Height="550px">
            <Items>
                <f:Grid ID="GridLot" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true" PageSize="30" IsDatabasePaging="true" AllowPaging="true"
                    DataKeyNames="PICINO,SL" OnPageIndexChange="GridLot_PageIndexChange">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center"></f:RowNumberField>
                        <f:BoundField Width="10px" DataField="PICINO" HeaderText="序号" TextAlign="Center" Hidden="true" />
                        <f:BoundField Width="110px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="140px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                        <f:BoundField DataField="UNIT" HeaderText="单位" Hidden="true" />
                        <f:BoundField Width="50px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="70px" DataField="KCSL" HeaderText="库存数量" TextAlign="Center" />
                        <f:TemplateField HeaderText="损益数量" Width="70px" TextAlign="Center">
                            <ItemTemplate>
                                <asp:TextBox runat="server" Width="98%" ID="tbxNumber" CssClass="number"
                                    TabIndex='<%# Container.DataItemIndex + 100 %>' Text='<%# Eval("SL") %>' onblur="checkNum(this)"></asp:TextBox>
                            </ItemTemplate>
                        </f:TemplateField>
                        <f:BoundField Width="100px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="TYPENAME" HeaderText="类型" TextAlign="Center"></f:BoundField>
                        <f:BoundField Width="150px" DataField="PSSNAME" HeaderText="配送商" />
                        <f:BoundField Width="0px" DataField="PSSID" HeaderText="配送商编码" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true"/>
                        <f:BoundField Width="150px" DataField="SUPNAME" HeaderText="供应商" />
                        <f:BoundField Width="0px" DataField="SUPID" HeaderText="供应商编码" EnableColumnHide="false" EnableHeaderMenu="false" Hidden="true"/>
                        <f:BoundField Width="120px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="YXQZ" HeaderText="有效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                        <f:BoundField Hidden="true" DataField="PICINO"></f:BoundField>
                    </Columns>
                    <Toolbars>
                        <f:Toolbar runat="server">
                            <Items>
                                <f:CheckBox ID="CbxShowAll" runat="server" Text="全部显示" CssStyle="margin-left:10px"></f:CheckBox>
                                <f:TriggerBox ID="tgbStrch" runat="server" TriggerIcon="Search" CssStyle="margin-left:60px" EmptyText="商品编码或名称" Label="商品信息" LabelWidth="70px" Width="550px" OnTriggerClick="tgbStrch_TriggerClick"></f:TriggerBox>
                                <f:Button Text="查询" runat="server" Icon="Magnifier" CssStyle="margin-left:20px" OnClick="tgbStrch_TriggerClick"></f:Button>
                            </Items>
                        </f:Toolbar>
                    </Toolbars>
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
        <f:Window ID="WindowScan" Title="赋码扫描(自动保存)" Hidden="true" EnableIFrame="false" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Layout="Fit" Width="600px" Height="360px">
            <Items>
                <f:Grid ID="GridSacn" ShowBorder="false" ShowHeader="false" AllowSorting="false" AutoScroll="true" runat="server" EnableColumnLines="true"
                    DataKeyNames="ONECODE" AllowCellEditing="true" ClicksToEdit="1">
                    <Columns>
                        <f:RowNumberField runat="server" Width="30px" />
                        <f:BoundField Width="90px" DataField="GDSEQ" HeaderText="商品编码" Hidden="true" />
                        <f:BoundField Width="145px" DataField="GDNAME" HeaderText="商品名称" />
                        <f:BoundField Width="91px" DataField="GDSPEC" HeaderText="商品规格" />
                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="55px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" />
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

                        <f:ToolbarFill runat="server"></f:ToolbarFill>
                        <f:TextBox ID="zsmScan" runat="server" Label="扫描条码" LabelWidth="70px" Width="350px" EmptyText="扫描追溯码" ShowRedStar="true" AutoPostBack="true" OnTextChanged="zsmScan_TextChanged"></f:TextBox>
                        <f:ToolbarSeparator runat="server" CssStyle="margin-left:30px" />
                        <f:Button ID="zsmDelete" Icon="Delete" Text="删 除" EnablePostBack="true" runat="server" ConfirmText="是否删除选中商品追溯码?" OnClick="zsmDelete_Click" />
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
                        <f:FormRow>
                            <Items>
                                <f:DropDownList ID="ddlReject" runat="server" Label="驳回原因" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                            </Items>
                        </f:FormRow>
                        <f:FormRow>
                            <Items>
                                <f:TextArea ID="txaMemo" runat="server" Label="详细说明" Height="80px" />
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
    </form>
    <f:HiddenField ID="hfdValue" runat="server" />
    <script type="text/javascript">
        function OnGridBeforeEdit(event,value,param)
        {
            var gridGood = F('<%= GridGoods.ClientID%>')
            if (param.columnId == "SYSL")
            {
                if (param.rowData.values["ISFLOAT"] != "Y") {
                    gridGood.updateCellValue(param.rowId, param.columnId, Math.round(value), false);
                }
            }
        }
        function onCellClick(grid, rowIndex, columnIndex, e) {
            var grid = F(gridGoods);
            $(grid.el.dom).find('.x-grid-row.SelectColor').removeClass('SelectColor');
            $.each((F(gridGoods).f_getSelectedCell()[0] + ',').split(','), function (index, item) {
                if (item !== '') {
                    var row = grid.getView().getNode(parseInt(item, 10)); f
                    $(row).addClass('SelectColor');
                }
            });
            var billState = F('<%= ddlFLAG.ClientID%>').getValue();
            if (billState == "M" || billState == "R")
                return true;
            else
                return false;
        }
        function checkNum(obj) {
            if (isNaN(obj.value)) {
                obj.value = "0";
                obj.focus();
            }

        }
        function btnPrint_onclick() {
            var billNo = F('<%= tbxBILLNO.ClientID%>').getValue();
            var billState = F('<%= ddlFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "Y") {
                F.alert("选择单据未审核或已被结算,不允许打印！");
                return;
            }
            ReportViewer.ReportURL = "/grf/GoodsSY.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GoodsSY&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }

    </script>
</body>
</html>
