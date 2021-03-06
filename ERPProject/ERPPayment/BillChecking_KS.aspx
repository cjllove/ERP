﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BillChecking_KS.aspx.cs" Inherits="ERPProject.ERPApply.BillChecking_KS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>科室结算管理</title>
</head>
<body>
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
                                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" ValidateForms="Formlist" OnClick="btnBill_Click" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="80px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="lstBILLNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="输入单据编号信息" />
                                                        <f:DropDownList ID="lstPSSID" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true" Required="true" ShowRedStar="true">
                                                        </f:DropDownList>
                                                        <f:DatePicker ID="lstLRRQ1" runat="server" Label="结算日期" Required="true" ShowRedStar="true" />
                                                        <f:DatePicker ID="lstLRRQ2" runat="server" Label="　至" Required="true" ShowRedStar="true" CompareControl="lstLRRQ1" CompareOperator="GreaterThanEqual" CompareType="String" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridList" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom"
                                    AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick"
                                    AllowSorting="true" EnableHeaderMenu="true" SortField="BILLNO" OnSort="GridList_Sort">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="30px" />
                                        <f:BoundField Width="130px" DataField="SEQNO" SortField="SEQNO" HeaderText="结算单号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAG" SortField="FLAG" ColumnID="FLAG" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" ColumnID="FLAGNAME" SortField="FLAGNAME" HeaderText="单据状态" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="BEGRQ" SortField="BEGRQ" HeaderText="上次结算日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="ENDRQ" SortField="ENDRQ" HeaderText="本次结算日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="90px" DataField="SYJE" SortField="SYJE" ColumnID="SYJE" HeaderText="损益金额" TextAlign="Center" DataFormatString="{0:F2}" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="XSJE" SortField="XSJE" ColumnID="XSJE" HeaderText="使用金额" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="THJE" SortField="THJE" ColumnID="THJE" HeaderText="退货金额" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="YJJJ" SortField="YJJJ" ColumnID="YJJJ" HeaderText="应结金额" TextAlign="Center" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="JSJE" SortField="JSJE" ColumnID="JSJE" HeaderText="实结金额" TextAlign="Center" DataFormatString="{0:F2}" Hidden="true" />
                                        <f:BoundField Width="90px" DataField="WJJE" SortField="WJJE" ColumnID="WJJE" HeaderText="未结金额" TextAlign="Center" DataFormatString="{0:F2}" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="SUPNAME" SortField="SUPNAME" HeaderText="送货商" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="GATFUNDCORP" SortField="GATFUNDCORP" HeaderText="收款单位" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="GATFUNDBANK" SortField="GATFUNDBANK" HeaderText="收款银行" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="LINKMAN" SortField="LINKMAN" HeaderText="业务联系人" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="LINKTEL" SortField="LINKTEL" HeaderText="业务联系电话" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="CWLINKMAN" SortField="CWLINKMAN" HeaderText="财务联系人" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="CWLINKTEL" SortField="CWLINKTEL" HeaderText="财务联系电话" TextAlign="Center" />
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
                                                <f:ToolbarText ID="ToolbarText1" CssStyle="" Text="操作信息：" runat="server" />
                                                <f:ToolbarFill ID="ToolbarFill2" runat="server" />
                                            </Items>
                                        </f:Toolbar>
                                    </Toolbars>
                                    <Items>
                                        <f:Form ID="FormDoc" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                            ShowHeader="False" LabelWidth="75px" runat="server">
                                            <Rows>
                                                <f:FormRow>
                                                    <Items>
                                                        <f:TextBox ID="docSEQNO" runat="server" Label="单据编号" MaxLength="20" EmptyText="结算单号" />
                                                        <f:DropDownList ID="docFLAG" runat="server" Label="单据状态" EnableEdit="true" ForceSelection="true" />
                                                        <f:DropDownList ID="ddlSUPID" runat="server" Label="配送商"></f:DropDownList>
                                                        <f:DropDownList ID="docLRY" runat="server" Label="结算员" ForceSelection="true" />
                                                    </Items>
                                                </f:FormRow>
                                            </Rows>
                                        </f:Form>
                                    </Items>
                                </f:Panel>
                                <f:Grid ID="GridGoods" AnchorValue="100% -82" ShowBorder="false" ShowHeader="false" SummaryPosition="Bottom" EnableSummary="true" EnableTextSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="BILLNO,BILLTYPE" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridLis_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="30px"></f:RowNumberField>
                                        <f:BoundField Width="120px" DataField="BILLNO" HeaderText="出库单号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="BILLTYPE" HeaderText="单据类别" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="BILLTYPENAME" ColumnID="BILLTYPENAME" HeaderText="单据类别" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="FLAG" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="FLAGNAME" HeaderText="单据状态" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="DEPTID" HeaderText="领用科室" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="DEPTIDNAME" HeaderText="领用科室" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="DEPTOUT" HeaderText="出库部门/库房" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="DEPTOUTNAME" HeaderText="出库部门/库房" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="XSTYPE" HeaderText="申领类别" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="70px" DataField="XSTYPENAME" HeaderText="申领类别" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="PRICE_HSJE" HeaderText="含税金额" ColumnID="PRICE_HSJE" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="PRICE_RTN" HeaderText="退货金额" ColumnID="PRICE_RTN" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="90px" DataField="PRICE" ColumnID="PRICE" HeaderText="应结金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="60px" DataField="LRYNAME" HeaderText="做单员" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="LRRQ" HeaderText="做单日期 " TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="110px" DataField="XSRQ" HeaderText="申领日期 " TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" Hidden="true" />
                                        <f:BoundField Width="60px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="SHRQ" HeaderText="审核日期 " TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="120px" DataField="SEQNO" HeaderText="结算单号" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
            </Tabs>
        </f:TabStrip>
        <f:HiddenField ID="hfdValue" runat="server" />
        <f:Window ID="Window1" Title="商品信息" Hidden="true" EnableIFrame="true" runat="server"
            EnableMaximize="false" EnableResize="true" Target="Parent" IsModal="True" Width="820px" Height="480px">
        </f:Window>
    </form>
</body>
</html>
