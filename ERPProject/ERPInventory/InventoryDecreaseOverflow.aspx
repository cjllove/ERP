﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InventoryDecreaseOverflow.aspx.cs" Inherits="ERPProject.ERPInventory.InventoryDecreaseOverflow" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>盘点损益管理</title>
    <style type="text/css" media="all">
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
                                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnExport" Icon="PageExcel" Text="导 出" DisableControlBeforePostBack="false" ConfirmText="是否确认导出此商品资料信息?" runat="server" EnableAjax="false" OnClick="btnExport_Click" Enabled="false" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnablePostBack="true" runat="server" ValidateForms="Formlist" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="90px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="TextBox1" runat="server" Label="计划盘点单号" />
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="盘点单号" />
                                                        <f:DropDownList ID="lstDEPTOUT" runat="server" Label="损益科室" EnableEdit="true" ForceSelection="true" />
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="录入日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="   至" Required="true" ShowRedStar="true" CompareType="String" CompareControl="lstLRRQ1" CompareOperator="GreaterThanEqual" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -68" ShowBorder="false" ShowHeader="false"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableColumnLines="true"
                                    EnableHeaderMenu="true" SortField="SEQNO" OnSort="GridList_Sort" SortDirection="ASC" AllowSorting="true">
                                    <Columns>
                                        <f:RowNumberField />
                                        <f:BoundField Width="150px" DataField="SEQNO" HeaderText="损益单号" SortField="SEQNO" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="STR1" HeaderText="计划盘点单号" TextAlign="Center" SortField="STR1" />
                                        <f:BoundField Width="150px" DataField="STR2" HeaderText="盘点单号" TextAlign="Center" SortField="STR2" />
                                        <f:BoundField Width="70px" DataField="BILLTYPE" HeaderText="单据类别" TextAlign="Center" Hidden="true" SortField="BILLTYPE" />
                                        <f:BoundField Width="130px" DataField="FLAG" HeaderText="单据状态" Hidden="true" SortField="FLAG" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" HeaderText="单据状态" TextAlign="Center" SortField="FLAGNAME" />
                                        <f:BoundField Width="70px" DataField="DEPTID" HeaderText="损益科室" TextAlign="Center" Hidden="true" SortField="DEPTID" />
                                        <f:BoundField Width="70px" DataField="DEPTIDNAME" HeaderText="损益科室" TextAlign="Center" SortField="DEPTIDNAME" ExpandUnusedSpace="true" />
                                        <f:BoundField Width="70px" DataField="SYTYPE" HeaderText="损益类别" TextAlign="Center" Hidden="true" SortField="SYTYPE" />
                                        <f:BoundField Width="70px" DataField="SYTYPENAME" HeaderText="损益类别" TextAlign="Center" SortField="SYTYPENAME" />
                                        <f:BoundField Width="120px" DataField="SUBSUM" HeaderText="损益金额" TextAlign="Center" DataFormatString="{0:F2}" SortField="SUBSUM" />
                                        <f:BoundField Width="120px" DataField="SUBNUM" HeaderText="明细条数" TextAlign="Center" SortField="SUBNUM" />
                                        <f:BoundField Width="110px" DataField="LRY" HeaderText="录入员" TextAlign="Center" Hidden="true" SortField="LRY" />
                                        <f:BoundField Width="110px" DataField="LRYNAME" HeaderText="录入员" TextAlign="Center" SortField="LRYNAME" />
                                        <f:BoundField Width="130px" DataField="LRRQ" HeaderText="录入日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="LRRQ" />
                                        <f:BoundField Width="110px" DataField="SPR" HeaderText="审核员" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="SPRNAME" HeaderText="审核员" TextAlign="Center" SortField="SPRNAME" />
                                        <f:BoundField Width="130px" DataField="SPRQ" HeaderText="审核日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" SortField="SPRQ" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" TextAlign="Center" SortField="MEMO" />
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
                                                <%-- <f:Button ID="btnNew" Icon="PageAdd" Text="新 增" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnDel" Icon="PageCancel" Text="删 除" EnablePostBack="true" ConfirmText="是否确认删除此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnSave" Icon="Disk" Text="保 存" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAudit" Icon="UserTick" Text="审 核" EnablePostBack="true" ConfirmText="是否确认审核此单据?" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCancel" Icon="UserCross" Text="驳 回" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:Button ID="btnCopy" Icon="PageCopy" Text="复 制" ConfirmText="是否确定已经保存数据并复制单据?" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnAddRow" Icon="Add" Text="增 行" runat="server" OnClick="btnBill_Click" ValidateForms="FormDoc" />
                                                <f:Button ID="btnDelRow" Icon="Delete" Text="删 行" EnablePostBack="true" ConfirmText="是否确认删除选中行信息?" runat="server" OnClick="btnBill_Click" />
                                                <f:ToolbarSeparator runat="server" />
                                                <f:Button ID="btnPrint" Icon="Magnifier" Text="科 室" EnablePostBack="true" runat="server" OnClick="btnadd_Click" />--%>
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="70px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxBILLNO" runat="server" Label="损益单号">
                                                        </f:TextBox>
                                                        <f:DropDownList ID="ddlFLAG" runat="server" Label="单据状态" Enabled="false">
                                                        </f:DropDownList>
                                                        <f:DropDownList ID="ddlLRY" runat="server" Label="录入员" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkLRRQ" runat="server" Label="录入日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="tbxSTR1" runat="server" Label="盘点单号" Enabled="false"></f:TextBox>
                                                        <f:TextBox ID="tbxSTR2" runat="server" Label="计划盘点单号" Enabled="false" LabelWidth="90px"></f:TextBox>
                                                        <f:DropDownList ID="ddlSPR" runat="server" Label="审批人" Enabled="false"></f:DropDownList>
                                                        <f:DatePicker ID="dpkSPRQ" runat="server" Label="审批日期" Enabled="false"></f:DatePicker>
                                                    </Items>
                                                </f:FormRow>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:DropDownList ID="ddlDEPTID" runat="server" Label="损益部门" ShowRedStar="true" Required="true" Enabled="false"></f:DropDownList>
                                                        <f:DropDownList ID="ddlSYTYPE" runat="server" Label="损益类别" Enabled="false"></f:DropDownList>
                                                        <f:NumberBox ID="nbxSUBSUM" runat="server" Label="损益金额" Enabled="false"></f:NumberBox>
                                                        <f:TextBox ID="tbxMEMO" runat="server" Label="备注" Enabled="false"></f:TextBox>
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -122" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" DataKeyNames="GDSEQ" EnableColumnLines="true">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="30px" TextAlign="Center"></f:RowNumberField>
                                        <f:BoundField Width="120px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                                        <f:BoundField Width="150px" DataField="GDNAME" ColumnID="GDNAME" HeaderText="商品名称" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="BARCODE" HeaderText="商品条码" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="GDSPEC" HeaderText="规格" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="HWID" HeaderText="货位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="SYSL" ColumnID="SYSL" HeaderText="损益数量" TextAlign="Right" />
                                        <f:BoundField Width="0px" DataField="UNIT" HeaderText="单位" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="BZHL" HeaderText="包装含量" TextAlign="Right" />
                                        <f:BoundField Width="70px" DataField="KCSL" ColumnID="KCSL" HeaderText="库存数量" TextAlign="Right" />
                                        <f:BoundField Width="90px" DataField="KCHSJE" ColumnID="KCHSJE" HeaderText="库存含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="JXTAX" HeaderText="进项税" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="HSJJ" HeaderText="含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                                        <f:BoundField Width="90px" DataField="HSJE" ColumnID="HSJE" HeaderText="损益含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="BHSJJ" HeaderText="不含税进价" TextAlign="Right" DataFormatString="{0:F4}" />
                                        <f:BoundField Width="100px" DataField="BHSJE" ColumnID="BHSJE" HeaderText="损益不含税金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="70px" DataField="LSJ" HeaderText="零售价" TextAlign="Right" DataFormatString="{0:F4}" />
                                        <f:BoundField Width="90px" DataField="LSJE" ColumnID="LSJE" HeaderText="损益零售金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="120px" DataField="PH" HeaderText="批号" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="PZWH" HeaderText="注册证号" TextAlign="Center" />
                                        <f:BoundField Width="120px" DataField="RQ_SC" HeaderText="生产日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="YXQZ" HeaderText="有效期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="ZPBH" HeaderText="制品编号" TextAlign="Center" />
                                        <f:BoundField Width="170px" DataField="MEMO" HeaderText="备注" TextAlign="Center" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
    </form>
</body>
</html>