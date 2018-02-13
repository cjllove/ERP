﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LibraryMaintain.aspx.cs" Inherits="ERPProject.ERPStorage.LibraryMaintain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>在库养护管理</title>
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
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1" runat="server" OnCustomEvent="PageManager1_CustomEvent" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right" EnableTabCloseMenu="false" ActiveTabIndex="0" runat="server">
            <Tabs>
                <f:Tab Title="生成养护单" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel" runat="server" ShowBorder="false" ShowHeader="false"
                            Layout="VBox" BoxFlex="1" BoxConfigAlign="Stretch" BoxConfigPosition="Start">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText4" CssStyle="" Text="操作信息：" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="btnCreate" Icon="Disk" Text="生成养护单" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认生成养护单信息?" runat="server" OnClick="btnCreate_Click" ValidateForms="fmForm" Enabled="false" />
                                        <f:ToolbarSeparator runat="server"></f:ToolbarSeparator>
                                        <f:Button ID="btnClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnClear_Click" />
                                        <f:Button ID="btnSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnSearch_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="fmForm" runat="server" LabelWidth="75px" ShowBorder="false" ShowHeader="false" MarginLeft="5px" MarginRight="5px" MarginTop="10px">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="docDEPTID" runat="server" Label="库房" ShowRedStar="true" Required="true" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                <f:TextBox ID="tbxGDSEQ" runat="server" Label="商品信息" MaxLength="20" EmptyText="可输入商品编码、名称、助记码或ERP编码"></f:TextBox>
                                                <f:TextBox ID="tbxHWID" runat="server" Label="货位" MaxLength="20" EmptyText="可输入货位信息">
                                                </f:TextBox>
                                                <f:TextBox ID="tbxPRODUCER" runat="server" Label="生产厂家" MaxLength="20" EmptyText="可输入厂家信息" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridLeft" ShowBorder="false" ShowHeader="false" AllowSorting="false" CssStyle="border-top: 1px solid #99bce8;border-bottom: 1px solid #99bce8;" EnableTextSelection="true"
                                    BoxFlex="1" AutoScroll="true" runat="server" DataKeyNames="GDSEQ,PICINO" PageSize="50" IsDatabasePaging="true" EnableMultiSelect="true" EnableCheckBoxSelect="true"
                                    AllowPaging="false" OnPageIndexChange="GridLeft_PageIndexChange" EnableColumnLines="true" KeepCurrentSelection="true">
                                    <Columns>
                                        <f:BoundField Width="80px" DataField="DEPTNAME" HeaderText="科室" />
                                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="GDID" HeaderText="商品条码" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="GDNAME" HeaderText="商品名称" />
                                        <f:BoundField Width="130px" DataField="GDSPEC" HeaderText="商品规格" />
                                        <f:BoundField Width="50px" DataField="UNIT" HeaderText="单位" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="ISGZ" HeaderText="是否贵重" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="80px" DataField="ISGZNAME" HeaderText="是否贵重" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                                        <f:BoundField Width="90px" DataField="KCSL" HeaderText="库存数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="HSJE" HeaderText="含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="HWID" HeaderText="货位" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="PHID" HeaderText="批号" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="YXQZ" HeaderText="有效期至" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="PIZNO" HeaderText="注册证号" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="PRODUCER" HeaderText="生产厂家" Hidden="true" />
                                        <f:BoundField Width="170px" DataField="PRODUCERNAME" HeaderText="生产厂家" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="BAR3" HeaderText="ERP编码" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="STR4" HeaderText="HIS编码" TextAlign="Center" />
                                        <f:BoundField Width="80px" DataField="PICINO" HeaderText="批次编号" TextAlign="Center" />
                                        <f:BoundField Width="0px" DataField="BZHL" HeaderText="包装含量" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="0px" DataField="MEMO" HeaderText="备注" TextAlign="Center" ExpandUnusedSpace="true" Hidden="true" />
                                        <f:BoundField Width="0px" DataField="ISLOT" HeaderText="批号管理" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="0px" DataField="ZPBH" HeaderText="制品编码" TextAlign="Center" Hidden="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据列表" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel1" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="PanelBody" ShowBorder="false" BodyPadding="0px"
                                    Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar1" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：双击打开结算单单明细!" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                                <f:Button ID="btnClearField" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnClearField_Click" />
                                                <f:Button ID="btnSear" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" ValidateForms="FormDoc" runat="server" OnClick="btnSear_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstSEQNO" runat="server" Label="养护单号"></f:TextBox>
                                                        <f:DropDownList ID="lstDEPTID" runat="server" Label="养护仓库" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="lstYHY" runat="server" Label="养护人" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="lstFLAG" runat="server" Label="养护状态" EnableEdit="true" ForceSelection="true"></f:DropDownList>
                                                        <f:DatePicker ID="dpkDATE1" runat="server" Label="日期" Required="true" />
                                                        <f:DatePicker ID="dpkDATE2" runat="server" Label="　　至" LabelSeparator="" Required="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridDoc" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="SEQNO,FLAG" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridDoc_RowDoubleClick"
                                    EnableHeaderMenu="true" OnRowDataBound="GridDoc_RowDataBound">
                                    <Columns>
                                        <f:RowNumberField Width="35px" TextAlign="Center" EnablePagingNumber="true" HeaderText="序号" />
                                        <f:BoundField Width="120px" DataField="SEQNO" SortField="SEQNO" HeaderText="养护单号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAG" ColumnID="FLAG" SortField="FLAG" HeaderText="养护状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" SortField="FLAGNAME" HeaderText="养护状态" ColumnID="FLAGNAME" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="YHYNAME" SortField="YHYNAME" HeaderText="养护人" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" SortField="DEPTNAME" HeaderText="养护仓库" />
                                        <f:BoundField Width="110px" DataField="YHRQ" SortField="YHRQ" HeaderText="养护日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="100px" DataField="SUBNUM" SortField="SUBNUM" HeaderText="明细条数" TextAlign="Center" />
                                        <f:BoundField Width="100px" DataField="SUBSUM" SortField="SUBSUM" HeaderText="合计金额" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="LRYNAME" SortField="LRYNAME" HeaderText="录入员" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="LRRQ" SortField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="70px" DataField="SHRNAME" SortField="SHRNAME" HeaderText="审核人" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="SHRQ" SortField="SHRQ" HeaderText="审核日期" TextAlign="Center" />
                                        <f:BoundField Width="0px" DataField="MEMO" SortField="MEMO" HeaderText="备注" ExpandUnusedSpace="true" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBOX"
                            ShowHeader="false">
                            <Items>
                                <f:Panel ID="Panel5" ShowBorder="false" BodyPadding="0px" Layout="Anchor" ShowHeader="False" runat="server">
                                    <Toolbars>
                                        <f:Toolbar ID="Toolbar3" runat="server">
                                            <Items>
                                                <f:ToolbarText ID="ToolbarText3" CssStyle="" Text="操作信息：双击打开出库单明细!" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                                <f:Button ID="btnNew" Icon="PageAdd" Text="新 单" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" Hidden="true" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="整单删除" EnableDefaultState="false" EnablePostBack="true" runat="server" ConfirmText="是否确认删除此养护单?" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnableDefaultState="false" Hidden="true" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnTJ" Icon="UserTick" Text="开始养护" EnableDefaultState="false" EnablePostBack="true" ConfirmText="是否确认提交此单据?" runat="server" OnClick="btnTJ_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保存" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="Formlis" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnableDefaultState="false" ValidateForms="Formlis" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Printer" Text="打印养护单" EnableDefaultState="false" EnablePostBack="false" runat="server" OnClientClick="btnPrint_onclick()" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btnBill_Click" ConfirmText="是否导出当前商品在库保养信息?" DisableControlBeforePostBack="false"
                                                    EnableAjax="false" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlis" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow Hidden="true">
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" Hidden="true" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="docBILLNO" runat="server" Label="养护单号" />
                                                        <f:DatePicker ID="dpkYHRQ" runat="server" Label="养护日期" />
                                                        <f:DropDownList ID="ddlLRY" runat="server" Label="录入员" />
                                                        <f:DropDownList ID="ddlSHR" runat="server" Label="审核员" />
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="养护仓库" EnableEdit="true" ForceSelection="true" ></f:DropDownList>
                                                        <f:DropDownList ID="ddlYHY" runat="server" Label="养护人" ForceSelection="true" />
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期" EnableEdit="true" />
                                                        <f:DatePicker ID="dpkSHRQ" runat="server" Label="审核日期"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow ColumnWidths="50% 25% 25%">
                                                    <Items>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="备注" MaxLength="80"></f:TextBox>
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" ForceSelection="true" />
                                                        <f:Label runat="server"></f:Label>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridLis" ClicksToEdit="1" BoxFlex="1" ShowBorder="false" ShowHeader="false" AllowCellEditing="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="SEQNO">
                                    <Columns>
                                        <f:RenderField Width="35px" TextAlign="Center" Locked="true" ColumnID="ROWNO" DataField="ROWNO" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="105px" ColumnID="GDSEQ" DataField="GDSEQ" FieldType="String"
                                            HeaderText="商品编码" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSEQ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" ColumnID="GDNAME" DataField="GDNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品名称" EnableLock="true" Locked="true">
                                            <Editor>
                                                <f:Label ID="lblGDNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="120px" ColumnID="GDSPEC" DataField="GDSPEC" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="商品规格">
                                            <Editor>
                                                <f:Label ID="lblEditorGDSPEC" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="80px" DataField="YHTYPE" ColumnID="YHTYPE" HeaderText="养护标准" TextAlign="Center" FieldType="String" EnableHeaderMenu="false" RendererFunction="renderYHTYPE">
                                            <Editor>
                                                <f:DropDownList ID="comYHTYPE" runat="server">
                                                    <f:ListItem Text="正常" Value="0" />
                                                    <f:ListItem Text="破损" Value="1" />
                                                    <f:ListItem Text="失效" Value="2" />
                                                    <f:ListItem Text="发霉" Value="3" />
                                                </f:DropDownList>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="180px" DataField="REASON" ColumnID="REASON" HeaderText="原因说明" FieldType="String" EnableHeaderMenu="false">
                                            <Editor>
                                                <f:TextArea ID="comREASON" runat="server" MaxLength="80"></f:TextArea>
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="70px" ColumnID="UNITNAME" DataField="UNITNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="单位" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="Label1" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="ISGZNAME" DataField="ISGZNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="是否贵重" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblISGZNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJJ" DataField="HSJJ" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税进价" TextAlign="Right" RendererFunction="round4">
                                            <Editor>
                                                <f:Label ID="lblHSJJ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="KCSL" DataField="KCSL" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="库存数量" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblKCSL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="90px" ColumnID="HSJE" DataField="HSJE" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="含税金额" TextAlign="Right" RendererFunction="round2">
                                            <Editor>
                                                <f:Label ID="lblHSJE" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="HWID" DataField="HWID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="货位" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblHWID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="PHID" DataField="PHID" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="批号" TextAlign="Right">
                                            <Editor>
                                                <f:Label ID="lblPHID" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="YXQZ" DataField="YXQZ" EnableHeaderMenu="false"
                                            HeaderText="有效期至" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblYXQZ" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="110px" ColumnID="RQ_SC" DataField="RQ_SC" EnableHeaderMenu="false"
                                            HeaderText="生产日期" TextAlign="Center" Renderer="Date" RendererArgument="yyyy-MM-dd">
                                            <Editor>
                                                <f:Label ID="lblRQ_SC" runat="server" />
                                            </Editor>
                                        </f:RenderField>

                                        <f:RenderField Width="0px" ColumnID="PZWH" DataField="PZWH" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="注册证号">
                                            <Editor>
                                                <f:Label ID="lblPZWH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="130px" ColumnID="PRODUCERNAME" DataField="PRODUCERNAME" FieldType="String" EnableHeaderMenu="false"
                                            HeaderText="生产厂家">
                                            <Editor>
                                                <f:Label ID="lblPRODUCERNAME" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ZPBH" DataField="ZPBH" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="制品编码">
                                            <Editor>
                                                <f:Label ID="lblZPBH" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="MEMO" DataField="MEMO" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="备注">
                                            <Editor>
                                                <f:Label ID="lblMEMO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="UNIT" DataField="UNIT" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="单位编码" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblEditorUNIT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PRODUCER" DataField="PRODUCER" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="生产厂家" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPRODUCER" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="PICINO" DataField="PICINO" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="批次编号" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblPICINO" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ISLOT" DataField="ISLOT" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="批号管理" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblISLOT" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="BZHL" DataField="BZHL" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="包装含量" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="lblBZHL" runat="server" />
                                            </Editor>
                                        </f:RenderField>
                                        <f:RenderField Width="0px" ColumnID="ISGZ" DataField="ISGZ" FieldType="String" EnableHeaderMenu="false" Hidden="true"
                                            HeaderText="是否贵重" TextAlign="Center">
                                            <Editor>
                                                <f:Label ID="ISGZ" runat="server" />
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
        <f:HiddenField ID="hfdOper" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px">
        </f:Window>
    </form>
    <script>
        var ddlYHTYPE = '<%= comYHTYPE.ClientID %>';
        function renderYHTYPE(value) {
            return F(ddlYHTYPE).getTextByValue(value);
        }
        function btnPrint_onclick() {
            var billNo = F('<%= docSEQNO.ClientID%>').getValue();
            var billState = F('<%= docFLAG.ClientID%>').getValue();
            if (billNo == "") {
                F.alert("请选择要打印的单据信息！");
                return;
            }
            if (billState != "N") {
                F.alert("选择单据未提交,不允许打印！");
                return;
            }
            ReportViewer.ReportURL = "/grf/zkyhd.grf";
            var dataurl = "/captcha/PrintReport.aspx?Method=GetZkyhData&osid=" + billNo;
            ReportViewer.Report.LoadDataFromURL(dataurl);
            ReportViewer.Start();
            ReportViewer.Report.PrintPreview(true);
            ReportViewer.Stop();
        }


    </script>
</body>
</html>