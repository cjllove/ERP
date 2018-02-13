<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BillChecking_sum.aspx.cs" Inherits="ERPProject.ERPApply.BillChecking_sum" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>结算管理</title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager EnableAjaxLoading="false" ID="PageManager1" AutoSizePanelID="TabStrip1"
            runat="server" />
        <f:TabStrip ID="TabStrip1" ShowBorder="false" TabPosition="Right"
            EnableTabCloseMenu="false" ActiveTabIndex="0"
            runat="server">
            <Tabs>
                <f:Tab Title="生成结算单" Icon="Table" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel2" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar2" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：双击打开明细！" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                        <f:Button ID="Bill_create" Icon="PackageStart" EnableDefaultState="false" Text="生成结算单" Enabled="false" ConfirmText="是否生成此时间段内的结算单?" runat="server" OnClick="Bill_create_Click" />
                                        <f:ToolbarSeparator runat="server" />
                                        <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                        <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" ValidateForms="Formlist" runat="server" OnClick="btnBill_Click" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="90px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlsup" runat="server" Label="送货商" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DropDownList ID="lstDEPTOUT" runat="server" Label="结算部门" EnableEdit="true" ForceSelection="true">
                                                </f:DropDownList>
                                                <f:DatePicker ID="lstLRRQ2" runat="server" Label="结算日期" Required="true" ShowRedStar="true"></f:DatePicker>
                                                <f:DropDownList ID="lstJSY" runat="server" Label="结算员" Enabled="false"></f:DropDownList>
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableSummary="true" SummaryPosition="Bottom" KeepCurrentSelection="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true" EnableTextSelection="true"
                                    DataKeyNames="DEPTID,PSSID,KSTIME,JSTIME" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableCheckBoxSelect="true">
                                    <Columns>
                                        <f:RowNumberField runat="server" Width="35px" />
                                        <f:BoundField Width="0px" DataField="DEPTID" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="0px" DataField="PSSID" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="150px" DataField="PSSNAME" ColumnID="PSSNAME" HeaderText="送货商" />
                                        <f:BoundField Width="150px" DataField="DEPTNAME" ColumnID="DEPTNAME" HeaderText="结算部门" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="KSTIME" HeaderText="结算开始日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="110px" DataField="JSTIME" HeaderText="结算结束日期" TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="130px" DataField="USESUM" HeaderText="使用品规数" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="PRICE_USE" ColumnID="PRICE_USE" HeaderText="使用金额" TextAlign="Right" DataFormatString="{0:F}" />
                                        <f:BoundField Width="130px" DataField="PRICE_RTN" ColumnID="PRICE_RTN" HeaderText="退货(调拨)金额" TextAlign="Right" DataFormatString="{0:F}" />
                                        <f:BoundField Width="130px" DataField="YJJJ" ColumnID="YJJJ" HeaderText="应结金额" TextAlign="Right" DataFormatString="{0:F}" />
                                    </Columns>
                                </f:Grid>
                            </Items>
                        </f:Panel>
                    </Items>
                </f:Tab>
                <f:Tab Title="单据信息" Icon="PageWord" Layout="Fit" runat="server">
                    <Items>
                        <f:Panel ID="Panel4" runat="server" ShowBorder="false" BodyPadding="0px" Layout="VBox"
                            ShowHeader="false">
                            <Toolbars>
                                <f:Toolbar ID="Toolbar3" runat="server">
                                    <Items>
                                        <f:ToolbarText ID="ToolbarText3" CssStyle="" Text="操作信息：双击打开出库单明细!" runat="server" />
                                        <f:ToolbarFill ID="ToolbarFill3" runat="server" />
                                    </Items>
                                </f:Toolbar>
                            </Toolbars>
                            <Items>
                                <f:Form ID="Formlis" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                                    ShowHeader="False" LabelWidth="80px" runat="server">
                                    <Rows>
                                        <f:FormRow>
                                            <Items>
                                                <f:DropDownList ID="ddlSUPID" runat="server" Label="送货商"></f:DropDownList>
                                                <f:DropDownList ID="ddlDEPTID" runat="server" Label="结算部门" Required="true" ShowRedStar="true" EnableEdit="true" ForceSelection="true" />
                                                <f:DatePicker ID="dpkBEGRQ" runat="server" Label="结算日期" />
                                                <f:DatePicker ID="dpkENDRQ" runat="server" Label=" 至" />
                                            </Items>
                                        </f:FormRow>
                                    </Rows>
                                </f:Form>
                                <f:Grid ID="GridLis" BoxFlex="1" ShowBorder="false" ShowHeader="false" EnableTextSelection="true" SummaryPosition="Bottom" EnableSummary="true"
                                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;" EnableColumnLines="true"
                                    DataKeyNames="SEQNO,BILLTYPE" EnableRowDoubleClickEvent="true" OnRowDoubleClick="GridLis_RowDoubleClick">
                                    <Columns>
                                        <f:RowNumberField Width="35px"></f:RowNumberField>
                                        <f:BoundField Width="130px" DataField="BILLNO" ColumnID="BILLNO" HeaderText="单据号" TextAlign="Center" />
                                        <f:BoundField Width="70px" DataField="BILLTYPE" HeaderText="单据类别" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="110px" DataField="BILLTYPENAME" HeaderText="单据类别" TextAlign="Center" />
                                        <f:BoundField Width="0px" DataField="DEPTID" HeaderText="部门编码" TextAlign="Center" Hidden="true" />
                                        <f:BoundField Width="130px" DataField="DEPTNAME" HeaderText="业务部门" TextAlign="Center" />
                                        <f:BoundField Width="90px" DataField="JE" ColumnID="JE" HeaderText="单据金额" TextAlign="Right" DataFormatString="{0:F2}" />
                                        <f:BoundField Width="60px" DataField="LRYNAME" HeaderText="做单员" TextAlign="Center" />
                                        <f:BoundField Width="110px" DataField="LRRQ" HeaderText="做单日期 " TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
                                        <f:BoundField Width="60px" DataField="SHRNAME" HeaderText="审核员" TextAlign="Center" />
                                        <f:BoundField Width="130px" DataField="SHRQ" HeaderText="审核日期 " TextAlign="Center" DataFormatString="{0:yyyy-MM-dd}" />
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
