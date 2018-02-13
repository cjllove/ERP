<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConstantAbnormalReport.aspx.cs" Inherits="ERPProject.ERPReport.ConstantAbnormalReport" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <f:PageManager ID="PageManager1" AutoSizePanelID="Panel1" runat="server" />
        <f:Panel ID="Panel1" ShowBorder="false" BodyPadding="0px" Layout="VBox" ShowHeader="False" runat="server">
                    <Toolbars>
                        <f:Toolbar ID="Toolbar1" runat="server">
                            <Items>
                                <f:ToolbarText ID="ToolbarText2" CssStyle="" Text="操作信息：查询提醒定数异常情况！" runat="server" />
                                <f:ToolbarFill ID="ToolbarFill1" runat="server" />
                                
                                <f:Button ID="bntClear" Icon="Erase" Text="清 除" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="bntSearch" Icon="Magnifier" Text="查 询" EnableDefaultState="false" EnablePostBack="true" runat="server" OnClick="btnBill_Click" ValidateForms="Formlist" />
                                <f:ToolbarSeparator runat="server" />
                                <f:Button ID="btnExport" Icon="DatabaseGo" Text="导 出" EnableDefaultState="false" OnClick="btExport_Click" ConfirmText="是否导出当前信息?" DisableControlBeforePostBack="false" EnablePostBack="true" EnableAjax="false" EnableAjaxLoading="true" AjaxLoadingType="Mask" runat="server" Enabled="false" Hidden="true" />

                            </Items>
                        </f:Toolbar>
                    </Toolbars>
            <Items>
                <f:Form ID="Formlist" ShowBorder="false" AutoScroll="false" BodyPadding="10px 10px 5px 10px"
                    ShowHeader="False" LabelWidth="80px" runat="server">
                    <Rows>
                        <f:FormRow ColumnWidths="50% 25% 25%">
                            <Items>
                                <f:DropDownList ID="lstDEPTID" runat="server" Label="科室" EnableEdit="true" ForceSelection="true" LabelWidth="60px" />
                                <f:NumberBox ID="QCQX" runat="server" Label="异常期限" MinValue="0" MaxValue="366" NoDecimal="true"></f:NumberBox>
                                <%--<f:DatePicker ID="lstKSSJ" runat="server" Label="开始时间" Required="true" ShowRedStar="true" />
                                <f:DatePicker ID="lstJSSJ" runat="server" Label="结束时间" LabelSeparator="" Required="true" ShowRedStar="true" />--%>
                            </Items>
                        </f:FormRow>
                    </Rows>
                </f:Form>

                <f:Grid ID="GridList" BoxFlex="1" ShowBorder="false" ShowHeader="false"
                    AllowSorting="false" AutoScroll="true" runat="server" CssStyle="border-top: 1px solid #99bce8;"
                    DataKeyNames="SEQNO" EnableRowDoubleClickEvent="true" OnRowDoubleClick="listRow_DoubleClick" EnableTextSelection="true" EnableColumnLines="true"
                    IsDatabasePaging="true" AllowPaging="true" PageSize="100" OnPageIndexChange="GridList_PageIndexChange">
                    <Columns>
                        <f:RowNumberField Width="30px" EnablePagingNumber="true" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="OUTRQ" HeaderText="出库日期" TextAlign="Center" />
                        <f:BoundField Width="50px" DataField="QX" HeaderText="天数" TextAlign="Center" />
                        <f:BoundField Width="150px" DataField="DEPTIN" HeaderText="科室" />
                        <f:BoundField Width="110px" DataField="OUTBILLNO" HeaderText="单据编号" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="GDSEQ" HeaderText="商品编码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="GDNAME" HeaderText="商品名称" TextAlign="Left" />
                        <f:BoundField Width="130px" DataField="GDSPEC" HeaderText="商品规格" TextAlign="Left" />
                        <f:BoundField Width="80px" DataField="DSHL" HeaderText="定数含量" TextAlign="Center" />
                        <f:BoundField Width="60px" DataField="UNITNAME" HeaderText="单位" TextAlign="Center" />
                        <f:BoundField Width="80px" DataField="HSJJ" HeaderText="价格" DataFormatString="{0:F4}" TextAlign="Center" />
                        <f:BoundField Width="100px" DataField="HSJJJE" HeaderText="金额" DataFormatString="{0:F4}" TextAlign="Right" />
                        <f:BoundField Width="200px" DataField="BARCODE" HeaderText="定数条码" TextAlign="Center" />
                        <f:BoundField Width="180px" DataField="PRODUCERNAME" HeaderText="厂家" TextAlign="Left" />
                    </Columns>
                </f:Grid>
            </Items>
        </f:Panel>
        <f:HiddenField ID="hfdQCRQ" runat="server"></f:HiddenField>
    </form>
</body>
</html>
